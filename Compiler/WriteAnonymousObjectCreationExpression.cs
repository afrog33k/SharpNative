// /*
//   SharpNative - C# to D Transpiler
//   (C) 2014 Irio Systems 
// */

#region Imports

using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

#endregion

namespace SharpNative.Compiler
{
    internal static class WriteAnonymousObjectCreationExpression
    {
        public static void Go(OutputWriter writer, AnonymousObjectCreationExpressionSyntax expression)
        {
            writer.Write("new ");
            writer.Write(TypeName(expression));
            writer.Write("(");

            bool first = true;
            foreach (var field in expression.Initializers.OrderBy(o => o.Name()))
            {
                if (first)
                    first = false;
                else
                    writer.Write(", ");

                Core.Write(writer, field.Expression);
            }

            writer.Write(")");
        }

        public static string TypeName(AnonymousObjectCreationExpressionSyntax expression)
        {
            return TypeName(TypeProcessor.GetTypeInfo(expression).Type.As<INamedTypeSymbol>());
        }

        public static string TypeName(INamedTypeSymbol symbol)
        {
            var fields = symbol.GetMembers().OfType<IPropertySymbol>();

            fields = fields as IPropertySymbol[] ?? fields.ToArray();
            var typeParams = fields.Where(o => o.Type.TypeKind == TypeKind.TypeParameter);

            var genericPrefix = typeParams.None()
                ? ""
                : (" template <" + string.Join(", ", typeParams.Select(o => "typename " + o.Type.Name).Distinct()) +
                                ">\r\n");

            return genericPrefix + "Anon_" + string.Join("__",
                fields
                    .OrderBy(o => o.Name)
                    .Select(o => o.Name + "_" + TypeProcessor.ConvertType(o.Type, false).Replace(".", "_")))
                // No need localizing these
            + genericPrefix;
        }

        public static void WriteAnonymousType(AnonymousObjectCreationExpressionSyntax syntax)
        {
            var type = TypeProcessor.GetTypeInfo(syntax).Type.As<INamedTypeSymbol>();

            Context.Instance.Type = type;

            TypeProcessor.ClearUsedTypes();

            var mynamespace = Context.Instance.Type.ContainingNamespace.FullName().RemoveFromEndOfString(".Namespace");
            // + "." + TypeState.Instance.TypeName;
            Context.Instance.Namespace = mynamespace;
            var myUsingDirective = SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(mynamespace));
            var SystemUsingDirective = SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System"));
            // Required as certain functions like boxing are in this namespace

            Context.Instance.UsingDeclarations =
                syntax.Parent.DescendantNodes().OfType<UsingDirectiveSyntax>().ToArray()
                    .Union(new[]
                {
                    myUsingDirective, SystemUsingDirective
                }).ToArray();

            //using (var writer = new CppWriter (TypeState.Instance.Namespace, )) {

            //Ty

            var anonName = TypeName(type);
            using (var writer = new OutputWriter(Context.Instance.Namespace, StripGeneric(anonName)))
            {
                var fields = type.GetMembers().OfType<IPropertySymbol>().OrderBy(o => o.Name).ToList();

//                writer.WriteLine("namespace anonymoustypes {");
                WriteStandardIncludes.Go(writer);

                writer.WriteIndent();
                writer.Write("class ");
                writer.Write(anonName);

                writer.OpenBrace();

                foreach (var field in fields)
                {
                    writer.WriteIndent();
                    writer.Write("public ");
                    writer.Write(TypeProcessor.ConvertType(field.Type) + " ");
                    writer.Write(WriteIdentifierName.TransformIdentifier(field.Name));
                    writer.Write(" = " + TypeProcessor.DefaultValue(field.Type));
                    writer.Write(";\r\n");
                }

                //Must Write a constructor here ...
                writer.Write("\r\nthis (");
                bool first = true;
                foreach (var field in fields)
                {
                    if (first)
                        first = false;
                    else
                        writer.Write(", ");

                    writer.Write(TypeProcessor.ConvertType(field.Type) + " ");
                    writer.Write("_" + WriteIdentifierName.TransformIdentifier(field.Name));
                }
                writer.Write(")\r\n");
                writer.OpenBrace();
                writer.Indent++;

                foreach (var field in fields)
                {
                    var fieldNAme = WriteIdentifierName.TransformIdentifier(field.Name);

                    writer.WriteLine(fieldNAme + " = _" + fieldNAme + ";");
                }
                writer.Indent--;

                writer.CloseBrace();

                writer.CloseBrace();
//                writer.Write("};");
//                writer.Write("}");
            }
        }

        private static string StripGeneric(string anonName)
        {
            var i = anonName.IndexOf('[');
            if (i == -1)
                return anonName;
            return anonName.Substring(0, i);
        }
    }
}