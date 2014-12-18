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
    internal static class WriteSizeOfExpression
    {
        public static void Go(OutputWriter writer, SizeOfExpressionSyntax expression)
        {
            var type = TypeProcessor.GetTypeInfo(expression.Type);
            //Use dTypes
            writer.Write("" + TypeProcessor.ConvertType(type.Type) + ".sizeof");
            //  writer.Write(SizeOf(type.Type).ToString());
        }

        private static int SizeOf(ITypeSymbol type)
        {
            switch (type.SpecialType)
            {
                case SpecialType.System_Byte:
                case SpecialType.System_SByte:
                    return 1;
                case SpecialType.System_Int16:
                case SpecialType.System_UInt16:
                    return 2;
                case SpecialType.System_Int32:
                case SpecialType.System_UInt32:
                    return 4;
                case SpecialType.System_Int64:
                case SpecialType.System_UInt64:
                    return 8;
                case SpecialType.System_Single:
                    return 4;
                case SpecialType.System_Double:
                    return 8;
                case SpecialType.System_Char:
                    return 2;
                case SpecialType.System_Boolean:
                    return 1;
                default:
                    throw new Exception("Need handler for sizeof " + type);
            }
        }
    }
}