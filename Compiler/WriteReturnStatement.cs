// /*
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
            ITypeSymbol returnTypeSymbol = null;

            if (method != null)
            {
                returnTypeSymbol = TypeProcessor.GetTypeInfo(method.ReturnType).Type;
            }

            if (property != null)
            {
                returnTypeSymbol = TypeProcessor.GetTypeInfo(property.Type).Type;
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

                    writer.Write(boxRight ? " BOX!(" + TypeProcessor.ConvertType(rightExpressionType.Type) + ")(" : " ");
                    Core.Write(writer, statement.Expression);

                    writer.Write(boxRight ? ")" : "");

                }
                

            }
            writer.Write(";\r\n");
        }
    }
}
