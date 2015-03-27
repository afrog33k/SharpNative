// /*
//   SharpNative - C# to D Transpiler
//   (C) 2014 Irio Systems 
// */

#region Imports

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

#endregion

namespace SharpNative.Compiler
{
    internal static class WriteLambdaExpression
    {
        public static void Go(OutputWriter writer, AnonymousMethodExpressionSyntax expression)
        {
            if (expression.ParameterList != null)
                Go(writer, expression.ParameterList.Parameters, expression.Block, TypeProcessor.GetTypeInfo(expression));
            else
                Go(writer, new SeparatedSyntaxList<ParameterSyntax>(), expression.Block, TypeProcessor.GetTypeInfo(expression));

        }

        public static void Go(OutputWriter writer, ParenthesizedLambdaExpressionSyntax expression)
        {
            Go(writer, expression.ParameterList.Parameters, expression.Body, TypeProcessor.GetTypeInfo(expression));
        }

        public static void Go(OutputWriter writer, SimpleLambdaExpressionSyntax expression)
        {
            Go(writer, new[] {expression.Parameter}, expression.Body, TypeProcessor.GetTypeInfo(expression));
        }

        private static void Go(OutputWriter writer, IEnumerable<ParameterSyntax> parameters, SyntaxNode body,
            TypeInfo type)
        {
            var methodSymbol = type.ConvertedType.As<INamedTypeSymbol>().DelegateInvokeMethod.As<IMethodSymbol>();

            var typeStringNoPtr = TypeProcessor.ConvertType(type.ConvertedType);

            if (type.ConvertedType.TypeKind == TypeKind.TypeParameter)
                writer.Write(" __TypeNew!(" + typeStringNoPtr + ")(");
            else
                writer.Write("new " + typeStringNoPtr + "(");

            writer.Write("(");

            var parameterSyntaxs = parameters as ParameterSyntax[] ?? parameters.ToArray();
            for (int pi = 0; pi < parameterSyntaxs.Count(); pi++)
            {
                var parameter = parameterSyntaxs.ElementAt(pi);
                if (pi > 0)
                    writer.Write(", ");

                if (parameter.Type != null)
                    writer.Write(TypeProcessor.ConvertType(parameter.Type) + " ");
                else
                    writer.Write(TypeProcessor.ConvertType(methodSymbol.Parameters[pi].Type) + " ");
                writer.Write(WriteIdentifierName.TransformIdentifier(parameter.Identifier.ValueText));
            }

            writer.Write(")");

            bool returnsVoid = methodSymbol.ReturnType.ToString() == "void";

            if (body is BlockSyntax)
            {
                writer.Write("\r\n");
                writer.OpenBrace();

                var statements = body.As<BlockSyntax>().Statements;

                var lastStatement = statements.LastOrDefault() as ReturnStatementSyntax;

                var returnStatements = FindReturnStatements(body);

                {
                    foreach (var statement in statements)
                    {
                        if (statement == lastStatement)
                        {
                            writer.WriteIndent();

                            Core.Write(writer, lastStatement);
//                            writer.Write(";\r\n");
                        }
                        else
                            Core.Write(writer, statement);
                    }
                }

                writer.Indent--;
                writer.WriteIndent();
                writer.Write("}");
            }
            else
            {
                if (!returnsVoid)
                {
                    writer.Write("=> ");
                    Core.Write(writer, body);
                }
                else
                {
                    writer.Write(" { ");
                    Core.Write(writer, body);
                    writer.Write("; }");
                }
                //writer.Write(" { ");
                //if (!returnsVoid)
                //	writer.Write ("return ");
                //    writer.Write("; }");
            }

//            if (!returnsVoid)
//                writer.Write(TypeProcessor.ConvertTypeWithColon(methodSymbol.ReturnType));
            writer.Write(")");
        }

        private static List<ReturnStatementSyntax> FindReturnStatements(SyntaxNode body)
        {
            var ret = new List<ReturnStatementSyntax>();
            FindReturnStatements(body, ret);
            return ret;
        }

        private static void FindReturnStatements(SyntaxNode node, List<ReturnStatementSyntax> ret)
        {
            if (node is ParenthesizedLambdaExpressionSyntax || node is SimpleLambdaExpressionSyntax)
                return; //any returns in a sub-lambda will be for that lambda. Ignore them.

            if (node is ReturnStatementSyntax)
                ret.Add(node.As<ReturnStatementSyntax>());

            foreach (var child in node.ChildNodes())
                FindReturnStatements(child, ret);
        }
    }
}