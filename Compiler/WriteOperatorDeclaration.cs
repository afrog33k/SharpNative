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
        public static void WriteParameterList(OutputWriter writer, ParameterListSyntax list)
        {
            writer.Write("(");
            var firstParam = true;
            foreach (var parameter in list.Parameters)
            {
                bool isRef = parameter.Modifiers.Any(SyntaxKind.OutKeyword) ||
                             parameter.Modifiers.Any(SyntaxKind.RefKeyword);
                if (firstParam)
                    firstParam = false;
                else
                    writer.Write(", ");
                var localSymbol = TypeProcessor.GetTypeInfo(parameter.Type);
                var ptr = (localSymbol.Type != null &&
                           !(localSymbol.Type.IsValueType || localSymbol.Type.TypeKind == TypeKind.TypeParameter));
                if (!isRef)
                {
                    //  var s = TypeProcessor.ConvertType(parameter.Type) + " " + ptr;
                    var s = TypeProcessor.ConvertType(parameter.Type) + " ";
                    writer.Write(s);
                }
                else
                {
                    //                    var s = "" + TypeProcessor.ConvertType(parameter.Type) + ptr + "& ";
                    var s = " ref " + TypeProcessor.ConvertType(parameter.Type) + " ";
                    writer.Write(s);
                    Program.RefOutSymbols.TryAdd(TypeProcessor.GetDeclaredSymbol(parameter), null);
                }
                writer.Write(WriteIdentifierName.TransformIdentifier(parameter.Identifier.ValueText));
                if (parameter.Default != null)
                {
                    writer.Write(" = ");
                    Core.Write(writer, parameter.Default.Value);
                }
            }
            writer.Write(") ");
        }


        private static readonly string[] binaryOperators =
        {
            "+", "-", "*", "/", "%", "^^", "&", "|",
            "^", "<<", ">>", ">>>", "~", "in"
        };

        private static readonly string[] unaryOperators =
        {
            "++", "--"
        };


        private static readonly string[] equalsOperators =
        {
            "==", "!=" // should overload Equal and NotEquals
        };

        private static readonly string[] cmpOperators =
        {
            "<", "<=", ">", ">="
        };

        private static readonly string[] assignOperators =
        {
            "="
        };

        private static readonly string[] assignOpOperators =
        {
            "+=", "-=", "*=", "/=", "%=", "^^=", "&=", "|=", "^=", "<<=", ">>=", ">>>=", "~="
        };

        public static void Go(OutputWriter writer, OperatorDeclarationSyntax method)
        {
            var methodSymbol = (IMethodSymbol) TypeProcessor.GetDeclaredSymbol(method);
            var ActualMethodName = OverloadResolver.MethodName(methodSymbol);
            //			var methodType = Program.GetModel(method).GetTypeInfo(method);

            writer.Write("\n");

            var isInterface = method.Parent is InterfaceDeclarationSyntax;

            if (method.Modifiers.Any(SyntaxKind.PublicKeyword) || method.Modifiers.Any(SyntaxKind.InternalKeyword) ||
                method.Modifiers.Any(SyntaxKind.ProtectedKeyword) || method.Modifiers.Any(SyntaxKind.AbstractKeyword) ||
                isInterface)
                writer.Write("public ");

            var returnType = "";
            if (method.ReturnType.ToString() == "void")
                writer.Write("void ");
            else
            {
//				var typeSymbol = TypeProcessor.GetTypeInfo(method.ReturnType).Type;

                //   var isPtr = typeSymbol != null && (typeSymbol.IsValueType || typeSymbol.TypeKind == TypeKind.TypeParameter) ? "" : "";
                //     var typeString = TypeProcessor.ConvertType(method.ReturnType) + isPtr + " ";

                //	            writer.Write(typeString);
                //	            writer.HeaderWriter.Write(typeString);

//				var isPtr = typeSymbol != null && (!typeSymbol.IsValueType || typeSymbol.TypeKind == TypeKind.TypeParameter);
                returnType = TypeProcessor.ConvertType(method.ReturnType) + " ";

                writer.Write(returnType);
            }

            var methodName = "";

            if (binaryOperators.Contains(method.OperatorToken.ValueText))
            {
                methodName = "opBinary";
                var typeSymbolParam0 = TypeProcessor.GetTypeInfo(method.ParameterList.Parameters[0].Type);

                var typeSymbolParent = (methodSymbol.ContainingType);

                if (typeSymbolParam0.Type != typeSymbolParent)
                    methodName += "Right";
            }

            if (unaryOperators.Contains(method.OperatorToken.ValueText))
                methodName = "opUnary";

            if (equalsOperators.Contains(method.OperatorToken.ValueText))
                methodName = "opEquals";

            if (cmpOperators.Contains(method.OperatorToken.ValueText))
                methodName = "opCmp";

            if (assignOperators.Contains(method.OperatorToken.ValueText))
                methodName = "opAssign";

            if (assignOpOperators.Contains(method.OperatorToken.ValueText))
                methodName = "opOpAssign"; // need to remove = from the name

            //method.ParameterList.Parameters[0];

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

            writer.Write(methodName +
                         String.Format(
                             " (string _op) ({0} other)\r\nif(_op==\"{2}\")\r\n{{ \r\nreturn {1}(this,other); \r\n}}\r\n",
                             TypeProcessor.ConvertType(paramType.Type), ActualMethodName, token));

            writer.Write("\r\n\r\npublic static " + returnType + " " + ActualMethodName);

            WriteParameterList(writer, method.ParameterList);

            writer.WriteLine();

            writer.OpenBrace();
            writer.WriteLine();
            if (method.Body != null)
            {
                foreach (var statement in method.Body.Statements)
                    Core.Write(writer, statement);

                TriviaProcessor.ProcessTrivias(writer, method.Body.DescendantTrivia());
            }

            writer.WriteLine();
            writer.CloseBrace();
            writer.WriteLine();
        }


     
    }
}