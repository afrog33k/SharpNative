﻿// /*
//   SharpNative - C# to D Transpiler
//   (C) 2014 Irio Systems 
// */

#region Imports

using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

#endregion

namespace SharpNative.Compiler
{
    internal static class WriteReturnStatement
    {
        public static void Go(OutputWriter writer, ReturnStatementSyntax statement)
        {

            var method = statement.Ancestors().OfType<MethodDeclarationSyntax>().FirstOrDefault();
            var property = statement.Ancestors().OfType<PropertyDeclarationSyntax>().FirstOrDefault();
            var indexer = statement.Ancestors().OfType<IndexerDeclarationSyntax>().FirstOrDefault();
            var converter = statement.Ancestors().OfType<ConversionOperatorDeclarationSyntax>().FirstOrDefault();
            var @operator = statement.Ancestors().OfType<OperatorDeclarationSyntax>().FirstOrDefault();

            ITypeSymbol returnTypeSymbol = null;

            if (method != null)
            {
                returnTypeSymbol = TypeProcessor.GetTypeInfo(method.ReturnType).Type;
            }

            if (property != null)
            {
                returnTypeSymbol = TypeProcessor.GetTypeInfo(property.Type).Type;
            }

            if (indexer != null)
            {
                returnTypeSymbol = TypeProcessor.GetTypeInfo(indexer.Type).Type;
            }

            if (converter != null)
            {
                returnTypeSymbol = TypeProcessor.GetTypeInfo(converter.Type).Type;
            }


            if (@operator != null)
            {
                returnTypeSymbol = TypeProcessor.GetTypeInfo(@operator.ReturnType).Type;
            }
           


            writer.WriteIndent();
            writer.Write("return");

            if (statement.Expression != null)
            {
                {

                    var rightExpressionType = TypeProcessor.GetTypeInfo(statement.Expression);

                    var boxRight = rightExpressionType.ConvertedType != null &&
                                   (rightExpressionType.Type != null &&
                                    ((rightExpressionType.Type.IsValueType ||
                                      rightExpressionType.Type.TypeKind == TypeKind.TypeParameter) &&
                                     (rightExpressionType.ConvertedType.IsReferenceType)));
                    boxRight = boxRight && (rightExpressionType.Type != returnTypeSymbol);

                    if (!Equals(returnTypeSymbol, rightExpressionType.Type))
                    {
                        writer.Write(" cast(" + TypeProcessor.ConvertType(returnTypeSymbol) + ")");
                    }

                   writer.Write (boxRight ? " BOX!(" + TypeProcessor.ConvertType(rightExpressionType.Type) + ")(" : " ");
                    Core.Write(writer, statement.Expression);

                    writer.Write(boxRight ? ")" : "");

                }
                

            }
            writer.Write(";\r\n");
        }
    }
}
