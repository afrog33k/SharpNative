// /*
//   SharpNative - C# to D Transpiler
//   (C) 2014 Irio Systems 
// */

#region Imports

using Microsoft.CodeAnalysis.CSharp.Syntax;

#endregion

namespace SharpNative.Compiler
{
    internal static class WriteConditionalExpression
    {
        public static void Go(OutputWriter writer, ConditionalExpressionSyntax expression)
        {
            writer.Write("(");
            Core.Write(writer, expression.Condition);
            writer.Write(") ? (");
            Core.Write(writer, expression.WhenTrue);
            writer.Write(") : (");
            Core.Write(writer, expression.WhenFalse);
            writer.Write(")");
        }
    }
}