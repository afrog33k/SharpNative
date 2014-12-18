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
    internal static class WriteLiteralExpression
    {
        public static void Go(OutputWriter writer, LiteralExpressionSyntax expression, bool isConst)
        {
            var str = expression.ToString();

            if (str.StartsWith("@"))
            {
                str = "\"" +
                      str.RemoveFromStartOfString("@\"")
                          .RemoveFromEndOfString("\"")
                          .Replace("\\", "\\\\")
                          .Replace("\"\"", "\\\"")
                          .Replace("\r\n", "\\n") + "\"";
            }

            var typeInfo = TypeProcessor.GetTypeInfo(expression);
            var type = typeInfo.Type;
            if (type == null)
                type = typeInfo.ConvertedType;

            if (type != null && type.SpecialType == SpecialType.System_String)
                writer.Write("_S(" + str + ")");
            else
                writer.Write(str);

            //TODO: will handle these separately
//            if (typeInfo.Type != null && typeInfo.ConvertedType != null)
//            {
//                if (isConst == false && typeInfo.ConvertedType.SpecialType ==SpecialType.System_Byte && typeInfo.Type.SpecialType == SpecialType.System_Int32)
//                    writer.Write(".toByte");
//            }
        }
    }
}