// /*
//   SharpNative - C# to D Transpiler
//   (C) 2014 Irio Systems 
// */

#region Imports

using Microsoft.CodeAnalysis.CSharp.Syntax;

#endregion

namespace SharpNative.Compiler
{
    internal static class WriteThisExpression
    {
        public static void Go(OutputWriter writer, ThisExpressionSyntax expression)
        {
            writer.Write("this");
        }
    }
}