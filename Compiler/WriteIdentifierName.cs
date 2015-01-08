// /*
//   SharpNative - C# to D Transpiler
//   (C) 2014 Irio Systems 
// */

#region Imports

using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

#endregion

namespace SharpNative.Compiler
{
    internal static class WriteIdentifierName
    {
        public static void Go(OutputWriter writer, IdentifierNameSyntax identifier, bool byRef = false)
        {
            var symbol = TypeProcessor.GetSymbolInfo(identifier).Symbol;

            if (symbol == null)
            {
                
            }

            if (symbol.IsStatic)
            {
                //                writer.Write(symbol.ContainingNamespace.FullNameWithDot());
                //                writer.Write(symbol.ContainingType.FullName());
                writer.Write(TypeProcessor.ConvertType(symbol.ContainingType)+".");
            }

          
            var binExpression = identifier.Parent as BinaryExpressionSyntax;
            string memberName = TransformIdentifier(identifier.Identifier.ToString());

            if (symbol.Kind == SymbolKind.Property) // Using dlang properties
            {
                if (symbol.ContainingType.TypeKind == TypeKind.Interface ||
                    Equals(symbol.ContainingType.FindImplementationForInterfaceMember(symbol), symbol))
                {
                    memberName =
                        Regex.Replace(
                            TypeProcessor.ConvertType(symbol.ContainingType.OriginalDefinition)
                                .RemoveFromStartOfString(symbol.ContainingNamespace + ".Namespace.") + "_" + memberName,
                            @" ?!\(.*?\)", string.Empty);
                }

                var interfaceMethods =
                    symbol.ContainingType.AllInterfaces.SelectMany(
                        u =>
                            u.GetMembers(memberName)).ToArray();

                ISymbol interfaceMethod =
                    interfaceMethods.FirstOrDefault(
                        o => symbol.ContainingType.FindImplementationForInterfaceMember(o) == symbol);

             

                if (interfaceMethod != null)
                {
                    if (symbol.ContainingType.SpecialType == SpecialType.System_Array)
                        writer.Write("");
                    else
                    {
                        var typenameI =
                            Regex.Replace(
                                TypeProcessor.ConvertType(interfaceMethod.ContainingType.ConstructedFrom, true),
                                @" ?!\(.*?\)", string.Empty);
                            //TODO: we should be able to get the original interface name, or just remove all generics from this
                        if (typenameI.Contains('.'))
                            typenameI = typenameI.SubstringAfterLast('.');
                        writer.Write(typenameI + "_");
                    }
                }
           }
                writer.Write(memberName);

        }

        public static string TransformIdentifier(string ident)
        {

            switch (ident)
            {
                case "version":
                case "body":
                    return "cs" + ident;
                default:
                    return ident;
            }
        }
    }
}