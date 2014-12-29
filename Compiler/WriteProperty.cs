// /*
//   SharpNative - C# to D Transpiler
//   (C) 2014 Irio Systems 
// */

#region Imports

using System;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

#endregion

namespace SharpNative.Compiler
{
   
    internal class WriteProperty
    {
       
        public static void Go(OutputWriter writer, PropertyDeclarationSyntax property, bool isProxy = false)
        {
            writer.WriteLine();
            //TODO, doesnt ref make things slower ?, though it makes proprties behave as in c#

            var isInterface = property.Parent is InterfaceDeclarationSyntax;

            var getter =
                property.AccessorList.Accessors.SingleOrDefault(
                    o => o.Keyword.RawKind == (decimal) SyntaxKind.GetKeyword);
            var setter =
                property.AccessorList.Accessors.SingleOrDefault(
                    o => o.Keyword.RawKind == (decimal) SyntaxKind.SetKeyword);

            var name = MemberUtilities.GetMethodName(property, ref isInterface);

            Action<AccessorDeclarationSyntax, bool> writeRegion = (region, get) =>
            {
                var accessString = "";

                //                var typeinfo = TypeProcessor.GetTypeInfo(property.Type);
                var isPtr = "";

                var typeString = TypeProcessor.ConvertType(property.Type) + isPtr + " ";

                //no inline in D
                if (get)
                    writer.Write(typeString + " ");
                else
                    writer.Write("void ");

               
                var explicitHeaderNAme = "";

               

                writer.Write((!String.IsNullOrEmpty(explicitHeaderNAme) ? explicitHeaderNAme : name) +
                             (get ? "()" : "( " + typeString + " value )") + " @property");

                if (property.Modifiers.Any(SyntaxKind.AbstractKeyword) || region.Body == null)
                    writer.Write(";\r\n");
                else
                {
                    writer.WriteLine();
                    writer.OpenBrace();
                    Core.WriteBlock(writer, region.Body.As<BlockSyntax>(), false);
                    writer.CloseBrace();
                    writer.WriteLine();
                }
            };

            if (getter == null && setter == null)
                throw new Exception("Property must have either a get or a set");
            {
                var type = property.Type;
                var isPtr = "";
                var typeString = TypeProcessor.ConvertType(type) + isPtr + " ";

                var acccessmodifiers = MemberUtilities.GetAccessModifiers(property, isInterface);

                //Auto Property
                var fieldName = "__prop_" + name;

                var isOverride = (property.Modifiers.Any(SyntaxKind.NewKeyword) ||
                                  property.Modifiers.Any(SyntaxKind.OverrideKeyword)) && !isInterface
                    ? " override "
                    : "";

                if (!isInterface) // Auto property
                {
                    if ((getter != null && getter.Body == null) &&
                        (setter != null && setter.Body == null) && (!property.Modifiers.Any(SyntaxKind.AbstractKeyword)))
                        writer.WriteLine("private " + typeString + fieldName + ";");
                }

                if (getter != null && isProxy)
                {
                    writer.WriteLine(acccessmodifiers  + typeString + "" + name + "() " + "@property " +
                                     "{ return Value." + name + ";}");
                }
                else
                    //Getter
                    if (getter != null && getter.Body == null)
                    {
                        if (isProxy)
                        {
                        }
                        else if (property.Modifiers.Any(SyntaxKind.AbstractKeyword) || isInterface)
                        {
                            writer.WriteLine(acccessmodifiers + typeString + " " + name + "()"  +
                                             " @property;");
                        }

                        else
                        {
                            writer.WriteLine(acccessmodifiers  + typeString + "" + name + "() " +
                                             "@property " + "{ return " + fieldName + ";}");
                        }
                    }
                    else if (getter != null)
                    {
                        writer.WriteIndent();
                        writer.Write(acccessmodifiers);
                        writeRegion(getter, true);
                    }

                if (setter != null && isProxy)
                {
                    writer.WriteLine(acccessmodifiers + " void " + name + "(" + typeString + " value) @property" +
                                     isOverride + " {  Value." + name + " = value;}");
                }
                else
                    //Setter
                    if (setter != null && setter.Body == null)
                    {
                        if (property.Modifiers.Any(SyntaxKind.AbstractKeyword) || isInterface)
                        {
                            writer.WriteLine(acccessmodifiers + " void " + name + "(" + typeString + " value)" +
                                             isOverride + "" + " @property;");
                        }
                        else
                        {
                            writer.WriteLine(acccessmodifiers + " void " + name + "(" + typeString + " value) @property" +
                                             isOverride + " {" + fieldName + " = value;}");
                        }
                    }
                    else if (setter != null)
                    {
                        writer.WriteIndent();
                        writer.Write(acccessmodifiers);
                        writeRegion(setter, false);
                    }
            }
        }
    }
}