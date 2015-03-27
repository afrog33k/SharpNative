// /*
//   SharpNative - C# to D Transpiler
//   (C) 2014 Irio Systems 
// */

#region Imports

using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using CSharpExtensions = Microsoft.CodeAnalysis.CSharpExtensions;

#endregion

namespace SharpNative.Compiler
{
    internal static class WriteField
    {
        public static void Go(OutputWriter writer, FieldDeclarationSyntax field)
        {
            foreach (var declaration in field.Declaration.Variables)
            {
              
                //TODO: Add support for threadstatic
                Go(writer, field, field.Modifiers,
                    WriteIdentifierName.TransformIdentifier(declaration.Identifier.ValueText), field.Declaration.Type,
                    declaration.Initializer,field.IsThreadStatic());
            }
        }

        public static void Go(OutputWriter writer, EventFieldDeclarationSyntax field)
        {
            foreach (var declaration in field.Declaration.Variables)
            {
               
                    Go(writer, field, field.Modifiers,
                        WriteIdentifierName.TransformIdentifier(declaration.Identifier.ValueText),
                        field.Declaration.Type, declaration.Initializer,field.IsThreadStatic());
                
            }
        }

        private static bool IsThreadStatic(this FieldDeclarationSyntax field)
        {
            return field.AttributeLists.Any(
                l =>
                    l.Attributes.Any(
                        a => a.Name is QualifiedNameSyntax ? (a.Name.As<QualifiedNameSyntax>().Right.Identifier.ValueText == "ThreadStatic") :
                        a.Name is IdentifierNameSyntax && a.Name.As<IdentifierNameSyntax>().Identifier.ValueText == "ThreadStatic"
                        ));
        }

        private static bool IsThreadStatic(this EventFieldDeclarationSyntax field)
        {
            return field.AttributeLists.Any(
                l =>
                    l.Attributes.Any(
                        a => a.Name is QualifiedNameSyntax ? (a.Name.As<QualifiedNameSyntax>().Right.Identifier.ValueText == "ThreadStatic") :
                        a.Name is IdentifierNameSyntax && a.Name.As<IdentifierNameSyntax>().Identifier.ValueText == "ThreadStatic"));
        }


        public static void Go(OutputWriter writer, MemberDeclarationSyntax field, SyntaxTokenList modifiers, string name,
            TypeSyntax type, EqualsValueClauseSyntax initializerOpt = null, bool isThreadStatic =false)
        {
            writer.WriteIndent();

            var typeinfo = TypeProcessor.GetTypeInfo(type);

//            var isPtr = typeinfo.Type != null && (typeinfo.Type.IsValueType || typeinfo.Type.TypeKind==TypeKind.TypeParameter) ? "" : "";

            var typeStringNoPtr = TypeProcessor.ConvertType(type);

            var typeString = typeStringNoPtr + " ";

            var isConst = IsConst(modifiers, initializerOpt, type);

            var isStatic = isConst;
            //Handle Auto Properties

            // if (modifiers.Any(SyntaxKind.PrivateKeyword)) // Reflection cannot work with this, cant get address or set value
            //   writer.Write("private ");

            if (modifiers.Any(SyntaxKind.PublicKeyword) || modifiers.Any(SyntaxKind.InternalKeyword) ||
                modifiers.Any(SyntaxKind.ProtectedKeyword) || modifiers.Any(SyntaxKind.AbstractKeyword))
                writer.Write("public ");


            if (isThreadStatic)
            {
                writer.Write("static ");
            }

            if (modifiers.Any(SyntaxKind.StaticKeyword) || modifiers.Any(SyntaxKind.ConstKeyword))
            {
                isStatic = true;
                writer.Write("__gshared ");
            }

          

            if (isConst)
            {
                writer.Write("const ");
//                writer.Write("const ");
            }

            var @event = field is EventFieldDeclarationSyntax;
            if (@event)
                typeString = ("__Event!(" + typeString + ")");

            writer.Write(typeString);
//                if (isStatic)
//                    writer.Write(typeString);

            writer.Write(name);
            if (isStatic)
            {
//                var fieldInfo = TypeProcessor.GetDeclaredSymbol(field.Parent);
//                if (fieldInfo != null)
//                    writer.Write((!string.IsNullOrEmpty(fieldInfo.ContainingNamespace.FullName())?(fieldInfo.ContainingNamespace.FullName()+".") :"") + fieldInfo.Name+"."+name);
//				writer.Write(name);
            }

            if (!isStatic || isConst)
            {
                if (initializerOpt != null)
                {
					writer.Write(String.Format(" = cast({0})", typeString));

                    if (CSharpExtensions.CSharpKind(initializerOpt.Value) == SyntaxKind.CollectionInitializerExpression ||
                        CSharpExtensions.CSharpKind(initializerOpt.Value) == SyntaxKind.ArrayInitializerExpression)
                    {
//                        writer.Write("gc::gc_ptr< " + typeStringNoPtr + " >(");
                        writer.Write(" new " + typeString + " (");
                        var intializer = initializerOpt.Value as InitializerExpressionSyntax;
                        intializer.WriteArrayInitializer(writer, type);

                        writer.Write(")");
                    }

                    else
                        Core.Write(writer, initializerOpt.Value);
                }

                else
                {
                   // if (typeinfo.Type.TypeKind != TypeKind.Struct)
                    {
                        writer.Write(" = ");
                        if (typeinfo.Type.TypeKind == TypeKind.Delegate)
                            writer.Write("new " + typeString + "()");
                        else
                            writer.Write(TypeProcessor.DefaultValue(type));
                    }
                }
            
            }
            else
            {
                var staticWriter = new TempWriter();

                if (initializerOpt != null)
                {
                    staticWriter.Write(name);

                    staticWriter.Write(" = ");

                    if (CSharpExtensions.CSharpKind(initializerOpt.Value) == SyntaxKind.CollectionInitializerExpression ||
                        CSharpExtensions.CSharpKind(initializerOpt.Value) == SyntaxKind.ArrayInitializerExpression)
                    {
                        staticWriter.Write("new " + typeStringNoPtr + " (");

                        var intializer = initializerOpt.Value as InitializerExpressionSyntax;

                        intializer.WriteArrayInitializer(staticWriter, type);
                        staticWriter.Write(")");
                    }
                    else

                        Core.Write(staticWriter, initializerOpt.Value);

                    staticWriter.Write(";");

                    staticWriter.WriteLine();

                    Context.Instance.StaticInits.Add(staticWriter.ToString());

                }

                else if (typeinfo.Type.TypeKind != TypeKind.Struct)

                {
                    staticWriter.Write(name);
                    staticWriter.Write(" = ");

                    if (typeinfo.Type.TypeKind == TypeKind.Delegate)
                        staticWriter.Write("new " + typeString + "()");
                    else
                        staticWriter.Write(TypeProcessor.DefaultValue(type));

                    staticWriter.Write(";");

                    staticWriter.WriteLine();

                    Context.Instance.StaticInits.Add(staticWriter.ToString());
                }

               


            }

           writer.Write(";");
                writer.WriteLine();
        }

        public static bool IsConst(SyntaxTokenList modifiers, EqualsValueClauseSyntax initializerOpt, TypeSyntax type)
        {
            var t = TypeProcessor.ConvertType(type);

            return modifiers.Any(SyntaxKind.ConstKeyword);
        }

        private static void WriteThreadStatic(OutputWriter writer, VariableDeclaratorSyntax declaration,
            FieldDeclarationSyntax field)
        {
            throw new NotImplementedException("ThreadStatic");
        }

        private static void WriteThreadStatic(OutputWriter writer, VariableDeclaratorSyntax declaration,
            EventFieldDeclarationSyntax field)
        {
            var type = TypeProcessor.ConvertType(field.Declaration.Type);
            throw new NotImplementedException("ThreadStatic");
        }
    }
}