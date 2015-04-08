// /*
//   SharpNative - C# to D Transpiler
//   (C) 2014 Irio Systems 
// */

#region Imports

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

#endregion

namespace SharpNative.Compiler
{
    internal static class WriteGoto
    {
        public static void Go(OutputWriter writer, GotoStatementSyntax method)
        {
            writer.WriteIndent();
            writer.Write("goto ");
            writer.Write((method.CaseOrDefaultKeyword != default(SyntaxToken))
                ? method.CaseOrDefaultKeyword.Text
                : "");
            writer.Write(" ");

            if (method.Expression != null)
			if (method.Expression.ToString ().Trim() == "null")
				writer.Write("-1");
			else
                Core.Write(writer, method.Expression);
            writer.Write(";\r\n");
        }
    }
}