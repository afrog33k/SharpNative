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
            var isEnumSwitch = false;

            var symbol = TypeProcessor.GetTypeInfo(switchStatement.Expression);

            if (symbol.Type.SpecialType == SpecialType.System_String)
                isStringSwitch = true;

            if (symbol.Type.TypeKind == TypeKind.Enum)
                isEnumSwitch = true;

            if (!(switchStatement.Expression is LiteralExpressionSyntax))
				writer.WriteLine ("switch(" + Core.WriteString (switchStatement.Expression) + (isStringSwitch ? ".Hash" : "") + (isEnumSwitch ? ".__Value" : "") + ")");
			else
			{
				var typeInfo = TypeProcessor.GetTypeInfo(switchStatement.Expression);
				if (typeInfo.Type.SpecialType == SpecialType.System_String)
				{
					writer.WriteLine ("switch(");
					WriteLiteralExpression.Go (writer, (LiteralExpressionSyntax)switchStatement.Expression, true, true);
					writer.WriteLine ((isStringSwitch ? ".Hash" : "") + ")");

				}
				else
				{
					writer.WriteLine ("switch(" + Core.WriteString (switchStatement.Expression) + (isStringSwitch ? ".Hash" : "") + (isEnumSwitch ? ".__Value" : "") + ")");

				}
			}
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
 

				foreach (var label in defaultSection.Labels) // Could be more than one label :P
				{
					writer.WriteIndent();
					if (label is CaseSwitchLabelSyntax)
						WriteLabel.Go (writer, (CaseSwitchLabelSyntax)label, isStringSwitch);
					else
						writer.WriteLine (label.ToFullString().Trim());
				}

               // writer.WriteLine("default:");
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