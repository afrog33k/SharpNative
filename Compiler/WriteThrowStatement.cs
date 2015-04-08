// /*
//   SharpNative - C# to D Transpiler
//   (C) 2014 Irio Systems 
// */

#region Imports

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

#endregion

namespace SharpNative.Compiler
{
    internal static class WriteThrowStatement
    {
        public static void Go(OutputWriter writer, ThrowStatementSyntax statement)
        {
            writer.WriteIndent();

            writer.Write("throw ");

            if (statement.Expression == null)
            {
                //On just "throw" with no exception name, navigate up the stack to find the nearest catch block and insert the exception's name
                CatchClauseSyntax catchBlock;
                SyntaxNode node = statement;
                do
                    catchBlock = (node = node.Parent) as CatchClauseSyntax;
                while (catchBlock == null);

                if (catchBlock == null)
                {
                    throw new Exception("throw statement with no exception name, and could not locate a catch block " +
                                        Utility.Descriptor(statement));
                }

                if (catchBlock.Declaration == null || catchBlock.Declaration.Identifier.Value == null)
                    //Some people write code in the form catch(Exception) ...grrr
                    writer.Write("__ex");
                else
                {
                    var exName = WriteIdentifierName.TransformIdentifier(catchBlock.Declaration.Identifier.Text);

                    if (string.IsNullOrWhiteSpace(exName))
                        writer.Write("__ex");
                    else
                        writer.Write(exName);
                }
            }
            else
                Core.Write(writer, statement.Expression);
            writer.Write(";\r\n");
        }

        private static bool ReturnsVoid(SyntaxNode node)
        {
            while (node != null)
            {
                var method = node as MethodDeclarationSyntax;
                if (method != null)
                    return method.ReturnType.ToString() == "void";

                var prop = node as PropertyDeclarationSyntax;
                if (prop != null)
                    return prop.Type.ToString() == "void";

                var lambda1 = node as ParenthesizedLambdaExpressionSyntax;
                var lambda2 = node as SimpleLambdaExpressionSyntax;
                if (lambda1 != null || lambda2 != null)
                {
                    var lambda = lambda1 == null ? lambda2 : (ExpressionSyntax) lambda1;
                    var methodSymbol =
                        TypeProcessor.GetTypeInfo(lambda)
                            .ConvertedType.As<INamedTypeSymbol>()
                            .DelegateInvokeMethod.As<IMethodSymbol>();

                    return methodSymbol.ReturnsVoid;
                }

                node = node.Parent;
            }

            throw new Exception("Node not in a body");
        }
    }
}