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
            //All d objects implement a lock
            writer.WriteLine("synchronized(" + Core.WriteString(statement.Expression)+")");
            writer.OpenBrace();
            Core.WriteStatementAsBlock(writer, statement.Statement, false);
            writer.CloseBrace();
        }
    }
}