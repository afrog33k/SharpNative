// /*
//   SharpNative - C# to D Transpiler
//   (C) 2014 Irio Systems 
// */

#region Imports

using Microsoft.CodeAnalysis.CSharp.Syntax;

#endregion

namespace SharpNative.Compiler
{
    internal static class WriteContinueStatement
    {
        public static void Go(OutputWriter writer, ContinueStatementSyntax statement)
        {
            writer.WriteLine("continue;");
        }
    }
}