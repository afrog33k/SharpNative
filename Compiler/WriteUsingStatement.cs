// /*
//   SharpNative - C# to D Transpiler
//   (C) 2014 Irio Systems 
// */

#region Imports

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

#endregion

namespace SharpNative.Compiler
{
    static class WriteUsingStatement
    {
        public static void Go(OutputWriter writer, UsingStatementSyntax usingStatement)
        {
            var expression = usingStatement.Expression;

            writer.WriteLine("//using block ... " + usingStatement.Declaration);
            writer.OpenBrace();
            //Ensure the using statement is a local variable - we can't deal with things we can't reliably repeat in the finally block
            var resource = Utility.TryGetIdentifier(expression);
//            if (resource == null)
//                throw new Exception("Using statements must reference a local variable. " + Utility.Descriptor(usingStatement));

            var variables = new SeparatedSyntaxList<VariableDeclaratorSyntax>();//.Select(o => o.Identifier.ValueText);
            if (usingStatement.Declaration != null)
            {
                Core.Write(writer, usingStatement.Declaration);
                variables = usingStatement.Declaration.Variables;

            }

            writer.WriteLine("try");
            Core.WriteStatementAsBlock(writer, usingStatement.Statement);
            writer.WriteLine("finally");
            writer.OpenBrace();
            foreach (var variable in variables)
            {
                var typeInfo = TypeProcessor.GetTypeInfo(usingStatement.Declaration.Type);
                if (!typeInfo.Type.IsValueType)
                    writer.WriteLine("if(" + variable.Identifier.ValueText + " !is null)");
                else if (typeInfo.Type.Name == "Nullable")
                    writer.WriteLine("if(" + variable.Identifier.ValueText + ".HasValue)");


                writer.WriteLine(variable.Identifier.ValueText + ".Dispose(cast(IDisposable)null);");
            }
            if (resource != null)
            {
                writer.WriteLine("if(" + resource + " !is null)");
                writer.WriteLine(resource + ".Dispose(cast(IDisposable)null);");
            }
            writer.CloseBrace();
            writer.CloseBrace();
        }
    }
}