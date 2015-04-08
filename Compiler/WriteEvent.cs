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

namespace SharpNative.Compiler //TODO: clean up this code and its output
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
            var eventSymbol = TypeProcessor.GetDeclaredSymbol(property);

            var methodSymbol =
                (TypeProcessor.GetDeclaredSymbol(add) ?? TypeProcessor.GetDeclaredSymbol(remove)) as IMethodSymbol;

            ITypeSymbol interfaceImplemented;
            ISymbol[] proxies;
            ;

            var methodName = WriteIdentifierName.TransformIdentifier(MemberUtilities.GetMethodName(eventSymbol, ref isInterface, out interfaceImplemented, out proxies));

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

               
                writer.Write("void ");
             
               
             

                writer.Write(
                    (get ? "Add_" : "Remove_") + methodName + "( " + typeString + " value");

                if (isInterface)
                {
                    writer.WriteLine(" , " + TypeProcessor.ConvertType(interfaceImplemented) + " __ij = null");
                }

                    writer.Write(" )");

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
                throw new Exception("Event must have both a add and remove");

            {
               
                var name = WriteIdentifierName.TransformIdentifier(property.Identifier.Text);
                var type = property.Type;
                var typeinfo = TypeProcessor.GetTypeInfo(type);
                var modifiers = property.Modifiers;
                var typeString = TypeProcessor.ConvertType(type)  + " ";
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

             

                var IsStatic = "";

                if (modifiers.Any(SyntaxKind.StaticKeyword))
                {
                    isStatic = true;
                    IsStatic = accessString += " static ";
                    //  writer.HeaderWriter.Write("static ");
                }

                var fieldName = "__evt__" + name + (interfaceImplemented!=null?TypeProcessor.ConvertType(interfaceImplemented)
                    .Replace("(","_").Replace("!", "_").Replace(")", "_").Replace(".", "_") :"");
                if (!(property.Parent is InterfaceDeclarationSyntax))
                {
                    if (!isStatic)
                    {
                        writer.Write("private " + "__Event!(" + typeString + ") " + fieldName + ";\r\n");
                        // Internal Field used for event
                        writer.Write(accessString);
                        writer.WriteLine("__Event!(" + typeString + ") " + name + "(" + (interfaceImplemented!=null ? (  TypeProcessor.ConvertType(interfaceImplemented) + " __ij = null") :"")+") @property");
                        writer.OpenBrace();

                        writer.WriteLine("if (" + fieldName + " is null)");
                        writer.OpenBrace();
                        writer.Write(fieldName + " =  new " + "__Event!(" + typeString + ")(new Action__G!(" +
                                     typeString +
                                     ")(&Add_" + name + "),new Action__G!(" + typeString + ")(&Remove_" + name + ") );");
                        writer.CloseBrace();
                        writer.Write("return " + fieldName + ";");
                        writer.CloseBrace();
                    }
                    else
                    {
                        writer.Write(IsStatic);
                        writer.Write("__Event!(" + typeString + ") " + name + ";\r\n");
                    }
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

                    staticWriter.Write(" =  new " + "__Event!(" + typeString + ")(new Action__G!(" + typeString +
                                       ")(&Add_" + name + "),new Action__G!(" + typeString +
                                       ")(&Remove_" + name + ") )");

                    staticWriter.Write(";");

                    staticWriter.WriteLine();

                    Context.Instance.StaticInits.Add(staticWriter.ToString());
                }

            }
        }

     
    }
}