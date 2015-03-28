// /*
//   SharpNative - C# to D Transpiler
//   (C) 2014 Irio Systems 
// */

#region Imports

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

#endregion

namespace SharpNative.Compiler
{
    internal static class WriteConversionOperatorDeclaration
    {
        public static void Go(OutputWriter writer, ConversionOperatorDeclarationSyntax method)
        {
            //TODO: Need to find a way of dealing with this ... Quickly moving towards the idea of compile/decompile using ILSPY
            //  if (method.ImplicitOrExplicitKeyword.RawKind != (decimal) SyntaxKind.ExplicitKeyword)
            //     throw new Exception("Implicit cast operators are not supported " + Utility.Descriptor(method));

            var temp = new TempWriter();

//            temp.WriteIndent();

            temp.Write("public static " + TypeProcessor.ConvertType(method.Type));

            temp.Write(" " +
                       ((method.ImplicitOrExplicitKeyword.RawKind != (decimal) SyntaxKind.ExplicitKeyword)
                           ? ("op_Implicit_" + TypeProcessor.ConvertType(method.Type,false,true,false).Replace(".", "_"))
                           : "op_Explicit"));
            //writer.Write(" op_Explicit");
//            writer.Write(TypeProcessor.ConvertType(method.Type));
            temp.Write("(");

            bool firstParam = true;
            foreach (var param in method.ParameterList.Parameters)
            {
                if (firstParam)
                    firstParam = false;
                else
                    temp.Write(", ");

                temp.Write(TypeProcessor.ConvertType(param.Type) + " ");

                temp.Write(WriteIdentifierName.TransformIdentifier(param.Identifier.ValueText));
            }

            temp.Write(")");

            writer.WriteLine(temp.ToString());

            writer.OpenBrace();

            foreach (var statement in method.Body.Statements)
                Core.Write(writer, statement);

            writer.CloseBrace();
        }
    }
}