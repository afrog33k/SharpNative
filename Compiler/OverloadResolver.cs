// /*
//   SharpNative - C# to D Transpiler
//   (C) 2014 Irio Systems 
// */

#region Imports

using System;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

#endregion

namespace SharpNative.Compiler
{
    public static class OverloadResolver
    {
        public static string MethodName(ISymbol symbol)
        {

            var method = symbol as IMethodSymbol;
            var property = symbol as IPropertySymbol;
            if (method != null || property != null)
            {
                if (method != null)
                {
                    if (!method.Parameters.Any(o => IsAmbiguousType(o.Type)))
                        return method.Name;
                }


                if (property != null)
                {
                    if (!property.Parameters.Any(o => IsAmbiguousType(o.Type)))
                        return property.Name;
                }
            }

            var overloadedGroup = symbol.ContainingType.GetMembers(symbol.Name).OfType<IMethodSymbol>().ToList();

            if (overloadedGroup.Count == 0)
                throw new Exception("Symbols not found");

            if (overloadedGroup.Count == 1)
                return symbol.Name;

            if (method != null)
                return ExpandedMethodName(method);

            if (property != null)
                return ExpandedMethodName(property);

            return null;
        }

        private static bool IsAmbiguousType(ITypeSymbol type)
        {
            switch (TypeProcessor.GenericTypeName(type))
            {
                case "System.UInt16":
                case "System.UInt32":
                case "System.UInt64":
                    return true;
                default:
                    return false;
            }
        }

        private static string ExpandedMethodName(IMethodSymbol method)
        {
            var ret = new StringBuilder(20);

            ret.Append(method.Name);
            ret.Append("_");

            foreach (var param in method.Parameters)
            {
                ret.Append(param.Type.Name);

                var named = param.Type as INamedTypeSymbol;
                if (named != null)
                {
                    foreach (var typeArg in named.TypeArguments)
                    {
                        if (typeArg.TypeKind != TypeKind.TypeParameter)
                            ret.Append(typeArg.Name);
                    }
                }

                ret.Append("_");
            }

            ret.Remove(ret.Length - 1, 1);
            return ret.ToString();
        }

        private static string ExpandedMethodName(IPropertySymbol method)
        {
            var ret = new StringBuilder(20);

            ret.Append(method.Name);
            ret.Append("_");

            foreach (var param in method.Parameters)
            {
                ret.Append(param.Type.Name);

                var named = param.Type as INamedTypeSymbol;
                if (named != null)
                {
                    foreach (var typeArg in named.TypeArguments)
                    {
                        if (typeArg.TypeKind != TypeKind.TypeParameter)
                            ret.Append(typeArg.Name);
                    }
                }

                ret.Append("_");
            }

            ret.Remove(ret.Length - 1, 1);
            return ret.ToString();
        }
    }
}