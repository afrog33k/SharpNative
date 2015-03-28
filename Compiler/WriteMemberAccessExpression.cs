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
    internal static class WriteMemberAccessExpression
    {
        public static void Go(OutputWriter writer, MemberAccessExpressionSyntax expression)
        {
            var memberName = expression.Name.Identifier.ValueText;
            var type = TypeProcessor.GetTypeInfo(expression.Expression).ConvertedType;
            var typeStr = TypeProcessor.GenericTypeName(type);

            var isLiteral = expression.Expression is LiteralExpressionSyntax;

            var isStringLiteral = false;
            if (isLiteral)
            {
                var literal = expression.Expression as LiteralExpressionSyntax;

                if (literal.RawKind == (decimal) SyntaxKind.StringLiteralExpression)
                {
                    isStringLiteral = true;
                    // writer.Write("((System.String)"); Not needed for strings at all
                }
            }

            memberName = WriteIdentifierName.TransformIdentifier(memberName);

            var typeInfo = TypeProcessor.GetTypeInfo(expression.Expression);
            var symbolInfo = TypeProcessor.GetSymbolInfo(expression);
            var methodSymbol = symbolInfo.Symbol;
            if (methodSymbol == null)
                symbolInfo = TypeProcessor.GetSymbolInfo(expression.Expression);
            if (type != null && methodSymbol != null)
                //if type is null, then we're just a namespace.  We can ignore these.
            {
                var directInvocationOnBasics = methodSymbol.ContainingType.IsBasicType() && methodSymbol.IsStatic;

                if (directInvocationOnBasics)
                {

                    //Extension methods in Dlang are straightforward, although this could lead to clashes without qualification
                    if (methodSymbol.ContainingType != Context.Instance.Type)
                    {
                        var extensionNamespace = TypeProcessor.ConvertType(type, true, false);

                        writer.Write(extensionNamespace);
                    }
                }
                else
                    WriteMember(writer, expression.Expression);

                if (isLiteral && !isStringLiteral)
                    writer.Write(")"); //Not needed for strings at all
                writer.Write(".");
                // Ideally Escape analysis should take care of this, but for now all value types are on heap and ref types on stack
            }

            if (methodSymbol != null && methodSymbol.Kind == SymbolKind.Property)
            {
                if (methodSymbol.ContainingType.TypeKind == TypeKind.Interface ||
                    Equals(methodSymbol.ContainingType.FindImplementationForInterfaceMember(methodSymbol),
                        methodSymbol))
                {
                   /* memberName =
                        Regex.Replace(
                            TypeProcessor.ConvertType(methodSymbol.ContainingType.OriginalDefinition)
                                .RemoveFromStartOfString(methodSymbol.ContainingNamespace + ".Namespace.") +
                            "_" + memberName,
                            @" ?!\(.*?\)", string.Empty);*/
                    if (methodSymbol.ContainingType.ContainingType != null)
                        memberName = memberName.RemoveFromStartOfString(methodSymbol.ContainingType.ContainingType.Name + ".");
                }

                string name = memberName;
                var interfaceMethods =
                    methodSymbol.ContainingType.AllInterfaces.SelectMany(
                        u =>
                            u.GetMembers(name)).ToArray();

                ISymbol interfaceMethod =
                    interfaceMethods.FirstOrDefault(
                        o =>
                            Equals(methodSymbol.ContainingType.FindImplementationForInterfaceMember(o), methodSymbol));

                if (interfaceMethod != null)

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

              /*  if (!methodSymbol.ContainingType.IsAnonymousType &&
                    (methodSymbol.DeclaringSyntaxReferences.Any() &&
                     methodSymbol.DeclaringSyntaxReferences.FirstOrDefault()
                         .GetSyntax()
                         .As<PropertyDeclarationSyntax>()
                         .Modifiers.Any(SyntaxKind.NewKeyword)))
                {
                    //TODO: this means that new is not supported on external libraries, anonymous types cannot be extended
                    //					//why doesnt roslyn give me this information ?
                    memberName += "_";
                }*/
            }

            var isGet = false;
            writer.Write(memberName);

            if (expression.Name is GenericNameSyntax)
            {
                var gen = expression.Name.As<GenericNameSyntax>();

                writer.Write("!(");

                bool first = true;
                foreach (var g in gen.TypeArgumentList.Arguments)
                {
                    if (first)
                        first = false;
                    else
                        writer.Write(", ");

                    writer.Write(TypeProcessor.ConvertType(g));
                }

                writer.Write(")");
            }
        }

        public static void WriteMember(OutputWriter writer, ExpressionSyntax expression)
        {
            var symbol = TypeProcessor.GetSymbolInfo(expression).Symbol;
            if (symbol is INamedTypeSymbol)
            {
                var str = TypeProcessor.ConvertType(symbol.As<INamedTypeSymbol>());
                if (str == "Array_T")
                    // Array is the only special case, otherwise generics have to be specialized to access static members
                    str = "Array";
                writer.Write(str);
            }
            else
                Core.Write(writer, expression);
        }
    }
}