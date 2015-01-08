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

            ITypeSymbol iface;
            ISymbol[] proxies;
           
            var name = MemberUtilities.GetMethodName(property, ref isInterface, out iface, out proxies);

            var modifiers = property.Modifiers;
            var type = TypeProcessor.GetTypeInfo(property).ConvertedType;

            var acccessmodifiers = MemberUtilities.GetAccessModifiers(property, isInterface);

            var typeString = TypeProcessor.ConvertType(type) + " ";

            var hasGetter = getter != null;
            var getterHasBody = hasGetter && getter.Body != null;
         
            var hasSetter = setter != null;
            var setterHasBody = hasSetter && setter.Body == null;

            string getterbody = null;
            if (getterHasBody)
                getterbody = Core.WriteString(getter.Body, false, writer.Indent + 2);
            string setterbody = null;
            if (setterHasBody)
                setterbody = Core.WriteString(setter.Body, false, writer.Indent + 2);

            Action<bool, string> writeRegion = (get, body) =>
            {
                //no inline in D
                if (get)
                    writer.Write(typeString + " ");
                else
                    writer.Write("void ");


                writer.Write((name) +
                             (get ? "("+(iface!=null?(TypeProcessor.ConvertType(iface)+" ig=null") : "")+")" : "(" + typeString + " value " + (iface != null ? (","+TypeProcessor.ConvertType(iface) + " ig=null") : "") + ")") + " @property");

               
                if (modifiers.Any(SyntaxKind.AbstractKeyword) || body == null)
                    writer.Write(";\r\n");
                else
                {
                    writer.WriteLine();
                    writer.OpenBrace();
                    writer.WriteLine(body);
                    writer.CloseBrace();
                    writer.WriteLine();
                }
            };

            if (getter == null && setter == null)
                throw new Exception("Property must have either a get or a set");
        
            

                //Auto Property
                var fieldName = "__prop_" + name;

                var isOverride = (modifiers.Any(SyntaxKind.NewKeyword) ||
                                  modifiers.Any(SyntaxKind.OverrideKeyword)) && !isInterface
                    ? " override "
                    : "";

        


            if (!isInterface) // Auto property
                {
                    if ((hasGetter && getterHasBody) &&
                        (hasSetter && setterHasBody) && (!modifiers.Any(SyntaxKind.AbstractKeyword)))
                        writer.WriteLine("private " + typeString + fieldName + ";");
                }

                if (hasGetter && isProxy)
                {
                    writer.WriteLine(acccessmodifiers  + typeString + "" + name + "(" + (iface != null ? (TypeProcessor.ConvertType(iface) + " ig=null") : "") + ") " + "@property " +
                                     "{ return Value." + name + ";}");
                }
                else
                    //Getter
                    if (hasGetter && !getterHasBody)
                    {
                        if (isProxy)
                        {
                        }
                        else if (modifiers.Any(SyntaxKind.AbstractKeyword) || isInterface)
                        {
                            writer.WriteLine(acccessmodifiers + typeString + " " + name + "(" + (iface != null ? (TypeProcessor.ConvertType(iface) + " ig=null") : "") + ")" +
                                             " @property;");
                        }

                        else
                        {
                            writer.WriteLine(acccessmodifiers  + typeString + "" + name + "(" + (iface != null ? (TypeProcessor.ConvertType(iface) + " ig=null") : "") + ")"+" @property " + "{ return " + fieldName + ";}");
                        }
                    }
                    else if (hasGetter)
                    {
                        writer.WriteIndent();
                        writer.Write(acccessmodifiers);
                     
                        writeRegion(true, getterbody);
                    }

                if (hasSetter && isProxy)
                {
                    writer.WriteLine(acccessmodifiers + " void " + name + "(" + typeString + " value" + (iface != null ? ("," + TypeProcessor.ConvertType(iface) + " ig=null") : "") +
                                             isOverride  + ") @property" +
                                     isOverride + " {  Value." + name + " = value;}");
                }
                else
                    //Setter
                    if (hasSetter && !setterHasBody)
                    {
                        if (modifiers.Any(SyntaxKind.AbstractKeyword) || isInterface)
                        {
                            writer.WriteLine(acccessmodifiers + " void " + name + "(" + typeString + " value" + (iface != null ? ("," + TypeProcessor.ConvertType(iface) + " ig=null") : "")  +
                                             isOverride  + ") @property;");
                        }
                        else
                        {
                            writer.WriteLine(acccessmodifiers + " void " + name + "(" + typeString + " value"+(iface != null ? ("," + TypeProcessor.ConvertType(iface) + " ig=null") : "")+") @property" +
                                             isOverride + " {" + fieldName + " = value;}");
                        }
                    }
                    else if (hasSetter)
                    {
                        writer.WriteIndent();
                        writer.Write(acccessmodifiers);
                       
                        writeRegion(true, setterbody);
            }
            
        }
    }
}