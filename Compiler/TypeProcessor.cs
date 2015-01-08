// /*
//   SharpNative - C# to D Transpiler
//   (C) 2014 Irio Systems 
// */

#region Imports

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

#endregion

namespace SharpNative.Compiler
{
    internal static class TypeProcessor
    {
        static TypeProcessor()
        {
        }


        public static void ClearUsedTypes()
        {
            Context.Instance.UsedTypes.Clear();
        }

        public static void AddUsedType(ITypeSymbol type)
        {
            if (Context.Instance != null && (!Context.Instance.UsedTypes.Contains(type) && type != null))
            {
                var arrayType = type as IArrayTypeSymbol;
                if (arrayType != null)
                    AddUsedType(arrayType.ElementType);

                Context.Instance.UsedTypes.Add(type);
            }
        }


        public static string DefaultValue(TypeSyntax type)
        {
            return DefaultValue(TryGetTypeSymbol(type));
        }

        public static string DefaultValue(ITypeSymbol type)
        {
            var dType = ConvertType(type);

            if (type.TypeKind == TypeKind.Struct)
                return dType + ".init";

            switch (dType)
            {
                case "int":
                case "float":
                case "double":
                case "System.Namespace.Single":
                case "System.Namespace.Int":
                case "System.Namespace.Int16":
                case "System.Namespace.Int32":
                case "System.Namespace.Int64":
                case "System.Namespace.UInt":
                case "System.Namespace.UInt16":
                case "System.Namespace.UInt32":
                case "System.Namespace.UInt64":
                case "System.Namespace.Long":
                case "System.Namespace.Double":
                case "System.Namespace.Short":
                    return "0";
                case "System.Namespace.Char":
                    return "0";
                case "System.Namespace.Boolean":
                    return "false";

                default:
                    return "cast(" + dType + ") null";
            }
        }

        public static string TryConvertType(SyntaxNode node, bool localize = true)
        {
            if (node == null)
                return null;

            var attrs = Utility.GetSharpNativeAttribute(node);
            if (attrs.ContainsKey("ReplaceWithType"))
                return attrs["ReplaceWithType"];

            var sym = TryGetTypeSymbol(node, localize);

            if (sym == null)
                throw new Exception("Could not get type of " + node + " at " + Utility.Descriptor(node));
            return TryConvertType(sym, localize);
        }

        public static ITypeSymbol TryGetTypeSymbol(SyntaxNode node, bool localize = true)
        {
            var model = Program.GetModel(node).As<SemanticModel>();
            var typeInfo = model.GetTypeInfo(node);

            if (typeInfo.ConvertedType is IErrorTypeSymbol || typeInfo.ConvertedType == null)
            {
                typeInfo = model.GetTypeInfo(node.Parent);
                    //not sure why Roslyn can't find the type of some type nodes, but telling it to use the parent's seems to work
            }

            ITypeSymbol t;

            //TODO: do we have to do a search for whether the other is a base class here ?
            t = typeInfo.ConvertedType;

            if (!Equals(typeInfo.ConvertedType, typeInfo.Type) && typeInfo.Type != null)
                t = typeInfo.Type;

            AddUsedType(typeInfo.Type);

            if (t == null || t is IErrorTypeSymbol)
                return null;

            return t;
        }

        public static SymbolInfo GetSymbolInfo(SyntaxNode node)
        {
            var model = Program.GetModel(node).As<SemanticModel>();
            SymbolInfo typeInfo = model.GetSymbolInfo(node);

            return typeInfo;
        }


        public static ISymbol GetDeclaredSymbol(SyntaxNode node)
        {
            var model = Program.GetModel(node).As<SemanticModel>();
            var typeInfo = model.GetDeclaredSymbol(node);

            return typeInfo;
        }

        public static TypeInfo GetTypeInfo(SyntaxNode node)
        {
            var model = Program.GetModel(node).As<SemanticModel>();
            var typeInfo = model.GetTypeInfo(node);

            if (typeInfo.ConvertedType is IErrorTypeSymbol || typeInfo.ConvertedType == null)
            {
                typeInfo = model.GetTypeInfo(node.Parent);
                    //not sure why Roslyn can't find the type of some type nodes, but telling it to use the parent's seems to work
            }

            //TODO: test this out
            AddUsedType(typeInfo.Type);

            return typeInfo;
        }


        public static string ConvertType(SyntaxNode node, bool localize = true)
        {
            var ret = TryConvertType(node, localize);

            if (ret == null)
                throw new Exception("Type could not be determined for " + node);

            if (localize) //TODO: We need to check for collisions and thus fully qualify
            {
                var name =
                    Context.Instance.UsingDeclarations.FirstOrDefault(
                        k => ret.StartsWith(k.Name.ToFullString() + ".Namespace.", StringComparison.Ordinal));

                if (name != null)
                    ret = ret.RemoveFromStartOfString(name.Name.ToFullString() + ".Namespace.");
            }

            if (ret == "Array_T")
                return "Array";
            return ret;
        }

        public static string ConvertTypeFixPointer(SyntaxNode node)
        {
            var ret = TryConvertType(node);

            if (ret == null)
                throw new Exception("Type could not be determined for " + node);

            return ret;
        }


        public static string ConvertTypeWithColon(ITypeSymbol node, bool localize = true)
        {
            var ret = TryConvertType(node);

            if (ret == null)
                ret = "";
            else
                ret = ret + ".";

            if (localize)
            {
                var name =
                    Context.Instance.UsingDeclarations.FirstOrDefault(
                        k => ret.StartsWith(k.Name.ToFullString() + ".Namespace.", StringComparison.Ordinal));

                if (name != null)
                    ret = ret.RemoveFromStartOfString(name.Name.ToFullString() + ".Namespace.");
            }

            return ret;
        }

        private static readonly ConcurrentDictionary<ITypeSymbol, string> _cachedTypes =
            new ConcurrentDictionary<ITypeSymbol, string>();

        public static string ConvertType(ITypeSymbol typeSymbol, bool localize = true, bool usebasicname=true)
        {
            AddUsedType(typeSymbol);

            var ret = TryConvertType(typeSymbol);

            if (ret == null)
                throw new Exception("Could not convert type " + typeSymbol);

            if (!usebasicname)
                if (typeSymbol.IsPrimitive())
                {
                    ret = typeSymbol.ContainingNamespace.FullName() + "." + typeSymbol.Name;
                }

            if (localize)
            {
                var name =
                    Context.Instance.UsingDeclarations.FirstOrDefault(
                        k => ret.StartsWith(k.Name.ToFullString() + ".Namespace.", StringComparison.Ordinal));

                if (name != null)
                    ret = ret.RemoveFromStartOfString(name.Name.ToFullString() + ".Namespace.");
            }

            if (ret == "Array_T")
                return "Array";

            return ret;

        }

        public static string TryConvertType(ITypeSymbol typeInfo, bool localize = true)
        {
            string cachedValue;
            if (_cachedTypes.TryGetValue(typeInfo, out cachedValue))
                return cachedValue;

            cachedValue = ConvertTypeUncached(typeInfo, localize);
            _cachedTypes.TryAdd(typeInfo, cachedValue);
            return cachedValue;
        }

        private static string ConvertTypeUncached(ITypeSymbol typeSymbol, bool localize)
        {
            if (typeSymbol.IsAnonymousType)
                return WriteAnonymousObjectCreationExpression.TypeName(typeSymbol.As<INamedTypeSymbol>());

            var array = typeSymbol as IArrayTypeSymbol;

            if (array != null)
            {
                var typeString = TryConvertType(array.ElementType, localize);
                if (localize)
                {
                    var name =
                        Context.Instance.UsingDeclarations.FirstOrDefault(
                            k => typeString.StartsWith(k.Name.ToFullString() + ".Namespace.", StringComparison.Ordinal));

                    if (name != null)
                        typeString = typeString.RemoveFromStartOfString(name.Name.ToFullString() + ".Namespace.");
                }
                return "Array_T!(" + typeString + ")";
            }

            if (typeSymbol.TypeKind == TypeKind.PointerType)
            {
                var pointer = typeSymbol as IPointerTypeSymbol;
                return ConvertType(pointer.PointedAtType) + "*";
            }

            var typeInfoStr = typeSymbol.ToString();

            var named = typeSymbol as INamedTypeSymbol;

            if (typeSymbol.TypeKind == TypeKind.TypeParameter)
                return typeSymbol.Name;

            if (named != null && (named.ContainingNamespace.ToString() == "System" && named.Name == "Exception"))
                return "System.Namespace.NException";

            //TODO: Add explicit support for Nullable
            if (named != null && (named.Name == "Nullable" && named.ContainingNamespace.ToString() == "System"))
            {
                //Nullable types
                if (named.TypeArguments.Any())
                {
                    var convertedType = TryConvertType(named.TypeArguments.FirstOrDefault());

                    switch (convertedType)
                    {
                        case "Int":
                            return "int";
                        case "Boolean":
                            return "bool";
                        case "Byte":
                            return "ubyte";
                        case "Short":
                            return "short";
                        case "Float":
                            return "float";
                        case "Double":
                            return "double";
                        case "Char":
                            return "wchar"; // C#'s chars are 16-bit unicode
                        case "Long":
                            return "long";
                        default:
                            return convertedType;
                    }
                }
            }

            var typeStr = GenericTypeName(typeSymbol);

            if (named != null && named.IsGenericType && !named.IsUnboundGenericType && TypeArguments(named).Any())
            {
                return TryConvertType(named.ConstructUnboundGenericType()) + "!(" +
                       string.Join(", ", TypeArguments(named).Select(o => TryConvertType(o))) + ")";
            }

            switch (typeStr)
            {
                case "System.Namespace.Void":
                    return "void";

                case "System.Namespace.Boolean":
                    return "bool";

                case "System.Object":
                case "System.Namespace.Object":
                    return "NObject";

                case "System.Namespace.UInt64":
                    return "ulong";

                case "System.Namespace.Double":
                    return "double";

                case "System.Namespace.Single":
                    return "float";

                case "System.Namespace.String":
                    return "String";

                case "System.Namespace.Int32":
                    return "int";

                case "System.Namespace.UInt16":
                    return "ushort";

                case "System.Namespace.Int64":
                    return "long";

                case "System.Namespace.UInt32":
                    return "long"; // Looks like d's uint32 is smaller than C#'s

                case "System.Namespace.Byte":
                    return "ubyte";

                case "System.Namespace.SByte":
                    return "byte";

                case "System.Namespace.Int16":
                    return "short";

                case "System.Namespace.Char":
                    return "wchar";

                case "System.Namespace.Array":
                    return "Array"; //All template (generic) classes have atleast one "_T" appended

                default:

                    if (named != null)
                        return typeSymbol.ContainingNamespace.FullNameWithDot() + WriteType.TypeName(named);

                    //This type does not get translated and gets used as-is
                    return typeSymbol.ContainingNamespace.FullNameWithDot() + typeSymbol.Name;
            }
        }

        private static IEnumerable<ITypeSymbol> TypeArguments(INamedTypeSymbol named)
        {
            if (named.ContainingType != null)
            {
                //Hard-code generic types for dictionaries, since I can't find a way to determine them programatically
                switch (named.Name)
                {
                    case "ValueCollection":
                        return new[]
                        {
                            named.ContainingType.TypeArguments.ElementAt(1)
                        };
                    case "KeyCollection":
                        return new[]
                        {
                            named.ContainingType.TypeArguments.ElementAt(0)
                        };
                    default:
                        return named.TypeArguments.ToList();
                }
            }

            return named.TypeArguments.ToList();
        }


        public static string GenericTypeName(ITypeSymbol typeSymbol)
        {
            if (typeSymbol == null)
                return null;

            var array = typeSymbol as IArrayTypeSymbol;
            if (array != null)
                return GenericTypeName(array.ElementType) + "[]";

            var named = typeSymbol as INamedTypeSymbol;

            if (named != null && named.IsGenericType && !named.IsUnboundGenericType)
                return GenericTypeName(named.ConstructUnboundGenericType());
            if (named != null && named.SpecialType != SpecialType.None)
            {
                return named.ContainingNamespace.FullNameWithDot() + named.Name;
                    //this forces C# shortcuts like "int" to never be used, and instead returns System.Int32 etc.
            }
            return typeSymbol.ToString();
        }


        public static bool IsPrimitiveType(string type)
        {
            switch (type)
            {
                case "byte":
                case "sbyte":
                case "int":
                case "float":
                case "double":
                case "short":
                case "long":
                case "wchar":
                case "bool":
                    return true;
                default:
                    return false;
            }
        }


        public static SemanticModel GetSemanticModel(SyntaxNode e)
        {
            return Program.GetModel(e);
        }
    }
}