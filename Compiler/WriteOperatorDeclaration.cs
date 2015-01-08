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

#endregion

namespace SharpNative.Compiler
{
    internal static class WriteOperatorDeclaration
    {
       


        private static readonly string[] BinaryOperators =
        {
            "+", "-", "*", "/", "%", "^^", "&", "|",
            "^", "<<", ">>", ">>>", "~", "in"
        };

        private static readonly string[] UnaryOperators =
        {
            "++", "--"
        };


        private static readonly string[] EqualsOperators =
        {
            "==", "!=" // should overload Equal and NotEquals
        };

        private static readonly string[] CmpOperators =
        {
            "<", "<=", ">", ">="
        };

        private static readonly string[] AssignOperators =
        {
            "="
        };

        private static readonly string[] AssignOpOperators =
        {
            "+=", "-=", "*=", "/=", "%=", "^^=", "&=", "|=", "^=", "<<=", ">>=", ">>>=", "~="
        };

        public static void Go(OutputWriter writer, OperatorDeclarationSyntax method)
        {
            var methodSymbol = (IMethodSymbol) TypeProcessor.GetDeclaredSymbol(method);
            var actualMethodName = OverloadResolver.MethodName(methodSymbol);

            writer.Write("\n");


            var returnType = "";
            if (method.ReturnType.ToString() == "void")
                returnType = ("void ");
            else
            {
                returnType = TypeProcessor.ConvertType(method.ReturnType) + " ";

//                writer.Write(returnType);
            }

            var methodName = "";

            if (BinaryOperators.Contains(method.OperatorToken.ValueText))
            {
                methodName = "opBinary";
                var typeSymbolParam0 = TypeProcessor.GetTypeInfo(method.ParameterList.Parameters[0].Type);

                var typeSymbolParent = (methodSymbol.ContainingType);

                if (typeSymbolParam0.Type != typeSymbolParent)
                    methodName += "Right";
            }

            if (UnaryOperators.Contains(method.OperatorToken.ValueText))
                methodName = "opUnary";

            if (EqualsOperators.Contains(method.OperatorToken.ValueText))
                methodName = "opEquals";

            if (CmpOperators.Contains(method.OperatorToken.ValueText))
                methodName = "opCmp";

            if (AssignOperators.Contains(method.OperatorToken.ValueText))
                methodName = "opAssign";

            if (AssignOpOperators.Contains(method.OperatorToken.ValueText))
                methodName = "opOpAssign"; // need to remove = from the name


            var paramType = method.ParameterList.Parameters[0];

            if (method.ParameterList.Parameters.Count == 2)
            {
                if (methodName.EndsWith("Right"))
                    paramType = method.ParameterList.Parameters[0];
                else
                    paramType = method.ParameterList.Parameters[1];
            }

            var token = method.OperatorToken.ValueText;

            if (methodName == "opOpAssign")
                token = token.Substring(0, 1);

            writer.WriteLine("public final " + returnType + " " + methodName +
                         String.Format(
                             " (string _op) ({0} other)\r\n\tif(_op==\"{2}\")\r\n\t{{ \r\n\t\treturn {1}(this,other); \r\n\t}}\r\n\r\n",
                             TypeProcessor.ConvertType(paramType.Type), actualMethodName, token));

            writer.WriteLine("public static " + returnType + " " + actualMethodName + WriteMethod.GetParameterListAsString(method.ParameterList.Parameters) );

           


            writer.OpenBrace();
            if (method.Body != null)
            {
                foreach (var statement in method.Body.Statements)
                    Core.Write(writer, statement);

                TriviaProcessor.ProcessTrivias(writer, method.Body.DescendantTrivia());
            }

            writer.CloseBrace();
        }


     
    }
}