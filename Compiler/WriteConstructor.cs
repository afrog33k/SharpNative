// /*
//   SharpNative - C# to D Transpiler
//   (C) 2014 Irio Systems 
// */

#region Imports

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

#endregion

namespace SharpNative.Compiler
{
    internal static class WriteDestructorBody
    {
        public static void WriteDestructor(OutputWriter writer, DestructorDeclarationSyntax destructor)
        {
            if (destructor.Body == null)
                return;

            writer.WriteLine("~this()");
            writer.OpenBrace();

            if (destructor.Body != null)
            {
                foreach (var statement in destructor.Body.As<BlockSyntax>().Statements)
                    Core.Write(writer, statement);
            }

            writer.CloseBrace();
        }
    }

    internal static class WriteConstructorBody
    {
        public static void Go(OutputWriter writer, ConstructorDeclarationSyntax constructor)
        {
            //Only write out static constructors here.  If we encounter an instance constructor, we can ignore it since it's already written out by WriteType
            if (constructor.Modifiers.Any(SyntaxKind.StaticKeyword))
                WriteStaticConstructor(writer, constructor, null);
            else
                WriteInstanceConstructor(writer, constructor);
        }

        public static void WriteInstanceConstructor(OutputWriter writer, ConstructorDeclarationSyntax method)
        {
            var methodSymbol = (IMethodSymbol) TypeProcessor.GetDeclaredSymbol(method);
            var methodName = OverloadResolver.MethodName(methodSymbol);
            //			var methodType = Program.GetModel(method).GetTypeInfo(method);

            //TODO: Improve partial class / method support
            if (method.Modifiers.Any(SyntaxKind.PartialKeyword) && method.Body == null)
            {
                //We only want to render out one of the two partial methods.  If there's another, skip this one.
                if (Context.Instance.Partials.SelectMany(o => o.Syntax.As<ClassDeclarationSyntax>().Members)
                    .OfType<ConstructorDeclarationSyntax>()
                    .Except(method).Any(o => o.Identifier.ValueText == method.Identifier.ValueText))
                    return;
            }

            //            if (method.Identifier.ValueText == "GetEnumerator")
            //                return; //TODO: Support enumerator methods

            writer.WriteLine();
            //            writer.WriteIndent();

            var accessmodifiers = "";

            var isInterface = method.Parent is InterfaceDeclarationSyntax;

            if (method.Modifiers.Any(SyntaxKind.PublicKeyword) || method.Modifiers.Any(SyntaxKind.InternalKeyword) ||
                method.Modifiers.Any(SyntaxKind.ProtectedKeyword) || method.Modifiers.Any(SyntaxKind.AbstractKeyword) ||
                isInterface)
                accessmodifiers += ("public ");

            if (method.Modifiers.Any(SyntaxKind.PrivateKeyword))
                accessmodifiers += ("private ");

            if (ShouldUseOverrideKeyword(method, methodSymbol))
                accessmodifiers += ("override ");

            //D does not use the virtual keyword
            //                if (method.Modifiers.Any(SyntaxKind.VirtualKeyword) || isInterface)
            //                    writer.Write("virtual ");
            //Need to improve performance by labling non virtual methods with "final" ... but we have to check the original definition of the method

            if (method.Modifiers.Any(SyntaxKind.StaticKeyword))
                accessmodifiers += ("static ");

            if (isInterface)
            {
//                writer.IsInterface = true;
            }

            //Constructors in d dont have return types
//            writer.Write("void ");
//            writer.HeaderWriter.Write("void ");

//            writer.Write("this"); 

//            writer.Write("(");

//            var firstParam = true;
//            foreach (var parameter in method.ParameterList.Parameters)
//            {
//                bool isRef = parameter.Modifiers.Any(SyntaxKind.OutKeyword) ||
//                             parameter.Modifiers.Any(SyntaxKind.RefKeyword);
//
//                if (firstParam)
//                    firstParam = false;
//                else
//                {
//                    writer.Write(", ");
//                }
//
//
//                var localSymbol = TypeProcessor.GetTypeInfo(parameter.Type);
//                var ptr = (localSymbol.Type != null && !localSymbol.Type.IsValueType) ? "" : "";
//
//                if (!isRef)
//                {
//                    var s = TypeProcessor.ConvertType(parameter.Type) + " " + ptr;
//                    writer.Write(s);
//                }
//                else
//                {
//
//
//
//                    var s = " ref " + TypeProcessor.ConvertType(parameter.Type) + " "; // Refs in D are simple
//
//                    writer.Write(s);
//
//                    Program.RefOutSymbols.TryAdd(TypeProcessor.GetDeclaredSymbol(parameter), null);
//                }
//
//                writer.Write(WriteIdentifierName.TransformIdentifier(parameter.Identifier.ValueText));
//
//                if (parameter.Default != null)
//                {
//                    writer.Write(" = ");
//                    Core.Write(writer, parameter.Default.Value);
//                }
//            }
//
//            writer.Write(")");

            writer.WriteLine(accessmodifiers + "this" + WriteMethod.GetParameterListAsString(method.ParameterList));

            if (isInterface || method.Modifiers.Any(SyntaxKind.AbstractKeyword))
            {
                writer.Write(" = 0;\r\n");

                return;
            }

            //if (!returnsVoid)
            //  writer.Write(" =");

//            writer.Write("\r\n");
            writer.OpenBrace();

            if (method.Initializer != null)
            {
                //writer.Write(":");
                Core.Write(writer, method.Initializer);
                writer.Write(";");
                writer.WriteLine(); //";\r\n");
            }

            if (method.Body != null)
            {
                foreach (var statement in method.Body.Statements)
                    Core.Write(writer, statement);

                TriviaProcessor.ProcessTrivias(writer, method.Body.DescendantTrivia());
            }

            writer.CloseBrace();
        }

        private static bool ShouldUseOverrideKeyword(ConstructorDeclarationSyntax method, IMethodSymbol symbol)
        {
            if (method.Modifiers.Any(SyntaxKind.StaticKeyword))
                return false;
            if (method.Modifiers.Any(SyntaxKind.NewKeyword))
                return symbol.ContainingType.BaseType.GetMembers(symbol.Name).Any(k => k.IsAbstract || k.IsVirtual);

            if (method.Modifiers.Any(SyntaxKind.PartialKeyword))
                //partial methods seem exempt from C#'s normal override keyword requirement, so we have to check manually to see if it exists in a base class
                return symbol.ContainingType.BaseType.GetMembers(symbol.Name).Any();

            return method.Modifiers.Any(SyntaxKind.OverrideKeyword);
        }

        public static string TypeParameter(TypeParameterSyntax prm, IMethodSymbol methodSymbol,
            MethodDeclarationSyntax methodSyntax)
        {
            var identifier = Utility.TypeConstraints(prm, methodSyntax.ConstraintClauses);

            return identifier;
        }


        public static void WriteStaticConstructor(OutputWriter writer, ConstructorDeclarationSyntax staticConstructor,
            List<string> otherStatics)
        {
            if (staticConstructor.Body == null && (otherStatics == null || otherStatics.Count == 0))
                return;

            writer.WriteLine();
            writer.WriteLine("static this()");
            writer.OpenBrace();

            if (staticConstructor.Body != null)
            {
                foreach (var statement in staticConstructor.Body.As<BlockSyntax>().Statements)
                    Core.Write(writer, statement);
            }

            if (otherStatics != null)
            {
                foreach (var statement in otherStatics)
                {
                    var nodeString = statement;
//				if (nodeString.EndsWith (" SharpNative;\n")) {
//					nodeString = nodeString.RemoveFromEndOfString(" SharpNative;\n")+";\n";
//				}
                    writer.WriteLine(nodeString);
                }
            }

            writer.CloseBrace();

            StaticConstructors.Add(Context.Instance.Partials.First().Symbol.ContainingNamespace.FullNameWithDot() +
                                   Context.Instance.TypeName);
        }

        private static readonly HashSet<string> StaticConstructors = new HashSet<string>();
        private static readonly HashSet<string> AllTypes = new HashSet<string>();


        public static void WriteConstructorsHelper(IEnumerable<INamedTypeSymbol> allTypes)
        {
            foreach (var t in allTypes.Select(o => o.ContainingNamespace.FullNameWithDot() + WriteType.TypeName(o)))
                AllTypes.Add(t);

            if (StaticConstructors.Count == 0)
                return; //no need for it.
        }
    }
}