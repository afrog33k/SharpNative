// /*
//   SharpNative - C# to D Transpiler
//   (C) 2014 Irio Systems 
// */

#region Imports

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

#endregion

namespace SharpNative.Compiler
{
    public static class Utility
    {

        public static ISymbol[] GetAllMembers(this ITypeSymbol symbol)
        {
            List<ISymbol> members = new List<ISymbol>();
            members = symbol.GetMembers().Union(symbol.AllInterfaces.SelectMany(j=>j.GetMembers())).ToList();
            

            return members.ToArray();
        }
        private static bool WriteArrayElements(InitializerExpressionSyntax initializer, OutputWriter writer,
            Dictionary<int, int> inferred, int level, bool omitbraces = false)
        {
            level++;
            if (!omitbraces)
                writer.Write("[");

            bool first = true;
            if (!inferred.ContainsKey(level))
                inferred.Add(level, initializer.Expressions.Count);
            foreach (var iexpression in initializer.Expressions)
            {
                if (first)
                    first = false;
                else
                    writer.Write(",");
                if (iexpression is InitializerExpressionSyntax)
                    WriteArrayElements(iexpression as InitializerExpressionSyntax, writer, inferred, level, omitbraces);
                else
                    Core.Write(writer, iexpression);
            }
            if (!omitbraces)
                writer.Write("]");
            return first;
        }

        public static void WriteArrayInitializer(this InitializerExpressionSyntax initializer, OutputWriter writer,
            TypeSyntax type = null)
        {
            bool first = true;
            ArrayTypeSyntax arrayType = null;
            ExpressionSyntax[] fullDimension = null;
            var inferredDimensions = new Dictionary<int, int>();
            ITypeSymbol aType = null;
            IArrayTypeSymbol _aType = null;
            if (type != null)
            {
                arrayType = type as ArrayTypeSyntax;

                if (arrayType != null)
                    aType = TypeProcessor.GetTypeInfo(arrayType).Type as IArrayTypeSymbol;
                else
                    aType = (ITypeSymbol) TypeProcessor.GetTypeInfo(type).Type;

                _aType = aType as IArrayTypeSymbol;
            }
            //For ommitedsizes, lets just count manually
            List<int> inferredSizes = new List<int>();
            if (initializer != null)
            {
                if (initializer.Expressions.Count > 0)
                {
                    if (_aType != null)
                    {
                        writer.Write("__CC!(" + TypeProcessor.ConvertType(_aType.ElementType)
                            /*+ Enumerable.Range (0, _aType.Rank).Select (l => "[]").Aggregate ((a, b) => a + b).ToString ()*/+
                                     "[])(");
                    }

                    if (_aType.Rank == 1)
                        first = WriteArrayElements(initializer, writer, inferredDimensions, 0);
                    else //multi
                    {
                        writer.Write("[");
                        first = WriteArrayElements(initializer, writer, inferredDimensions, 0, true);
                        writer.Write("]");
                    }
                    //inferredDimensions = inferredDimensions.Take (_aType.Rank).ToList();
                    inferredSizes = inferredDimensions.Select(k => k.Value).ToList();
                    if (_aType != null)
                        writer.Write(")");
                }
            }

            if (type != null)
            {
                if (arrayType != null && _aType != null)
                {
                    //Don't do this for jagged arrays ...rank = 1 and .GetLength(>0) throws exception
                    //	if(aType.Rank > 1)
                    {
                        fullDimension =
                            arrayType.RankSpecifiers.Select(o => o.Sizes).SelectMany(j => j).ToArray();

                        if (_aType.Rank > 1)
                        {
                            //If we have a multi-array, we have to have dimensions
                            if (!fullDimension.Any(l => l is OmittedArraySizeExpressionSyntax))
                            {
                                for (int i = 0, fullDimensionLength = fullDimension.Length;
                                    i < fullDimensionLength;
                                    i++)
                                {
                                    var dimensions = fullDimension[i];
                                    if (dimensions is OmittedArraySizeExpressionSyntax)
                                        continue;
                                    if (first)
                                        first = false;
                                    else
                                        writer.Write(" , ");
                                    // MultidimArray
                                    Core.Write(writer, dimensions);
                                }
                            }
                            else
                            {
                                for (int i = 0, fullDimensionLength = inferredSizes.Count; i < fullDimensionLength; i++)
                                {
                                    var dimensions = inferredSizes[i];

                                    if (first)
                                        first = false;
                                    else
                                        writer.Write(" , ");
                                    // MultidimArray
                                    writer.Write(dimensions.ToString());
                                }
                            }
                        }
                        else
                        {
                            var dimensions = fullDimension[0];

                            if (!(dimensions is OmittedArraySizeExpressionSyntax))
                            {
                                if (first)
                                    first = false;
                                else
                                    writer.Write(" , ");

                                Core.Write(writer, dimensions);
                            }
                        }
                    }
                }
            }
        }

        public static AttributeSyntax GetAttribute(this SyntaxNode node, INamedTypeSymbol name)
        {
            AttributeSyntax attr = null;

            if (node is BaseMethodDeclarationSyntax || node is BaseTypeDeclarationSyntax ||
                node is BaseFieldDeclarationSyntax)
            {
                var list = (node is BaseMethodDeclarationSyntax
                    ? node.As<BaseMethodDeclarationSyntax>().AttributeLists
                    : node is BaseTypeDeclarationSyntax
                        ? node.As<BaseTypeDeclarationSyntax>().AttributeLists
                        : node.As<BaseFieldDeclarationSyntax>().AttributeLists).ToList();

                if (list.Count > 0)
                {
                    attr =
                        list.SelectMany(o => o.Attributes)
                            .SingleOrDefault(o => TypeProcessor.TryGetTypeSymbol(o) == name);
                    if (attr != null)
                        return attr;
                }
            }

            return null;
        }

       


        public static bool IsBasicType(this INamedTypeSymbol containingType)
        {
            if (!containingType.IsValueType)
                return false;
            var specialType = containingType.SpecialType;
            if (specialType == SpecialType.System_Boolean || specialType == SpecialType.System_Byte ||
                specialType == SpecialType.System_Char
                || specialType == SpecialType.System_Double || specialType == SpecialType.System_Enum ||
                specialType == SpecialType.System_Int16
                || specialType == SpecialType.System_Int32 || specialType == SpecialType.System_Int64 ||
                specialType == SpecialType.System_IntPtr
                || specialType == SpecialType.System_Decimal || specialType == SpecialType.System_Double ||
                specialType == SpecialType.System_Single || specialType == SpecialType.System_SByte
                || specialType == SpecialType.System_UInt16 || specialType == SpecialType.System_UInt32 ||
                specialType == SpecialType.System_UInt64 || specialType == SpecialType.System_UIntPtr ||
                specialType == SpecialType.System_Void)

                return true;

            return false;
        }

        public static IEnumerable<T> DistinctBy<T, TIdentity>(this IEnumerable<T> source,
            Func<T, TIdentity> identitySelector)
        {
            return source.Distinct(By(identitySelector));
        }

        public static IEqualityComparer<TSource> By<TSource, TIdentity>(Func<TSource, TIdentity> identitySelector)
        {
            return new DelegateComparer<TSource, TIdentity>(identitySelector);
        }

        private class DelegateComparer<T, TIdentity> : IEqualityComparer<T>
        {
            private readonly Func<T, TIdentity> identitySelector;

            public DelegateComparer(Func<T, TIdentity> identitySelector)
            {
                this.identitySelector = identitySelector;
            }

            public bool Equals(T x, T y)
            {
                return Equals(identitySelector(x), identitySelector(y));
            }

            public int GetHashCode(T obj)
            {
                return identitySelector(obj).GetHashCode();
            }
        }


        public static string NamespaceModuleName = "Namespace";

        public static string GlobalNamespaceName = "CsRoot";

        public static string BoxedPrefix = "__Boxed_";


        public static string GetModuleName(this INamespaceSymbol typeSymbol)
        {
            var fullName = typeSymbol.FullName();

//            if (typeSymbol.IsGlobalNamespace)
//            {
//                fullName = GlobalNamespaceName;
//            }
//            string result = fullName +"."+ NamespaceModuleName;
            return fullName;
        }

        

        public static string GetModuleName(this ITypeSymbol type)
        {
            return type.ContainingNamespace.FullName(false) + "." + type.GetNameD() + "." + type.GetNameD();
        }

        public static string GetBoxedModuleName(this ITypeSymbol type)
        {

          

          //Better use typeprocessor as this is leading to many issues

            return type.ContainingNamespace.FullName(false) + "."  + type.GetNameD() + "." + BoxedPrefix + type.GetNameD();

        
        }

        public static string GetNameD(this ITypeSymbol type, bool removeGenerics=true)
        {
            var name=TypeProcessor.ConvertType(type,true,false).RemoveFromStartOfString(type.ContainingNamespace.FullName()+".");

            return removeGenerics? Regex.Replace(name, @" ?!\(.*?\)", string.Empty) : name;

        }


        public static string GetFullNameD(this ITypeSymbol typeInfo, bool includeNamespace = true)
        {
            return TypeProcessor.ConvertType(typeInfo, false);
        }

        public static string GetFullNameCSharp(this ITypeSymbol typeInfo, bool includeNamespace = true)
        {

//            if (typeInfo is INamedTypeSymbol)
//                return (typeInfo as INamedTypeSymbol).FullName();

            //Todo add support for global

            var name = (typeInfo.ContainingNamespace != null && includeNamespace
                ? (typeInfo.ContainingNamespace + ".")
                : "") +
                       (typeInfo.ContainingType != null ? (typeInfo.ContainingType.FullName() + ".") : "");

            var namedType = typeInfo as INamedTypeSymbol;

            if (namedType == null)
                return name + typeInfo.Name;

            name = name.Replace("<global namespace>.", "");

            if (namedType != null && namedType.TypeArguments.Count() == 0)
                return name + typeInfo.Name;

            name += name + typeInfo.Name + "<" +
                    String.Join(",",
                        ((INamedTypeSymbol) typeInfo).TypeArguments.Select(o => o.ToString())) + ">";

            return name;
        }

        public static void Parallel<T>(this IEnumerable<T> list, Action<T> action)
        {
#if true
            System.Threading.Tasks.Parallel.ForEach(list, action);
#else
			foreach (var t in list)
				action(t);
#endif
        }

        public static SyntaxTokenList GetModifiers(this MemberDeclarationSyntax member)
        {
            var field = member as FieldDeclarationSyntax;
            if (field != null)
                return field.Modifiers;

            var method = member as BaseMethodDeclarationSyntax;
            if (method != null)
                return method.Modifiers;

            var property = member as PropertyDeclarationSyntax;
            if (property != null)
                return property.Modifiers;

            var dlg = member as DelegateDeclarationSyntax;
            if (dlg != null)
                return dlg.Modifiers;

            var type = member as BaseTypeDeclarationSyntax;
            if (type != null)
                return type.Modifiers;

            var indexer = member as IndexerDeclarationSyntax;
            if (indexer != null)
                return indexer.Modifiers;

            throw new Exception("Need handler for member of type " + member.GetType().Name);
        }

        public static string FullName(this ITypeSymbol ns)
        {
            var name =
                (!(ns.ContainingSymbol is INamespaceSymbol) && !String.IsNullOrEmpty(ns.Name)
                    ? (ns.ContainingSymbol.Name + ".")
                    : "")+ ns.Name.Replace(".", "_");
            name = name.Replace(ns.ContainingNamespace.FullName() + "_", ns.ContainingNamespace.FullName() + ".");

            return name;
        }

        public static string FullName(this INamespaceSymbol ns, bool namespacesuffix=true)
        {
            if (ns == null)
                return "";
            if (ns.IsGlobalNamespace)
                return "CsRoot" +  (namespacesuffix? ("."+NamespaceModuleName) :"");
            else
                return ns.ToString() + (namespacesuffix ? ("." + NamespaceModuleName) : "");
        }

        public static string FullNameWithDot(this INamespaceSymbol ns, bool namespacesuffix = true)
        {
            if (ns == null)
                return "";
            if (ns.IsGlobalNamespace)
                return "CsRoot" + (namespacesuffix ? ("." + NamespaceModuleName) : "") + ".";
            else
                return ns.ToString() + (namespacesuffix ? ("." + NamespaceModuleName) : "") + ".";
        }

        public static string FullNameWithDotCSharp(this INamespaceSymbol ns, bool namespacesuffix = true)
        {
            if (ns == null)
                return "";
            if (ns.IsGlobalNamespace)
                return "";
            else
                return ns.ToString() + ".";
        }

        public static string AttributeOrNull(this XElement element, string attrName)
        {
            var a = element.Attribute(attrName);
            if (a == null)
                return null;
            else
                return a.Value;
        }

        public static T As<T>(this object o)
        {
            return (T) o;
        }

        public static string SubstringSafe(this string s, int startAt, int length)
        {
            if (s.Length < startAt + length)
                return s.Substring(startAt);
            else
                return s.Substring(startAt, length);
        }

        public static bool None<T>(this IEnumerable<T> a, Func<T, bool> pred)
        {
            return !a.Any(pred);
        }

        public static bool None<T>(this IEnumerable<T> a)
        {
            return !a.Any();
        }

        public static string SubstringAfterLast(this string s, char c)
        {
            int i = s.LastIndexOf(c);
            if (i == -1)
                throw new Exception("char not found");
            return s.Substring(i + 1);
        }

        public static string TrySubstringAfterLast(this string s, char c)
        {
            int i = s.LastIndexOf(c);
            if (i == -1)
                return s;
            else
                return s.Substring(i + 1);
        }

        public static string TrySubstringBeforeFirst(this string s, char c)
        {
            int i = s.IndexOf(c);
            if (i == -1)
                return s;
            else
                return s.Substring(0, i);
        }

        public static string SubstringBeforeLast(this string s, char c)
        {
            int i = s.LastIndexOf(c);
            if (i == -1)
                throw new Exception("char not found");
            return s.Substring(0, i);
        }

        public static IMethodSymbol UnReduce(this IMethodSymbol methodSymbol)
        {
            while (methodSymbol.ReducedFrom != null)
                methodSymbol = methodSymbol.ReducedFrom;

            return methodSymbol;
        }

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> array, bool throwOnDuplicate)
        {
            var hs = new HashSet<T>();
            foreach (var t in array)
            {
                if (throwOnDuplicate && hs.Contains(t))
                    throw new ArgumentException("Duplicate key: " + t.ToString());
                hs.Add(t);
            }
            return hs;
        }


        public static IEnumerable<T> Concat<T>(this IEnumerable<T> array, T item)
        {
            return array.Concat(new T[] {item});
        }

        public static IEnumerable<T> Except<T>(this IEnumerable<T> array, T item)
        {
            return array.Except(new T[] {item});
        }


        public static string Descriptor(SyntaxNode node)
        {
            var sb = new StringBuilder();
            sb.Append(node.Span.ToString() + " ");

            while (node != null)
            {
                if (node is BaseTypeDeclarationSyntax)
                    sb.Append("Type: " + node.As<BaseTypeDeclarationSyntax>().Identifier.ValueText + ", ");
                else if (node is MethodDeclarationSyntax)
                    sb.Append("Method: " + node.As<MethodDeclarationSyntax>().Identifier.ValueText + ", ");
                else if (node is PropertyDeclarationSyntax)
                    sb.Append("Property: " + node.As<PropertyDeclarationSyntax>().Identifier.ValueText + ", ");
                node = node.Parent;
            }

            if (sb.Length > 2)
                sb.Remove(sb.Length - 2, 2);
            return sb.ToString();
        }

        public static Dictionary<string, string> GetSharpNativeAttribute(SyntaxNode node)
        {
            AttributeSyntax attr = null;

            while (node != null)
            {
                if (node is BaseMethodDeclarationSyntax || node is BaseTypeDeclarationSyntax ||
                    node is BaseFieldDeclarationSyntax)
                {
                    var list = node is BaseMethodDeclarationSyntax
                        ? node.As<BaseMethodDeclarationSyntax>().AttributeLists
                        : node is BaseTypeDeclarationSyntax
                            ? node.As<BaseTypeDeclarationSyntax>().AttributeLists
                            : node.As<BaseFieldDeclarationSyntax>().AttributeLists;

                    attr = list.SelectMany(o => o.Attributes).SingleOrDefault(o => o.Name.ToString() == "SharpNative");
                    if (attr != null)
                        break;
                }
                node = node.Parent;
            }

            if (attr == null || attr.ArgumentList == null)
                return new Dictionary<string, string>();

            return attr.ArgumentList.Arguments.ToDictionary(GetAttributeName,
                o => o.Expression.As<LiteralExpressionSyntax>().Token.ValueText);
        }

        private static string GetAttributeName(AttributeArgumentSyntax attr)
        {
            return attr.NameEquals.Name.ToString();
        }

        public static string RemoveFromStartOfString(this string s, string toRemove)
        {
            if (!s.StartsWith(toRemove))
                return s;
            return s.Substring(toRemove.Length);
        }

        public static string RemoveFromEndOfString(this string s, string toRemove)
        {
            if (!s.EndsWith(toRemove))
                return s;
            return s.Substring(0, s.Length - toRemove.Length);
        }

        public static string TryGetIdentifier(ExpressionSyntax expression)
        {
            var identifier = expression as IdentifierNameSyntax;

            if (identifier != null)
                return WriteIdentifierName.TransformIdentifier(identifier.Identifier.ValueText);

            var thisSyntax = expression as ThisExpressionSyntax;
            if (thisSyntax != null)
                return "this";

            var memAccess = expression as MemberAccessExpressionSyntax;
            if (memAccess != null && memAccess.Expression is ThisExpressionSyntax)
                return memAccess.ToString();

            return null;
        }

       

       

        public static string Name(this AnonymousObjectMemberDeclaratorSyntax member)
        {
            return member.NameEquals != null
                ? member.NameEquals.Name.Identifier.ValueText
                : member.Expression.ToString();
        }

        public static bool IsNumeric(ITypeSymbol typeSymbol)
        {
            switch (typeSymbol.SpecialType)
            {
                case SpecialType.System_Int16:
                case SpecialType.System_Int32:
                case SpecialType.System_Int64:
                case SpecialType.System_UInt16:
                case SpecialType.System_UInt32:
                case SpecialType.System_UInt64:
                case SpecialType.System_Double:
                case SpecialType.System_Single:
                    return true;
                default:
                    return false;
            }
        }
    }
}