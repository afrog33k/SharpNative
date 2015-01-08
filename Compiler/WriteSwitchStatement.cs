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
          //  writer.WriteIndent();
            var isStringSwitch = false;
            var symbol = TypeProcessor.GetTypeInfo(switchStatement.Expression);

            if (symbol.Type.SpecialType == SpecialType.System_String)
                isStringSwitch = true;

            writer.WriteLine("switch("+ Core.WriteString(switchStatement.Expression) + (isStringSwitch ? ".Text" : "")+")");

            writer.OpenBrace();

            //First process all blocks except the section with the default block
            foreach (
                var section in
                    switchStatement.Sections.Where(
                        o => o.Labels.None(z => z.Keyword.RawKind == (decimal) SyntaxKind.DefaultKeyword)))
            {

                foreach (var label in section.Labels)
                {
                    writer.WriteIndent();
                    WriteLabel.Go(writer, (CaseSwitchLabelSyntax) label, isStringSwitch);
                }
          
                writer.OpenBrace(false);

                foreach (var statement in section.Statements)
                {
                    if (!(statement is BreakStatementSyntax))
                        writer.Write(Core.WriteString(statement, false, writer.Indent+2));
                }

                writer.WriteLine("break;\r\n");
                writer.CloseBrace(false);
            }

            //Now write the default section
            var defaultSection =
                switchStatement.Sections.SingleOrDefault(
                    o => o.Labels.Any(z => z.Keyword.RawKind == (decimal) SyntaxKind.DefaultKeyword));
            if (defaultSection != null)
            {
 

                writer.WriteLine("default:");
                writer.OpenBrace(false);
                foreach (var statement in defaultSection.Statements)
                {
                    if (!(statement is BreakStatementSyntax))
                        writer.Write(Core.WriteString(statement,false, writer.Indent+2));
                }
                writer.WriteLine("break;");
                writer.CloseBrace(false);
            }
            else
            {
                writer.WriteLine("default:");
                    writer.WriteLine("break;");
            }

            writer.CloseBrace();
        }
    }
}