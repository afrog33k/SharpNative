// /*
//   SharpNative - C# to D Transpiler
//   (C) 2014 Irio Systems 
// */

#region Imports

using Microsoft.CodeAnalysis.CSharp.Syntax;

#endregion

namespace SharpNative.Compiler
{
    internal static class WriteForStatement
    {
        public static void Go(OutputWriter writer, ForStatementSyntax forStatement)
        {
            //   writer.WriteLine(); // Beautify the code ...

            var tempWriter = new TempWriter();
            tempWriter.WriteIndent();
            tempWriter.Write("for (");

            foreach (var init in forStatement.Initializers)
            {
                Core.Write(tempWriter, init);
                if (init != forStatement.Initializers.Last())
                    tempWriter.Write(",");
            }

            if (forStatement.Declaration != null)
            {
                var variables = forStatement.Declaration.Variables;
                foreach (var variable in variables)
                {
                    //                    writer.WriteIndent();
                    tempWriter.Write(TypeProcessor.ConvertType(forStatement.Declaration.Type) + " ");
                    tempWriter.Write(WriteIdentifierName.TransformIdentifier(variable.Identifier.ValueText));

                    if (variable.Initializer != null)
                    {
                        tempWriter.Write(" = ");
                        Core.Write(tempWriter, variable.Initializer.Value);
                    }

                    if (variable != variables.LastOrDefault())
                        tempWriter.Write(",");
                }
            }

            tempWriter.Write(";");

            if (forStatement.Condition == null)
                tempWriter.Write("true");
            else
                Core.Write(tempWriter, forStatement.Condition);

            tempWriter.Write(";");

            foreach (var iterator in forStatement.Incrementors)
            {
                Core.Write(tempWriter, iterator);

                if (iterator != forStatement.Incrementors.LastOrDefault())
                    tempWriter.Write(",");
            }

            tempWriter.Write(")");

            writer.WriteLine(tempWriter.ToString());
            writer.OpenBrace();
            Core.WriteStatementAsBlock(writer, forStatement.Statement, false);
            writer.CloseBrace();

            //  writer.WriteLine(); // Beautify the code ...
        }
    }
}