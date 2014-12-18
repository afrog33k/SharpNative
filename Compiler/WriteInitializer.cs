// /*
//   SharpNative - C# to D Transpiler
//   (C) 2014 Irio Systems 
// */

#region Imports

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using CSharpExtensions = Microsoft.CodeAnalysis.CSharpExtensions;

#endregion

namespace SharpNative.Compiler
{
    internal static class WriteInitializer
    {
        public static void Go(OutputWriter writer, InitializerExpressionSyntax initializer)
        {
            writer.WriteIndent();
            var isCollectionInit = false;
            if (CSharpExtensions.CSharpKind(initializer) == SyntaxKind.CollectionInitializerExpression ||
                CSharpExtensions.CSharpKind(initializer) == SyntaxKind.ArrayInitializerExpression)
            {
                var tx = TypeProcessor.GetTypeInfo(initializer);

                var t = tx.Type;
                if (t == null)
                    t = tx.ConvertedType;
                if (t != null) // Initializer within initializer
                {
                    var elementType = t.As<IArrayTypeSymbol>().ElementType;
                    var ptr = !elementType.IsValueType; // ? "" : "";
                    var type = TypeProcessor.ConvertType(elementType);
                    var typeString = "Array_T!(" + type + ")";

                    if (elementType.TypeKind == TypeKind.TypeParameter)
                        writer.Write(" __TypeNew!(" + typeString + ")(");
                    else
                        writer.Write("new " + typeString + "(");
                }
                var variableDeclarationSyntax = initializer.Parent.Parent.Parent as VariableDeclarationSyntax;
                if (variableDeclarationSyntax != null)
                {
                    var atype = variableDeclarationSyntax.Type;
                    initializer.WriteArrayInitializer(writer, atype);
                }
                else
                    initializer.WriteArrayInitializer(writer);
                if (t != null)
                    writer.Write(")");
            }
            else
            {
                //            writer.Write("goto ");
                //            foreach (var expressionSyntax in method.Expressions)
                //            {
                //                Core.Write(writer, expressionSyntax);
                //            }

                bool first = true;
                foreach (var expression in initializer.Expressions)
                {
                    if (first)
                        first = false;
                    else
                        writer.Write(", ");

                    Core.Write(writer, expression);
                }
            }

            //            writer.Write(";");
        }
    }
}