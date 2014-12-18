// /*
//   SharpNative - C# to D Transpiler
//   (C) 2014 Irio Systems 
// */

#region Imports

using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

#endregion

namespace SharpNative.Compiler
{
    internal static class WriteSwitchStatement
    {
        public static void Go(OutputWriter writer, SwitchStatementSyntax switchStatement)
        {
            writer.WriteIndent();
            var isStringSwitch = false;
            var symbol = TypeProcessor.GetTypeInfo(switchStatement.Expression);

            if (symbol.Type.SpecialType == SpecialType.System_String)
                isStringSwitch = true;

            writer.Write("switch( ");
            Core.Write(writer, switchStatement.Expression);

            writer.Write(isStringSwitch ? ".Text" : "");

            writer.Write(" )\n");
            writer.OpenBrace();

            //First process all blocks except the section with the default block
            foreach (
                var section in
                    switchStatement.Sections.Where(
                        o => o.Labels.None(z => z.Keyword.RawKind == (decimal) SyntaxKind.DefaultKeyword)))
            {
                writer.WriteIndent();

                foreach (var label in section.Labels)
                {
                    //Core.Write(writer, label, true);
                    WriteLabel.Go(writer, (CaseSwitchLabelSyntax) label, isStringSwitch);
                }
                //                writer.Write(" :\r\n");
                writer.Indent++;

                foreach (var statement in section.Statements)
                {
                    if (!(statement is BreakStatementSyntax))
                        Core.Write(writer, statement);
                }

                writer.WriteLine("break;");
                writer.Indent--;
            }

            //Now write the default section
            var defaultSection =
                switchStatement.Sections.SingleOrDefault(
                    o => o.Labels.Any(z => z.Keyword.RawKind == (decimal) SyntaxKind.DefaultKeyword));
            if (defaultSection != null)
            {
                // if (defaultSection.Labels.Count > 1)
                //   throw new Exception("Cannot fall-through into or out of the default section of switch statement " + Utility.Descriptor(defaultSection));

                writer.WriteLine("default:\r\n");
                writer.Indent++;

                foreach (var statement in defaultSection.Statements)
                {
                    if (!(statement is BreakStatementSyntax))
                        Core.Write(writer, statement);
                }
                writer.WriteLine("break;");
                writer.Indent--;
            }
            else
                writer.WriteLine("default:\r\nbreak;\r\n");

            writer.CloseBrace();
        }
    }
}