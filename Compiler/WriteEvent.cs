// /*
//   SharpNative - C# to D Transpiler
//   (C) 2014 Irio Systems 
// */

#region Imports

using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

#endregion

namespace SharpNative.Compiler
{
    internal static class WriteEvent
    {
        public static void Go(OutputWriter writer, EventDeclarationSyntax property)
        {
            writer.WriteLine("\r\n");
            var rEf = ""; //" ref ";// ref should be used based on analysis, is the return type a single var or not
            //TODO, doesnt ref make things slower ?, though it makes proprties behave as in c#

            var isInterface = property.Parent is InterfaceDeclarationSyntax;

            var add =
                property.AccessorList.Accessors.SingleOrDefault(
                    o => o.Keyword.RawKind == (decimal) SyntaxKind.AddKeyword);
            var remove =
                property.AccessorList.Accessors.SingleOrDefault(
                    o => o.Keyword.RawKind == (decimal) SyntaxKind.RemoveKeyword);

            var methodSymbol =
                (TypeProcessor.GetDeclaredSymbol(add) ?? TypeProcessor.GetDeclaredSymbol(remove)) as IMethodSymbol;

            Action<AccessorDeclarationSyntax, bool> writeRegion = (region, get) =>
            {
                writer.WriteIndent();

                //                if (property.Modifiers.Any(SyntaxKind.PrivateKeyword))
                //                    writer.HeaderWriter.Write("private:\n");
                //
                //                if (property.Modifiers.Any(SyntaxKind.PublicKeyword) || property.Modifiers.Any(SyntaxKind.InternalKeyword))
                //                    writer.HeaderWriter.Write("public ");
                var typeinfo = TypeProcessor.GetTypeInfo(property.Type);
                var isPtr = "";

                var typeString = TypeProcessor.ConvertType(property.Type) + isPtr + " ";

                if (property.Modifiers.Any(SyntaxKind.AbstractKeyword) || region.Body == null)
                    writer.Write(" abstract ");

                if (property.Modifiers.Any(SyntaxKind.OverrideKeyword))
                    writer.Write(" override ");

                //TODO: look at final and other optimizations
                //				if (!(property.Modifiers.Any (SyntaxKind.VirtualKeyword) || (property.Modifiers.Any (SyntaxKind.AbstractKeyword)) || isInterface || property.Modifiers.Any(SyntaxKind.OverrideKeyword)) ) {
                //					writer.Write(" final ");
                //				}

                //no inline in D
                //	if (get)
                //	{

                //		writer.Write(" " + rEf+ typeString + " "); 
                //                    writer.Write(typeString + " ");
                //	}
                //	else
                //	{
                //                    writer.Write("_=(value");
                //                    writer.Write(TypeProcessor.ConvertTypeWithColon(property.Type));
                //                    writer.Write(")");

                writer.Write("void ");
                //                    writer.Write(" " + "void ");

                //	}
                //not needed for Dlang
                var methodName = (get ? "" : "") +
                                 WriteIdentifierName.TransformIdentifier(property.Identifier.ValueText);
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

                    //   writer.Write((methodSymbol.ContainingType.FullName() + "." + methodName) + (get ? "()" : "( " + typeString + " value )")); //Dealing with explicit VMT7
                }
                if (property.Modifiers.Any(SyntaxKind.NewKeyword))
                    methodName += "_";

                writer.Write(
                    //(!String.IsNullOrEmpty(explicitHeaderNAme) ? explicitHeaderNAme : methodName)
                    (get ? "Add_" : "Remove_") + methodName + "( " + typeString + " value )");

                if (property.Modifiers.Any(SyntaxKind.AbstractKeyword) || region.Body == null)
                    writer.Write(";\r\n");
                else
                {
                    //                    writer.Write(";\r\n");

                    writer.OpenBrace();
                    // writer.Write(" =\r\n");
                    Core.WriteBlock(writer, region.Body.As<BlockSyntax>());

                    writer.CloseBrace();
                    writer.Write("\r\n");
                }
            };

            if (add == null && remove == null)
                throw new Exception("Property must have either a get or a set");

            // if (getter != null && setter != null && setter.Body == null && getter.Body == null)
            {
                //Both get and set are null, which means this is an automatic property.  For our purposes, this is the equivilant of a field//Nope

                //                WriteField.Go(writer,property, property.Modifiers, WriteIdentifierName.TransformIdentifier(property.Identifier.ValueText), property.Type);
                var name = WriteIdentifierName.TransformIdentifier(property.Identifier.ValueText);
                var type = property.Type;
                var typeinfo = TypeProcessor.GetTypeInfo(type);
                var modifiers = property.Modifiers;
                var isPtr = typeinfo.Type != null &&
                            (typeinfo.Type.IsValueType || typeinfo.Type.TypeKind == TypeKind.TypeParameter)
                    ? ""
                    : "";
                var typeString = TypeProcessor.ConvertType(type) + isPtr + " ";
                var isStatic = false;
                //Handle Auto Properties

                var accessors = property.AccessorList.Accessors; //.Where(o=>o.Body==null);
                var accessString = "";
                if (modifiers.Any(SyntaxKind.PrivateKeyword))
                    accessString += (" private ");

                if (modifiers.Any(SyntaxKind.PublicKeyword) || modifiers.Any(SyntaxKind.InternalKeyword) ||
                    modifiers.Any(SyntaxKind.ProtectedKeyword) || modifiers.Any(SyntaxKind.AbstractKeyword) ||
                    isInterface)
                    accessString += (" public ");

                //				if (!(property.Modifiers.Any (SyntaxKind.VirtualKeyword) || (property.Modifiers.Any (SyntaxKind.AbstractKeyword)))) {
                //					writer.Write(" final ");
                //				}

                //                if (modifiers.Any(SyntaxKind.PublicKeyword) || method.Modifiers.Any(SyntaxKind.InternalKeyword) || modifiers.Any(SyntaxKind.ProtectedKeyword) || modifiers.Any(SyntaxKind.AbstractKeyword))
                //                        writer.HeaderWriter.WriteLine("public: ");

                var IsStatic = "";

                if (modifiers.Any(SyntaxKind.StaticKeyword))
                {
                    isStatic = true;
                    IsStatic = accessString += " static ";
                    //  writer.HeaderWriter.Write("static ");
                }

                var fieldName = "__evt__" + name;
                if (!isStatic)
                {
                    writer.Write("private " + "__Event!(" + typeString + ") " + fieldName + ";\r\n");
                        // Internal Field used for event
                    writer.Write(accessString);
                    writer.WriteLine("__Event!(" + typeString + ") " + name + "() @property");
                    writer.OpenBrace();

                    writer.WriteLine("if (" + fieldName + " is null)");
                    writer.OpenBrace();
                    writer.Write(fieldName + " =  new " + "__Event!(" + typeString + ")(new Action_T!(" + typeString +
                                 ")(&Add_" + name + "),new Action_T!(" + typeString + ")(&Remove_" + name + ") );");
                    writer.CloseBrace();
                    writer.Write("return " + fieldName + ";");
                    writer.CloseBrace();
                }
                else
                {
                    writer.Write(IsStatic);
                    writer.Write("__Event!(" + typeString + ") " + name + ";\r\n");
                }

//
                var isOverride = property.Modifiers.Any(SyntaxKind.NewKeyword) ||
                                 property.Modifiers.Any(SyntaxKind.OverrideKeyword)
                    ? " override "
                    : "";
                var isVirtual = //property.Modifiers.Any(SyntaxKind.VirtualKeyword) ||
                    property.Modifiers.Any(SyntaxKind.AbstractKeyword) || isInterface
                        ? " abstract "
                        : "";
//				if(!isInterface)
//				if ((add!=null && add.Body == null) ||
//					(remove != null && remove.Body == null) && (!property.Modifiers.Any(SyntaxKind.AbstractKeyword)))
//				{
//
//					writer.Write (IsStatic);
//					var initString = "";//isStatic?"":(" = " + TypeProcessor.DefaultValue (type));
//					writer.Write (typeString + fieldName + initString + ";\r\n");
//
//				}

                //Adder
                if (add != null && add.Body == null)
                {
                    writer.Write(IsStatic);
                    //if (property.Modifiers.Any(SyntaxKind.AbstractKeyword)||isInterface)
                    {
                        writer.WriteLine(" " + isVirtual + " void Add_" + name + "(" + typeString + " value)" +
                                         isOverride + "" + ";");
                    }
//					else
//						writer.WriteLine(" "+isVirtual + " void Add_" + name + "(" + typeString + " value)" +
//							isOverride + " {" + fieldName + " = value;}");
                }
                else if (add != null)
                {
                    writer.Write(IsStatic);
                    writeRegion(add, true);
                }

                //Remover
                if (remove != null && remove.Body == null)
                {
                    writer.Write(IsStatic);
                    //if (property.Modifiers.Any(SyntaxKind.AbstractKeyword)||isInterface)
                    {
                        writer.WriteLine(" " + isVirtual + " void Remove_" + name + "(" + typeString + " value)" +
                                         isOverride + "" + ";");
                    }
                    //else
                    //	writer.WriteLine(" "+isVirtual + " void Remove_" + name + "(" + typeString + " value)" +
                    //		isOverride + " {" + fieldName + " = value;}");
                }
                else if (remove != null)
                {
                    writer.Write(IsStatic);
                    writeRegion(remove, false);
                }

                if (isStatic)
                {
                    var staticWriter = new OutputWriter("", "", false);

                    staticWriter.Write(name);

                    staticWriter.Write(" =  new " + "__Event!(" + typeString + ")(new Action_T!(" + typeString +
                                       ")(__ToDelegate(&Add_" + name + ")),new Action_T!(" + typeString +
                                       ")(__ToDelegate(&Remove_" + name + ")) )");

                    staticWriter.Write(";");

                    staticWriter.WriteLine();

                    Context.Instance.StaticInits.Add(staticWriter.ToString());
                }
                else
                {
                    var staticWriter = new OutputWriter("", "", false);

                    staticWriter.Write(name);

                    staticWriter.Write(" =  new " + "__Event!(" + typeString + ")(new Action_T!(" + typeString +
                                       ")((&Add_" + name + ")),new Action_T!(" + typeString + ")((&Remove_" + name +
                                       ")) )");

                    staticWriter.Write(";");

                    staticWriter.WriteLine();

                    Context.Instance.InstanceInits.Add(staticWriter.ToString());
                }
//						if (CSharpExtensions.CSharpKind (initializerOpt.Value) == SyntaxKind.CollectionInitializerExpression || CSharpExtensions.CSharpKind (initializerOpt.Value) == SyntaxKind.ArrayInitializerExpression)
//						{
//
//
//							staticWriter.Write ("new " + typeStringNoPtr + " ([");
//
//
//						}
//						Core.Write (staticWriter, initializerOpt.Value);
//						if (CSharpExtensions.CSharpKind (initializerOpt.Value) == SyntaxKind.CollectionInitializerExpression || CSharpExtensions.CSharpKind (initializerOpt.Value) == SyntaxKind.ArrayInitializerExpression)
//						{
//
//
//							staticWriter.Write ("])");
//						}
            }
        }

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

                    */

        //                if (isStatic)
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
}