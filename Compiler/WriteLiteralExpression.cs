// /*
//   SharpNative - C# to D Transpiler
//   (C) 2014 Irio Systems 
// */

#region Imports

using System;
using System.Linq;
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


            if (str.Trim() == "null")
            {
                writer.Write("null");
                return;
            }

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
            {
                if (str != "null")
                    writer.Write("_S(" + str + ")");
                else
                {
                    writer.Write("null");
                }
            }
            else
            {
                if(type.SpecialType==SpecialType.System_Boolean)
                    writer.Write(str);
                else
                {

                    //Number literals //TODO: make these convert to D Literal Suffixes
                    var suffix = realTypeSuffixes.Where(j => str.EndsWith(j)).FirstOrDefault();
                    if (suffix != null)
                        str = str.RemoveFromEndOfString(suffix);

                     suffix = integerTypeSuffixes.Where(j => str.EndsWith(j)).FirstOrDefault();
                    if (suffix != null)
                        str = str.RemoveFromEndOfString(suffix);

                    writer.Write(str);
                }
            }


        }

        static string[] realTypeSuffixes =  { "F", "f","D", "d", "M", "m"};
        static string[] integerTypeSuffixes = { "U", "u", "L" ,"l", "UL", "Ul", "uL", "ul", "LU", "Lu", "lU", "lu" };

    }
}