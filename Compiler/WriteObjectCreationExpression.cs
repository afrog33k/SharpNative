// /*
//   SharpNative - C# to D Transpiler
//   (C) 2014 Irio Systems 
// */

#region Imports

using System.Collections.Generic;
using System.Linq;
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
                writer.Write("(");
                Core.Write(writer, expression.Initializer);
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
                var methodSymbol = TypeProcessor.GetSymbolInfo(expression).Symbol.As<IMethodSymbol>();

                var typeString = TypeProcessor.ConvertType(expression.Type);

                WriteNewOperator(writer, type, typeString);

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

        private static IEnumerable<TransformedArgument> TranslateParameters(IEnumerable<ArgumentSyntax> list)
        {
            return list.Select(o => new TransformedArgument(o));
        }
    }
}