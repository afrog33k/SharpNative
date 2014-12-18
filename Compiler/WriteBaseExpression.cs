// /*
//   SharpNative - C# to D Transpiler
//   (C) 2014 Irio Systems 
// */

#region Imports

using Microsoft.CodeAnalysis.CSharp.Syntax;

#endregion

namespace SharpNative.Compiler
{
    internal static class WriteBaseExpression
    {
        public static void Go(OutputWriter writer, BaseExpressionSyntax expression)
        {
            var baseType = TypeProcessor.GetTypeInfo(expression).Type;

            writer.Write("super");
        }
    }
}