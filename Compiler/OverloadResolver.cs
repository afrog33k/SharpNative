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
        public static string MethodName(IMethodSymbol method)
        {
            if (!method.Parameters.Any(o => IsAmbiguousType(o.Type)))
                return method.Name;

            var overloadedGroup = method.ContainingType.GetMembers(method.Name).OfType<IMethodSymbol>().ToList();

            if (overloadedGroup.Count == 0)
                throw new Exception("Symbols not found");

            if (overloadedGroup.Count == 1)
                return method.Name;

            return ExpandedMethodName(method);
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
    }
}