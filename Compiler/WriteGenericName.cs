// /*
//   SharpNative - C# to D Transpiler
//   (C) 2014 Irio Systems 
// */

#region Imports

using Microsoft.CodeAnalysis.CSharp.Syntax;

#endregion

namespace SharpNative.Compiler
{
    internal static class WriteGenericName
    {
        public static void Go(OutputWriter writer, GenericNameSyntax name)
        {
            writer.Write(WriteIdentifierName.TransformIdentifier(name.Identifier.ValueText));
            writer.Write("[");

            bool first = true;
            foreach (var gen in name.TypeArgumentList.Arguments)
            {
                if (first)
                    first = false;
                else
                    writer.Write(", ");

                writer.Write(TypeProcessor.ConvertType(gen));
            }
            writer.Write("]");
        }
    }
}