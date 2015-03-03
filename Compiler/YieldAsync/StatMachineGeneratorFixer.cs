using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SharpNative.Compiler.YieldAsync
{
    public class StatMachineGeneratorFixer : CSharpSyntaxRewriter
    {
        private Compilation compilation;
        private SyntaxTree syntaxTree;
        private SemanticModel semanticModel;
        private string enclosingTypeName;

        public StatMachineGeneratorFixer(Compilation compilation, SyntaxTree syntaxTree, SemanticModel semanticModel, string enclosingTypeName)
        {
            this.compilation = compilation;
            this.syntaxTree = syntaxTree;
            this.semanticModel = semanticModel;
            this.enclosingTypeName = enclosingTypeName;
        }

        public override SyntaxNode VisitGenericName(GenericNameSyntax node)
        {
            if (node.ToString().Contains("Request"))
                Console.WriteLine();

            if (!(node.Parent is MemberAccessExpressionSyntax) || ((MemberAccessExpressionSyntax)node.Parent).Expression == node)
            {
                if (node.GetContainingMethod() == null)
                {
                    return base.VisitGenericName(node);
                }

                var containingType = node.GetContainingType();
                if (containingType == null || !containingType.Name.StartsWith(enclosingTypeName))
                    return node;

                var symbol = semanticModel.GetSymbolInfo(node).Symbol;
                if (symbol == null || (new[] { SymbolKind.Field, SymbolKind.Event, SymbolKind.Method, SymbolKind.Property }.Contains(symbol.Kind) && !symbol.ContainingType.Name.StartsWith(enclosingTypeName) && !symbol.IsStatic))
                {
                    return SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, SyntaxFactory.IdentifierName("_this"), node);
                }
            }
            return base.VisitGenericName(node);
        }

        public override SyntaxNode VisitIdentifierName(IdentifierNameSyntax node)
        {
            if (!(node.Parent is MemberAccessExpressionSyntax) || ((MemberAccessExpressionSyntax)node.Parent).Expression == node)
            {
                if (node.GetContainingMethod() == null)
                {
                    return base.VisitIdentifierName(node);
                }

                var containingType = node.GetContainingType();
                if (containingType == null || !containingType.Name.StartsWith(enclosingTypeName))
                    return node;

                var symbol = semanticModel.GetSymbolInfo(node).Symbol;
                var isObjectInitializer = node.Parent != null && node.Parent.Parent is InitializerExpressionSyntax;
                if (!isObjectInitializer)
                {
                    if (symbol == null || (new[] { SymbolKind.Field, SymbolKind.Event, SymbolKind.Method, SymbolKind.Property }.Contains(symbol.Kind) && !symbol.ContainingType.Name.StartsWith(enclosingTypeName) && !symbol.IsStatic))
                    {
                        return SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, SyntaxFactory.IdentifierName("_this"), node);
                    }
                }
            }
            return base.VisitIdentifierName(node);
        }

        /*
                public override SyntaxNode VisitThisExpression(ThisExpressionSyntax node)
                {
                    var containingType = node.GetContainingType();
                    if (containingType == null || !containingType.Name.StartsWith("YieldEnumerator_"))
                        return node;

                    var symbol = semanticModel.GetSymbolInfo(node).Symbol;
                    if (symbol == null)
                    {
                        return Syntax.IdentifierName("_this");
                    }

                    return base.VisitThisExpression(node);
                }
        */
    }
}