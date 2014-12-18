// /*
//   SharpNative - C# to D Transpiler
//   (C) 2014 Irio Systems 
// */

#region Imports

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

#endregion

namespace SharpNative.Compiler
{
    internal static class WriteElementAccessExpression
    {
        public static void Go(OutputWriter writer, ElementAccessExpressionSyntax expression)
        {
            var type = TypeProcessor.GetTypeInfo(expression.Expression).Type;
            var typeStr = TypeProcessor.GenericTypeName(type);

            //Todo if we are using unsafe / fast mode, just use array->Data()[i] should bypass bounds check and indirection also should be as fast as pure c++ arrays
            //though fixed syntax could fix this too ??
//            writer.Write("(*");

            Core.Write(writer, expression.Expression);

            if (type.SpecialType == SpecialType.System_Array)
//            writer.Write(")");
                writer.Write(".Items["); //TODO test this thoroughly

            else
                writer.Write("["); //TODO test this thoroughly

            var first = true;
            foreach (var argument in expression.ArgumentList.Arguments)
            {
                if (first)
                    first = false;
                else
                    writer.Write(", ");

                Core.Write(writer, argument.Expression);
            }
            writer.Write("]");
        }
    }
}