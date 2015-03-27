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
            tempWriter.Indent = writer.Indent;

            var initializers = forStatement.Initializers;
            var initCount = initializers.Count + (forStatement.Declaration!=null?forStatement.Declaration.Variables.Count :0);
            if (initCount > 1)
            {
                tempWriter.WriteLine("//For loop");
                tempWriter.OpenBrace();
                foreach (var init in initializers)
                {
                    tempWriter.WriteLine(Core.WriteString(init, false, tempWriter.Indent) + ";");
                }

                if (forStatement.Declaration != null)
                {
                    var variables = forStatement.Declaration.Variables;
                    foreach (var variable in variables)
                    {
                        var vardec = (TypeProcessor.ConvertType(forStatement.Declaration.Type) + " ");
                        vardec +=(WriteIdentifierName.TransformIdentifier(variable.Identifier.ValueText));

                        if (variable.Initializer != null)
                        {
                            vardec+=(" = ");
                            vardec += Core.WriteString(variable.Initializer.Value,false,tempWriter.Indent);
                        }

                   
                        tempWriter.WriteLine(vardec + ";");

                    }
                }
               
            }
           
            tempWriter.Write("\r\n");
            tempWriter.WriteIndent();
            tempWriter.Write("for (");

            if (initCount == 1)
            {
                foreach (var init in initializers)
                {
                    Core.Write(tempWriter, init);
                    if (init != initializers.Last())
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

            if (initCount > 1)
            {
                writer.CloseBrace();
                writer.WriteLine("//End of for loop");
            }
            //  writer.WriteLine(); // Beautify the code ...
        }
    }
}