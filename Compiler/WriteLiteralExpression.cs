// /*
//   SharpNative - C# to D Transpiler
//   (C) 2014 Irio Systems 
// */

#region Imports

using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

#endregion

namespace SharpNative.Compiler
{
    internal static class WriteLiteralExpression
    {
		public static void Go(OutputWriter writer, LiteralExpressionSyntax expression, bool isConst, bool inSwitch=false)
        {
            var str = expression.ToString();


            if (str.Trim() == "null")
            {
                writer.Write("null");
                return;
            }

            if (str.StartsWith("@\""))
            {
                str = "r" + FixUpLiterals(EncodeNonAsciiCharacters(FixUpLiterals(str.Substring(1))));
                /*str = "\"" +
                      str.RemoveFromStartOfString("@\"")
                          .RemoveFromEndOfString("\"")
                          .Replace("\\", "\\\\")
                          .Replace("\"\"", "\\\"")
                          .Replace("\r\n", "\\n") + "\"";*/
            }

            var typeInfo = TypeProcessor.GetTypeInfo(expression);
            var type = typeInfo.Type;
            if (type == null)
                type = typeInfo.ConvertedType;

         

            if (type != null && type.SpecialType == SpecialType.System_String)
            {
                if (str.Contains("\\u") || str.Contains("\\x"))
               
                str = FixUpLiterals(EncodeNonAsciiCharacters(FixUpLiterals(str)));
                if (str == "null")
                {
                    if (inSwitch)
                        writer.Write("-1");
                    else
                        writer.Write("null");
                }
                else if (inSwitch)
                {
                    writer.Write(str);
                }
                else
                {
                }
                writer.Write("_S(" + str + ")");
            }
            else if (type != null && type.Name == "Nullable")//Nullable Support
            {
               
                var atype = TypeProcessor.ConvertType(type);
                writer.Write(atype + "()");
                
            }
            else
            {
                if(type.SpecialType==SpecialType.System_Boolean)
                    writer.Write(str);
                else if (type.SpecialType == SpecialType.System_Char)
                {

if(str.Contains("\\u") || str.Contains("\\x"))
                    writer.Write("cast(wchar)"+FixUpCharacterLiteral(str));
else
{
    writer.Write(str);
}
                }
                else
                {

                    //No need to convert these ... lets just put the correct suffix ourselves


                    if (typeInfo.ConvertedType != typeInfo.Type && typeInfo.ConvertedType.IsPrimitiveInteger() && typeInfo.Type.IsPrimitiveInteger())
                    {
                        writer.Write("cast({0})",TypeProcessor.ConvertType(typeInfo.ConvertedType)); //TODO: add better, less clumsy conversion
                    }

//                    //Number literals //TODO: make these convert to D Literal Suffixes
                    var suffix = realTypeSuffixes.FirstOrDefault (j => str.EndsWith (j));
                    if (suffix != null)
						str = str.Replace(suffix,drealTypeSuffixes[Array.IndexOf(realTypeSuffixes,suffix)]);
					else
					{
						suffix = integerTypeSuffixes.FirstOrDefault (j => str.EndsWith (j));
						if (suffix != null)
							str = str.Replace(suffix,dintegerTypeSuffixes[Array.IndexOf(integerTypeSuffixes,suffix)]);
					}

                    writer.Write(str);
                }
            }


        }

        static string[] realTypeSuffixes =  { "F", "f","D", "d", "M", "m"};
		static string[] drealTypeSuffixes =  { "F", "F","", "", "", ""}; //TODO:Have to add support for decimals
        static string[] integerTypeSuffixes = {  "UL", "Ul", "uL", "ul", "LU", "Lu", "lU", "lu","U", "u", "L" ,"l" };
		static string[] dintegerTypeSuffixes = {  "UL", "UL", "UL", "UL", "UL", "UL", "UL", "UL", "U", "U", "L", "L" };

        static string EncodeNonAsciiCharacters(string value)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in value)
            {
                if (c > 127)
                {
                    // This character is too big for ASCII
                    string encodedValue = "\\u" + ((int)c).ToString("x4");
                    sb.Append(encodedValue);
                }
                else
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        static string FixUpLiterals(string value)
        {
            string v = Regex.Replace(
                            value,
                            @"\\x(?<Value>[a-zA-Z0-9]{1,2})",
                            m =>
            {
                return "\\x" + (int.Parse(m.Groups["Value"].Value, NumberStyles.HexNumber)).ToString("x2");
            });
            v = Regex.Replace(
                            v,
                            @"\\u(?<Value>[a-zA-Z0-9]{4})",
                            m =>
            {
                return "\\u" + (int.Parse(m.Groups["Value"].Value, NumberStyles.HexNumber)).ToString("x4");
            });
            return v;
        }

        static string FixUpCharacterLiteral(string value)
        {
            string v = Regex.Replace(
                            value,
                            @"\\x(?<Value>[a-zA-Z0-9]{1,2})",
                            m =>
            {
                return "0x" + (int.Parse(m.Groups["Value"].Value, NumberStyles.HexNumber)).ToString("x2");
            });
            v = Regex.Replace(
                            v,
                            @"\\u(?<Value>[a-zA-Z0-9]{4})",
                            m =>
            {
                return "0x" + (int.Parse(m.Groups["Value"].Value, NumberStyles.HexNumber)).ToString("x4");
            });
            return v.Replace("'", "");
        }

    }
}