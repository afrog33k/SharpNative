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
      

        public static void Go(OutputWriter writer, IndexerDeclarationSyntax indexer)
        {
            var rEf = ""; //" ref ";// ref should be used based on analysis, is the return type a single var or not
            //TODO, doesnt ref make things slower ?, though it makes proprties behave as in c#

            var isInterface = indexer.Parent is InterfaceDeclarationSyntax;

            var getter =
                indexer.AccessorList.Accessors.SingleOrDefault(
                    o => o.Keyword.RawKind == (decimal) SyntaxKind.GetKeyword);
            var setter =
                indexer.AccessorList.Accessors.SingleOrDefault(
                    o => o.Keyword.RawKind == (decimal) SyntaxKind.SetKeyword);
            var methodSymbol =
                (TypeProcessor.GetDeclaredSymbol(getter) ?? TypeProcessor.GetDeclaredSymbol(setter)) as IMethodSymbol;


            ITypeSymbol iface;
            ISymbol[] proxies;
            var name = MemberUtilities.GetMethodName(indexer, ref isInterface, out iface, proxies: out proxies);

            Action<AccessorDeclarationSyntax, bool> writeRegion = (region, get) =>
            {
                var typeinfo = TypeProcessor.GetTypeInfo(indexer.Type);
                var isPtr = "";

                var typeString = TypeProcessor.ConvertType(indexer.Type) + isPtr + " ";

                if (indexer.Modifiers.Any(SyntaxKind.AbstractKeyword) || region.Body == null)
                    writer.Write(" abstract ");

                if (indexer.Modifiers.Any(SyntaxKind.OverrideKeyword))
                    writer.Write(" override ");

                //TODO: look at final and other optimizations

                //no inline in D
                if (get)
                    writer.Write(rEf + typeString + " ");
                else
                    writer.Write("void ");

                var methodName = (get ? "opIndex" : "opIndexAssign");

                

                var explicitHeaderNAme = "";

                if (indexer.Modifiers.Any(SyntaxKind.NewKeyword))
                    methodName += "_";

                var parameters =  WriteMethod.GetParameterListAsString(indexer.ParameterList.Parameters, iface: proxies == null ? iface : null, writebraces:false);

                writer.Write((!String.IsNullOrEmpty(explicitHeaderNAme) ? explicitHeaderNAme : methodName) + "(" +
                             (get ? "" : (typeString + " value, ")) + parameters + ")");

                if (indexer.Modifiers.Any(SyntaxKind.AbstractKeyword) || region.Body == null)
                    writer.Write(";\r\n");
                else
                {
                    Core.WriteBlock(writer, region.Body.As<BlockSyntax>());
                    writer.Write("\r\n");
                }
            };

            if (getter == null && setter == null)
                throw new Exception("indexer must have either a get or a set");
            {
                var type = indexer.Type;
                var typeinfo = TypeProcessor.GetTypeInfo(type);
                var modifiers = indexer.Modifiers;
               
                var typeString = TypeProcessor.ConvertType(type)  + " ";
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

            if (proxies != null)
            {
                foreach (var proxy in proxies)
                {
                    //Need to write proxy signature here ...

                  /*  methodSignatureString = methodName + genericParameters;
                    var @params2 = GetParameterListAsString(method.ParameterList.Parameters, iface: proxy.ContainingType);
                    var @params3 = GetParameterListAsString(method.ParameterList.Parameters, iface: null, includeTypes: false);

                    writer.WriteLine(accessString + returnTypeString + methodSignatureString + @params2 + constraints);

                    writer.OpenBrace();

                    if (method.ReturnType.ToString() == "void")
                        writer.WriteLine("" + methodName + @params3 + ";");
                    else
                        writer.WriteLine("return " + methodName + @params3 + ";");

                    writer.CloseBrace();
                    */
                }
            }
        }
    }
}