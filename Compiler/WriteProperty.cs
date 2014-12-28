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
        private static void CppStuff()
        {
            //                writer.Write(IsStatic);
            /*
                MSVC++ cannot work with anonymous lambdas (works well with gcc and clang thouugh) .... ? why

                    this doesnt work

                       Property<double > Area = Property<double >(
		[&](){ return get_Area(); },
		[&](double  value){ set_Area(value); })

                this is the fix
                     Property<double > Area = Property<double >(
		std::function< double() >([&](){ return get_Area(); }),
		std::function< void(double) >([&](double  value){ set_Area(value); })
	);

                    */ //                if (isStatic)
            //                {
            //                    writer.WriteLine(String.Format(@"Property<{0}> {1};", typeString, name));
            //                    writer.WriteLine(String.Format(@"Property<{0}> {2}.{1} = Property<{0}>(
            //		std::function< {0}()> ([&](){{ return {2}::get_{1}(); }}),
            //		std::function< void({0}) > ([&]({0} value){{ {2}::set_{1}(value); }})
            //	);", typeString, name, (methodSymbol.ContainingType.FullName())));
            //
            //              
            //
            //                }
            //                else
            //                {
            //                    writer.WriteLine(String.Format(@"Property<{0}> {1} = Property<{0}>(
            //		std::function< {0}()> ([&](){{ return get_{1}(); }}),
            //		std::function< void({0}) > ([&]({0} value){{ set_{1}(value); }})
            //	);", typeString, name, (methodSymbol.ContainingType.FullName() + "." + name)));
            //                }
        }

        public static void Go(OutputWriter writer, PropertyDeclarationSyntax property, bool isProxy = false)
        {
            writer.WriteLine();
            //var rEf = "";//" ref ";// ref should be used based on analysis, is the return type a single var or not
            //TODO, doesnt ref make things slower ?, though it makes proprties behave as in c#

            var isInterface = property.Parent is InterfaceDeclarationSyntax;

            var getter =
                property.AccessorList.Accessors.SingleOrDefault(
                    o => o.Keyword.RawKind == (decimal) SyntaxKind.GetKeyword);
            var setter =
                property.AccessorList.Accessors.SingleOrDefault(
                    o => o.Keyword.RawKind == (decimal) SyntaxKind.SetKeyword);
            var methodSymbol = TypeProcessor.GetDeclaredSymbol(property);
            var name = WriteIdentifierName.TransformIdentifier(property.Identifier.ValueText);

            if (methodSymbol.ContainingType.TypeKind == TypeKind.Interface)
                isInterface = true;

            if (methodSymbol.ContainingType.TypeKind == TypeKind.Interface ||
                Equals(methodSymbol.ContainingType.FindImplementationForInterfaceMember(methodSymbol), methodSymbol))
            {
                name = Regex.Replace(
                    TypeProcessor.ConvertType(methodSymbol.ContainingType.ConstructedFrom) + "_" + name,
                    @" ?!\(.*?\)", string.Empty);
            }

            if (name.Contains(".")) // Explicit Interface method
            {
                //				
                name = name.SubstringAfterLast('.');
                name = name.Replace('.', '_');
            }

            var interfaceMethods =
                methodSymbol.ContainingType.AllInterfaces.SelectMany(
                    u =>
                        u.GetMembers(name)).ToArray();

            ISymbol interfaceMethod =
                interfaceMethods.FirstOrDefault(
                    o => methodSymbol.ContainingType.FindImplementationForInterfaceMember(o) == methodSymbol);

            if (interfaceMethod != null)
            {
                //This is an interface property //TO
                if (methodSymbol.ContainingType.SpecialType == SpecialType.System_Array)
                    writer.Write("");
                else
                {
                    var typenameI =
                        Regex.Replace(TypeProcessor.ConvertType(interfaceMethod.ContainingType.ConstructedFrom),
                            @" ?!\(.*?\)", string.Empty);
                        //TODO: we should be able to get the original interface name, or just remove all generics from this

                    if (typenameI.Contains('.'))
                        typenameI = typenameI.SubstringAfterLast('.');
                    name = (typenameI + "_") + name;
                }
            }

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

                var methodName = (get ? "" : "") +
                                 name;
                var explicitHeaderNAme = "";

                if (property.Modifiers.Any(SyntaxKind.NewKeyword))
                    methodName += "_";

                writer.Write((!String.IsNullOrEmpty(explicitHeaderNAme) ? explicitHeaderNAme : methodName) +
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
                var typeinfo = TypeProcessor.GetTypeInfo(type);
                var modifiers = property.Modifiers;
                var isPtr = "";
                var typeString = TypeProcessor.ConvertType(type) + isPtr + " ";
                var isStatic = false;

                var acccessmodifiers = "";

                if (modifiers.Any(SyntaxKind.PrivateKeyword))
                    acccessmodifiers += ("private ");

                if (modifiers.Any(SyntaxKind.PublicKeyword) || modifiers.Any(SyntaxKind.InternalKeyword) ||
                    modifiers.Any(SyntaxKind.ProtectedKeyword) || modifiers.Any(SyntaxKind.AbstractKeyword) ||
                    isInterface)
                    acccessmodifiers += ("public ");

                if (property.Modifiers.Any(SyntaxKind.AbstractKeyword))
                    acccessmodifiers += "abstract ";

                if (property.Modifiers.Any(SyntaxKind.OverrideKeyword) && !isInterface)
                    acccessmodifiers += "override ";

                //TODO: look at final and other optimizations

                var IsStatic = "";

                if (modifiers.Any(SyntaxKind.StaticKeyword))
                {
                    isStatic = true;
                    IsStatic = "static ";
                    acccessmodifiers += IsStatic;
                }

                //Auto Property
                var fieldName = "__prop_" + name;

                var isOverride = (property.Modifiers.Any(SyntaxKind.NewKeyword) ||
                                  property.Modifiers.Any(SyntaxKind.OverrideKeyword)) && !isInterface
                    ? " override "
                    : "";
//                var isVirtual = //property.Modifiers.Any(SyntaxKind.VirtualKeyword) ||
//                    property.Modifiers.Any(SyntaxKind.AbstractKeyword) || isInterface
//                        ? " abstract "
//                        : "";

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