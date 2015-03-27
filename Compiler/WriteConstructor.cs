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
				WriteInstanceConstructor(writer, constructor,null);
        }

		public static void WriteInstanceConstructor(OutputWriter writer, ConstructorDeclarationSyntax method,
			List<string> otherInits)
        {
           
           

            writer.WriteLine();

            var accessmodifiers = "";

            var isInterface = method.Parent is InterfaceDeclarationSyntax;

            if (method.Modifiers.Any(SyntaxKind.PublicKeyword) || method.Modifiers.Any(SyntaxKind.InternalKeyword) ||
                method.Modifiers.Any(SyntaxKind.ProtectedKeyword) || method.Modifiers.Any(SyntaxKind.AbstractKeyword) ||
                isInterface)
                accessmodifiers += ("public ");

            // Reflection cannot work with this, cant get address or set value
            //if (method.Modifiers.Any(SyntaxKind.PrivateKeyword))
            //    accessmodifiers += ("private ");


            if (method.Modifiers.Any(SyntaxKind.StaticKeyword))
                accessmodifiers += ("static ");

          
			var constructorName = "this";

			if (Context.Instance.Type.TypeKind == TypeKind.Struct) // Struct
			{
				constructorName = " void __init";
			}

				writer.WriteLine(accessmodifiers + constructorName + WriteMethod.GetParameterListAsString(method.ParameterList.Parameters));

            if (isInterface || method.Modifiers.Any(SyntaxKind.AbstractKeyword))
            {
                writer.Write(" = 0;\r\n");

                return;
            }

            //if (!returnsVoid)
            //  writer.Write(" =");

//            writer.Write("\r\n");
            writer.OpenBrace();

			if (otherInits != null) //We need to write the static initializers before anything else
			{
				foreach (var statement in otherInits)
				{
					var nodeString = statement;

					if (nodeString != null) writer.WriteLine(nodeString);
				}
			}

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

      


        public static void WriteStaticConstructor(OutputWriter writer, ConstructorDeclarationSyntax staticConstructor,
            List<string> otherStatics)
        {
            if (staticConstructor.Body == null && (otherStatics == null || otherStatics.Count == 0))
                return;

            writer.WriteLine();
            writer.WriteLine("static this()");
            writer.OpenBrace();

            if (otherStatics != null) //We need to write the static initializers before anything else
            {
                foreach (var statement in otherStatics)
                {
                    var nodeString = statement;

                    if (nodeString != null) writer.WriteLine(nodeString);
                }
            }

            if (staticConstructor.Body != null)
            {
                foreach (var statement in staticConstructor.Body.As<BlockSyntax>().Statements)
                    Core.Write(writer, statement);
            }

           

            writer.CloseBrace();

            StaticConstructors.Add(Context.Instance.Partials.First().Symbol.ContainingNamespace.FullNameWithDot() +
                                   Context.Instance.TypeName);
        }

        private static readonly HashSet<string> StaticConstructors = new HashSet<string>();
        private static readonly HashSet<string> AllTypes = new HashSet<string>();


     
    }
}