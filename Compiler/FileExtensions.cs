using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeGeneration;
using Accessibility = Microsoft.CodeAnalysis.Accessibility;
using IEventSymbol = Microsoft.CodeAnalysis.IEventSymbol;
using IMethodSymbol = Microsoft.CodeAnalysis.IMethodSymbol;
using IPropertySymbol = Microsoft.CodeAnalysis.IPropertySymbol;
using ISymbol = Microsoft.CodeAnalysis.ISymbol;
using SymbolDisplayFormat = Microsoft.CodeAnalysis.SymbolDisplayFormat;
using SymbolKind = Microsoft.CodeAnalysis.SymbolKind;

namespace SharpNative.Compiler
{

    internal static partial class ISymbolExtensions
    {
       
        public static Accessibility GetResultantVisibility(this ISymbol symbol)
        {
            
            // Start by assuming it's visible.
            var visibility = Accessibility.Public;

            switch (symbol.Kind)
            {
                case SymbolKind.Alias:
                    // Aliases are uber private.  They're only visible in the same file that they
                    // were declared in.
                    return Accessibility.Private;

                case SymbolKind.Parameter:
                    // Parameters are only as visible as their containing symbol
                    return GetResultantVisibility(symbol.ContainingSymbol);

                case SymbolKind.TypeParameter:
                    // Type Parameters are private.
                    return Accessibility.Private;
            }

            while (symbol != null && symbol.Kind != SymbolKind.Namespace)
            {
                switch (symbol.DeclaredAccessibility)
                {
                    // If we see anything private, then the symbol is private.
                    case Accessibility.NotApplicable:
                    case Accessibility.Private:
                        return Accessibility.Private;

                    // If we see anything internal, then knock it down from public to
                    // internal.
                    case Accessibility.Internal:
                    case Accessibility.ProtectedAndInternal:
                        visibility = Accessibility.Internal;
                        break;

                        // For anything else (Public, Protected, ProtectedOrInternal), the
                        // symbol stays at the level we've gotten so far.
                }

                symbol = symbol.ContainingSymbol;
            }

            return visibility;
        }

        public static ISymbol OverriddenMember(this ISymbol symbol)
        {
            switch (symbol.Kind)
            {
                case SymbolKind.Event:
                    return ((IEventSymbol)symbol).OverriddenEvent;

                case SymbolKind.Method:
                    return ((IMethodSymbol)symbol).OverriddenMethod;

                case SymbolKind.Property:
                    return ((IPropertySymbol)symbol).OverriddenProperty;
            }

            return null;
        }


        public static bool IsNew(this ISymbol symbol)
        {
            switch (symbol.Kind)
            {
                case SymbolKind.Event:
                {
                    var @event = symbol as IEventSymbol;

                    if (@event != null && (@event.ContainingType.BaseType != null && @event.ContainingType.BaseType.GetAllMembers().OfType<IEventSymbol>().Any(k => k.Name==@event.Name) && @event.IsOverride==false))
                    {
                        return true;
                    }


                    }
                    break;

                case SymbolKind.Method:
                {
                        var @method = symbol as IMethodSymbol;
                        if (@method != null && (@method.ContainingType.BaseType != null && @method.ContainingType.BaseType.GetAllMembers().OfType<IMethodSymbol>().Any(k => MemberUtilities.CompareMethods(k,@method)) && @method.IsOverride == false))
                        {
                            return true;
                        }
                     
                        

                    }
                    break;
                    

                case SymbolKind.Property:
                {
                        var @property = symbol as IPropertySymbol;

                        if (@property != null && (@property.ContainingType.BaseType != null && @property.ContainingType.BaseType.GetAllMembers().OfType<IPropertySymbol>().Any(k => k.Name == @property.Name) && @property.IsOverride == false))
                        {
                            return true;
                        }

                    }
                    
                    break;
                    

                case SymbolKind.Field: // New Field is nothing special, base field
                {
                      /*  var @field = symbol as IFieldSymbol;
                        if (@field != null && (@field.ContainingType.BaseType != null && @field.ContainingType.BaseType.GetAllMembers().OfType<IFieldSymbol>().Any(k => k.Name == @field.Name) && @field.IsOverride == false))
                        {
                            return true;
                        }*/
                        return false;
                    }
                    break;
            }

            return false;
        }

        //        public static ImmutableArray<ISymbol> ExplicitInterfaceImplementations(this ISymbol symbol)
        //        {
        //            return symbol.TypeSwitch(
        //                (IEventSymbol @event) => @event.ExplicitInterfaceImplementations.As<ISymbol>(),
        //                (IMethodSymbol method) => method.ExplicitInterfaceImplementations.As<ISymbol>(),
        //                (IPropertySymbol property) => property.ExplicitInterfaceImplementations.As<ISymbol>(),
        //                _ => ImmutableArray.Create<ISymbol>());
        //        }

        public static bool IsOverridable(this ISymbol symbol)
        {
            return
                symbol != null &&
                symbol.ContainingType != null &&
                symbol.ContainingType.TypeKind == TypeKind.Class &&
                (symbol.IsVirtual || symbol.IsAbstract || symbol.IsOverride) &&
                !symbol.IsSealed;
        }

        public static bool IsImplementable(this ISymbol symbol)
        {
            if (symbol != null &&
                symbol.ContainingType != null &&
                symbol.ContainingType.TypeKind == TypeKind.Interface)
            {
                if (symbol.Kind == SymbolKind.Event)
                {
                    return true;
                }

                if (symbol.Kind == SymbolKind.Property)
                {
                    return true;
                }

                if (symbol.Kind == SymbolKind.Method && ((IMethodSymbol)symbol).MethodKind == MethodKind.Ordinary)
                {
                    return true;
                }
            }

            return false;
        }

        public static INamedTypeSymbol GetContainingTypeOrThis(this ISymbol symbol)
        {
            if (symbol is INamedTypeSymbol)
            {
                return (INamedTypeSymbol)symbol;
            }

            return symbol.ContainingType;
        }

        public static bool IsPointerType(this ISymbol symbol)
        {
            return symbol is IPointerTypeSymbol;
        }

        public static bool IsErrorType(this ISymbol symbol)
        {
            return (symbol as ITypeSymbol).IsErrorType() == true;
        }

        public static bool IsModuleType(this ISymbol symbol)
        {
            return (symbol as ITypeSymbol).IsModuleType() == true;
        }

        public static bool IsInterfaceType(this ISymbol symbol)
        {
            return (symbol as ITypeSymbol).IsInterfaceType() == true;
        }

        public static bool IsArrayType(this ISymbol symbol)
        {
            return symbol.Kind == SymbolKind.ArrayType;
        }

        public static bool IsAnonymousFunction(this ISymbol symbol)
        {
            return (symbol as IMethodSymbol) .MethodKind == MethodKind.AnonymousFunction;
        }

        public static bool IsKind(this ISymbol symbol, SymbolKind kind)
        {
            return symbol.MatchesKind(kind);
        }

        public static bool MatchesKind(this ISymbol symbol, SymbolKind kind)
        {
            return symbol .Kind == kind;
        }

        public static bool MatchesKind(this ISymbol symbol, SymbolKind kind1, SymbolKind kind2)
        {
            return symbol != null
                && (symbol.Kind == kind1 || symbol.Kind == kind2);
        }

        public static bool MatchesKind(this ISymbol symbol, SymbolKind kind1, SymbolKind kind2, SymbolKind kind3)
        {
            return symbol != null
                && (symbol.Kind == kind1 || symbol.Kind == kind2 || symbol.Kind == kind3);
        }

        public static bool MatchesKind(this ISymbol symbol, params SymbolKind[] kinds)
        {
            return symbol != null
                && kinds.Contains(symbol.Kind);
        }

        public static bool IsReducedExtension(this ISymbol symbol)
        {
            return symbol is IMethodSymbol && ((IMethodSymbol)symbol).MethodKind == MethodKind.ReducedExtension;
        }

        public static bool IsExtensionMethod(this ISymbol symbol)
        {
            return symbol.Kind == SymbolKind.Method && ((IMethodSymbol)symbol).IsExtensionMethod;
        }

        public static bool IsModuleMember(this ISymbol symbol)
        {
            return symbol != null && symbol.ContainingSymbol is INamedTypeSymbol && symbol.ContainingType.TypeKind == TypeKind.Module;
        }

        public static bool IsConstructor(this ISymbol symbol)
        {
            return (symbol as IMethodSymbol) .MethodKind == MethodKind.Constructor;
        }

        public static bool IsStaticConstructor(this ISymbol symbol)
        {
            return (symbol as IMethodSymbol) .MethodKind == MethodKind.StaticConstructor;
        }

        public static bool IsDestructor(this ISymbol symbol)
        {
            return (symbol as IMethodSymbol) .MethodKind == MethodKind.Destructor;
        }

        public static bool IsUserDefinedOperator(this ISymbol symbol)
        {
            return (symbol as IMethodSymbol) .MethodKind == MethodKind.UserDefinedOperator;
        }

        public static bool IsConversion(this ISymbol symbol)
        {
            return (symbol as IMethodSymbol) .MethodKind == MethodKind.Conversion;
        }

        public static bool IsOrdinaryMethod(this ISymbol symbol)
        {
            return (symbol as IMethodSymbol) .MethodKind == MethodKind.Ordinary;
        }

        public static bool IsDelegateType(this ISymbol symbol)
        {
            return symbol is ITypeSymbol && ((ITypeSymbol)symbol).TypeKind == TypeKind.Delegate;
        }

        public static bool IsAnonymousType(this ISymbol symbol)
        {
            return symbol is INamedTypeSymbol && ((INamedTypeSymbol)symbol).IsAnonymousType;
        }

        public static bool IsNormalAnonymousType(this ISymbol symbol)
        {
            return symbol.IsAnonymousType() && !symbol.IsDelegateType();
        }

        public static bool IsAnonymousDelegateType(this ISymbol symbol)
        {
            return symbol.IsAnonymousType() && symbol.IsDelegateType();
        }

        public static bool IsAnonymousTypeProperty(this ISymbol symbol)
        {
            return symbol is IPropertySymbol && symbol.ContainingType.IsNormalAnonymousType();
        }

        public static bool IsIndexer(this ISymbol symbol)
        {
            return (symbol as IPropertySymbol) .IsIndexer == true;
        }

        public static bool IsWriteableFieldOrProperty(this ISymbol symbol)
        {
            var fieldSymbol = symbol as IFieldSymbol;
            if (fieldSymbol != null)
            {
                return !fieldSymbol.IsReadOnly
                    && !fieldSymbol.IsConst;
            }

            var propertySymbol = symbol as IPropertySymbol;
            if (propertySymbol != null)
            {
                return !propertySymbol.IsReadOnly;
            }

            return false;
        }

        public static ITypeSymbol GetMemberType(this ISymbol symbol)
        {
            switch (symbol.Kind)
            {
                case SymbolKind.Field:
                    return ((IFieldSymbol)symbol).Type;
                case SymbolKind.Property:
                    return ((IPropertySymbol)symbol).Type;
                case SymbolKind.Method:
                    return ((IMethodSymbol)symbol).ReturnType;
                case SymbolKind.Event:
                    return ((IEventSymbol)symbol).Type;
            }

            return null;
        }

        public static int GetArity(this ISymbol symbol)
        {
            switch (symbol.Kind)
            {
                case SymbolKind.NamedType:
                    return ((INamedTypeSymbol)symbol).Arity;
                case SymbolKind.Method:
                    return ((IMethodSymbol)symbol).Arity;
                default:
                    return 0;
            }
        }

        public static ISymbol GetOriginalUnreducedDefinition(this ISymbol symbol)
        {
            if (symbol.IsReducedExtension())
            {
                // note: ReducedFrom is only a method definition and includes no type arguments.
                symbol = ((IMethodSymbol)symbol).GetConstructedReducedFrom();
            }

            if (symbol.IsFunctionValue())
            {
                var method = symbol.ContainingSymbol as IMethodSymbol;
                if (method != null)
                {
                    symbol = method;

                    if (method.AssociatedSymbol != null)
                    {
                        symbol = method.AssociatedSymbol;
                    }
                }
            }

            if (symbol.IsNormalAnonymousType() || symbol.IsAnonymousTypeProperty())
            {
                return symbol;
            }

            var parameter = symbol as IParameterSymbol;
            if (parameter != null)
            {
                var method = parameter.ContainingSymbol as IMethodSymbol;
                if (method .IsReducedExtension() == true)
                {
                    symbol = method.GetConstructedReducedFrom().Parameters[parameter.Ordinal + 1];
                }
            }

            return symbol .OriginalDefinition;
        }

        public static bool IsFunctionValue(this ISymbol symbol)
        {
            return symbol is ILocalSymbol && ((ILocalSymbol)symbol).IsFunctionValue;
        }

        public static bool IsThisParameter(this ISymbol symbol)
        {
            return symbol != null && symbol.Kind == SymbolKind.Parameter && ((IParameterSymbol)symbol).IsThis;
        }

        public static ISymbol ConvertThisParameterToType(this ISymbol symbol)
        {
            if (symbol.IsThisParameter())
            {
                return ((IParameterSymbol)symbol).Type;
            }

            return symbol;
        }

//        public static bool IsParams(this ISymbol symbol)
//        {
//            var parameters = symbol.GetParameters();
//            return parameters.Length > 0 && parameters[parameters.Length - 1].IsParams;
//        }
//
//      
//
//        public static ImmutableArray<ITypeSymbol> GetAllTypeArguments(this ISymbol symbol)
//        {
//            var results = new List<ITypeSymbol>(symbol.GetTypeArguments());
//
//            var containingType = symbol.ContainingType;
//            while (containingType != null)
//            {
//                results.AddRange(containingType.TypeArguments);
//                containingType = containingType.ContainingType;
//            }
//
//            return ImmutableArray.CreateRange(results);
//        }

//        public static bool IsAttribute(this ISymbol symbol)
//        {
//            return (symbol as ITypeSymbol);
//        }

        /// <summary>
        /// Returns true if this symbol contains anything unsafe within it.  for example
        /// List&lt;int*[]&gt; is unsafe, as it "int* Foo { get; }"
        /// </summary>
//        public static bool IsUnsafe(this ISymbol member)
//        {
//            // TODO(cyrusn): Defer to compiler code to handle this once it can.
//            return member .Accept(new IsUnsafeVisitor()) == true;
//        }

//        public static ITypeSymbol ConvertToType(
//            this ISymbol symbol,
//            Compilation compilation,
//            bool extensionUsedAsInstance = false)
//        {
//            var type = symbol as ITypeSymbol;
//            if (type != null)
//            {
//                return type;
//            }
//
//            var method = (IMethodSymbol)symbol;
//            if (method != null && !method.Parameters.Any(p => p.RefKind != RefKind.None))
//            {
//                // Convert the symbol to Func<...> or Action<...>
//                if (method.ReturnsVoid)
//                {
//                    var count = extensionUsedAsInstance ? method.Parameters.Length - 1 : method.Parameters.Length;
//                    var skip = extensionUsedAsInstance ? 1 : 0;
//                    count = Math.Max(0, count);
//                    if (count == 0)
//                    {
//                        // Action
//                        return compilation.ActionType();
//                    }
//                    else
//                    {
//                        // Action<TArg1, ..., TArgN>
//                        var actionName = "System.Action`" + count;
//                        var actionType = compilation.GetTypeByMetadataName(actionName);
//
//                        if (actionType != null)
//                        {
//                            var types = method.Parameters
//                                .Skip(skip)
//                                .Select(p =>
//                                    (object)p.Type == null ?
//                                    compilation.GetSpecialType(SpecialType.System_Object) :
//                                    p.Type)
//                                .ToArray();
//                            return actionType.Construct(types);
//                        }
//                    }
//                }
//                else
//                {
//                    // Func<TArg1,...,TArgN,TReturn>
//                    //
//                    // +1 for the return type.
//                    var count = extensionUsedAsInstance ? method.Parameters.Length - 1 : method.Parameters.Length;
//                    var skip = extensionUsedAsInstance ? 1 : 0;
//                    var functionName = "System.Func`" + (count + 1);
//                    var functionType = compilation.GetTypeByMetadataName(functionName);
//
//                    if (functionType != null)
//                    {
//                        var types = method.Parameters
//                            .Skip(skip)
//                            .Select(p => p.Type)
//                            .Concat(method.ReturnType)
//                            .Select(t =>
//                                (object)t == null ?
//                                compilation.GetSpecialType(SpecialType.System_Object) :
//                                t)
//                            .ToArray();
//                        return functionType.Construct(types);
//                    }
//                }
//            }
//
//            // Otherwise, just default to object.
//            return compilation.ObjectType;
//        }

        public static bool IsDeprecated(this ISymbol symbol)
        {
            // TODO(cyrusn): Implement this
            return false;
        }

        public static bool IsStaticType(this ISymbol symbol)
        {
            return symbol != null && symbol.Kind == SymbolKind.NamedType && symbol.IsStatic;
        }

        public static bool IsNamespace(this ISymbol symbol)
        {
            return symbol .Kind == SymbolKind.Namespace;
        }

        public static bool IsOrContainsAccessibleAttribute(this ISymbol symbol, ISymbol withinType, IAssemblySymbol withinAssembly)
        {
            var alias = symbol as IAliasSymbol;
            if (alias != null)
            {
                symbol = alias.Target;
            }

            var namespaceOrType = symbol as INamespaceOrTypeSymbol;
            if (namespaceOrType == null)
            {
                return false;
            }

//            if (namespaceOrType.IsAttribute() && namespaceOrType.IsAccessibleWithin(withinType ?? withinAssembly))
//            {
//                return true;
//            }

            return namespaceOrType.GetMembers().Any(nt => nt.IsOrContainsAccessibleAttribute(withinType, withinAssembly));
        }

        public static IEnumerable<IPropertySymbol> GetValidAnonymousTypeProperties(this ISymbol symbol)
        {
            //Contract.ThrowIfFalse(symbol.IsNormalAnonymousType());
            return ((INamedTypeSymbol)symbol).GetMembers().OfType<IPropertySymbol>().Where(p => p.CanBeReferencedByName);
        }

        public static Accessibility ComputeResultantAccessibility(this ISymbol symbol, ITypeSymbol finalDestination)
        {
            if (symbol == null)
            {
                return Accessibility.Private;
            }

            switch (symbol.DeclaredAccessibility)
            {
                default:
                    return symbol.DeclaredAccessibility;
                case Accessibility.ProtectedAndInternal:
                    return symbol.ContainingAssembly.GivesAccessTo(finalDestination.ContainingAssembly)
                        ? Accessibility.ProtectedAndInternal
                        : Accessibility.Internal;
                case Accessibility.ProtectedOrInternal:
                    return symbol.ContainingAssembly.GivesAccessTo(finalDestination.ContainingAssembly)
                        ? Accessibility.ProtectedOrInternal
                        : Accessibility.Protected;
            }
        }

        /// <returns>
        /// Returns true if symbol is a local variable and its declaring syntax node is 
        /// after the current position, false otherwise (including for non-local symbols)
        /// </returns>
        public static bool IsInaccessibleLocal(this ISymbol symbol, int position)
        {
            if (symbol.Kind != SymbolKind.Local)
            {
                return false;
            }

            // Implicitly declared locals (with Option Explicit Off in VB) are scoped to the entire
            // method and should always be considered accessible from within the same method.
            if (symbol.IsImplicitlyDeclared)
            {
                return false;
            }

            var declarationSyntax = symbol.DeclaringSyntaxReferences.Select(r => r.GetSyntax()).FirstOrDefault();
            return declarationSyntax != null && position < declarationSyntax.SpanStart;
        }

        
        public static bool IsAccessor(this ISymbol symbol)
        {
            return symbol.IsPropertyAccessor() || symbol.IsEventAccessor();
        }

        public static bool IsPropertyAccessor(this ISymbol symbol)
        {
            return (symbol as IMethodSymbol).MethodKind.HasFlag(MethodKind.PropertyGet | MethodKind.PropertySet);
        }

        public static bool IsEventAccessor(this ISymbol symbol)
        {
            var method = symbol as IMethodSymbol;
            return method != null &&
                (method.MethodKind == MethodKind.EventAdd ||
                 method.MethodKind == MethodKind.EventRaise ||
                 method.MethodKind == MethodKind.EventRemove);
        }

//        public static DeclarationModifiers GetSymbolModifiers(this ISymbol symbol)
//        {
//            return new DeclarationModifiers(
//                isStatic: symbol.IsStatic,
//                isAbstract: symbol.IsAbstract,
//                isUnsafe: symbol.IsUnsafe(),
//                isVirtual: symbol.IsVirtual,
//                isOverride: symbol.IsOverride,
//                isSealed: symbol.IsSealed);
//        }

        public static ITypeSymbol GetSymbolType(this ISymbol symbol)
        {
            var localSymbol = symbol as ILocalSymbol;
            if (localSymbol != null)
            {
                return localSymbol.Type;
            }

            var fieldSymbol = symbol as IFieldSymbol;
            if (fieldSymbol != null)
            {
                return fieldSymbol.Type;
            }

            var propertySymbol = symbol as IPropertySymbol;
            if (propertySymbol != null)
            {
                return propertySymbol.Type;
            }

            var parameterSymbol = symbol as IParameterSymbol;
            if (parameterSymbol != null)
            {
                return parameterSymbol.Type;
            }

            var aliasSymbol = symbol as IAliasSymbol;
            if (aliasSymbol != null)
            {
                return aliasSymbol.Target as ITypeSymbol;
            }

            return symbol as ITypeSymbol;
        }

       
        /// <summary>
        /// If the <paramref name="symbol"/> is a method symbol, returns True if the method's return type is "awaitable".
        /// If the <paramref name="symbol"/> is a type symbol, returns True if that type is "awaitable".
        /// An "awaitable" is any type that exposes a GetAwaiter method which returns a valid "awaiter". This GetAwaiter method may be an instance method or an extension method.
        /// </summary>
        public static bool IsAwaitable(this ISymbol symbol, SemanticModel semanticModel, int position)
        {
            IMethodSymbol methodSymbol = symbol as IMethodSymbol;
            ITypeSymbol typeSymbol = null;

            if (methodSymbol == null)
            {
                typeSymbol = symbol as ITypeSymbol;
                if (typeSymbol == null)
                {
                    return false;
                }
            }
            else
            {
                if (methodSymbol.ReturnType == null)
                {
                    return false;
                }

                // dynamic
                if (methodSymbol.ReturnType.TypeKind == TypeKind.Dynamic &&
                    methodSymbol.MethodKind != MethodKind.BuiltinOperator)
                {
                    return true;
                }
            }

            // otherwise: needs valid GetAwaiter
            var potentialGetAwaiters = semanticModel.LookupSymbols(position,
                                                                   container: typeSymbol ?? methodSymbol.ReturnType.OriginalDefinition,
                                                                   name: WellKnownMemberNames.GetAwaiter,
                                                                   includeReducedExtensionMethods: true);
            var getAwaiters = potentialGetAwaiters.OfType<IMethodSymbol>().Where(x => !x.Parameters.Any());
            return getAwaiters.Any(VerifyGetAwaiter);
        }

        private static bool VerifyGetAwaiter(IMethodSymbol getAwaiter)
        {
            var returnType = getAwaiter.ReturnType;
            if (returnType == null)
            {
                return false;
            }

            // bool IsCompleted { get }
            if (!returnType.GetMembers().OfType<IPropertySymbol>().Any(p => p.Name == WellKnownMemberNames.IsCompleted && p.Type.SpecialType == SpecialType.System_Boolean && p.GetMethod != null))
            {
                return false;
            }

            var methods = returnType.GetMembers().OfType<IMethodSymbol>();

            // NOTE: (vladres) The current version of C# Spec, §7.7.7.3 'Runtime evaluation of await expressions', requires that
            // NOTE: the interface method INotifyCompletion.OnCompleted or ICriticalNotifyCompletion.UnsafeOnCompleted is invoked
            // NOTE: (rather than any OnCompleted method conforming to a certain pattern).
            // NOTE: Should this code be updated to match the spec?

            // void OnCompleted(Action) 
            // Actions are delegates, so we'll just check for delegates.
            if (!methods.Any(x => x.Name == WellKnownMemberNames.OnCompleted && x.ReturnsVoid && x.Parameters.Length == 1 && x.Parameters.First().Type.TypeKind == TypeKind.Delegate))
            {
                return false;
            }

            // void GetResult() || T GetResult()
            return methods.Any(m => m.Name == WellKnownMemberNames.GetResult && !m.Parameters.Any());
        }

  

        public static ITypeSymbol InferAwaitableReturnType(this ISymbol symbol, SemanticModel semanticModel, int position)
        {
            var methodSymbol = symbol as IMethodSymbol;
            if (methodSymbol == null)
            {
                return null;
            }

            var returnType = methodSymbol.ReturnType;
            if (returnType == null)
            {
                return null;
            }

            var potentialGetAwaiters = semanticModel.LookupSymbols(position, container: returnType, name: WellKnownMemberNames.GetAwaiter, includeReducedExtensionMethods: true);
            var getAwaiters = potentialGetAwaiters.OfType<IMethodSymbol>().Where(x => !x.Parameters.Any());
            if (!getAwaiters.Any())
            {
                return null;
            }

            var getResults = getAwaiters.SelectMany(g => semanticModel.LookupSymbols(position, container: g.ReturnType, name: WellKnownMemberNames.GetResult));

            var getResult = getResults.OfType<IMethodSymbol>().FirstOrDefault(g => !g.IsStatic);
            if (getResult == null)
            {
                return null;
            }

            return getResult.ReturnType;
        }

  
    }

public static class FileExtensions
    {

        public static void DeleteFilesByWildcards(string pattern, string path = "./")
        {
            var files = Directory.GetFiles(path, pattern);
            foreach (var file in files)
            {
                try
                {
                    File.Delete(file);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception: " + ex);
                }
            }
        }

        private static int _bufferSize = 16384;

        public static string ReadFile(string filename)
        {
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                using (FileStream fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read))
                using (StreamReader streamReader = new StreamReader(fileStream))
                {
                    stringBuilder.Append(streamReader.ReadToEnd());
                }

                return stringBuilder.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Cannot read file: " + filename);
                return "-1";
            }
        }
        public static string ExecuteCommand(this string pathToExe, string arguments = "", string workingDirectory = "")
        {
            if (Driver.Verbose)
            {
                Console.WriteLine("Running:" + pathToExe + " " + arguments);
            }

            if (workingDirectory == "")
            {
                workingDirectory = Path.GetDirectoryName(pathToExe);
            }

            var process = new Process
            {
                StartInfo =
                {

                    FileName = pathToExe,
                    Arguments = arguments,
                    WorkingDirectory = workingDirectory,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden | ProcessWindowStyle.Minimized,
                    UseShellExecute = false
                }
            };

            StringBuilder output = new StringBuilder();
            StringBuilder error = new StringBuilder();

            using (AutoResetEvent outputWaitHandle = new AutoResetEvent(false))
            using (AutoResetEvent errorWaitHandle = new AutoResetEvent(false))
            {
                process.OutputDataReceived += (sender, e) =>
                {
                    if (e.Data == null)
                    {
                        try
                        {
                            outputWaitHandle.Set();
                        }
                        catch (Exception)
                        {
                            
                            
                        }
                       
                    }
                    else
                    {
                        output.AppendLine(e.Data);
                    }
                };
                process.ErrorDataReceived += (sender, e) =>
                {
                    if (e.Data == null)
                    {
                        try
                        {
                            errorWaitHandle.Set();
                        }
                        catch (Exception)
                        {
                        }
                    }
                    else
                    {
                        error.AppendLine(e.Data);
                    }
                };

                var start = DateTime.Now;
               
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                //240 seconds ... for some tests .. i.e. benchmarks
                if (process.WaitForExit(240000) &&
                    outputWaitHandle.WaitOne(240000) &&
                    errorWaitHandle.WaitOne(240000))
                {
                    // Process completed. Check process.ExitCode here.
                    var standardOutput = output.ToString();
                    var standardError = error.ToString();

                    var end = DateTime.Now - start;
                    if (Driver.Verbose)
                        Console.WriteLine("Process took " + end.TotalMilliseconds + " ms");
                    return String.IsNullOrWhiteSpace(standardOutput)
                        ? standardError
                        : String.IsNullOrWhiteSpace(standardError)
                            ? standardOutput
                            : standardOutput + Environment.NewLine + standardError;
                }
                else
                {
                    // Timed out.
                    return "Process terminated immaturely";
                }
            }
        }



        public static void DeleteFile(this string fileName)
        {
            try
            {
                if (File.Exists(fileName))
                    File.Delete(fileName);
            }
            catch (Exception ex)
            {
            }
        }

        public static void ToFile(this string text, string fileName)
        {
            File.WriteAllText(fileName, text);
        }

        public static void ToFile(this StringBuilder text, string fileName)
        {
            File.WriteAllText(fileName, text.ToString());
        }



    }
}
