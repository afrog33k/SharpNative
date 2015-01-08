// /*
//   SharpNative - C# to D Transpiler
//   (C) 2014 Irio Systems 
// */

#region Imports

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

#endregion

namespace SharpNative.Compiler
{
    internal static class WriteEnum
    {
        //TODO should enum be light class of static members ? or just a plain enum ? (using plain enum for now)
        public static void Go(OutputWriter writer, IEnumerable<EnumMemberDeclarationSyntax> allChildren)
        {
//            writer.IsInterface = true;
            writer.Write("enum ");
            writer.Write(WriteType.TypeName(Context.Instance.Type, false));

//            writer.Write(Context.Instance.TypeName);
            writer.Write(":" + TypeProcessor.ConvertType(Context.Instance.Type.EnumUnderlyingType) + "\r\n");
            writer.OpenBrace();

            long lastEnumValue = 0;
            var children = allChildren.ToArray();
            var values =
                children.Select(
                    o => new {Syntax = o, Value = o.EqualsValue != null ? o.EqualsValue.Value : null})
                    .ToList();

            foreach (var value in values)
            {


                var text = "";

                text = WriteIdentifierName.TransformIdentifier(value.Syntax.Identifier.ValueText);

                if (value.Value != null)
                {
                    //lets try parsing the value so we can evaluate it
                    var expression =value.Value;
                    if (expression != null)
                    {
                        var temp = new TempWriter();
                        Core.Write(temp,expression);
                        text += " = " + temp.ToString();
                        temp.Dispose();;
                    }
                    else
                    {
                        text += value.Value;
                    }
                }
                
                writer.WriteLine(text + ",");

            }

         

            writer.CloseBrace();
//            writer.Write(";");
        }




     
    }
}