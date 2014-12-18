// /*
//   SharpNative - C# to D Transpiler
//   (C) 2014 Irio Systems 
// */

#region Imports

using Microsoft.CodeAnalysis.CSharp.Syntax;

#endregion

namespace SharpNative.Compiler
{
    internal static class WriteUsingStatement
    {
        public static void Go(OutputWriter writer, UsingStatementSyntax usingStatement)
        {
            var expression = usingStatement.Expression;

            //Ensure the using statement is a local variable - we can't deal with things we can't reliably repeat in the finally block
            var resource = Utility.TryGetIdentifier(expression);
//            if (resource == null)
//                throw new Exception("Using statements must reference a local variable. " + Utility.Descriptor(usingStatement));

            writer.WriteLine("try");
            Core.WriteStatementAsBlock(writer, usingStatement.Statement);
            writer.WriteLine("finally");
            writer.OpenBrace();
            writer.WriteLine(resource + ".IDisposable_Dispose();");
            writer.CloseBrace();
        }
    }
}