// /*
//   SharpNative - C# to D Transpiler
//   (C) 2014 Irio Systems 
// */

#region Imports

using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

#endregion

namespace SharpNative.Compiler
{
    internal static class WriteTryStatement
    {
        public static void Go(OutputWriter writer, TryStatementSyntax tryStatement)
        {
            writer.WriteLine("try");
            Core.Write(writer, tryStatement.Block);

            var catches = tryStatement.Catches.Where(o => Program.DoNotWrite.ContainsKey(o) == false).ToList();

            if (catches.Count > 0)
            {
                foreach (var catchClause in catches)
                {
                    if (catchClause.Declaration == null)
                        writer.WriteLine("catch(Exception __ex)");
                    else
                    {
                        writer.WriteLine("catch(" + TypeProcessor.ConvertType(catchClause.Declaration.Type) + " " +
                                         (string.IsNullOrWhiteSpace(catchClause.Declaration.Identifier.Text)
                                             ? "__ex"
                                             : WriteIdentifierName.TransformIdentifier(
                                                 catchClause.Declaration.Identifier.Text)) + ")");
                    }
                    writer.OpenBrace();

                    Core.WriteBlock(writer, catchClause.Block, false);
                    //                    foreach (var statement in catchClause.Block.Statements)
                    //                        Core.Write(writer, statement);
                    writer.CloseBrace();
                }
            }

            if (tryStatement.Finally != null)
            {
                writer.WriteLine("finally");
                //                Core.Write(writer, tryStatement.Finally.Block);

                writer.OpenBrace();

                Core.WriteBlock(writer, tryStatement.Finally.Block, false);
                //                    foreach (var statement in catchClause.Block.Statements)
                //                        Core.Write(writer, statement);
                writer.CloseBrace();
            }
        }
    }
}