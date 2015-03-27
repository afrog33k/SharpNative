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

        const string _get = "";//"get"; //TODO: Fix these when we have a better impl;
        private const string _set = "";//"set";

        static void WriteRegion(bool get, string body, ITypeSymbol iface, OutputWriter writer, string typeString, string name, SyntaxTokenList modifiers, string parameters, bool isindexer, bool hasGetter)
        {
            //no inline in D
//            if (get)
                writer.Write((get ? (typeString) : (hasGetter ? typeString : "void")) + " ");
//            else
//                writer.Write("void ");

            if (!isindexer)
            {
               
                writer.Write( (get?(_get+name):(_set+(name))) +
                             (get
                                 ? "(" + (iface != null ? (TypeProcessor.ConvertType(iface) + " __ig=null") : "") + ")"
                                 : "(" + typeString + " value " +
                                   (iface != null ? ("," + TypeProcessor.ConvertType(iface) + " __ig=null") : "") + ")") +
                             " ");
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

            var isYield = getter != null && getter.DescendantNodes().OfType<YieldStatementSyntax>().Any();
            var isStatic = property.Modifiers.Any (k=>k.IsKind(SyntaxKind.StaticKeyword));

            ITypeSymbol iface;
            ISymbol[] proxies;

            var name = MemberUtilities.GetMethodName(property, ref isInterface, out iface, out proxies);

            var modifiers = property.Modifiers;
            var propertySymbol = (IPropertySymbol)TypeProcessor.GetDeclaredSymbol(property);
            var type = propertySymbol.Type;


            var acccessmodifiers = MemberUtilities.GetAccessModifiers(property, isInterface);

            var typeString = TypeProcessor.ConvertType(type);

            var hasGetter = getter != null;
            var getterHasBody = hasGetter && getter.Body != null;

            var hasSetter = setter != null;
            var setterHasBody = hasSetter && setter.Body != null;



			var indexerDeclarationSyntax = property as IndexerDeclarationSyntax;
			var isindexer = indexerDeclarationSyntax != null;
            string getterbody = null;
            if (getterHasBody)
            {

              

                getterbody = Core.WriteBlock(getter.Body, false, writer.Indent + 2);

                if (!isProxy && isYield)
                {
                    var namedTypeSymbol = propertySymbol.Type as INamedTypeSymbol;
                    if (namedTypeSymbol != null)
                    {
                        //                        var iteratortype = namedTypeSymbol.TypeArguments[0];
                        //                        getterbody=String.Format("return new __IteratorBlock!({0})(delegate(__IteratorBlock!({0}) __iter){{ {1} }});",
                        //                            TypeProcessor.ConvertType(iteratortype),getterbody);

                        var className = propertySymbol.GetYieldClassName()+(
                   (((INamedTypeSymbol)propertySymbol.Type).TypeArguments.Any() && ((INamedTypeSymbol)propertySymbol.Type).TypeArguments[0].TypeKind == TypeKind.TypeParameter) ? "__G" : "");




                        // writer.WriteLine(accessString + returnTypeString + methodSignatureString + @params2 + constraints);

                        //writer.OpenBrace();

                        if (!propertySymbol.IsStatic)
                        {

                            getterbody = writer.WriteIndentToString() + ("return new " + className + "(this);");
                        }
                        else
                        {
                            getterbody = writer.WriteIndentToString() + ("return new " + className + "();");

                        }
                    }
                }
            }
            string setterbody = null;
			if (setterHasBody)
			{
				setterbody = Core.WriteString (setter.Body, false, writer.Indent + 2);
				if (isindexer)
				{
					setterbody +=  writer.WriteIndentToString()+ "return value;";
				}
				else
				{
				    if (hasGetter)
				    {
				        setterbody += writer.WriteIndentToString() + "return " + name + ";";
				    }

				}
			}

            if (getter == null && setter == null)
                throw new Exception("Property must have either a get or a set");

            string isOverride;
           
            var fieldName = WriteAutoFieldName(writer, name, modifiers, isInterface, hasGetter, getterHasBody,
                    hasSetter, setterHasBody, typeString, out isOverride, (property is IndexerDeclarationSyntax));


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

            WriteSetter(writer, isProxy, hasSetter, acccessmodifiers, name, typeString, proxies == null ? iface : null, isOverride, setterHasBody, modifiers, isInterface, fieldName, setterbody, parameters, isindexer,hasGetter);

//			if (!isindexer && !isInterface) //TODO: Find a better solution
//			{
//				var fieldacccessmodifiers = acccessmodifiers.Replace ("abstract", "").Replace ("virtual","").Replace("override","");
//
//				writer.WriteLine(fieldacccessmodifiers +  "__Property!(" + typeString + ")" + name + ";");
//				if (isStatic)
//				{
//					var staticWriter = new TempWriter ();
//
//					staticWriter.WriteLine (name + String.Format (" = __Property!(" + typeString + ")(__ToDelegate(&set{0}), __ToDelegate(&get{0}));", name));
//					Context.Instance.StaticInits.Add (staticWriter.ToString ());
//				}
//				else
//				{
//					var instanceWriter = new TempWriter ();
//
//					instanceWriter.WriteLine (name + String.Format (" = __Property!(" + typeString + ")((&set{0}), (&get{0}));", name));
//					Context.Instance.InstanceInits.Add (instanceWriter.ToString ());
//				}
//			}


            if (proxies != null)
            {
                foreach (var proxy in proxies)
                {

                    if (indexerDeclarationSyntax == null)
                    {
                        setterbody = writer.WriteIndentToString() + name + "=" + "value" + ";";
                        getterbody = writer.WriteIndentToString() + "return " + name + ";";
                    }
                    else
                    {
                        string parameters2="";
                        if (@params != null)
                        {
                            parameters2 = WriteMethod.GetParameterListAsString(@params.Parameters, iface: null, includeTypes: false,writebraces:false);
                           // parameters2 = WriteMethod.GetParameterListAsString(@params.Parameters, iface: proxies == null ? iface : null, writebraces: false);
                        }

                        setterbody = writer.WriteIndentToString() + "return opIndexAssign(value," + parameters2 + ");";// + "=" + "value" + ";";
                        getterbody = writer.WriteIndentToString() + "return opIndex(" + parameters2 + ");";
                    }


                    parameters = null;
                    if (@params != null)
                    {
                        parameters = WriteMethod.GetParameterListAsString(@params.Parameters, iface: proxy.ContainingType, writebraces: false);
                    }

                    WriteGetter(writer, isProxy, hasGetter, acccessmodifiers, typeString, name, proxy.ContainingType, getterHasBody, modifiers, isInterface, fieldName, getterbody, parameters, isindexer);

					WriteSetter (writer, isProxy, hasSetter, acccessmodifiers, name, typeString, proxy.ContainingType, isOverride, setterHasBody, modifiers, isInterface, fieldName, setterbody, parameters, isindexer,hasGetter);
                }
            }

        }

        private static void WriteSetter(OutputWriter writer, bool isProxy, bool hasSetter, string acccessmodifiers, string name, string typeString, ITypeSymbol iface, string isOverride, bool setterHasBody, SyntaxTokenList modifiers, bool isInterface, string fieldName, string setterbody, string parameters, bool isindexer,bool hasGetter)
        {

            if (isindexer)
                name = "opIndexAssign";

            var args = _set;
            if (hasSetter && isProxy)
            {
                if (!isindexer)
                {
					writer.WriteLine (string.Format ("{0} {2} {5}{1}({2} value{3}{4}){4} {{  __Value.{1} = value; return value;}}", acccessmodifiers, name, hasGetter ? typeString : "void", (iface != null ? ("," + TypeProcessor.ConvertType (iface) + " __ig=null") : ""), isOverride, args));
                }
                else
                {
					writer.WriteLine (string.Format ("{0} {2} {1}({2} value,{3}{4}) {4} {{  __Value.{1} = value;return value;}}", acccessmodifiers, name, typeString, parameters, isOverride));
                }
            }
            else if (hasSetter && !setterHasBody) //Setter
            {
                if (modifiers.Any(SyntaxKind.AbstractKeyword) || isInterface)
                {
                    if (!isindexer)
                    {
						writer.WriteLine (string.Format ("{0} {2} {5}{1}({2} value{3}{4});", acccessmodifiers, name, hasGetter ? typeString : "void", (iface != null ? ("," + TypeProcessor.ConvertType (iface) + " __ig=null") : ""), isOverride, args));
                    }
                    else
                    {
						writer.WriteLine (string.Format ("{0} {2} {1}({2} value,{3}{4});", acccessmodifiers, name, typeString, parameters, isOverride));
                    }
                }
                else
                {
                    if (!isindexer)
                    {
                        var returnValue = hasGetter ? writer.WriteIndentToString() + "return value;" :"";
                        writer.WriteLine (string.Format("{0} {2} {6}{1}({2} value{3}){4} {{{5} = value;{7}}}", acccessmodifiers, name, hasGetter?typeString :"void", (iface != null ? ("," + TypeProcessor.ConvertType (iface) + " __ig=null") : ""), isOverride, fieldName, args,returnValue));
                    }
                    else
                    {
                        var returnValue = hasGetter ? writer.WriteIndentToString() + "return value;" : "";

                        writer.WriteLine (string.Format("{0} {2} {1}({2} value,{3}){4} {{{5} = value;{6}}}" , acccessmodifiers, name, typeString, parameters, isOverride, fieldName, returnValue));
                    }
                }
            }
            else if (hasSetter)
            {
                writer.WriteIndent();
                writer.Write(acccessmodifiers);

                WriteRegion(false, setterbody, iface, writer, typeString, name, modifiers,parameters,isindexer,hasGetter);
            }
        }

        private static void WriteGetter(OutputWriter writer, bool isProxy, bool hasGetter, string acccessmodifiers, string typeString, string name, ITypeSymbol iface, bool getterHasBody, SyntaxTokenList modifiers, bool isInterface, string fieldName, string getterbody, string parameters, bool isindexer)
        {
            if (isindexer)
                name = "opIndex";

            var args =_get;
            if (hasGetter && isProxy)
            {
                if (!isindexer)
                {
					writer.WriteLine (string.Format ("{0}{1} {4}{2}({3}) {{ return __Value.{2};}}", acccessmodifiers, typeString, name, (iface != null ? (TypeProcessor.ConvertType (iface) + " __ig=null") : ""), args));
                }
                else
                {
					writer.WriteLine (string.Format ("{0}{1} {2}({3})  {{ return __Value.{2};}}", acccessmodifiers, typeString, name, parameters));
                }
            }
            else if (hasGetter && !getterHasBody) //Getter
            {
                if (modifiers.Any(SyntaxKind.AbstractKeyword) || isInterface)
                {
                    if (!isindexer)
                    {
						writer.WriteLine (string.Format ("{0}{1} {4}{2}({3});", acccessmodifiers, typeString, name, (iface != null ? (TypeProcessor.ConvertType (iface) + " __ig=null") : ""), args));
                    }
                    else
                    {
						writer.WriteLine (string.Format ("{0}{1} {2}({3});", acccessmodifiers, typeString, name, parameters));
                    }
                }

                else
                {
                    if (!isindexer)
                    {
						writer.WriteLine (string.Format ("{0}{1} {5}{2}({3}) {{ return {4};}}", acccessmodifiers, typeString, name, (iface != null ? (TypeProcessor.ConvertType (iface) + " __ig=null") : ""), fieldName, args));
                    }
                    else
                    {
						writer.WriteLine (string.Format ("{0}{1} {2}({3})  {{ return {4};}}", acccessmodifiers, typeString, name, parameters, fieldName));
                    }
                }
            }
            else if (hasGetter)
            {
                writer.WriteIndent();
                writer.Write(acccessmodifiers);
                WriteRegion(true, getterbody, iface, writer, typeString, name, modifiers,parameters,isindexer,hasGetter);
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

            var isStatic = modifiers.Any(SyntaxKind.StaticKeyword);

            if (!isInterface &&!isIndexer) // Auto property
            {
                if ((hasGetter && !getterHasBody) &&
                    (hasSetter && !setterHasBody) && (!modifiers.Any(SyntaxKind.AbstractKeyword)))
                {
                   
                    writer.WriteLine("private " + (isStatic?"static " :"") + typeString + " " + fieldName + ";");

                }
            }
            return fieldName;
        }
    }
}