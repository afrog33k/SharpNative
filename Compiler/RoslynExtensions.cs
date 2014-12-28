// /*
//   SharpNative - C# to D Transpiler
//   (C) 2014 Irio Systems 
// */

#region Imports

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

#endregion

namespace SharpNative.Compiler
{
    public static class RoslynExtensions
    {
        public static bool IsSubclassOf(this ITypeSymbol type, ITypeSymbol baseTypeSymbol)
        {
            if (type == null || type.BaseType == null)
                return false;
            if (type.BaseType == baseTypeSymbol || type.AllInterfaces.Contains(baseTypeSymbol))
                return true;

            return IsSubclassOf(type.BaseType, baseTypeSymbol);
        }

        //       

        


      
//        public static string GetFullName(this INamespaceSymbol namespaceSymbol)
//        {
//            string result = namespaceSymbol.MetadataName;
//            if (!namespaceSymbol.IsGlobalNamespace && !namespaceSymbol.ContainingNamespace.IsGlobalNamespace)
//            {
//                result =
//                    Context.Instance.SymbolNames[
//                        namespaceSymbol.ContainingNamespace, namespaceSymbol.ContainingNamespace.GetFullName()] + "." +
//                    result;
//            }
//            return result;
//        }


        public static bool IsAssignableFrom(this ITypeSymbol baseType, ITypeSymbol type)
        {
            if (IsImplicitNumericCast(baseType, type)) return true;

            var current = type;
            while (current != null)
            {
                if (Equals(current, baseType))
                    return true;
                current = current.BaseType;
            }
            foreach (var intf in type.AllInterfaces)
            {
                if (Equals(intf, baseType))
                    return true;
            }
            return false;
        }

        private static bool IsImplicitNumericCast(ITypeSymbol baseType, ITypeSymbol type)
        {
            if (type.SpecialType == SpecialType.System_SByte)
            {
                switch (baseType.SpecialType)
                {
                    case SpecialType.System_SByte: // Safety precaution?
                    case SpecialType.System_Int16:
                    case SpecialType.System_Int32:
                    case SpecialType.System_Int64:
                    case SpecialType.System_Single:
                    case SpecialType.System_Double:
                    case SpecialType.System_Decimal:
                        // TODO: Need to work on decimal, its treated as a normal int ... should we use D's 80-bit type ? 

                        return true;
                }
            }

            if (type.SpecialType == SpecialType.System_Char)
            {
                switch (baseType.SpecialType)
                {
                    case SpecialType.System_Char:
                    case SpecialType.System_UInt16:
                    case SpecialType.System_Int32:
                    case SpecialType.System_UInt32:
                    case SpecialType.System_Int64:
                    case SpecialType.System_UInt64:
                    case SpecialType.System_Single:
                    case SpecialType.System_Double:
                    case SpecialType.System_Decimal:
                        // TODO: Need to work on decimal, its treated as a normal int ... should we use D's 80-bit type ? 

                        return true;
                }
            }

            if (type.SpecialType == SpecialType.System_Byte)
            {
                switch (baseType.SpecialType)
                {
                    case SpecialType.System_Byte:
                    case SpecialType.System_Int16:
                    case SpecialType.System_UInt16:
                    case SpecialType.System_Int32:
                    case SpecialType.System_UInt32:
                    case SpecialType.System_Int64:
                    case SpecialType.System_UInt64:
                    case SpecialType.System_Single:
                    case SpecialType.System_Double:
                    case SpecialType.System_Decimal:
                        // TODO: Need to work on decimal, its treated as a normal int ... should we use D's 80-bit type ? 

                        return true;
                }
            }

            if (type.SpecialType == SpecialType.System_Int16)
            {
                switch (baseType.SpecialType)
                {
                    case SpecialType.System_Int16:
                    case SpecialType.System_Int32:
                    case SpecialType.System_Int64:
                    case SpecialType.System_Single:
                    case SpecialType.System_Double:
                    case SpecialType.System_Decimal:
                        // TODO: Need to work on decimal, its treated as a normal int ... should we use D's 80-bit type ? 

                        return true;
                }
            }

            if (type.SpecialType == SpecialType.System_UInt16)
            {
                switch (baseType.SpecialType)
                {
                    case SpecialType.System_UInt16:
                    case SpecialType.System_Int32:
                    case SpecialType.System_UInt32:
                    case SpecialType.System_Int64:
                    case SpecialType.System_UInt64:
                    case SpecialType.System_Single:
                    case SpecialType.System_Double:
                    case SpecialType.System_Decimal:
                        // TODO: Need to work on decimal, its treated as a normal int ... should we use D's 80-bit type ? 

                        return true;
                }
            }

            if (type.SpecialType == SpecialType.System_Int32)
            {
                switch (baseType.SpecialType)
                {
                    case SpecialType.System_Int32:
                    case SpecialType.System_Int64:
                    case SpecialType.System_Single:
                    case SpecialType.System_Double:
                    case SpecialType.System_Decimal:
                        // TODO: Need to work on decimal, its treated as a normal int ... should we use D's 80-bit type ? 

                        return true;
                }
            }

            if (type.SpecialType == SpecialType.System_UInt32)
            {
                switch (baseType.SpecialType)
                {
                    case SpecialType.System_UInt32:
                    case SpecialType.System_Int64:
                    case SpecialType.System_UInt64:
                    case SpecialType.System_Single:
                    case SpecialType.System_Double:
                    case SpecialType.System_Decimal:
                        // TODO: Need to work on decimal, its treated as a normal int ... should we use D's 80-bit type ? 

                        return true;
                }
            }

            if (type.SpecialType == SpecialType.System_Int64)
            {
                switch (baseType.SpecialType)
                {
                    case SpecialType.System_Int64:
                    case SpecialType.System_Single:
                    case SpecialType.System_Double:
                    case SpecialType.System_Decimal:
                        // TODO: Need to work on decimal, its treated as a normal int ... should we use D's 80-bit type ? 

                        return true;
                }
            }

            if (type.SpecialType == SpecialType.System_Single)
            {
                switch (baseType.SpecialType)
                {
                    case SpecialType.System_Single:
                    case SpecialType.System_Double:
                        // TODO: Need to work on decimal, its treated as a normal int ... should we use D's 80-bit type ? 

                        return true;
                }
            }

            if (type.SpecialType == SpecialType.System_UInt64)
            {
                switch (baseType.SpecialType)
                {
                    case SpecialType.System_Single:
                    case SpecialType.System_Double:
                    case SpecialType.System_Decimal:
                        // TODO: Need to work on decimal, its treated as a normal int ... should we use D's 80-bit type ? 

                        return true;
                }
            }

            return false;
        }


        public static ITypeSymbol GetGenericArgument(this ITypeSymbol type, ITypeSymbol unconstructedType,
            int argumentIndex)
        {
            var current = type;
            while (current != null)
            {
                if (Equals(current.OriginalDefinition, unconstructedType))
                    return ((INamedTypeSymbol) current).TypeArguments[argumentIndex];
                current = current.BaseType;
            }
            if (type is INamedTypeSymbol)
            {
                var namedTypeSymbol = (INamedTypeSymbol) type;
                foreach (var intf in namedTypeSymbol.AllInterfaces)
                {
                    if (Equals(intf.OriginalDefinition, unconstructedType))
                        return intf.TypeArguments[argumentIndex];
                }
            }
            return null;
        }

        public static T GetAttributeValue<T>(this ISymbol type, INamedTypeSymbol attributeType, string propertyName,
            T defaultValue = default(T))
        {
            var jsAttribute = type.GetAttributes().SingleOrDefault(x => Equals(x.AttributeClass, attributeType));
            if (jsAttribute != null)
            {
                // If the type is inlined, all the methods of the class will be written
                // at the same (root) level as the class declaration would have.  This is useful
                // for creating Javascript-Global functions.
                var isInlinedArgument = jsAttribute.NamedArguments.SingleOrDefault(x => x.Key == propertyName);
                if (isInlinedArgument.Value.Value != null)
                    return (T) isInlinedArgument.Value.Value;
            }
            return defaultValue;
        }

        public static ISymbol[] GetAllMembers(this INamedTypeSymbol type, string name)
        {
            if (type.BaseType != null)
                return type.BaseType.GetAllMembers(name).Concat(type.GetMembers(name).ToArray()).ToArray();
            return type.GetMembers(name).ToArray();
        }

        public static ISymbol[] GetAllMembers(this INamedTypeSymbol type)
        {
            if (type.BaseType != null)
                return type.BaseType.GetAllMembers().Concat(type.GetMembers().ToArray()).ToArray();
            return type.GetMembers().ToArray();
        }

        public static ITypeSymbol GetContainingType(this SyntaxNode node)
        {
            var classDeclaration = node.FirstAncestorOrSelf<ClassDeclarationSyntax>(x => true);
            if (classDeclaration == null)
                return null;
            return (ITypeSymbol) ModelExtensions.GetDeclaredSymbol(Context.Compilation.GetSemanticModel(classDeclaration.SyntaxTree), classDeclaration);
        }

        public static IMethodSymbol GetContainingMethod(this SyntaxNode node)
        {
            var method =
                node.FirstAncestorOrSelf<SyntaxNode>(
                    x => x is ConstructorDeclarationSyntax || x is MethodDeclarationSyntax);
            if (method == null)
                return null;
            if (method is ConstructorDeclarationSyntax)
            {
                return
                    (IMethodSymbol) ModelExtensions.GetDeclaredSymbol(Context.Compilation.GetSemanticModel(method.SyntaxTree), (ConstructorDeclarationSyntax) method);
            }
            return
                (IMethodSymbol) ModelExtensions.GetDeclaredSymbol(Context.Compilation.GetSemanticModel(method.SyntaxTree), (MethodDeclarationSyntax) method);
        }

        public static IMethodSymbol GetRootOverride(this IMethodSymbol method)
        {
            if (method.OverriddenMethod == null)
                return method;
            return method.OverriddenMethod.GetRootOverride();
        }

        public static IPropertySymbol GetRootOverride(this IPropertySymbol property)
        {
            if (property.OverriddenProperty == null)
                return property;
            return property.OverriddenProperty.GetRootOverride();
        }

        public static bool HasOrIsEnclosedInGenericParameters(this INamedTypeSymbol type)
        {
            return type.TypeParameters.Any() ||
                   (type.ContainingType != null && type.ContainingType.HasOrIsEnclosedInGenericParameters());
        }

/*
        public static bool HasOrIsEnclosedInUnconstructedType(this NamedTypeSymbol type)
        {
            return (type.TypeParameters.Count > 0 && type.TypeArguments.Any(x => IsUnconstructedType(x))) || (type.ContainingType != null && type.ContainingType.HasOrIsEnclosedInGenericParameters());
        }
*/

        public static bool IsUnconstructedType(this ITypeSymbol type)
        {
            var namedTypeSymbol = type as INamedTypeSymbol;
            if (type is ITypeParameterSymbol)
                return true;
            if (namedTypeSymbol != null)
            {
                return (namedTypeSymbol.TypeParameters.Any() &&
                        namedTypeSymbol.TypeArguments.Any(x => IsUnconstructedType(x))) ||
                       (type.ContainingType != null && type.ContainingType.IsUnconstructedType());
            }
            return false;
//            namedTypeSymbol.ConstructedFrom.ToString() != namedTypeSymbol.ToString() && namedTypeSymbol.ConstructedFrom.ConstructedFrom.ToString() == namedTypeSymbol.ConstructedFrom.ToString() && namedTypeSymbol.HasOrIsEnclosedInGenericParameters()
        }

        public static ParameterSyntax[] GetParameters(this ExpressionSyntax lambda)
        {
            if (lambda is SimpleLambdaExpressionSyntax)
                return new[] {((SimpleLambdaExpressionSyntax) lambda).Parameter};
            if (lambda is ParenthesizedLambdaExpressionSyntax)
                return ((ParenthesizedLambdaExpressionSyntax) lambda).ParameterList.Parameters.ToArray();
            throw new Exception();
        }

        public static CSharpSyntaxNode GetBody(this ExpressionSyntax lambda)
        {
            if (lambda is SimpleLambdaExpressionSyntax)
                return ((SimpleLambdaExpressionSyntax) lambda).Body;
            if (lambda is ParenthesizedLambdaExpressionSyntax)
                return ((ParenthesizedLambdaExpressionSyntax) lambda).Body;
            throw new Exception();
        }

/*
        public static bool IsAssignment(this SyntaxKind type)
        {
            switch (type)
            {
                case SyntaxKind.AssignExpression:
                case SyntaxKind.AddAssignExpression:
                case SyntaxKind.AndAssignExpression:
                case SyntaxKind.DivideAssignExpression:
                case SyntaxKind.ExclusiveOrAssignExpression:
                case SyntaxKind.LeftShiftAssignExpression:
                case SyntaxKind.RightShiftAssignExpression:
                case SyntaxKind.ModuloAssignExpression:
                case SyntaxKind.MultiplyAssignExpression:
                case SyntaxKind.OrAssignExpression:
                case SyntaxKind.SubtractAssignExpression:
                    return true;
                default:
                    return false;
            }
        }
*/

        public static StatementSyntax GetNextStatement(this StatementSyntax statement)
        {
            if (statement.Parent is BlockSyntax)
            {
                var block = (BlockSyntax) statement.Parent;
                var indexOfStatement = block.Statements.IndexOf(statement);
                if (indexOfStatement == -1)
                    throw new Exception();
                if (indexOfStatement < block.Statements.Count - 1)
                    return block.Statements[indexOfStatement + 1];
                return null;
            }
            if (statement.Parent is SwitchSectionSyntax)
            {
                var section = (SwitchSectionSyntax) statement.Parent;
                var indexOfStatement = section.Statements.IndexOf(statement);
                if (indexOfStatement == -1)
                    throw new Exception();
                if (indexOfStatement < section.Statements.Count - 1)
                    return section.Statements[indexOfStatement + 1];
                return null;
            }
            return null;
        }

        public static IMethodSymbol GetMethodByName(this INamedTypeSymbol type, string name)
        {
            return type.GetMembers(name).OfType<IMethodSymbol>().Single();
        }

        public static IMethodSymbol GetMethod(this INamedTypeSymbol type, string name,
            params ITypeSymbol[] parameterTypes)
        {
            IMethodSymbol method;
            if (!TryGetMethod(type, name, out method, parameterTypes))
                throw new Exception();
            return method;
        }

        public static bool TryGetMethod(this INamedTypeSymbol type, string name, out IMethodSymbol method,
            params ITypeSymbol[] parameterTypes)
        {
            var candidates = type.GetMembers(name).OfType<IMethodSymbol>().ToArray();
            if (candidates.Length == 1)
            {
                method = candidates[0];
                return true;
            }
            foreach (var candidate in candidates)
            {
                if (candidate.Parameters.Count() != parameterTypes.Length)
                    continue;
                bool valid = true;
                foreach (
                    var item in
                        parameterTypes.Zip(candidate.Parameters.Select(x => x.Type),
                            (x, y) => new {ParameterType = x, Candidate = y}))
                {
                    if (!Equals(item.Candidate, item.ParameterType))
                    {
                        valid = false;
                        break;
                    }
                }
                if (valid)
                {
                    method = candidate;
                    return true;
                }
            }
            method = null;
            return false;
        }

        public static TypeSyntax ToTypeSyntax(this ITypeSymbol symbol)
        {
            return SyntaxFactory.ParseTypeName(symbol.ToDisplayString());
        }


        public static InvocationExpressionSyntax Invoke(this IMethodSymbol method, params ExpressionSyntax[] arguments)
        {
            var methodTarget = SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                method.ContainingType.ToTypeSyntax(),
                SyntaxFactory.IdentifierName(method.Name));
            return arguments.Any()
                ? SyntaxFactory.InvocationExpression(methodTarget,
                    SyntaxFactory.ArgumentList(
                        SyntaxFactory.SeparatedList(arguments.Select(x => SyntaxFactory.Argument(x)),
                            arguments.Skip(1).Select(_ => SyntaxFactory.Token(SyntaxKind.CommaToken)))))
                : SyntaxFactory.InvocationExpression(methodTarget);
        }

        public static bool IsTrue(this ExpressionSyntax expression)
        {
            var literal = (LiteralExpressionSyntax) expression;
            return literal.Token.IsKind(SyntaxKind.TrueKeyword);
        }

        public static Compilation Recompile(this Compilation compilation, CompilationUnitSyntax compilationUnit)
        {
            var document = Context.Instance.Project.GetDocument(compilationUnit.SyntaxTree);
            document = document.WithSyntaxRoot(compilationUnit);
            SyntaxTree syntaxTree;
            document.TryGetSyntaxTree(out syntaxTree);
            compilation = compilation.ReplaceSyntaxTree(compilationUnit.SyntaxTree, syntaxTree);
            return compilation;
        }

        public static Compilation Recompile(this Compilation compilation, SyntaxNode oldNode, SyntaxNode newNode)
        {
            while (oldNode != null)
            {
                var oldParent = oldNode.Parent;
                var newParent = oldParent.ReplaceNode(oldNode, newNode);

                oldNode = oldParent;
                newNode = newParent;

                if (oldNode is CompilationUnitSyntax)
                    break;
            }
            return compilation.Recompile((CompilationUnitSyntax) newNode);
        }

        public static INamedTypeSymbol FindType(this Compilation compilation, string fullName)
        {
            var result = compilation.GetTypeByMetadataName(fullName);
            if (result == null)
            {
                foreach (
                    var assembly in
                        Context.Instance.Project.MetadataReferences.Select(compilation.GetAssemblyOrModuleSymbol)
                            .Cast<IAssemblySymbol>())
                {
                    result = assembly.GetTypeByMetadataName(fullName);
                    if (result != null)
                        break;
                }
            }
            return result;
        }

        public static IEnumerable<ITypeSymbol> GetAllInnerTypes(this ITypeSymbol type)
        {
            foreach (var innerType in type.GetMembers().OfType<ITypeSymbol>())
            {
                yield return innerType;
                foreach (var inner in innerType.GetAllInnerTypes())
                    yield return inner;
            }
        }

        public static void ReportError(this SyntaxNode node, string message)
        {
            var fileName = node.SyntaxTree.FilePath;
            var text = node.SyntaxTree.GetText();
            var span = node.GetLocation().SourceSpan;
            var startLine = text.Lines.GetLinePosition(span.Start);
            var endLine = text.Lines.GetLinePosition(span.End);

            Console.WriteLine("{0} ({1},{2},{3},{4}): error: {5}", fileName, startLine.Line + 1, startLine.Character + 1,
                endLine.Line + 1, endLine.Character + 1, message);
        }

        public static bool IsPrimitive(this ITypeSymbol type)
        {
            switch (type.SpecialType)
            {
                case SpecialType.System_Boolean:
                case SpecialType.System_Byte:
                case SpecialType.System_Char:
                case SpecialType.System_Decimal:
                case SpecialType.System_Double:
                case SpecialType.System_Enum:
                case SpecialType.System_Int16:
                case SpecialType.System_Int32:
                case SpecialType.System_Int64:
                case SpecialType.System_Nullable_T:
                case SpecialType.System_SByte:
                case SpecialType.System_Single:
                case SpecialType.System_UInt16:
                case SpecialType.System_UInt32:
                case SpecialType.System_UInt64:
                case SpecialType.System_Void:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsPrimitiveInteger(this ITypeSymbol type)
        {
            switch (type.SpecialType)
            {
                case SpecialType.System_Boolean:
                case SpecialType.System_Byte:
                case SpecialType.System_Char:
                case SpecialType.System_Decimal:
                case SpecialType.System_Double:
                case SpecialType.System_Enum:
                case SpecialType.System_Int16:
                case SpecialType.System_Int32:
                case SpecialType.System_Int64:
                case SpecialType.System_SByte:
                case SpecialType.System_Single:
                case SpecialType.System_UInt16:
                case SpecialType.System_UInt32:
                case SpecialType.System_UInt64:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsAsync(this MethodDeclarationSyntax method)
        {
            return method.Modifiers.Any(x => x.IsKind(SyntaxKind.AsyncKeyword));
        }

        public static bool IsPointer(this ITypeSymbol type)
        {
            return type != null && type.IsValueType;
        }
    }
}