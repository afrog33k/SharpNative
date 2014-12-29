using System;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SharpNative.Compiler
{
    internal class MemberUtilities
    {
        public static string GetAccessModifiers(MemberDeclarationSyntax member, bool isInterface)
        {
            bool isStatic;
            var acccessmodifiers = "";
            var modifiers = member.GetModifiers();

            if (modifiers.Any(SyntaxKind.PublicKeyword) || modifiers.Any(SyntaxKind.InternalKeyword) ||
                modifiers.Any(SyntaxKind.ProtectedKeyword) || modifiers.Any(SyntaxKind.AbstractKeyword) ||
                isInterface)
                acccessmodifiers += ("public ");

            if (modifiers.Any(SyntaxKind.PrivateKeyword))
                acccessmodifiers += ("private ");
            

            if (modifiers.Any(SyntaxKind.StaticKeyword))
            {
                acccessmodifiers += "static ";
            }

            if (member.GetModifiers().Any(SyntaxKind.AbstractKeyword))
                acccessmodifiers += "abstract ";

           

            bool isoverride = ShouldUseOverrideKeyword(member,isInterface);
            if (isoverride)
                acccessmodifiers += (" override ");

            return acccessmodifiers;
        }

        private static bool ShouldUseOverrideKeyword(MemberDeclarationSyntax member, bool isInterface)
        {
            if (member.GetModifiers().Any(SyntaxKind.OverrideKeyword) && !isInterface)
               return  true;

            ISymbol symbol = TypeProcessor.GetDeclaredSymbol(member);

            if (symbol.ContainingType.TypeKind == TypeKind.Struct ||
                symbol.ContainingType.TypeKind == TypeKind.Interface)
            {
                return false;
                // Structs dont have a base class to override (maybe opEquals) ... but that will be handled separately
                //Interfaces are contracts, so no overriding here// maybe we should compare the methods 
            }
            if (member.GetModifiers().Any(SyntaxKind.StaticKeyword))
                return false;
            //			if (method.Modifiers.Any(SyntaxKind.NewKeyword))
            //				return  symbol.ContainingType.BaseType.GetMembers(symbol.Name).Any(k=>k.IsAbstract || k.IsVirtual);

            if (member.GetModifiers().Any(SyntaxKind.PartialKeyword))
                //partial methods seem exempt from C#'s normal override keyword requirement, so we have to check manually to see if it exists in a base class
                return symbol.ContainingType.BaseType.GetMembers(symbol.Name).Any();

            return member.GetModifiers().Any(SyntaxKind.OverrideKeyword);
        }


        public static string GetMethodName(MemberDeclarationSyntax member, ref bool isInterface)
        {
            var methodSymbol = TypeProcessor.GetDeclaredSymbol(member);
            var name = WriteIdentifierName.TransformIdentifier(OverloadResolver.MethodName(methodSymbol));

            if (methodSymbol.ContainingType.TypeKind == TypeKind.Interface)
                isInterface = true;

            var isinterfacemethod = Equals(methodSymbol.ContainingType.FindImplementationForInterfaceMember(methodSymbol),
                methodSymbol);
            if (methodSymbol.ContainingType.TypeKind == TypeKind.Interface ||
                (isinterfacemethod && methodSymbol.IsOverride))
            {
                name = Regex.Replace(
                    TypeProcessor.ConvertType(methodSymbol.ContainingType.ConstructedFrom) + "_" + name,
                    @" ?!\(.*?\)", String.Empty);

                if (methodSymbol.ContainingType.ContainingType != null)
                    name = name.RemoveFromStartOfString(methodSymbol.ContainingType.ContainingType.Name + ".");
            }

            if (name.Contains(".")) // Explicit Interface method
            {
                //              
                name = name.SubstringAfterLast('.');
                name = name.Replace('.', '_');
            }

            var interfaceMethods =
                methodSymbol.ContainingType.AllInterfaces.SelectMany(
                    u =>
                        u.GetMembers(name)).ToArray();

            ISymbol interfaceMethod =
                interfaceMethods.FirstOrDefault(
                    o => methodSymbol.ContainingType.FindImplementationForInterfaceMember(o) == methodSymbol);

            if (interfaceMethod == null)
            {
                //TODO: fix this for virtual method test 7, seems roslyn cannot deal with virtual 
                // overrides of interface methods ... so i'll provide a kludge
                if (!member.GetModifiers().Any(SyntaxKind.NewKeyword) && methodSymbol is IMethodSymbol) // This is not neccessary for properties
                {
                    interfaceMethod =
                        interfaceMethods.FirstOrDefault(k => CompareMethods(k as IMethodSymbol, (IMethodSymbol) methodSymbol));
                }
            }

            if (interfaceMethod != null)
            {
                //This is an interface property //TO
                if (methodSymbol.ContainingType.SpecialType == SpecialType.System_Array)
                    name += ("");
                else
                {
                    var typenameI =
                        Regex.Replace(TypeProcessor.ConvertType(interfaceMethod.ContainingType.ConstructedFrom),
                            @" ?!\(.*?\)", String.Empty);
                    //TODO: we should be able to get the original interface name, or just remove all generics from this

                    if (typenameI.Contains('.'))
                        typenameI = typenameI.SubstringAfterLast('.');
                    name = (typenameI + "_") + name;
                }
            }

            if (member.GetModifiers().Any(SyntaxKind.NewKeyword)) //Take care of new
                name += "_";

            return name;
        }

        private static bool CompareMethods(IMethodSymbol interfaceMethod, IMethodSymbol methodSymbol)
        {
            if (interfaceMethod == null || methodSymbol == null)
                return false;
            return interfaceMethod.Name == methodSymbol.Name && interfaceMethod.ReturnType == methodSymbol.ReturnType &&
                   interfaceMethod.Parameters == methodSymbol.Parameters &&
                   interfaceMethod.TypeArguments == methodSymbol.TypeArguments;
        }

     

    }
}