// /*
//   SharpNative - C# to D Transpiler
//   (C) 2014 Irio Systems 
// */

#region Imports

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

#endregion

namespace SharpNative.Compiler
{
    internal static class WriteConstructorInitializer
    {
        public static void Go(OutputWriter writer, ConstructorInitializerSyntax method)
        {
            writer.WriteIndent();
            //    var symbl = TypeProcessor.GetSymbolInfo(method);
            //	var mysymbl = TypeProcessor.GetSymbolInfo(method.Parent);

            //   var className = symbl.Symbol.ContainingType;
            if (method.ThisOrBaseKeyword.RawKind == (int) SyntaxKind.ThisKeyword)
                writer.Write("this");
            else
                writer.Write("super");
//                writer.Write(TypeProcessor.ConvertType(className));

            writer.Write("(");
            bool first = true;
            foreach (var expression in method.ArgumentList.Arguments)
            {
                if (first)
                    first = false;
                else
                    writer.Write(", ");

                Core.Write(writer, expression.Expression);
            }
            writer.Write(")");
        }
    }
}