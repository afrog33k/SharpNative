// /*
//   SharpNative - C# to D Transpiler
//   (C) 2014 Irio Systems 
// */

#region Imports

using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

#endregion

namespace SharpNative.Compiler
{
    internal static class WriteMethod
    {
        public static string GetParameterListAsString(ParameterListSyntax list, bool includeTypes = true)
        {
            var writer = new TempWriter(); // Temp Writer
            writer.Write("(");
            var firstParam = true;
            foreach (var parameter in list.Parameters)
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

                var s = refKeyword + TypeProcessor.ConvertType(parameter.Type) + " ";
                if (includeTypes)
                    writer.Write(s);

                writer.Write(WriteIdentifierName.TransformIdentifier(parameter.Identifier.ValueText));
                if (parameter.Default == null)
                    continue;
                writer.Write(" = ");
                Core.Write(writer, parameter.Default.Value);
            }
            writer.Write(")");
            return writer.ToString();
        }

        private static bool CompareMethods(IMethodSymbol interfaceMethod, IMethodSymbol methodSymbol)
        {
            if (interfaceMethod == null || methodSymbol == null)
                return false;
            return interfaceMethod.Name == methodSymbol.Name && interfaceMethod.ReturnType == methodSymbol.ReturnType &&
                   interfaceMethod.Parameters == methodSymbol.Parameters &&
                   interfaceMethod.TypeArguments == methodSymbol.TypeArguments;
        }


        public static void WriteIt(OutputWriter writer, MethodDeclarationSyntax method, bool isProxy = true)
        {
            writer.WriteLine();
            var methodSymbol = (IMethodSymbol) TypeProcessor.GetDeclaredSymbol(method);
            var methodName = OverloadResolver.MethodName(methodSymbol);

            var pinvokeAttributes = method.GetAttribute(Context.DllImport);
                // Fix this to actually look for the type, not just by name ...

            //TODO: Improve partial class / method support -- partials classes work, methods need minor work ...
            if (method.Modifiers.Any(SyntaxKind.PartialKeyword) && method.Body == null)
            {
                //We only want to render out one of the two partial methods.  If there's another, skip this one.
                if (Context.Instance.Partials.SelectMany(o => o.Syntax.As<ClassDeclarationSyntax>().Members)
                    .OfType<MethodDeclarationSyntax>()
                    .Except(method).Any(o => o.Identifier.ValueText == method.Identifier.ValueText))
                    return;
            }

            bool isoverride = ShouldUseOverrideKeyword(method, methodSymbol);

            var accessString = "";
            if (isoverride)
                accessString += (" override ");

            var isInterface = method.Parent is InterfaceDeclarationSyntax;
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
                if (method.Modifiers.Any(SyntaxKind.PublicKeyword) || method.Modifiers.Any(SyntaxKind.InternalKeyword) ||
                    method.Modifiers.Any(SyntaxKind.ProtectedKeyword) ||
                    method.Modifiers.Any(SyntaxKind.AbstractKeyword) || isInterface)
                    accessString += ("public ");

                if (method.Modifiers.Any(SyntaxKind.PrivateKeyword))
                    accessString += ("private ");

                //	            if (method.Modifiers.Any(SyntaxKind.VirtualKeyword) || isInterface)
                //	                writer.Write("virtual ");

                //				if (!(method.Modifiers.Any (SyntaxKind.VirtualKeyword) || (method.Modifiers.Any (SyntaxKind.AbstractKeyword) || isInterface || isoverride))) {
                //					writer.Write(" final ");
                //				}

                if (method.Modifiers.Any(SyntaxKind.AbstractKeyword))
                    accessString += (" abstract ");

                if (method.Modifiers.Any(SyntaxKind.StaticKeyword))
                    accessString += ("static ");
            }

            //	        if (isInterface)
            //	        {
            //                writer.IsInterface = true;
            //	        }

            var returnTypeString = TypeProcessor.ConvertType(method.ReturnType) + " ";
            var methodSignatureString = "";
            if (method.ReturnType.ToString() == "void")
                returnTypeString = ("void ");
            else
                //  bool returnsVoid = method.ReturnType.ToString() == "void";
                //if (!returnsVoid)
            {
                var typeSymbol = TypeProcessor.GetTypeInfo(method.ReturnType).Type;

                //   var isPtr = typeSymbol != null && (typeSymbol.IsValueType || typeSymbol.TypeKind == TypeKind.TypeParameter) ? "" : "";
                //     var typeString = TypeProcessor.ConvertType(method.ReturnType) + isPtr + " ";

                //	            writer.Write(typeString);
                //	            writer.HeaderWriter.Write(typeString);

                var isPtr = typeSymbol != null &&
                            (!typeSymbol.IsValueType || typeSymbol.TypeKind == TypeKind.TypeParameter);
            }

            if (methodSymbol.ContainingType.TypeKind == TypeKind.Interface ||
                Equals(methodSymbol.ContainingType.FindImplementationForInterfaceMember(methodSymbol), methodSymbol))
            {
                methodName =
                    Regex.Replace(
                        TypeProcessor.ConvertType(methodSymbol.ContainingType.ConstructedFrom) + "_" + methodName,
                        @" ?!\(.*?\)", string.Empty);
            }

            if (methodName.Contains(".")) // Explicit Interface method
            {
                //				
                methodName = methodName.SubstringAfterLast('.');
                methodName = methodName.Replace('.', '_');
            }

            //			var typenameI = Regex.Replace (TypeProcessor.ConvertType (interfaceMethod.ContainingType), @" ?!\(.*?\)", string.Empty);

            var interfaceMethods =
                methodSymbol.ContainingType.AllInterfaces.SelectMany(
                    u =>
                        u.GetMembers(methodName)).ToArray();

            ISymbol interfaceMethod =
                interfaceMethods.FirstOrDefault(
                    o => methodSymbol.ContainingType.FindImplementationForInterfaceMember(o) == methodSymbol);

            if (interfaceMethod == null)
            {
                //TODO: fix this for virtual method test 7, seems roslyn cannot deal with virtual 
                // overrides of interface methods ... so i'll provide a kludge
                if (!method.Modifiers.Any(SyntaxKind.NewKeyword))
                {
                    interfaceMethod =
                        interfaceMethods.FirstOrDefault(k => CompareMethods(k as IMethodSymbol, methodSymbol));
                }
            }

            if (interfaceMethod != null)
                // && CompareMethods(interfaceMethod ,methodSymbol)) {
            {
//This is an interface method //TO
                if (methodSymbol.ContainingType.SpecialType == SpecialType.System_Array)
                    methodSignatureString += ("");
                else
                {
                    var typenameI =
                        Regex.Replace(TypeProcessor.ConvertType(interfaceMethod.ContainingType.ConstructedFrom),
                            @" ?!\(.*?\)", string.Empty);
                        //TODO: we should be able to get the original interface name, or just remove all generics from this
                    if (typenameI.Contains('.'))
                        typenameI = typenameI.SubstringAfterLast('.');
                    methodName = (typenameI + "_") + methodName;
                }
            }

            //            var explicitHeaderNAme = "";
            //FIX ME: To support explicits, all method calls are going to be prequalified with the interface name, lets just hope we dont have similar interfaces
            //
            // if (methodSymbol.MethodKind == MethodKind.ExplicitInterfaceImplementation)
            //	        {
            //	           // var implementations = methodSymbol.ExplicitInterfaceImplementations[0];
            //	            if (implementations != null)
            //	            {
            //	              //  explicitHeaderNAme = implementations.Name;
            //                     methodName = TypeProcessor.ConvertType(implementations.ReceiverType) + "_" +implementations.Name; //Explicit fix ?
            //
            //	                //			writer.Write(methodSymbol.ContainingType + "." + methodName);
            //	                //Looks like internal classes are not handled properly here ...
            //	            }
            //	        }
            // methodName = methodName.Replace(methodSymbol.ContainingNamespace.FullName() + ".", methodSymbol.ContainingNamespace.FullName().Replace(".", "::") + "::");
            //           (methodSymbol as).Is
            //	        (methodSymbol as Microsoft.CodeAnalysis.CSharp.Symbols.SourceMethodSymbol);
            if (method.Modifiers.Any(SyntaxKind.NewKeyword))
                methodName += "_";

            //            writer.Write( TypeProcessor.ConvertType(methodSymbol.ContainingType)+ "." + methodName); //Dealting with explicit VMT7
            // writer.Write(!String.IsNullOrEmpty(explicitHeaderNAme)? explicitHeaderNAme : methodName);
            //            writer.Write(methodName);
            methodSignatureString += methodName;

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
                    methodSignatureString += ("(");
                    methodSignatureString += (string.Join(",", genericArgs.Select(o => " " + o)));
                    methodSignatureString += (")");
                }
            }

            var @params = GetParameterListAsString(method.ParameterList);

            var constraints = "";

            if (method.ConstraintClauses.Count > 0)
            {
                constraints += (" if (");
                bool isFirst = true;
                foreach (var constraint in method.ConstraintClauses)
                {
                    foreach (var condition in constraint.Constraints)
                    {
                        string dlangCondition = condition.ToString();

                        if (dlangCondition == "new()")
                            continue;
                        if (dlangCondition == "class") // TODO: is there a better way to do this ?
                            dlangCondition = "NObject";

                        if (dlangCondition == "struct")
                            constraints += ((isFirst ? "" : "&&") + " !is(" + constraint.Name + " : NObject)");
                        else
                        {
                            constraints += ((isFirst ? "" : "&&") + " is(" + constraint.Name + " : " + dlangCondition +
                                            ")");
                        }

                        isFirst = false;

                        //								Console.WriteLine (condition);
                    }
                }

                constraints += (")");
            }

            if (isInterface || method.Modifiers.Any(SyntaxKind.AbstractKeyword))
            {
                writer.WriteLine(accessString + returnTypeString + methodSignatureString + @params + constraints + ";");

                return;
            }

            writer.WriteLine(accessString + returnTypeString + methodSignatureString + @params + constraints);

            writer.OpenBrace();

            if (isProxy)
            {
                @params = GetParameterListAsString(method.ParameterList, false);

                if (method.ReturnType.ToString() == "void")
                    writer.WriteLine("Value." + methodName + @params + ";");
                else
                    writer.WriteLine("return Value." + methodName + @params + ";");
            }
            else
            {
                if (method.Body != null)
                {
                    foreach (var statement in method.Body.Statements)
                        Core.Write(writer, statement);

                    TriviaProcessor.ProcessTrivias(writer, method.Body.DescendantTrivia());
                }

                if (pinvokeAttributes != null)
                    WritePInvokeMethodBody.Go(writer, methodName, methodSymbol, pinvokeAttributes);
            }

            writer.CloseBrace();
        }


        public static void Go(OutputWriter writer, MethodDeclarationSyntax method)
        {
            WriteIt(writer, method, false);
        }

        private static bool ShouldUseOverrideKeyword(MethodDeclarationSyntax method, IMethodSymbol symbol)
        {
            if (symbol.ContainingType.TypeKind == TypeKind.Struct ||
                symbol.ContainingType.TypeKind == TypeKind.Interface)
            {
                return false;
                    // Structs dont have a base class to override (maybe opEquals) ... but that will be handled separately
                //Interfaces are contracts, so no overriding here// maybe we should compare the methods 
            }
            if (method.Modifiers.Any(SyntaxKind.StaticKeyword))
                return false;
            //			if (method.Modifiers.Any(SyntaxKind.NewKeyword))
            //				return  symbol.ContainingType.BaseType.GetMembers(symbol.Name).Any(k=>k.IsAbstract || k.IsVirtual);

            if (method.Modifiers.Any(SyntaxKind.PartialKeyword))
                //partial methods seem exempt from C#'s normal override keyword requirement, so we have to check manually to see if it exists in a base class
                return symbol.ContainingType.BaseType.GetMembers(symbol.Name).Any();

            return method.Modifiers.Any(SyntaxKind.OverrideKeyword);
        }

        public static string TypeParameter(TypeParameterSyntax prm, IMethodSymbol methodSymbol,
            MethodDeclarationSyntax methodSyntax)
        {
            var identifier = Utility.TypeConstraints(prm, methodSyntax.ConstraintClauses);

            return identifier;
        }
    }
}