// /*
//   SharpNative - C# to D Transpiler
//   (C) 2014 Irio Systems 
// */

#region Imports

using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

#endregion

namespace SharpNative.Compiler
{
    internal static class WriteIndexer
    {
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
                var type = TypeProcessor.ConvertType(parameter.Type);

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

        public static void Go(OutputWriter writer, IndexerDeclarationSyntax property)
        {
//			writer.WriteLine ("\r\n");
            var rEf = ""; //" ref ";// ref should be used based on analysis, is the return type a single var or not
            //TODO, doesnt ref make things slower ?, though it makes proprties behave as in c#

            var isInterface = property.Parent is InterfaceDeclarationSyntax;

            var getter =
                property.AccessorList.Accessors.SingleOrDefault(
                    o => o.Keyword.RawKind == (decimal) SyntaxKind.GetKeyword);
            var setter =
                property.AccessorList.Accessors.SingleOrDefault(
                    o => o.Keyword.RawKind == (decimal) SyntaxKind.SetKeyword);
            var methodSymbol =
                (TypeProcessor.GetDeclaredSymbol(getter) ?? TypeProcessor.GetDeclaredSymbol(setter)) as IMethodSymbol;

            Action<AccessorDeclarationSyntax, bool> writeRegion = (region, get) =>
            {
                var typeinfo = TypeProcessor.GetTypeInfo(property.Type);
                var isPtr = "";

                var typeString = TypeProcessor.ConvertType(property.Type) + isPtr + " ";

                if (property.Modifiers.Any(SyntaxKind.AbstractKeyword) || region.Body == null)
                    writer.Write(" abstract ");

                if (property.Modifiers.Any(SyntaxKind.OverrideKeyword))
                    writer.Write(" override ");

                //TODO: look at final and other optimizations

                //no inline in D
                if (get)
                    writer.Write(rEf + typeString + " ");
                else
                    writer.Write("void ");

                var methodName = (get ? "opIndex" : "opIndexAssign");
                var explicitHeaderNAme = "";
                if (methodSymbol != null && methodSymbol.MethodKind == MethodKind.ExplicitInterfaceImplementation)
                {
                    var implementations = methodSymbol.ExplicitInterfaceImplementations[0];
                    if (implementations != null)
                    {
                        explicitHeaderNAme = implementations.Name;
                        methodName = //implementations.ReceiverType.FullName() + "." +
                            implementations.Name; //Explicit fix ?

                        //			writer.Write(methodSymbol.ContainingType + "." + methodName);
                        //Looks like internal classes are not handled properly here ...
                    }
                }
                if (methodSymbol != null)
                {
                    // methodName = methodName.Replace(methodSymbol.ContainingNamespace.FullName() + ".", methodSymbol.ContainingNamespace.FullName() + ".");

                    //   writer.Write((methodSymbol.ContainingType.FullName() + "." + methodName) + (get ? "()" : "( " + typeString + " value )")); //Dealting with explicit VMT7
                }
                if (property.Modifiers.Any(SyntaxKind.NewKeyword))
                    methodName += "_";

                var parameters = GetParameterList(methodSymbol.Parameters);

                writer.Write((!String.IsNullOrEmpty(explicitHeaderNAme) ? explicitHeaderNAme : methodName) + "( " +
                             (get ? "" : (typeString + " value, ")) + parameters + " )");

                if (property.Modifiers.Any(SyntaxKind.AbstractKeyword) || region.Body == null)
                    writer.Write(";\r\n");
                else
                {
                    writer.OpenBrace();
                    Core.WriteBlock(writer, region.Body.As<BlockSyntax>());

                    writer.CloseBrace();
                    writer.Write("\r\n");
                }
            };

            if (getter == null && setter == null)
                throw new Exception("Property must have either a get or a set");

            {
//				var name = WriteIdentifierName.TransformIdentifier (property..ValueText);
                var type = property.Type;
                var typeinfo = TypeProcessor.GetTypeInfo(type);
                var modifiers = property.Modifiers;
                var isPtr = typeinfo.Type != null &&
                            (typeinfo.Type.IsValueType || typeinfo.Type.TypeKind == TypeKind.TypeParameter)
                    ? ""
                    : "";
                var typeString = TypeProcessor.ConvertType(type) + isPtr + " ";
                var isStatic = false;

                var acccessmodifiers = "";

                if (modifiers.Any(SyntaxKind.PrivateKeyword))
                    acccessmodifiers += ("private ");

                if (modifiers.Any(SyntaxKind.PublicKeyword) || modifiers.Any(SyntaxKind.InternalKeyword) ||
                    modifiers.Any(SyntaxKind.ProtectedKeyword) || modifiers.Any(SyntaxKind.AbstractKeyword) ||
                    isInterface)
                    acccessmodifiers += ("public ");

                var IsStatic = "";

                if (modifiers.Any(SyntaxKind.StaticKeyword))
                {
                    isStatic = true;
                    IsStatic = "static ";
                    acccessmodifiers += IsStatic;
                }

                //Auto Property

//				var isOverride = property.Modifiers.Any (SyntaxKind.NewKeyword) ||
//				                 property.Modifiers.Any (SyntaxKind.OverrideKeyword)
//					? " override "
//					: "";
//				var isVirtual = //property.Modifiers.Any(SyntaxKind.VirtualKeyword) ||
//					property.Modifiers.Any (SyntaxKind.AbstractKeyword) || isInterface
//					? " abstract "
//					: "";
//				if (!isInterface)

                //Getter
                if (getter != null)
                {
                    writer.WriteIndent();
                    writer.Write(acccessmodifiers);
                    writeRegion(getter, true);
                }

                //Setter
                if (setter != null)
                {
                    writer.WriteIndent();
                    writer.Write(acccessmodifiers);
                    writeRegion(setter, false);
                }
            }
        }
    }
}