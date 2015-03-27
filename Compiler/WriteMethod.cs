// /*
//   SharpNative - C# to D Transpiler
//   (C) 2014 Irio Systems 
// */

#region Imports

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SharpNative.Compiler.YieldAsync;

#endregion

namespace SharpNative.Compiler
{
    internal static class WriteMethod
    {

        public static string GetParameterListAsString(ImmutableArray<IParameterSymbol> parameters, bool includeTypes = true, ITypeSymbol iface = null, bool writebraces = true)
        {

            var writer = new TempWriter(); // Temp Writer
            if (writebraces)
                writer.Write("(");
            var firstParam = true;
            foreach (var parameter in parameters)
            {
                var refKeyword = "";

                if (parameter.RefKind==RefKind.Ref)
                    refKeyword = " ref ";

                if (parameter.RefKind==RefKind.Out)
                    refKeyword = " out ";

             //   if (parameter.RefKind==RefKind.None) //????
               //     refKeyword = " in ";

                bool isRef = parameter.RefKind == RefKind.Ref || parameter.RefKind == RefKind.Out;// parameter.Modifiers.Any(SyntaxKind.OutKeyword) ||
                           //  parameter.Modifiers.Any(SyntaxKind.RefKeyword);
                // || parameter.Modifiers.Any (SyntaxKind.InKeyword);
                if (firstParam)
                    firstParam = false;
                else
                    writer.Write(", ");

                var isParams = parameter.IsParams;//.Any(SyntaxKind.ParamsKeyword);

                //if (!isParams)
                {
                    var s = refKeyword + TypeProcessor.ConvertType(parameter.Type) + " ";
                    if (includeTypes)
                        writer.Write(s);
                }
                /* else //Framework depends too much on managed arrays even for params
                 {
                     var type = (TypeProcessor.GetTypeInfo(parameter.Type).Type ?? TypeProcessor.GetTypeInfo(parameter.Type).ConvertedType) as IArrayTypeSymbol;
                     var s = refKeyword + TypeProcessor.ConvertType(type.ElementType) + "[]";

                     if (includeTypes)
                         writer.Write(s);

                 }*/

                writer.Write(WriteIdentifierName.TransformIdentifier(parameter.Name));
                if (!parameter.HasExplicitDefaultValue)
                    continue;
                writer.Write(" = ");
                Core.Write(writer, (SyntaxNode) parameter.ExplicitDefaultValue); //TODO ??
            }
            if (iface != null)
            {
                if (firstParam)
                    firstParam = false;
                else
                    writer.Write(", ");

                writer.Write(TypeProcessor.ConvertType(iface) + " __j = null");
            }
            if (writebraces)
                writer.Write(")");
            return writer.ToString();
        }


        public static string GetParameterListAsString(SeparatedSyntaxList<ParameterSyntax> parameters,  bool includeTypes = true, ITypeSymbol iface=null, bool writebraces = true)
        {
           
            var writer = new TempWriter(); // Temp Writer
            if(writebraces)
            writer.Write("(");
            var firstParam = true;
                foreach (var parameter in parameters)
            {
                var refKeyword = "";

                if (parameter.Modifiers.Any(SyntaxKind.RefKeyword))
                    refKeyword = " ref ";

                if (parameter.Modifiers.Any(SyntaxKind.OutKeyword))
                    refKeyword = " out ";

                if (parameter.Modifiers.Any(SyntaxKind.InKeyword))
                    refKeyword = " in ";

                bool isRef = parameter.Modifiers.Any(SyntaxKind.OutKeyword) ||
                             parameter.Modifiers.Any(SyntaxKind.RefKeyword);
                    // || parameter.Modifiers.Any (SyntaxKind.InKeyword);
                if (firstParam)
                    firstParam = false;
                else
                    writer.Write(", ");

                var isParams = parameter.Modifiers.Any(SyntaxKind.ParamsKeyword);

                //if (!isParams)
                {
                    var s = refKeyword + TypeProcessor.ConvertType(parameter.Type) + " ";
                    if (includeTypes)
                        writer.Write(s);
                }
               /* else //Framework depends too much on managed arrays even for params
                {
                    var type = (TypeProcessor.GetTypeInfo(parameter.Type).Type ?? TypeProcessor.GetTypeInfo(parameter.Type).ConvertedType) as IArrayTypeSymbol;
                    var s = refKeyword + TypeProcessor.ConvertType(type.ElementType) + "[]";

                    if (includeTypes)
                        writer.Write(s);

                }*/

                writer.Write(WriteIdentifierName.TransformIdentifier(parameter.Identifier.ValueText));
                if (parameter.Default == null)
                    continue;
                writer.Write(" = ");
                Core.Write(writer, parameter.Default.Value);
            }
            if (iface != null)
            {
                if (firstParam)
                    firstParam = false;
                else
                    writer.Write(", ");
               
                writer.Write(TypeProcessor.ConvertType(iface) + " __j = null");
            }
            if (writebraces)
                writer.Write(")");
            return writer.ToString();
        }



        public static void WriteIt(OutputWriter writer, MethodDeclarationSyntax method, bool isProxy = true)
        {
            var methodSymbol = (IMethodSymbol) TypeProcessor.GetDeclaredSymbol(method);
            var isYield = YieldChecker.HasYield(method);//method.DescendantNodes().OfType<YieldStatementSyntax>().Any();


            writer.WriteLine();




            var pinvokeAttributes = method.GetAttribute(Context.DllImport);
            var isInternalPInvoke = pinvokeAttributes == null && method.Modifiers.Any(SyntaxKind.ExternKeyword);

            //TODO: Improve partial class / method support -- partials classes work, methods need minor work ...
            if (method.Modifiers.Any(SyntaxKind.PartialKeyword) && method.Body == null)
            {
                //We only want to render out one of the two partial methods.  If there's another, skip this one.
                if (Context.Instance.Partials.SelectMany(o => o.Syntax.As<ClassDeclarationSyntax>().Members)
                    .OfType<MethodDeclarationSyntax>()
                    .Except(method).Any(o => o.Identifier.ValueText == method.Identifier.ValueText))
                    return;
            }



            var accessString = "";



            var isInterface = method.Parent is InterfaceDeclarationSyntax;

            ITypeSymbol iface;
            ISymbol[] proxies;
            var methodName = MemberUtilities.GetMethodName(method, ref isInterface, out iface, out proxies); //
            if (methodName == "Main" /*&& method.Modifiers.Any(SyntaxKind.PublicKeyword)*/&&
                method.Modifiers.Any(SyntaxKind.StaticKeyword))
            {
                accessString = ("public ");

                accessString += ("static ");

                var methodCall = methodSymbol.ContainingNamespace.FullName() + "." +
                                 methodSymbol.ContainingType.FullName() +
                                 "." + methodName + (method.ParameterList.Parameters.Count < 1 ? "();" : "(null);");
                //: "(new Array_T!(String)(args));"); // for now args not supported
                Context.Instance.EntryMethod = methodCall;
            }

            else
            {
                accessString = MemberUtilities.GetAccessModifiers(method, isInterface);
            }


            var returnTypeString = TypeProcessor.ConvertType(method.ReturnType, true) + " ";
            var methodSignatureString = "";
            if (method.ReturnType.ToString() == "void")
                returnTypeString = ("void ");

            methodSignatureString += methodName;

            var genericParameters = "";

            if (method.TypeParameterList != null)
            {
                var genericArgs = method.TypeParameterList.Parameters.ToList();

                //				if (genericArgs.Count > 0)
                //				{
                //					writer.Write("( ");
                //					writer.Write(string.Join(" , ", genericArgs.Select(o => " " + o)));
                //					writer.Write(" )\r\n");
                //				}

                if (genericArgs.Count > 0) // && !methodSymbol.ContainingType.IsGenericType) // doesnt matter
                {
                    genericParameters += ("(");
                    genericParameters += (string.Join(",", genericArgs.Select(o => o)));
                    genericParameters += (")");
                }
            }
            methodSignatureString += genericParameters;
            var @params = GetParameterListAsString(method.ParameterList.Parameters,
                iface: proxies == null ? iface : null);

            string constraints = GetMethodConstraints(method);

            if (isInterface || method.Modifiers.Any(SyntaxKind.AbstractKeyword))
            {
                writer.WriteLine(accessString + returnTypeString + methodSignatureString + @params + constraints + ";");

                return;
            }

            writer.WriteLine(accessString + returnTypeString + methodSignatureString + @params + constraints);

            writer.OpenBrace();

            if (isProxy)
            {
                @params = GetParameterListAsString(method.ParameterList.Parameters, includeTypes: false);

                if (method.ReturnType.ToString() == "void")
                    writer.WriteLine("__Value." + methodName + @params + ";");
                else
                    writer.WriteLine("return __Value." + methodName + @params + ";");
            }
            else
            {

                if (!isProxy && isYield)
                {
                    var namedTypeSymbol = methodSymbol.ReturnType as INamedTypeSymbol;
                    if (namedTypeSymbol != null)
                    {
                       // var iteratortype = namedTypeSymbol.TypeArguments[0];

                        var className =  methodSymbol.GetYieldClassName() +(
                   ( ((INamedTypeSymbol)methodSymbol.ReturnType).TypeArguments.Any() && ((INamedTypeSymbol)methodSymbol.ReturnType).TypeArguments[0].TypeKind==TypeKind.TypeParameter) ? "__G" : "");


                        methodSignatureString = methodName + genericParameters;
                        var @params2 = GetParameterListAsString(method.ParameterList.Parameters, iface: methodSymbol.ContainingType);
                        var @params3 = GetParameterListAsString(method.ParameterList.Parameters, iface: null,
                            includeTypes: false,writebraces:false);

                        // writer.WriteLine(accessString + returnTypeString + methodSignatureString + @params2 + constraints);

                        //writer.OpenBrace();

                        if (!methodSymbol.IsStatic)
                        {
                            if(method.ParameterList.Parameters.Count >0)
                            writer.WriteLine("return new " + className + "(this," + @params3  +");");
                            else
                            {
                            writer.WriteLine("return new " + className + "(this);");

                            }
                        }
                        else
                        {
                            writer.WriteLine("return new " + className + "(" + @params3 + ");");

                        }

                       

                    }
                }
                else if (method.Body != null)
                {
                    foreach (var statement in method.Body.Statements)
                    {
                        
                        Core.Write(writer, statement);
                    }

                    TriviaProcessor.ProcessTrivias(writer, method.Body.DescendantTrivia());
                }

                if (pinvokeAttributes != null)
                    WritePInvokeMethodBody.Go(writer, methodName, methodSymbol, pinvokeAttributes);

                if (isInternalPInvoke)
                    WritePInvokeMethodBody.Go(writer, methodName, methodSymbol, null);

                if (!isProxy && isYield)
                {
                    //writer.WriteLine("});");

                }
            }

            writer.CloseBrace();

            if (proxies != null)
            {
                foreach (var proxy in proxies)
                {
                    //Need to write proxy signature here ...

                    methodSignatureString = methodName + genericParameters;
                    var @params2 = GetParameterListAsString(method.ParameterList.Parameters, iface: proxy.ContainingType);
                    var @params3 = GetParameterListAsString(method.ParameterList.Parameters, iface: null,
                        includeTypes: false);

                    writer.WriteLine(accessString + returnTypeString + methodSignatureString + @params2 + constraints);

                    writer.OpenBrace();

                    if (method.ReturnType.ToString() == "void")
                        writer.WriteLine("" + methodName + @params3 + ";");
                    else
                        writer.WriteLine("return " + methodName + @params3 + ";");

                    writer.CloseBrace();
                }
            }

          
        }

        private static string GetMethodConstraints(MethodDeclarationSyntax method)
        {
            string constraints = "";
            if (method.ConstraintClauses.Count > 0)
            {
                constraints += (" if (");
                bool isFirst = true;
                foreach (var constraint in method.ConstraintClauses)
                {
                    foreach (var condition in constraint.Constraints)
                    {
                        string dlangCondition = condition.ToString();

                        if (dlangCondition == "new()") // haven't got around to this yet
                            continue;
                        if (dlangCondition == "class") // TODO: is there a better way to do this ?
                            dlangCondition = "NObject";

                        if (dlangCondition == "struct")
                            constraints += ((isFirst ? "" : "&&") + " !is(" + constraint.Name + " : NObject)");
                        else
                        {
							//TODO: fix this up better
							constraints += ((isFirst ? "" : "&&") + " is(" + constraint.Name + " : " + dlangCondition.Replace("<","!(").Replace(">",")") +
                                            ")");
                        }

                        isFirst = false;

                        //								Console.WriteLine (condition);
                    }
                }

                constraints += (")");
            }
            return constraints;
        }


        public static void Go(OutputWriter writer, MethodDeclarationSyntax method)
        {
            WriteIt(writer, method, false);
        }
    }
}