#region License
//-----------------------------------------------------------------------
// <copyright>
// The MIT License (MIT)
// 
// Copyright (c) 2014 Kirk S Woll
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// </copyright>
//-----------------------------------------------------------------------
#endregion

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SharpNative.Compiler.YieldAsync
{
    public class YieldClassGenerator
    {
        private Compilation compilation;
        private ClassDeclarationSyntax classDeclarationSyntax;
        private MemberDeclarationSyntax node;
        private ISymbol method;

        public const string isStarted = "_isStarted";
        public const string isStartedLocal = "_isStartedLocal";

        public YieldClassGenerator(Compilation compilation, ClassDeclarationSyntax classDeclarationSyntax, MemberDeclarationSyntax node)
        {
            this.compilation = compilation;
            this.classDeclarationSyntax = classDeclarationSyntax;
            this.node = node;

            method = ModelExtensions.GetDeclaredSymbol(compilation.GetSemanticModel(node.SyntaxTree), node);
        }

        public ClassDeclarationSyntax CreateEnumerator()
        {
            var members = new List<MemberDeclarationSyntax>();
            FieldDeclarationSyntax thisField = null;
            if (!method.IsStatic)
            {
                thisField = Cs.Field(method.ContainingType.ToTypeSyntax(), "_this");
                members.Add(thisField);
            }

            var stateGenerator = new YieldStateGenerator(compilation, node);
            stateGenerator.GenerateStates();
            var states = stateGenerator.States;

            var isStartedField = Cs.Field(Cs.Bool(), isStarted);
            members.Add(isStartedField);

            var stateField = Cs.Field(Cs.Int(), StateGenerator.state);
            members.Add(stateField);


            ITypeSymbol elementType = null;
            if(method is IMethodSymbol)
            elementType = ((INamedTypeSymbol)(method as IMethodSymbol).ReturnType).TypeArguments[0];
            else if (method is IPropertySymbol)
            {
                elementType = ((INamedTypeSymbol)(method as IPropertySymbol).Type).TypeArguments[0];
            }

            var currentProperty = Cs.Property(elementType.ToTypeSyntax(), "Current");
            members.Add(currentProperty);

            if(node is MethodDeclarationSyntax)
            foreach (var parameter in (node as MethodDeclarationSyntax).ParameterList.Parameters)
            {
                var parameterField = Cs.Field(parameter.Type, parameter.Identifier);
                members.Add(parameterField);
            }
            foreach (var variable in stateGenerator.HoistedVariables)
            {
                var variableField = Cs.Field(variable.Item2, variable.Item1);
                members.Add(variableField);
            }

            var className =  method.GetYieldClassName();

            var constructorParameters = new List<ParameterSyntax>();
            if (!method.IsStatic)
            {
                constructorParameters.Add(SyntaxFactory.Parameter(SyntaxFactory.Identifier("_this")).WithType(thisField.Declaration.Type));
            }
            if (node is MethodDeclarationSyntax)
                constructorParameters.AddRange((node as MethodDeclarationSyntax).ParameterList.Parameters.Select(x => SyntaxFactory.Parameter(x.Identifier).WithType(x.Type)));

            var constructor = SyntaxFactory.ConstructorDeclaration(className)
                .AddModifiers(Cs.Public())
                .WithParameterList(constructorParameters.ToArray())
                .WithBody(
                    SyntaxFactory.Block(
                        // Assign fields
                        constructorParameters.Select(x => Cs.Express(Cs.Assign(Cs.This().Member(x.Identifier), SyntaxFactory.IdentifierName(x.Identifier))))
                    )
                    .AddStatements(
                        Cs.Express(Cs.Assign(Cs.This().Member(StateGenerator.state), Cs.Integer(1)))
                    )
                );
            members.Add(constructor);


            var ienumerable_g = SyntaxFactory.ParseTypeName("System.Collections.Generic.IEnumerable<" + elementType.ToDisplayString() + ">");
            var ienumerator_g = SyntaxFactory.ParseTypeName("System.Collections.Generic.IEnumerator<" + elementType.ToDisplayString() + ">");

            var ienumerable = SyntaxFactory.ParseTypeName("System.Collections.IEnumerable");
            var ienumerator = SyntaxFactory.ParseTypeName("System.Collections.IEnumerator");
            // IEnumerator IEnumerable.GetEnumerator()
            //{
            //    return GetEnumerator();
            //}

            var iegetEnumerator = SyntaxFactory.MethodDeclaration(ienumerator, "GetEnumerator")
           .AddModifiers(Cs.Public())
           .WithExplicitInterfaceSpecifier(SyntaxFactory.ExplicitInterfaceSpecifier(SyntaxFactory.ParseName("System.Collections.IEnumerable")))
           .WithBody(Cs.Block(
                   Cs.Return(Cs.This().Member("GetEnumerator").Invoke())));
            members.Add(iegetEnumerator);

            // public void Dispose()
            //{
            //}


            var dispose = SyntaxFactory.MethodDeclaration(Context.Void.ToTypeSyntax(), "Dispose")
          .AddModifiers(Cs.Public())
          .WithBody(Cs.Block());
            members.Add(dispose);

            // public void Reset()
            // {
            //     throw new NotImplementedException();
            // }

            var reset = SyntaxFactory.MethodDeclaration(Context.Void.ToTypeSyntax(), "Reset")
    .AddModifiers(Cs.Public())
    .WithBody(Cs.Block(Cs.Throw(Cs.New(Context.Compilation.FindType("System.NotImplementedException").ToTypeSyntax()))));
            members.Add(reset);


            // object IEnumerator.Current
            //{
            //    get { return Current; }
            //}
            //IEnumerator
            var iecurrent =
                Cs.Property(Context.Object.ToTypeSyntax(), "Current", true, false, Cs.Block(
                    Cs.Return(Cs.This().Member("Current"))
                    )).WithExplicitInterfaceSpecifier(SyntaxFactory.ExplicitInterfaceSpecifier(SyntaxFactory.ParseName("System.Collections.IEnumerator")));


            members.Add(iecurrent);

            // Generate the GetEnumerator method, which looks something like:
            // var $isStartedLocal = $isStarted;
            // $isStarted = true;
            // if ($isStartedLocal) 
            //     return this.Clone().GetEnumerator();
            // else
            //     return this;
            var getEnumerator = SyntaxFactory.MethodDeclaration(ienumerator_g, "GetEnumerator")
                .AddModifiers(Cs.Public())
                .WithBody(Cs.Block(
                    Cs.Local(isStartedLocal, SyntaxFactory.IdentifierName(isStarted)),
                    Cs.Express(SyntaxFactory.IdentifierName(isStarted).Assign(Cs.True())),
                    Cs.If(
                        SyntaxFactory.IdentifierName(isStartedLocal),
                        Cs.Return(Cs.This().Member("Clone").Invoke().Member("GetEnumerator").Invoke()),
                        Cs.Return(Cs.This()))));
            members.Add(getEnumerator);

            // Generate the MoveNext method, which looks something like:
            // $top:
            // while (true)
            // {
            //     switch (state) 
            //     {
            //         case 0: ...
            //         case 1: ...
            //     }
            // }
            var moveNextBody = SyntaxFactory.LabeledStatement("_top", Cs.While(Cs.True(),
                Cs.Switch(Cs.This().Member(StateGenerator.state), states.Select((x, i) =>
                    Cs.Section(Cs.Integer(i), x.Statements.ToArray())).ToArray())));

            var moveNext = SyntaxFactory.MethodDeclaration(Cs.Bool(), "MoveNext")
                .AddModifiers(Cs.Public())
                .WithBody(SyntaxFactory.Block(moveNextBody, Cs.Return(SyntaxFactory.ParseExpression("false"))));
            members.Add(moveNext);

            TypeSyntax classNameWithTypeArguments = SyntaxFactory.IdentifierName(className);
            if(method is IMethodSymbol)
            if ((method as IMethodSymbol).TypeParameters.Any())
            {
                classNameWithTypeArguments = SyntaxFactory.GenericName(
                    SyntaxFactory.Identifier(className),
                    SyntaxFactory.TypeArgumentList(SyntaxFactory.SeparatedList(
                        (method as IMethodSymbol).TypeParameters.Select(x => SyntaxFactory.ParseTypeName(x.Name)),
                        (method as IMethodSymbol).TypeParameters.Select(x => x).Skip(1).Select(_ => SyntaxFactory.Token(SyntaxKind.CommaToken)))
                    ));
            }

            var cloneBody = Cs.Block(
                Cs.Return(classNameWithTypeArguments.New(
                    constructorParameters.Select(x => SyntaxFactory.IdentifierName(x.Identifier)).ToArray()
                ))
            );
            var clone = SyntaxFactory.MethodDeclaration(ienumerable_g, "Clone")
                .AddModifiers(Cs.Public())
                .WithBody(SyntaxFactory.Block(cloneBody));
            members.Add(clone);
            //IEnumerable<T>,IEnumerator<T>
            SimpleBaseTypeSyntax[] baseTypes = new[]
            {
                SyntaxFactory.SimpleBaseType(ienumerable_g),
                SyntaxFactory.SimpleBaseType(ienumerator_g)
            };
            ClassDeclarationSyntax result = SyntaxFactory.ClassDeclaration(className).WithBaseList(baseTypes).WithMembers(members.ToArray());

            if(method is IMethodSymbol)
            if ((method as IMethodSymbol).TypeParameters.Any())
            {
                result = result.WithTypeParameterList((node as MethodDeclarationSyntax).TypeParameterList);
            }

            return result;
        }
    }
}
