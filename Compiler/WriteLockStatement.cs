// /*
//   SharpNative - C# to D Transpiler
//   (C) 2014 Irio Systems 
// */

#region Imports

using Microsoft.CodeAnalysis.CSharp.Syntax;

#endregion

namespace SharpNative.Compiler
{
    internal static class WriteLockStatement
    {
        public static void Go(OutputWriter writer, LockStatementSyntax statement)
        {
//            if (statement.DescendantNodes().OfType<ReturnStatementSyntax>().Any())
//                throw new Exception("Cannot return from within a lock statement " + Utility.Descriptor(statement));
            //synchronized
            writer.WriteIndent();
            writer.Write("synchronized("); //All d object implement a lock
            Core.Write(writer, statement.Expression);
            writer.Write(")"); //, () =>\r\n");
            writer.OpenBrace();
            Core.WriteStatementAsBlock(writer, statement.Statement, false);
            writer.Indent--;
            writer.WriteIndent();
            writer.CloseBrace();
            writer.WriteLine();
//            writer.Write("};\r\n");
        }
    }
}