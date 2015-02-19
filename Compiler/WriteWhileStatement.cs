// /*
//   SharpNative - C# to D Transpiler
//   (C) 2014 Irio Systems 
// */

#region Imports

using Microsoft.CodeAnalysis.CSharp.Syntax;

#endregion

namespace SharpNative.Compiler
{
    internal static class WriteWhileStatement
    {
        public static void Go(OutputWriter writer, WhileStatementSyntax whileStatement)
        {
            var info = new LoopInfo(whileStatement);

            
            writer.WriteIndent();
            writer.Write("while (");
            Core.Write(writer, whileStatement.Condition);
            writer.Write(")\r\n");

            writer.OpenBrace();
            Core.WriteStatementAsBlock(writer, whileStatement.Statement, false);
            writer.CloseBrace();
        }
    }
}