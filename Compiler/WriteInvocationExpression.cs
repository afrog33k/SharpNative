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
using CSharpExtensions = Microsoft.CodeAnalysis.CSharpExtensions;
using VisualBasicExtensions = Microsoft.CodeAnalysis.VisualBasic.VisualBasicExtensions;

#endregion

namespace SharpNative.Compiler
{
    internal static class WriteInvocationExpression
    {
        public static void Go(OutputWriter writer, InvocationExpressionSyntax invocationExpression)
        {
            var symbolInfo = TypeProcessor.GetSymbolInfo(invocationExpression);
            var expressionSymbol = TypeProcessor.GetSymbolInfo(invocationExpression.Expression);

            var symbol = symbolInfo.Symbol ?? symbolInfo.CandidateSymbols.FirstOrDefault(); // Resolution error

            var methodSymbol = symbol.OriginalDefinition.As<IMethodSymbol>().UnReduce();

            var memberReferenceExpressionOpt = invocationExpression.Expression as MemberAccessExpressionSyntax;
            var firstParameter = true;

            var extensionNamespace = methodSymbol.IsExtensionMethod
                ? methodSymbol.ContainingNamespace.FullNameWithDot() + methodSymbol.ContainingType.FullName()
                : null; //null means it's not an extension method, non-null means it is
            string methodName;
            string typeParameters = null;
            ExpressionSyntax subExpressionOpt;

          

            if (expressionSymbol.Symbol is IEventSymbol)
            {
                methodName = "Invoke";
                   
            }
            else if (methodSymbol.MethodKind == MethodKind.DelegateInvoke)
                methodName = null;
            else
                methodName = OverloadResolver.MethodName(methodSymbol);

            if (methodSymbol.MethodKind == MethodKind.DelegateInvoke)
                subExpressionOpt = invocationExpression.Expression;
            else if (memberReferenceExpressionOpt != null)
            {
                if (memberReferenceExpressionOpt.Expression is PredefinedTypeSyntax)
                    subExpressionOpt = null;
                else
                    subExpressionOpt = memberReferenceExpressionOpt.Expression;
            }
            else
                subExpressionOpt = null;

            //When the code specifically names generic arguments, include them in the method name ... dmd needs help with inference, so we give it the types anyway
            if (methodSymbol.IsGenericMethod)
            {

                //return TryConvertType(named) + (named.TypeKind == TypeKind.Struct && o.ConstraintTypes.Any(k => k.TypeKind == //TypeKind.Interface) ? ".__Boxed_" : "");

                var named = ((IMethodSymbol)symbol);
                typeParameters = "!(" +
                named.TypeArguments.Select(o => TypeProcessor.GetGenericParameterType(named.TypeParameters[named.TypeArguments.IndexOf(o)], o)).Aggregate((a, b) => a + ", " + b)
                + ")";

//                typeParameters = "!( " +
//                                 string.Join(", ",
//                                     ((IMethodSymbol) symbol).TypeArguments.Select(r => TypeProcessor.ConvertType(r)  )) +
//                                 " )";
            }

            //Determine if it's an extension method called in a non-extension way.  In this case, just pretend it's not an extension method
            if (extensionNamespace != null && subExpressionOpt != null &&
                TypeProcessor.GetTypeInfo(subExpressionOpt).ConvertedType.ToString() ==
                methodSymbol.ContainingNamespace.FullName() + "." + methodSymbol.ContainingType.FullName())
                extensionNamespace = null;

            var memberType = memberReferenceExpressionOpt == null
                ? null
                : TypeProcessor.GetTypeInfo(memberReferenceExpressionOpt.Expression).Type;
            var isNullableEnum = memberType != null &&
                                 (memberType.Name == "Nullable" && memberType.ContainingNamespace.FullName() == "System") &&
                                 memberType.As<INamedTypeSymbol>().TypeArguments.Single().TypeKind == TypeKind.Enum;
            //          if (isNullableEnum && methodSymbol.Name == "ToString")
            //          {
            //              extensionNamespace = null; //override Translations.xml for nullable enums. We want them to convert to the enum's ToString method
            //              methodName = "toString";
            //          }

            //Invocation on basics should come from boxing the basic then calling on the boxed type, unless static
            var directInvocationOnBasics = methodSymbol.ContainingType.IsBasicType() && methodSymbol.IsStatic;
            //&& methodSymbol.ContainingType!=Context.Instance.Type; //If we are currently working on a basic type e.g. in corlib, don't alter the code

            //Extension methods in Dlang are straightforward, although this could lead to clashes without qualification

            if (extensionNamespace != null || directInvocationOnBasics)
            {
                if (extensionNamespace == null)
                {
                    extensionNamespace = TypeProcessor.ConvertType(methodSymbol.ContainingType, true, false);
                    // methodSymbol.ContainingNamespace.FullName() + "." + methodSymbol.ContainingType.Name;
                    //memberType.ContainingNamespace.FullName() +"."+ memberType.Name;
                }

                writer.Write(extensionNamespace);

                if (methodName != null)
                {
                    methodName = WriteIdentifierName.TransformIdentifier(methodName);
                    //                    if (symbolInfo.Symbol.ContainingType != Context.Instance.Type)
                    writer.Write(".");
                    writer.Write(methodName);
                }

                WriteTypeParameters(writer, typeParameters, invocationExpression);

                writer.Write("(");

                if (subExpressionOpt != null)
                {
                    firstParameter = false;
                    Core.Write(writer, subExpressionOpt);
                }
            }
            else
            {
                if (memberReferenceExpressionOpt != null)
                {
                   

                       
                    
                }

                if (subExpressionOpt != null)
                {
                    WriteMemberAccessExpression.WriteMember(writer, subExpressionOpt);

                    //                    if (!(subExpressionOpt is BaseExpressionSyntax))
                    //                    {
                    //                        if (methodName != null && methodSymbol.IsStatic)
                    //                            writer.Write(".");
                    //                        else
                    //                      {   
                    if (methodSymbol.MethodKind != MethodKind.DelegateInvoke)
                        writer.Write(".");
                    //                        }
                    //                    }
                    //                  writer.Write(".");
                }
                else if (methodSymbol.IsStatic && extensionNamespace == null)
                {
                    if (methodSymbol.ContainingType != Context.Instance.Type)
                    {
                        var str = TypeProcessor.ConvertType(methodSymbol.ContainingType);
                        if (str == "Array_T")
                            // Array is the only special case, otherwise generics have to be specialized to access static members
                            str = "Array";

                        writer.Write(str);
                        //                    writer.Write(methodSymbol.ContainingNamespace.FullNameWithDot());
                        //                    writer.Write(WriteType.TypeName(methodSymbol.ContainingType));
                        writer.Write(".");
                    }
                }

                if (methodSymbol.MethodKind != MethodKind.DelegateInvoke)
                {
                    var declaringSyntaxReferences =
                        methodSymbol.DeclaringSyntaxReferences.Select(j => j.GetSyntax())
                            .OfType<MethodDeclarationSyntax>();
                    var any = declaringSyntaxReferences.Any();
                    if (any &&
                        declaringSyntaxReferences.FirstOrDefault()
                            .As<MethodDeclarationSyntax>()
                            .Modifiers.Any(SyntaxKind.NewKeyword))
                    {
                        //TODO: this means that new is not supported on external libraries
                        //                  //why doesnt roslyn give me this information ?
                        methodName += "_";
                    }

                    if (any &&
                        declaringSyntaxReferences.FirstOrDefault()
                            .As<MethodDeclarationSyntax>()
                            .Modifiers.Any(SyntaxKind.NewKeyword))
                    {
//                        if (symbolInfo.Symbol.ContainingType != Context.Instance.Type)
                        writer.Write(TypeProcessor.ConvertType(methodSymbol.ContainingType) + ".");
                    }

                    //TODO: fix this for abstract too or whatever other combination
                    //TODO: create a better fix fot this
                    //                  ISymbol interfaceMethod =
                    //                      methodSymbol.ContainingType.AllInterfaces.SelectMany (
                    //                          u =>
                    //                      u.GetMembers (methodName)
                    //                      .Where (
                    //                              o =>
                    //                          methodSymbol.ContainingType.FindImplementationForInterfaceMember (o) ==
                    //                              methodSymbol)).FirstOrDefault ();

                    /*      if (methodSymbol.ContainingType.TypeKind == TypeKind.Interface ||
                Equals(methodSymbol.ContainingType.FindImplementationForInterfaceMember(methodSymbol), methodSymbol))

                              if ((interfaceMethod != null /*&& interfaceMethod == methodSymbol || methodSymbol.ContainingType.TypeKind == TypeKind.Interface)
                          {
                              //This is an interface method //TO

                              var typenameM = Regex.Replace (TypeProcessor.ConvertType (methodSymbol.ContainingType), @" ?!\(.*?\)", string.Empty);
                              if (typenameM.Contains ('.'))
                                  typenameM = typenameM.SubstringAfterLast ('.');
                              if (methodSymbol.ContainingType.SpecialType == SpecialType.System_Array)
                                  writer.Write ("");
                              else if (interfaceMethod != null)
                              {
                                  var typenameI = Regex.Replace (TypeProcessor.ConvertType (interfaceMethod.ContainingType), @" ?!\(.*?\)", string.Empty);
                                  if (typenameI.Contains ('.'))
                                      typenameI = typenameI.SubstringAfterLast ('.');                       
                                  writer.Write (typenameI + "_");
                              }
                              else // this is the interface itself
                                      writer.Write (typenameM + "_");

                          }*/

                    if (methodSymbol.ContainingType.TypeKind == TypeKind.Interface ||
                        Equals(methodSymbol.ContainingType.FindImplementationForInterfaceMember(methodSymbol),
                            methodSymbol))
                    {
                        /*    methodName =
                            Regex.Replace(TypeProcessor.ConvertType(methodSymbol.ContainingType.OriginalDefinition)
                                    .RemoveFromStartOfString(methodSymbol.ContainingNamespace + ".Namespace.") + "_" +
                                methodName,
                                @" ?!\(.*?\)", string.Empty);*/
                        if (methodSymbol.ContainingType.ContainingType != null)
                            methodName = methodName.RemoveFromStartOfString(methodSymbol.ContainingType.ContainingType.Name + ".");
                    }

                    var interfaceMethods =
                        methodSymbol.ContainingType.AllInterfaces.SelectMany(
                            u =>
                                u.GetMembers(methodName)).ToArray();

                    ISymbol interfaceMethod =
                        interfaceMethods.FirstOrDefault(
                            o => methodSymbol.ContainingType.FindImplementationForInterfaceMember(o) == methodSymbol);

                    //                    if (interfaceMethod == null)
                    //                    {
                    //                        //TODO: fix this for virtual method test 7, seems roslyn cannot deal with virtual 
                    //                        // overrides of interface methods ... so i'll provide a kludge
                    //                        if (!method.Modifiers.Any(SyntaxKind.NewKeyword))
                    //                            interfaceMethod = interfaceMethods.FirstOrDefault(k => CompareMethods(k as IMethodSymbol, methodSymbol));
                    //                    }

                    if (interfaceMethod != null)
                        // && CompareMethods(interfaceMethod ,methodSymbol)) {
                    {
//This is an interface method //TO
                        if (methodSymbol.ContainingType.SpecialType == SpecialType.System_Array)
                            writer.Write("");
                        else
                        {
                            /* var typenameI =
                                Regex.Replace(
                                    TypeProcessor.ConvertType(interfaceMethod.ContainingType.ConstructedFrom, true),
                                    @" ?!\(.*?\)", string.Empty);
                                //TODO: we should be able to get the original interface name, or just remove all generics from this
                            if (typenameI.Contains('.'))
                                typenameI = typenameI.SubstringAfterLast('.');
                            writer.Write(typenameI + "_");*/
                        }
                    }

                    //                  var acc = methodSymbol.DeclaredAccessibility;

                    //                  if (methodSymbol.MethodKind == MethodKind.ExplicitInterfaceImplementation)
                    //                  {
                    //                      var implementations = methodSymbol.ExplicitInterfaceImplementations[0];
                    //                      if (implementations != null)
                    //                      {
                    //                          //  explicitHeaderNAme = implementations.Name;
                    //                          methodName = TypeProcessor.ConvertType(implementations.ReceiverType) + "_" +implementations.Name; //Explicit fix ?
                    //
                    //                          //          writer.Write(methodSymbol.ContainingType + "." + methodName);
                    //                          //Looks like internal classes are not handled properly here ...
                    //                      }
                    //                  }
                    methodName = WriteIdentifierName.TransformIdentifier(methodName);

                    writer.Write(methodName);
                }
                WriteTypeParameters(writer, typeParameters, invocationExpression);
                writer.Write("(");
            }

            bool inParams = false;
            bool foundParamsArray = false;

            var arguments = invocationExpression.ArgumentList.Arguments;

            ITypeSymbol typeSymbol = null;

            bool isOverloaded = methodSymbol.ContainingType.GetMembers(methodSymbol.Name).OfType<IMethodSymbol>().Any(j => j.TypeParameters == methodSymbol.TypeParameters && ParameterMatchesWithRefOutIn(methodSymbol, j));
            foreach (var arg in arguments.Select(o => new TransformedArgument(o)))
            {
                if (firstParameter)
                    firstParameter = false;
                else
                    writer.Write(", ");

                var argumentType = TypeProcessor.GetTypeInfo(arg.ArgumentOpt.Expression);

                if (!inParams && IsParamsArgument(invocationExpression, arg.ArgumentOpt, methodSymbol))
                {
                    foundParamsArray = true;
                    typeSymbol = TypeProcessor.GetTypeInfo(arg.ArgumentOpt.Expression).ConvertedType;

                    if (
                        !TypeProcessor.ConvertType(typeSymbol)
                            .StartsWith("Array_T"))
                    {
                        inParams = true;
                        // var elemType = TypeProcessor.ConvertType((typeSymbol as IArrayTypeSymbol).ElementType);
                        var s = TypeProcessor.ConvertType(typeSymbol);
                        writer.Write("__ARRAY!(" + s + ")([");
                    }
                }

                //Not needed for dlang
                //                if (arg != null
                //                    && arg.ArgumentOpt.RefOrOutKeyword.RawKind != (decimal) SyntaxKind.None
                //                    && TypeProcessor.GetSymbolInfo(arg.ArgumentOpt.Expression).Symbol is IFieldSymbol)
                //                {
                //                    
                //                }
                //                    throw new Exception("ref/out cannot reference fields, only local variables.  Consider using ref/out on a local variable and then assigning it into the field. " + Utility.Descriptor(invocationExpression));

                //              if (argumentType.Type == null) {
                //                  if (argumentType.ConvertedType == null)
                //                      writer.Write ("null");
                //                  else
                //                      writer.Write ("(cast("+TypeProcessor.ConvertType(argumentType.ConvertedType)+") null)");
                //              }
                //               
                //                else if (argumentType.Type.IsValueType && !argumentType.ConvertedType.IsValueType)
                //                {
                //                    //Box
                //                  writer.Write("BOX!("+TypeProcessor.ConvertType(argumentType.Type) +")(");
                //                    //When passing an argument by ref or out, leave off the .Value suffix
                //                    if (arg != null && arg.ArgumentOpt.RefOrOutKeyword.RawKind != (decimal)SyntaxKind.None)
                //                        WriteIdentifierName.Go(writer, arg.ArgumentOpt.Expression.As<IdentifierNameSyntax>(), true);
                //                    else
                //                        arg.Write(writer);
                //
                //                    writer.Write(")");
                //                }
                //                else if (!argumentType.Type.IsValueType && argumentType.ConvertedType.IsValueType)
                //                {
                //                    //UnBox
                //                  writer.Write("cast(" + TypeProcessor.ConvertType(argumentType.Type) + ")(");
                //                    if (arg != null && arg.ArgumentOpt.RefOrOutKeyword.RawKind != (decimal)SyntaxKind.None)
                //                        WriteIdentifierName.Go(writer, arg.ArgumentOpt.Expression.As<IdentifierNameSyntax>(), true);
                //                    else
                //                        arg.Write(writer);
                //                    writer.Write(")");
                //                }
                //                else
                //                {
                //                    if (arg != null && arg.ArgumentOpt.RefOrOutKeyword.RawKind != (decimal)SyntaxKind.None)
                //                        WriteIdentifierName.Go(writer, arg.ArgumentOpt.Expression.As<IdentifierNameSyntax>(), true);
                //                    else
                //                        arg.Write(writer);
                //                }
                ProcessArgument(writer, arg.ArgumentOpt, isOverloaded && argumentType.Type.IsValueType && (arg.ArgumentOpt.RefOrOutKeyword.RawKind == (decimal)SyntaxKind.None));
            }

            if (inParams)
            {
                writer.Write("])");
            }

            if (!foundParamsArray && methodSymbol.Parameters.Any() && methodSymbol.Parameters.Last().IsParams)
            {
                if (typeSymbol != null)
                {
                    var s = TypeProcessor.ConvertType(typeSymbol);
                    writer.Write("__ARRAY!(" + s + ")([])");

                }
                else
                    writer.Write("null"); //params method called without any params argument.  Send null.
             }

            if (symbol.ContainingType.TypeKind == TypeKind.Interface) // Need it as specialized as possible
            {
                if (!firstParameter)
                    writer.Write(",");
                writer.Write("cast({0}) null",TypeProcessor.ConvertType(symbol.ContainingType));
            }
        
            writer.Write(")");
        }

        private static bool ParameterMatchesWithRefOutIn(IMethodSymbol methodSymbol, IMethodSymbol methodSymbol1)
        {
            if (methodSymbol.Parameters.Count() != methodSymbol1.Parameters.Count())
                return false;
            for (int index = 0; index < methodSymbol.Parameters.Count(); index++)
            {
                var parameterSymbol = methodSymbol.Parameters[index];
                if (parameterSymbol.Type != methodSymbol1.Parameters[index].Type)
                    return false;

                if (parameterSymbol.RefKind == methodSymbol1.Parameters[index].RefKind)
                    return false;
            }
            return true;
        }


        private static void ProcessArgument(OutputWriter writer, ArgumentSyntax variable, bool isOverloaded = false)
        {
            if (variable != null)
            {
                if (variable.NameColon != null)
                {
                }

                if (CSharpExtensions.CSharpKind(variable) == SyntaxKind.CollectionInitializerExpression)
                    return;
                var value = variable;
                var initializerType = TypeProcessor.GetTypeInfo(value.Expression);
                var memberaccessexpression = value.Expression as MemberAccessExpressionSyntax;
                var nameexpression = value.Expression as NameSyntax;
                var nullAssignment = value.ToFullString().Trim() == "null";
                var shouldBox = initializerType.Type != null && ((initializerType.Type.IsValueType || initializerType.Type.TypeKind == TypeKind.TypeParameter) &&
                    (!initializerType.ConvertedType.IsValueType));
                var shouldCast=  initializerType.Type != null && (initializerType.Type.TypeKind==TypeKind.Interface && initializerType.ConvertedType.SpecialType==SpecialType.System_Object);
                var shouldUnBox = initializerType.Type != null && !initializerType.Type.IsValueType &&
                                  initializerType.ConvertedType.IsValueType;
                var isname = value.Expression is NameSyntax;
                var ismemberexpression = value.Expression is MemberAccessExpressionSyntax ||
                                         (isname &&
                                         TypeProcessor.GetSymbolInfo(value.Expression as NameSyntax).Symbol.Kind ==
                                         SymbolKind.Method);
                var isdelegateassignment = ismemberexpression &&
                                           initializerType.ConvertedType!=null&& initializerType.ConvertedType.TypeKind == TypeKind.Delegate;
                var isstaticdelegate = isdelegateassignment &&
                                       ((memberaccessexpression != null &&
                                       TypeProcessor.GetSymbolInfo(memberaccessexpression).Symbol.IsStatic) ||
                                       (isname && TypeProcessor.GetSymbolInfo(nameexpression).Symbol.IsStatic));

               
                if (isOverloaded)
                {
                    writer.Write("cast(const({0}))", TypeProcessor.ConvertType(initializerType.Type));
                }

                if (shouldCast)
                {
                    writer.Write("cast(NObject)");
                    writer.Write("(");
                    Core.Write(writer, value.Expression);
                    writer.Write(")");
                    return;
                }

                if (nullAssignment)
                {
                    writer.Write("null");
                    return;
                }

                if (shouldBox)
                {
                    bool useType = true;

                    //We should start with exact converters and then move to more generic convertors i.e. base class or integers which are implicitly convertible
                    var correctConverter = initializerType.Type.GetImplicitCoversionOp(initializerType.ConvertedType,
                                               initializerType.Type, true);
                    //                            initializerType.Type.GetMembers("op_Implicit").OfType<IMethodSymbol>().FirstOrDefault(h => h.ReturnType == initializerType.Type && h.Parameters[0].Type == initializerType.ConvertedType);

                    if (correctConverter == null)
                    {
                        useType = false;
                        correctConverter =
                            initializerType.ConvertedType.GetImplicitCoversionOp(initializerType.ConvertedType,
                            initializerType.Type, true);
                        //.GetMembers("op_Implicit").OfType<IMethodSymbol>().FirstOrDefault(h => h.ReturnType == initializerType.Type && h.Parameters[0].Type == initializerType.ConvertedType);
                    }

                    if (correctConverter != null)
                    {
                        if (useType)
                        {
                            writer.Write(TypeProcessor.ConvertType(initializerType.Type) + "." + "op_Implicit_" +
                                TypeProcessor.ConvertType(correctConverter.ReturnType).Replace(".", "_"));
                        }
                        else
                        {
                            writer.Write(TypeProcessor.ConvertType(initializerType.ConvertedType) + "." + "op_Implicit_" +
                                TypeProcessor.ConvertType(correctConverter.ReturnType).Replace(".", "_"));
                        }
                        writer.Write("(");
                        Core.Write(writer, value.Expression);
                        writer.Write(")");
                        return;
                    }
                }
                else if (shouldUnBox)
                {
                    bool useType = true;

                    //We should start with exact converters and then move to more generic convertors i.e. base class or integers which are implicitly convertible
                    var correctConverter = initializerType.Type.GetImplicitCoversionOp(initializerType.Type,
                                               initializerType.ConvertedType, true);
                    //                            initializerType.Type.GetMembers("op_Implicit").OfType<IMethodSymbol>().FirstOrDefault(h => h.ReturnType == initializerType.Type && h.Parameters[0].Type == initializerType.ConvertedType);

                    if (correctConverter == null)
                    {
                        useType = false;
                        correctConverter =
                            initializerType.ConvertedType.GetImplicitCoversionOp(initializerType.Type,
                            initializerType.ConvertedType, true);
                        //.GetMembers("op_Implicit").OfType<IMethodSymbol>().FirstOrDefault(h => h.ReturnType == initializerType.Type && h.Parameters[0].Type == initializerType.ConvertedType);
                    }

                    if (correctConverter != null)
                    {
                        if (useType)
                        {
                            writer.Write(TypeProcessor.ConvertType(initializerType.Type) + "." + "op_Implicit_" +
                                TypeProcessor.ConvertType(correctConverter.ReturnType).Replace(".", "_"));
                        }
                        else
                        {
                            writer.Write(TypeProcessor.ConvertType(initializerType.ConvertedType) + "." + "op_Implicit_" +
                                TypeProcessor.ConvertType(correctConverter.ReturnType).Replace(".", "_"));
                        }
                        writer.Write("(");
                        Core.Write(writer, value.Expression);
                        writer.Write(")");
                        return;
                    }
                }



                if (shouldBox)
                {
                    
                    //Box
                    writer.Write("BOX!(" + TypeProcessor.ConvertType(initializerType.Type) + ")(");
                    //When passing an argument by ref or out, leave off the .Value suffix
                    Core.Write(writer, value.Expression);
                    writer.Write(")");
                    return;
                }
                if (shouldUnBox)
                {
                    //UnBox
                    writer.Write("Cast!(" + TypeProcessor.ConvertType(initializerType.Type) + ")(");
                    Core.Write(writer, value.Expression);
                    writer.Write(")");
                }
                if (isdelegateassignment)
                {
                    var typeString = TypeProcessor.ConvertType(initializerType.ConvertedType);

                    var createNew = !(value.Expression is ObjectCreationExpressionSyntax);

                    if (createNew)
                    {
                        if (initializerType.ConvertedType.TypeKind == TypeKind.TypeParameter)
                            writer.Write(" __TypeNew!(" + typeString + ")(");
                        else
                            writer.Write("new " + typeString + "(");
                    }

                    var isStatic = isstaticdelegate;
                    //                    if (isStatic)
                    //                        writer.Write("__ToDelegate(");
                    MemberUtilities.WriteMethodPointer(writer, value.Expression);
                    //                    if (isStatic)
                    //                        writer.Write(")");

                    if (createNew)
                        writer.Write(")");
                    return;
                }
                if (initializerType.Type == null && initializerType.ConvertedType == null)
                {
                    writer.Write("null");
                    return;
                }
                Core.Write(writer, value.Expression);
            }
        }


        private static void WriteTypeParameters(OutputWriter writer, string typeParameters,
            InvocationExpressionSyntax invoke)
        {
            if (typeParameters != null)
                writer.Write(typeParameters);
        }

        private static bool IsParamsArgument(InvocationExpressionSyntax invocationExpression, ArgumentSyntax argumentOpt,
            IMethodSymbol methodSymbol)
        {
            if (argumentOpt == null)
                return false;

            if (invocationExpression.ArgumentList.Arguments.Any(o => o.NameColon != null))
                return false; //params cannot be used with named arguments

            int i = invocationExpression.ArgumentList.Arguments.IndexOf(argumentOpt);
            if(i>=methodSymbol.Parameters.Length)
                return false;
            return methodSymbol.Parameters.ElementAt(i).IsParams;
        }

       

       
    }
}