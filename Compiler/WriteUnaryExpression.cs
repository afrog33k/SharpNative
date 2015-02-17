﻿// /*
//   SharpNative - C# to D Transpiler
//   (C) 2014 Irio Systems 
// */
using Microsoft.CodeAnalysis;

#region Imports

using System;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

#endregion

namespace SharpNative.Compiler
{
    internal static class WriteUnaryExpression
    {
        public static void WritePrefix(OutputWriter writer, PrefixUnaryExpressionSyntax expression)
        {
			var isProperty = false;

			var symbol = TypeProcessor.GetSymbolInfo (expression.Operand);
			if (symbol.Symbol is IPropertySymbol)
				isProperty = true;


			if (isProperty)
			{
				var symbolName = expression.ToString ().Trim ().RemoveFromStartOfString (expression.OperatorToken.ValueText);
				switch (expression.OperatorToken.RawKind)
				{
				case (int) SyntaxKind.MinusMinusToken:
					if((symbol.Symbol as IPropertySymbol).IsIndexer)
						writer.Write (String.Format ("/*--{0}*/((){{ auto v = {0};{0}=(--v);return v;}})()", symbolName));
					else
						writer.Write (String.Format ("/*--{0}*/((){{ auto v = {0};{0}(--v);return v;}})()", symbolName));
					break;
				case (int) SyntaxKind.PlusPlusToken:
					if((symbol.Symbol as IPropertySymbol).IsIndexer)
						writer.Write (String.Format ("/*++{0}*/((){{ auto v = {0};{0}=(++v);return v;}})()", symbolName));
					else
						writer.Write (String.Format ("/*++{0}*/((){{ auto v = {0};{0}(++v);return v;}})()", symbolName));
						
					break;
				default:
					Core.Write (writer, expression.Operand);
					writer.Write (expression.OperatorToken.ValueText);
					break;
				}
			}
			else
			{
//			Console.WriteLine (expression.ToFullString());
				if (expression.OperatorToken.RawKind == (decimal)SyntaxKind.MinusMinusToken)
				{

					writer.Write ("--");
					Core.Write (writer, expression.Operand);
				}
				else if (expression.OperatorToken.RawKind == (decimal)SyntaxKind.PlusPlusToken)
				{
					writer.Write ("++");
					Core.Write (writer, expression.Operand);
				}
				else
				{
					//TODO: cannot take addresses of structs in 32/64-bit mode and subtract them ... really weird d-bug ... leads to wrong math ... should we do a shift ?
//				if (expression.OperatorToken.CSharpKind () == SyntaxKind.AmpersandToken) // Take address
//				{
//					var memberAccess = expression.Operand as MemberAccessExpressionSyntax;
//					var simpleName = expression.Operand as NameSyntax;
//
//					TypeInfo typeOperand;
//
//					if (memberAccess != null)
//						typeOperand = TypeProcessor.GetTypeInfo (memberAccess.Expression);
//					if (simpleName != null)
//						typeOperand = TypeProcessor.GetTypeInfo (simpleName);
//
//					var failed = true;

//					if (memberAccess != null)
//					{
//						 failed = false;
//						if (typeOperand.Type.TypeKind == TypeKind.Struct)
//						{
//							var sNAme = (memberAccess.Expression as SimpleNameSyntax).Identifier;
//							
//							writer.Write ("(cast(ulong)(&" + sNAme.ToFullString () + ") + (");
//							Core.Write (writer, expression.Operand);
//							writer.Write (".offsetof))");
//						}
//						else
//							failed = true;
//					}
//					else if (simpleName != null)
//					{
//						failed = false;
//
//						if (typeOperand.Type.TypeKind == TypeKind.Struct)
//						{
//							writer.Write ("(&" + simpleName.ToFullString () + " + (");
//							Core.Write (writer, expression.Operand);
//							writer.Write (".offsetof))");
//						}
//						else
//							failed = true;
//					}
//
//					if(failed)
//					{
//						writer.Write (expression.OperatorToken.ToString ());
//						Core.Write (writer, expression.Operand);
//					}
//
//				}
//				else
					{
						writer.Write (expression.OperatorToken.ToString ());
						Core.Write (writer, expression.Operand);
					}
				}
			}
        }

        public static void WritePostfix(OutputWriter writer, PostfixUnaryExpressionSyntax expression)
        {
			var isProperty = false;

			var symbol = TypeProcessor.GetSymbolInfo (expression.Operand);
			if (symbol.Symbol is IPropertySymbol)
				isProperty = true;


			if (isProperty)
			{
				var symbolName = expression.ToString().Trim().RemoveFromEndOfString(expression.OperatorToken.ValueText);
				switch (expression.OperatorToken.RawKind)
				{
				case (int) SyntaxKind.MinusMinusToken:
					if((symbol.Symbol as IPropertySymbol).IsIndexer)
						writer.Write (String.Format("/*{0}--*/((){{auto v={0};auto y=v;{0}=(--y);return v;}})()",symbolName));
					else
						writer.Write (String.Format("/*{0}--*/((){{auto v={0};auto y=v;{0}(--y);return v;}})()",symbolName));
						
					break;
				case (int) SyntaxKind.PlusPlusToken:
					if((symbol.Symbol as IPropertySymbol).IsIndexer)
						writer.Write (String.Format("/*{0}++*/((){{auto v={0},y={0};{0}=(++y);return v;}})()",symbolName));
					else
						writer.Write (String.Format("/*{0}++*/((){{auto v={0},y={0};{0}(++y);return v;}})()",symbolName));
						
					break;
				default:
					Core.Write (writer, expression.Operand);
					writer.Write (expression.OperatorToken.ValueText);
					break;
				}
			}
			else
			{
//            if (expression.Operand is MemberAccessExpressionSyntax)
//            {
//                var memberAccess = expression.Operand as MemberAccessExpressionSyntax;
//                var typeInfo = TypeProcessor.GetSymbolInfo(memberAccess.Name);

//                if (typeInfo.Symbol.Kind == SymbolKind.Property)
//                {
//                    switch (expression.OperatorToken.RawKind)
//                    {
//                        case (int)SyntaxKind.MinusMinusToken:
//                          var refactored=  SyntaxFactory.BinaryExpression(SyntaxKind.SimpleAssignmentExpression,
//                                expression.Operand,
//                                SyntaxFactory.BinaryExpression(SyntaxKind.AddExpression, expression.Operand,
//                                    SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression,SyntaxFactory.Literal(-1)))
//                                ).NormalizeWhitespace();
//                            Core.Write(writer,refactored);
//                            break;
//                        case (int)SyntaxKind.PlusPlusToken:
//                            var refactored1 = SyntaxFactory.BinaryExpression(SyntaxKind.SimpleAssignmentExpression,
//                                 expression.Operand,
//                                 SyntaxFactory.BinaryExpression(SyntaxKind.AddExpression, expression.Operand,
//                                     SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(+1)))
//                                 ).NormalizeWhitespace();
//                            Core.Write(writer, refactored1);
//                            break;
//                        default:
//                            throw new Exception("No support for " + expression.OperatorToken.RawKind + " at " +
//                                                Utility.Descriptor(expression));
//                    }
//                }
//                else
//                {
//                    switch (expression.OperatorToken.RawKind)
//                    {
//                        case (int)SyntaxKind.MinusMinusToken:
//                            Core.Write(writer, expression.Operand);
//                            writer.Write("--");
//                            break;
//                        case (int)SyntaxKind.PlusPlusToken:
//                            Core.Write(writer, expression.Operand);
//                            writer.Write("++");
//                            break;
//                        default:
//                            throw new Exception("No support for " + expression.OperatorToken.RawKind + " at " +
//                                                Utility.Descriptor(expression));
//                    }
//                }
//            
//            }
//            else
//            {
				switch (expression.OperatorToken.RawKind)
				{
				case (int) SyntaxKind.MinusMinusToken:

					Core.Write (writer, expression.Operand);
					writer.Write ("--");
					break;
				case (int) SyntaxKind.PlusPlusToken:
					Core.Write (writer, expression.Operand);
					writer.Write ("++");
					break;
				default:
					throw new Exception ("No support for " + expression.OperatorToken.RawKind + " at " +
					Utility.Descriptor (expression));
				}
			}
//            }
        }
    }
}