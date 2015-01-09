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

        

        static void WriteRegion(bool get, string body, ITypeSymbol iface, OutputWriter writer, string typeString, string name, SyntaxTokenList modifiers, string parameters, bool isindexer)
        {
            //no inline in D
            if (get)
                writer.Write(typeString + " ");
            else
                writer.Write("void ");

            if (!isindexer)
            {
                writer.Write((name) +
                             (get
                                 ? "(" + (iface != null ? (TypeProcessor.ConvertType(iface) + " ig=null") : "") + ")"
                                 : "(" + typeString + " value " +
                                   (iface != null ? ("," + TypeProcessor.ConvertType(iface) + " ig=null") : "") + ")") +
                             " @property");
            }
            else
            {
                writer.Write((name) +
                            (get
                                ? "(" + parameters + ")"
                                : "(" + typeString + " value, " +
                                  parameters + ")") +
                            "");
            }


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
        }
        public static void Go(OutputWriter writer, BasePropertyDeclarationSyntax property, bool isProxy = false)
        {
            writer.WriteLine();
            //TODO, doesnt ref make things slower ?, though it makes proprties behave as in c#

            var isInterface = property.Parent is InterfaceDeclarationSyntax;

            var getter =
                property.AccessorList.Accessors.SingleOrDefault(
                    o => o.Keyword.RawKind == (decimal)SyntaxKind.GetKeyword);
            var setter =
                property.AccessorList.Accessors.SingleOrDefault(
                    o => o.Keyword.RawKind == (decimal)SyntaxKind.SetKeyword);

            ITypeSymbol iface;
            ISymbol[] proxies;

            var name = MemberUtilities.GetMethodName(property, ref isInterface, out iface, out proxies);

            var modifiers = property.Modifiers;
            var type = ((IPropertySymbol)TypeProcessor.GetDeclaredSymbol(property)).Type;


            var acccessmodifiers = MemberUtilities.GetAccessModifiers(property, isInterface);

            var typeString = TypeProcessor.ConvertType(type) + " ";

            var hasGetter = getter != null;
            var getterHasBody = hasGetter && getter.Body != null;

            var hasSetter = setter != null;
            var setterHasBody = hasSetter && setter.Body != null;

            string getterbody = null;
            if (getterHasBody)
                getterbody = Core.WriteString(getter.Body, false, writer.Indent + 2);
            string setterbody = null;
            if (setterHasBody)
                setterbody = Core.WriteString(setter.Body, false, writer.Indent + 2);

            if (getter == null && setter == null)
                throw new Exception("Property must have either a get or a set");

            string isOverride;
           
            var fieldName = WriteAutoFieldName(writer, name, modifiers, isInterface, hasGetter, getterHasBody,
                    hasSetter, setterHasBody, typeString, out isOverride, (property is IndexerDeclarationSyntax));

            var indexerDeclarationSyntax = property as IndexerDeclarationSyntax;
            BracketedParameterListSyntax @params = null;

            if (indexerDeclarationSyntax != null)
            {
                @params = indexerDeclarationSyntax.ParameterList;
            }

            string parameters = null;
            if (@params != null)
            {
                parameters = WriteMethod.GetParameterListAsString(@params.Parameters, iface: proxies == null ? iface : null, writebraces: false);
            }

            WriteGetter(writer, isProxy, hasGetter, acccessmodifiers, typeString, name,  proxies == null ? iface : null, getterHasBody, modifiers, isInterface, fieldName, getterbody, parameters, indexerDeclarationSyntax != null);

            WriteSetter(writer, isProxy, hasSetter, acccessmodifiers, name, typeString, proxies == null ? iface : null, isOverride, setterHasBody, modifiers, isInterface, fieldName, setterbody, parameters, indexerDeclarationSyntax!=null);

            if (proxies != null)
            {
                foreach (var proxy in proxies)
                {

                    if (indexerDeclarationSyntax == null)
                    {
                        setterbody = " " + name + "=" + "value" + ";";
                        getterbody = " return " + name + ";";
                    }
                    else
                    {
                        string parameters2="";
                        if (@params != null)
                        {
                            parameters2 = WriteMethod.GetParameterListAsString(@params.Parameters, iface: null, includeTypes: false,writebraces:false);
                           // parameters2 = WriteMethod.GetParameterListAsString(@params.Parameters, iface: proxies == null ? iface : null, writebraces: false);
                        }

                        setterbody = " opIndexAssign(value," + parameters2 + ");";// + "=" + "value" + ";";
                        getterbody = " return opIndex(" + parameters2 + ");";
                    }


                    parameters = null;
                    if (@params != null)
                    {
                        parameters = WriteMethod.GetParameterListAsString(@params.Parameters, iface: proxy.ContainingType, writebraces: false);
                    }

                    WriteGetter(writer, isProxy, hasGetter, acccessmodifiers, typeString, name, proxy.ContainingType, getterHasBody, modifiers, isInterface, fieldName, getterbody, parameters, indexerDeclarationSyntax != null);

                    WriteSetter(writer, isProxy, hasSetter, acccessmodifiers, name, typeString, proxy.ContainingType, isOverride, setterHasBody, modifiers, isInterface, fieldName, setterbody, parameters, indexerDeclarationSyntax != null);
                }
            }

        }

        private static void WriteSetter(OutputWriter writer, bool isProxy, bool hasSetter, string acccessmodifiers, string name, string typeString, ITypeSymbol iface, string isOverride, bool setterHasBody, SyntaxTokenList modifiers, bool isInterface, string fieldName, string setterbody, string parameters, bool isindexer)
        {

            if (isindexer)
                name = "opIndexAssign";

            if (hasSetter && isProxy)
            {
                if (!isindexer)
                {
                    writer.WriteLine(acccessmodifiers + " void " + name + "(" + typeString + " value" +
                                     (iface != null ? ("," + TypeProcessor.ConvertType(iface) + " ig=null") : "") +
                                     isOverride + ") @property" +
                                     isOverride + " {  Value." + name + " = value;}");
                }
                else
                {
                    writer.WriteLine(acccessmodifiers + " void " + name + "(" + typeString + " value," +
                                   parameters +
                                    isOverride + ") " +
                                    isOverride + " {  Value." + name + " = value;}");
                }
            }
            else if (hasSetter && !setterHasBody) //Setter
            {
                if (modifiers.Any(SyntaxKind.AbstractKeyword) || isInterface)
                {
                    if (!isindexer)
                    {
                        writer.WriteLine(acccessmodifiers + " void " + name + "(" + typeString + " value" +
                                          (iface != null ? ("," + TypeProcessor.ConvertType(iface) + " ig=null") : "")  +
                                         isOverride + ") @property;");
                    }
                    else
                    {
                        writer.WriteLine(acccessmodifiers + " void " + name + "(" + typeString + " value," +
                                        parameters +
                                        isOverride + ");");
                    }
                }
                else
                {
                    if (!isindexer)
                    {
                        writer.WriteLine(acccessmodifiers + " void " + name + "(" + typeString + " value"+
                                          (iface != null ? ("," + TypeProcessor.ConvertType(iface) + " ig=null") : "") + 
                                         ") @property" +
                                         isOverride + " {" + fieldName + " = value;}");
                    }
                    else
                    {
                        writer.WriteLine(acccessmodifiers + " void " + name + "(" + typeString + " value," +parameters +
                                         ")" +
                                         isOverride + " {" + fieldName + " = value;}");
                    }
                }
            }
            else if (hasSetter)
            {
                writer.WriteIndent();
                writer.Write(acccessmodifiers);

                WriteRegion(false, setterbody, iface, writer, typeString, name, modifiers,parameters,isindexer);
            }
        }

        private static void WriteGetter(OutputWriter writer, bool isProxy, bool hasGetter, string acccessmodifiers, string typeString, string name, ITypeSymbol iface, bool getterHasBody, SyntaxTokenList modifiers, bool isInterface, string fieldName, string getterbody, string parameters, bool isindexer)
        {
            if (isindexer)
                name = "opIndex";

            if (hasGetter && isProxy)
            {
                if (!isindexer)
                {
                    writer.WriteLine(acccessmodifiers + typeString + "" + name + "(" +
                                     (iface != null ? (TypeProcessor.ConvertType(iface) + " ig=null") : "")  +
                                     ")" + " @property " +
                                     "{ return Value." + name + ";}");
                }
                else
                {
                    writer.WriteLine(acccessmodifiers + typeString + "" + name + "(" + parameters  +
                                    ")" + "  " +
                                    "{ return Value." + name + ";}");
                }
            }
            else if (hasGetter && !getterHasBody) //Getter
            {
                if (modifiers.Any(SyntaxKind.AbstractKeyword) || isInterface)
                {
                    if (!isindexer)
                    {
                        writer.WriteLine(acccessmodifiers + typeString + " " + name + "(" +
                                         (iface != null ? (TypeProcessor.ConvertType(iface) + " ig=null") : "") + ")" +
                                         " @property;");
                    }
                    else
                    {
                        writer.WriteLine(acccessmodifiers + typeString + " " + name + "(" +
                                        parameters+ ")" +
                                        ";");
                    }
                }

                else
                {
                    if (!isindexer)
                    {
                        writer.WriteLine(acccessmodifiers + typeString + "" + name + "(" +
                                         (iface != null ? (TypeProcessor.ConvertType(iface) + " ig=null") : "") + ")" +
                                         " @property " + "{ return " + fieldName + ";}");
                    }
                    else
                    {
                        writer.WriteLine(acccessmodifiers + typeString + "" + name + "(" + parameters+ ")" +
                                         "  " + "{ return " + fieldName + ";}");
                    }
                }
            }
            else if (hasGetter)
            {
                writer.WriteIndent();
                writer.Write(acccessmodifiers);
                WriteRegion(true, getterbody, iface, writer, typeString, name, modifiers,parameters,isindexer);
            }
        }

        private static string WriteAutoFieldName(OutputWriter writer, string name, SyntaxTokenList modifiers, bool isInterface,
            bool hasGetter, bool getterHasBody, bool hasSetter, bool setterHasBody, string typeString, out string isOverride, bool isIndexer=false)
        {
//Auto Property
            var fieldName = "__prop_" + name;

            isOverride = (modifiers.Any(SyntaxKind.NewKeyword) ||
                          modifiers.Any(SyntaxKind.OverrideKeyword)) && !isInterface
                ? " override "
                : "";

            if (!isInterface &&!isIndexer) // Auto property
            {
                if ((hasGetter && !getterHasBody) &&
                    (hasSetter && !setterHasBody) && (!modifiers.Any(SyntaxKind.AbstractKeyword)))
                    writer.WriteLine("private " + typeString + fieldName + ";");
            }
            return fieldName;
        }
    }
}