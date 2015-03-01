// /*
//   SharpNative - C# to D Transpiler
//   (C) 2014 Irio Systems 
// */

#region Imports

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

#endregion

namespace SharpNative.Compiler
{
    internal static class WriteObjectCreationExpression
    {
        public static void Go(OutputWriter writer, ObjectCreationExpressionSyntax expression)
        {
            var type = TypeProcessor.GetTypeInfo(expression).Type;

            if (expression.Initializer != null)
            {
                if (expression.Initializer.IsKind(SyntaxKind.CollectionInitializerExpression))
                {
                    var isPtr = type.IsPointer() ? "" : "";
                    var typeString = TypeProcessor.ConvertType(type); // + isPtr + " ";

                    WriteNewOperator(writer, type, typeString);

                    writer.Write("(");
                    var intializer = expression.Initializer;
                    intializer.WriteArrayInitializer(writer, expression.Type);

                    writer.Write(")");

                    return;
                }

                if (expression.Initializer.IsKind(SyntaxKind.ObjectInitializerExpression))
                {
                    var tempName = "__tempVar";
                    var typeString = TypeProcessor.ConvertType(type); 

                    writer.Write(string.Format("(){{auto {0} = {1}();", tempName,WriteNewOperatorToString(type, typeString)));
                    foreach (var iexpression in expression.Initializer.Expressions)
                    {
                        var localExpression = (iexpression as AssignmentExpressionSyntax);
                        if (localExpression == null)
                            continue;
                        var right = localExpression;
                        var mar = right;
                        writer.Write(tempName + "." + Core.WriteString(mar) + ";");

                    }

                    writer.Write("return " + tempName + ";");
                    writer.Write("}()");
                    return;
                }
                else
                {
                      writer.Write("(");
                Core.Write(writer, expression.Initializer);
                writer.Write(")");
                }
              
            }

            if (type.SpecialType == SpecialType.System_Object)
            {
                //new object() results in the NObject type being made.  This is only really useful for locking
                writer.Write("new NObject()");
            }
            else if (type.OriginalDefinition is INamedTypeSymbol &&
                     type.OriginalDefinition.As<INamedTypeSymbol>().SpecialType == SpecialType.System_Nullable_T)
            {
                //new'ing up a Nullable<T> has special sematics in C#.  If we're calling this with no parameters, just use null. Otherwise just use the parameter.
                if (expression.ArgumentList.Arguments.Count == 0)
                    writer.Write("null");
                else
                    Core.Write(writer, expression.ArgumentList.Arguments.Single().Expression);
            }
            else
            {
                //                var methodSymbol = TypeProcessor.GetSymbolInfo(expression).Symbol.As<IMethodSymbol>();

                var typeString = TypeProcessor.ConvertType(expression.Type);


                WriteNewOperator(writer, type, typeString);

                writer.Write("(");
                if (expression.ArgumentList != null)
                {
                    bool first = true;
                    foreach (var param in TranslateParameters(expression.ArgumentList.Arguments))
                    {
                        if (first)
                            first = false;
                        else
                            writer.Write(", ");

                        param.Write(writer);
                    }
                }

                writer.Write(")");
            }
        }

        private static void WriteNewOperator(OutputWriter writer, ITypeSymbol type, string typeString)
        {
            if (type != null && type.TypeKind == TypeKind.TypeParameter)
                writer.Write(string.Format(" {0}!(", InternalNames.TypeNewName) + typeString + ")");
            else if (type.IsValueType)
                writer.Write(string.Format("  {0}", typeString));
            else
                writer.Write(string.Format(" new {0}", typeString));
        }

        private static string WriteNewOperatorToString(ITypeSymbol type, string typeString)
        {
            var sb = new StringBuilder();
            if (type != null && type.TypeKind == TypeKind.TypeParameter)
                sb.Append(string.Format(" {0}!(", InternalNames.TypeNewName) + typeString + ")");
            else if (type.IsValueType)
                sb.Append(string.Format("  {0}", typeString));
            else
                sb.Append(string.Format(" new {0}", typeString));
            return sb.ToString();
        }

        private static IEnumerable<TransformedArgument> TranslateParameters(IEnumerable<ArgumentSyntax> list)
        {
            return list.Select(o => new TransformedArgument(o));
        }
    }
}