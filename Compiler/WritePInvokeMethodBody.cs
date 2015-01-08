// /*
//   SharpNative - C# to D Transpiler
//   (C) 2014 Irio Systems 
// */

#region Imports

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using CSharpExtensions = Microsoft.CodeAnalysis.CSharpExtensions;

#endregion

namespace SharpNative.Compiler
{
    internal static class WritePInvokeMethodBody
    {
        private static string GetDelegateRawType(string dType)
        {
            return "__TINFO!(" + dType + ").P";
        }

        internal enum PInvokeMode
        {
            None
        }

        public static string ConvertPInvokeType(ITypeSymbol returnType, PInvokeMode mode = PInvokeMode.None)
        {
            var dType = TypeProcessor.ConvertType(returnType);

            if (returnType.TypeKind == TypeKind.Delegate)
            {
                return GetDelegateRawType(dType);

//				var dlg =returnType.OriginalDefinition as IMethodSymbol;
//				if (dlg.Parameters.Length == 0)
////					return "() => " + TryConvertType (dlg.ReturnType);
////				else
////					return "(" + string.Join (", ", dlg.Parameters.ToList ().Select (o => TryConvertType (o.Type))) + ") => " + TryConvertType (dlg.ReturnType);
//
//			
//
//				return TypeProcessor.ConvertType (dlg.ReturnType) + " delegate" +
//						dlg.Parameters.ToList ().Select (o => ConvertPInvokeType (o.Type)) + ")";

//				return dType.RemoveFromStartOfString("Delegate!(").SubstringBeforeLast(')');
            }

            //TODO this should become a class with different options like CharSet etc ..
            switch (dType)
            {
                case "String":
                    return "char *"; //TODO: Should be dependent on the charset
                case "Array_T!(String)":
                    return "char**";
            }
            return dType;
        }

        public static void Go(OutputWriter writer, string methodName, IMethodSymbol methodSymbol,
            AttributeSyntax pinvokeAttributes)
        {
            //Do Pinvoke stuff
            //PInvokeFunction < int (int, double)> foo(library, "foo");
            //int i = foo(42, 2.0);
           
//			var methodParams = methodSymbol.Parameters.Any () ? methodSymbol.Parameters.Select (h => ConvertPInvokeType (h.Type)).Aggregate ((k, y) => (k) + " ," + y) : "";
            var returnString = ConvertPInvokeType(methodSymbol.ReturnType);

            if (pinvokeAttributes != null)
            {
                var attributeArgumentSyntax = pinvokeAttributes.ArgumentList.Arguments.FirstOrDefault(k => k.Expression != null);
                string dllImport = attributeArgumentSyntax.ToFullString();
//                AttributeSyntax import =
//                    Context.Instance.DllImports.FirstOrDefault(
//                        d =>
//                            d.ArgumentList.Arguments.FirstOrDefault(
//                                k =>
//                                    k.Expression != null &&
//                                    k.Expression.ToString() ==
//                                    pinvokeAttributes.ArgumentList.Arguments.FirstOrDefault(g => g.Expression != null)
//                                        .Expression.ToString()) != null);
//                if (import != null)
//                    dllImportId = Context.Instance.DllImports.IndexOf(import);
//
//                if (dllImportId == -1)
//                {
//                    Context.Instance.DllImports.Add(pinvokeAttributes);
//                    dllImportId = Context.Instance.DllImports.IndexOf(pinvokeAttributes);
//                }

                var functionCall = String.Format("extern (C) {0} function ({1})", returnString,
                    GetParameterList(methodSymbol.Parameters));
                writer.WriteLine("alias " + functionCall + " " + methodName + "_func_alias;");
                var convertedParameters = ConvertParameters(methodSymbol.Parameters);

                if (attributeArgumentSyntax.Expression is IdentifierNameSyntax)
                {
                    writer.WriteLine(
                        String.Format("auto {1} = cast({2}) __LoadLibraryFunc(__DllImportMap[{0}.text], \"{1}\");",
                            dllImport, methodName, methodName + "_func_alias"));
                }
                else
                {
                    writer.WriteLine(
                       String.Format("auto {1} = cast({2}) __LoadLibraryFunc(__DllImportMap[{0}], \"{1}\");",
                           dllImport, methodName, methodName + "_func_alias"));
                }

                writer.WriteLine(String.Format((returnString != "void" ? "return " : "") + "{0}({1});", methodName,
                    convertedParameters.Count > 0
                        ? convertedParameters.Select(h => h).Aggregate((k, y) => (k) + " ," + y)
                        : ""));


                if (dllImport != null && !Context.Instance.DllImports.Contains(dllImport))
                {
                             
                    var staticWriter = new TempWriter();

                    if (attributeArgumentSyntax.Expression is IdentifierNameSyntax)
                        staticWriter.WriteLine("__SetupDllImport({0}.text);", dllImport);
                    else
                        staticWriter.WriteLine("__SetupDllImport({0});", dllImport);

                    Context.Instance.StaticInits.Add(staticWriter.ToString());

                    Context.Instance.DllImports.Add(dllImport);
                }

            }
            else
            {
               
                var convertedParameters = ConvertParameters(methodSymbol.Parameters);
               writer.WriteLine("//Extern (Internal) Method Call");
                var methodInternalName = TypeProcessor.ConvertType(methodSymbol.ContainingType,false).Replace(".Namespace.","_").Replace(".","_")+ "_" + methodName;

                writer.WriteLine(String.Format((returnString != "void" ? "return " : "") + "{0}({1});", methodInternalName,
                    convertedParameters.Count > 0
                        ? convertedParameters.Select(h => h).Aggregate((k, y) => (k) + " ," + y)
                        : ""));
            }
        }


        public static string GetParameterList(ImmutableArray<IParameterSymbol> paramlist)
        {
            var list = "";
//			list+= ("(");
            var firstParam = true;
            foreach (var parameter in paramlist)
            {
                var refKeyword = "";

                if (parameter.RefKind == RefKind.Ref) //.Any (SyntaxKind.OutKeyword))
                    refKeyword = " ref ";

                if (parameter.RefKind == RefKind.Out)
                    refKeyword = " out ";

//				if (parameter.Modifiers.Any (SyntaxKind.InKeyword))
//					refKeyword = " in ";

//				bool isRef = parameter.Modifiers.Any (SyntaxKind.OutKeyword) || parameter.Modifiers.Any (SyntaxKind.RefKeyword);// || parameter.Modifiers.Any (SyntaxKind.InKeyword);
                if (firstParam)
                    firstParam = false;
                else
                    list += (", ");
                //				var localSymbol = TypeProcessor.GetTypeInfo (parameter.Type);
                //				var ptr = (localSymbol.Type != null && !(localSymbol.Type.IsValueType || localSymbol.Type.TypeKind == TypeKind.TypeParameter));
                //				if (!isRef) {
                //  var s = TypeProcessor.ConvertType(parameter.Type) + " " + ptr;
                //					var s = TypeProcessor.ConvertType (parameter.Type) + " ";
                //					writer.Write (s);
                //				}
                //				else {

                //                    var s = "" + TypeProcessor.ConvertType(parameter.Type) + ptr + "& ";
                var type = ConvertPInvokeType(parameter.Type);

                list += (refKeyword + type + " ");
                //					Program.RefOutSymbols.TryAdd (TypeProcessor.GetDeclaredSymbol (parameter), null); //TODO: clean this completely out, not at all needed for Dlang
                //				}
                list += (WriteIdentifierName.TransformIdentifier(parameter.Name));
//				if (parameter.Default != null) //TODO: centralize this, not needed for pinvoke though
//				{
//					writer.Write (" = ");
//					Core.Write (writer, parameter.Default.Value);
//				}
            }
//			list+= (") ");

            return list;
        }

        private static List<string> ConvertParameters(ImmutableArray<IParameterSymbol> parameters)
        {
            List<string> converted = new List<string>();
            foreach (var parameter in parameters)
            {
                var type = parameter.Type;
                var name = parameter.Name;

                if (type.SpecialType == SpecialType.System_String)
                    // Need to detect if we actually want a native string returned
                    name = "cast(" + ConvertPInvokeType(type) + ")" + name + "";
                else if (ConvertPInvokeType(type) == "char**") //TODO: this should not be a special case:
                    name = "cast(" + ConvertPInvokeType(type) + ")" + name + ".Items";
                if (type.TypeKind == TypeKind.Delegate)
                    name = name + ".Function";

                converted.Add(name);
            }
            return converted;
        }
    }
}