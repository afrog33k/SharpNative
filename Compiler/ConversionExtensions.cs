// /*
//   SharpNative - C# to D Transpiler
//   (C) 2014 Irio Systems 
// */

#region Imports

using System;
using System.Linq;
using Microsoft.CodeAnalysis;

#endregion

namespace SharpNative.Compiler
{
    public static class ConversionExtensions
    {
        public static IMethodSymbol GetImplicitCoversionOp(this ITypeSymbol type, ITypeSymbol returnTypeSymbol,
            ITypeSymbol originalTypeSymbol, bool allowNarrowing = false)
        {
            if (!returnTypeSymbol.IsPrimitiveInteger())
                allowNarrowing = false;

            if (!allowNarrowing)
            {
                return
                    type.GetMembers("op_Implicit")
                        .OfType<IMethodSymbol>()
                        .FirstOrDefault(
                            h =>
                                returnTypeSymbol.IsAssignableFrom(h.ReturnType) &&
                                h.Parameters[0].Type.IsAssignableFrom(originalTypeSymbol));
            }
            IMethodSymbol bestMatch = null;
            int lastSizeDiff = 1000;
            foreach (var member in type.GetMembers("op_Implicit")
                .OfType<IMethodSymbol>().Where(h => h.Parameters[0].Type.IsAssignableFrom(originalTypeSymbol)))
            {
                var sizediff = Math.Abs(member.ReturnType.SizeOf() - returnTypeSymbol.SizeOf());
                if (sizediff <= lastSizeDiff)
                {
                    lastSizeDiff = sizediff;
                    bestMatch = member;
                }
            }
            return bestMatch;
        }

        public static int SizeOf(this ITypeSymbol typeSymbol)
        {
            switch (typeSymbol.SpecialType)
            {
                case SpecialType.System_Boolean:
                    return sizeof (bool);
                case SpecialType.System_Byte:
                    return sizeof (byte);
                case SpecialType.System_Char:
                    return sizeof (char);
                case SpecialType.System_Decimal:
                    return sizeof (decimal);
                case SpecialType.System_Double:
                    return sizeof (double);
                case SpecialType.System_Enum: //TODO: Fix this
                    return sizeof (int);
                case SpecialType.System_Int16:
                    return sizeof (short);
                case SpecialType.System_Int32:
                    return sizeof (int);
                case SpecialType.System_Int64:
                    return sizeof (long);
                case SpecialType.System_SByte:
                    return sizeof (sbyte);
                case SpecialType.System_Single:
                    return sizeof (float);
                case SpecialType.System_UInt16:
                    return sizeof (ushort);
                case SpecialType.System_UInt32:
                    return sizeof (uint);
                case SpecialType.System_UInt64:
                    return sizeof (ulong);
                default:
                    return 100000;
            }
        }

        public static IMethodSymbol GetExplictCoversionOp(this ITypeSymbol type, ITypeSymbol returnTypeSymbol,
            ITypeSymbol originalTypeSymbol, bool allowNarrowing = false)
        {
            if (!returnTypeSymbol.IsPrimitiveInteger())
                allowNarrowing = false;

            if (!allowNarrowing)
            {
                return
                    type.GetMembers("op_Explicit")
                        .OfType<IMethodSymbol>()
                        .FirstOrDefault(
                            h =>
                                returnTypeSymbol.IsAssignableFrom(h.ReturnType) &&
                                h.Parameters[0].Type.IsAssignableFrom(originalTypeSymbol));
            }
            IMethodSymbol bestMatch = null;
            int lastSizeDiff = 1000;
            foreach (var member in type.GetMembers("op_Explicit")
                .OfType<IMethodSymbol>().Where(h =>
                    h.Parameters[0].Type.IsAssignableFrom(originalTypeSymbol)))
            {
                var sizediff = member.ReturnType.SizeOf() - returnTypeSymbol.SizeOf();
                if (sizediff <= lastSizeDiff)
                {
                    lastSizeDiff = sizediff;
                    bestMatch = member;
                }
            }
            return bestMatch;
        }
    }
}