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
		public static void Go(OutputWriter writer, LiteralExpressionSyntax expression, bool isConst, bool inSwitch=false)
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
				if(inSwitch)
                    writer.Write(  str );
				else
					writer.Write("_S(" + str + ")");
					
                else
                {

					if(inSwitch)
						writer.Write( "-1");
					else
						writer.Write("null");

                    
                }
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
                else
                {

                    //Number literals //TODO: make these convert to D Literal Suffixes
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


    }
}