// /*
//   SharpNative - C# to D Transpiler
//   (C) 2014 Irio Systems 
// */

#region Imports

using Microsoft.CodeAnalysis.CSharp.Syntax;

#endregion

namespace SharpNative.Compiler
{
    internal static class WriteDoStatement
    {
        public static void Go(OutputWriter writer, DoStatementSyntax statement)
        {
            var info = new LoopInfo(statement);

            writer.WriteLine("do");
            writer.OpenBrace();
            Core.WriteStatementAsBlock(writer, statement.Statement, false);
            writer.CloseBrace();

            writer.WriteIndent();
            writer.Write("while (");
            Core.Write(writer, statement.Condition);
            writer.Write(");\r\n");
        }
    }
}