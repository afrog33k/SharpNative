// /*
//   SharpNative - C# to D Transpiler
//   (C) 2014 Irio Systems 
// */

#region Imports

using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;

#endregion

namespace SharpNative.Compiler.DlangAst
{
    public class CSharpToDlangRewriter : CSharpSyntaxRewriter
    {
        private readonly SemanticModel _semanticModel;

        public CSharpToDlangRewriter(SemanticModel semanticModel)
        {
            _semanticModel = semanticModel;
        }

        public override SyntaxNode Visit(SyntaxNode node)
        {
            Context.LastNode = node;
            try
            {

                if (node is NameEqualsSyntax)
                    return (node);

               

                //			SyntaxNode visit; //Needs update
                //                     if (FixPropertyUnaryExpressions(node, out visit))
                //                return visit;
                  if (node is IdentifierNameSyntax &&
                   //   (node.Parent is ExpressionSyntax || node.Parent is MethodDeclarationSyntax || node.Parent is PropertyDeclarationSyntax
                     // || node.Parent is BlockSyntax) &&
                      !(node.Parent.Parent is InitializerExpressionSyntax) &&
                      !(node.Parent is QualifiedNameSyntax) && !(node.Parent is MemberAccessExpressionSyntax) && !(node.Parent is ThisExpressionSyntax))
                  {
                      //Lets fully qualify these so that we can have property code working
                      var symbolInfo = _semanticModel.GetSymbolInfo(node);

                      if (symbolInfo.Symbol != null &&
                          (symbolInfo.Symbol.Kind == SymbolKind.Field || symbolInfo.Symbol.Kind == SymbolKind.Property))
                      {
                          if (symbolInfo.Symbol.ContainingType != null && symbolInfo.Symbol.IsStatic)
                          {
                              var newName = symbolInfo.Symbol.ContainingType.GetFullNameCSharp() + "." +
                                            symbolInfo.Symbol.Name;
                              return SyntaxFactory.ParseExpression(newName);
                          }
                          else
                          {

                              var firstParent =
                                  TypeProcessor.GetDeclaredSymbol(node.Ancestors().OfType<BaseTypeDeclarationSyntax>().First());
                              //_semanticModel.GetSymbolInfo(node.Ancestors().OfType<BaseTypeDeclarationSyntax>().First());
                              if (symbolInfo.Symbol.ContainingType != null && !symbolInfo.Symbol.IsStatic && symbolInfo.Symbol.ContainingType == firstParent)
                                  return SyntaxFactory.ParseExpression("this." + symbolInfo.Symbol.Name);
                          }
                          return base.Visit(node);
                      }
                  }

                if (node is MemberAccessExpressionSyntax &&
                //   (node.Parent is ExpressionSyntax || node.Parent is MethodDeclarationSyntax || node.Parent is PropertyDeclarationSyntax
                // || node.Parent is BlockSyntax) &&
                !(node.Parent.Parent is InitializerExpressionSyntax) &&
              !(node.Parent is QualifiedNameSyntax) && !(node.Parent is MemberAccessExpressionSyntax) && !(node.Parent is ThisExpressionSyntax))
            {
                var nodeasMember = node as MemberAccessExpressionSyntax;
                if (!(nodeasMember.Expression is ThisExpressionSyntax))
                { 
                //Lets fully qualify these so that we can have property code working
                var symbolInfo = _semanticModel.GetSymbolInfo(node);

                    if (symbolInfo.Symbol != null &&
                        (symbolInfo.Symbol.Kind == SymbolKind.Field || symbolInfo.Symbol.Kind == SymbolKind.Property))
                    {
                        if (symbolInfo.Symbol.ContainingType != null && symbolInfo.Symbol.IsStatic)
                        {
                            var newName = symbolInfo.Symbol.ContainingType.GetFullNameCSharp() + "." +
                                          symbolInfo.Symbol.Name;
                            return SyntaxFactory.ParseExpression(newName);
                        }
                        else
                        {
                            ISymbol symbol = TypeProcessor.GetSymbolInfo((node as MemberAccessExpressionSyntax).Expression).Symbol;
                            if (
                                symbol != null && (!(symbol
                                    is
                                    ILocalSymbol) && !(symbol is IParameterSymbol) && symbol.Name != ".ctor"))
                            {
                                var firstParent =
                                    TypeProcessor.GetDeclaredSymbol(
                                        node.Ancestors().OfType<BaseTypeDeclarationSyntax>().First());
                                //_semanticModel.GetSymbolInfo(node.Ancestors().OfType<BaseTypeDeclarationSyntax>().First());
                                if (symbolInfo.Symbol.ContainingType != null && !symbolInfo.Symbol.IsStatic &&
                                    symbolInfo.Symbol.ContainingType == firstParent)
                                    return SyntaxFactory.ParseExpression("this." + node.ToFullString());
                            }

                        }
                    }
                    return base.Visit(node);
                }
            }

            return base.Visit(node);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        public override SyntaxNode VisitSwitchSection(SwitchSectionSyntax node) //TODO: Should fix string switch here
        {
            return base.VisitSwitchSection(node);
        }

        private static bool FixPropertyUnaryExpressions(SyntaxNode node, out SyntaxNode visit)
        {
            if (node is PostfixUnaryExpressionSyntax)
            {
                var expression = node as PostfixUnaryExpressionSyntax;
                if (expression.Operand is MemberAccessExpressionSyntax)
                {
                    var memberAccess = expression.Operand as MemberAccessExpressionSyntax;
                    var typeInfo = TypeProcessor.GetSymbolInfo(memberAccess.Name);

                    if (typeInfo.Symbol.Kind == SymbolKind.Property)
                    {
                        switch (expression.OperatorToken.RawKind)
                        {
                            case (int) SyntaxKind.MinusMinusToken:
                                var refactored = SyntaxFactory.BinaryExpression(SyntaxKind.SimpleAssignmentExpression,
                                    expression.Operand,
                                    SyntaxFactory.BinaryExpression(SyntaxKind.SubtractExpression, expression.Operand,
                                        SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression,
                                            SyntaxFactory.Literal(1)))
                                    ).NormalizeWhitespace();
                            {
                                visit = refactored;
                                return true;
                            }
                            case (int) SyntaxKind.PlusPlusToken:
                                var refactored1 = SyntaxFactory.BinaryExpression(SyntaxKind.SimpleAssignmentExpression,
                                    expression.Operand,
                                    SyntaxFactory.BinaryExpression(SyntaxKind.AddExpression, expression.Operand,
                                        SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression,
                                            SyntaxFactory.Literal(1)))
                                    ).NormalizeWhitespace();
                            {
                                visit = refactored1;
                                return true;
                            }
                            default:
                                throw new Exception("No support for " + expression.OperatorToken.RawKind + " at " +
                                                    Utility.Descriptor(expression));
                        }
                    }
                }
            }
            if (node is PrefixUnaryExpressionSyntax)
            {
                var expression = node as PrefixUnaryExpressionSyntax;
                if (expression.Operand is MemberAccessExpressionSyntax)
                {
                    var memberAccess = expression.Operand as MemberAccessExpressionSyntax;
                    var typeInfo = TypeProcessor.GetSymbolInfo(memberAccess.Name);

                    if (typeInfo.Symbol.Kind == SymbolKind.Property)
                    {
                        switch (expression.OperatorToken.RawKind)
                        {
                            case (int) SyntaxKind.MinusMinusToken:
                                var refactored = SyntaxFactory.BinaryExpression(SyntaxKind.SimpleAssignmentExpression,
                                    expression.Operand,
                                    SyntaxFactory.BinaryExpression(SyntaxKind.SubtractExpression, expression.Operand,
                                        SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression,
                                            SyntaxFactory.Literal(1)))
                                    ).NormalizeWhitespace();
                            {
                                visit = refactored;
                                return true;
                            }
                            case (int) SyntaxKind.PlusPlusToken:
                                var refactored1 = SyntaxFactory.BinaryExpression(SyntaxKind.SimpleAssignmentExpression,
                                    expression.Operand,
                                    SyntaxFactory.BinaryExpression(SyntaxKind.AddExpression, expression.Operand,
                                        SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression,
                                            SyntaxFactory.Literal(1)))
                                    ).NormalizeWhitespace();
                            {
                                visit = refactored1;
                                return true;
                            }
                            default:
                                throw new Exception("No support for " + expression.OperatorToken.RawKind + " at " +
                                                    Utility.Descriptor(expression));
                        }
                    }
                }
            }
            visit = null;
            return false;
        }
    }
}