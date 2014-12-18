// /*
//   SharpNative - C# to D Transpiler
//   (C) 2014 Irio Systems 
// */

#region Imports

using Microsoft.CodeAnalysis.CSharp.Syntax;

#endregion

namespace SharpNative.Compiler
{
    internal static class WriteIfStatement
    {
        public static void Go(OutputWriter writer, IfStatementSyntax ifStatement, bool indent = true)
        {
            //  writer.WriteLine();

            var tempWriter = new TempWriter();
            if (indent)
                tempWriter.WriteIndent();
            tempWriter.Write("if (");
            Core.Write(tempWriter, ifStatement.Condition);
            tempWriter.Write(")");

            writer.WriteLine(tempWriter.ToString());

            Core.WriteStatementAsBlock(writer, ifStatement.Statement);

            if (ifStatement.Else == null)
                return;

            writer.WriteIndent();
            writer.Write("else");

            if (ifStatement.Else.Statement is BlockSyntax)
            {
                writer.Write("\r\n");
                Core.WriteBlock(writer, ifStatement.Else.Statement.As<BlockSyntax>());
            }
            else if (ifStatement.Else.Statement is IfStatementSyntax)
            {
                writer.Write(" ");
                Go(writer, ifStatement.Else.Statement.As<IfStatementSyntax>(), false);
            }
            else
            {
                writer.Write("\r\n");
                Core.WriteStatementAsBlock(writer, ifStatement.Else.Statement);
            }
        }
    }
}