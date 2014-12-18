// /*
//   SharpNative - C# to D Transpiler
//   (C) 2014 Irio Systems 
// */

#region Imports

using Microsoft.CodeAnalysis.CSharp.Syntax;

#endregion

namespace SharpNative.Compiler
{
    internal static class WriteReturnStatement
    {
        public static void Go(OutputWriter writer, ReturnStatementSyntax statement)
        {
            if (Context.Instance.InLambdaBreakable > 0)
            {
                if (statement.Expression != null)
                {
                    writer.WriteIndent();
                    writer.Write("__lambdareturn = ");
                    Core.Write(writer, statement.Expression);
                    writer.Write(";\r\n");
                }

                writer.WriteLine("__lambdabreak.break();");
            }
            else
            {
                writer.WriteIndent();
                writer.Write("return");

                if (statement.Expression != null)
                {
                    writer.Write(" (");
                    Core.Write(writer, statement.Expression);
                    writer.Write(")");
                }
                writer.Write(";\r\n");
            }
        }
    }
}