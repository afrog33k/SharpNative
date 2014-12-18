// /*
//   SharpNative - C# to D Transpiler
//   (C) 2014 Irio Systems 
// */

#region Imports

using Microsoft.CodeAnalysis.CSharp.Syntax;

#endregion

namespace SharpNative.Compiler
{
    internal static class WriteParenthesizedExpression
    {
        public static void Go(OutputWriter writer, ParenthesizedExpressionSyntax expression)
        {
            writer.Write("(");
            Core.Write(writer, expression.Expression);
            writer.Write(")");
        }
    }
}