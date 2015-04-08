// /*
//   SharpNative - C# to D Transpiler
//   (C) 2014 Irio Systems 
// */

#region Imports

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

#endregion

namespace SharpNative.Compiler
{
    internal static class WriteOperatorDeclaration
    {



        public static readonly Dictionary<string, string> BinaryOperators = new Dictionary<string, string>
        {
           {"op_Addition","+"},
{"op_Subtraction","-"},
{"op_Multiply","*"},
{"op_Division","/"},
{"op_Modulus","%"},
{"op_ExclusiveOr","^"},
{"op_BitwiseAnd","&"},
{"op_BitwiseOr","|"},
{"op_LogicalAnd","&&"},
{"op_LogicalOr","||"},
{"op_LeftShift","<<"},
{"op_RightShift",">>"},

        };

        public static readonly Dictionary<string, string> AllBinaryOperators = new Dictionary<string, string>
        {
            {"op_Addition", "+"},
            {"op_Subtraction", "-"},
            {"op_Multiply", "*"},
            {"op_Division", "/"},
            {"op_Modulus", "%"},
            {"op_ExclusiveOr", "^"},
            {"op_BitwiseAnd", "&"},
            {"op_BitwiseOr", "|"},
            {"op_LogicalAnd", "&&"},
            {"op_LogicalOr", "||"},
            {"op_LeftShift", "<<"},
            {"op_RightShift", ">>"},   {"op_Equality","=="},
{"op_Inequality","!="},    {"op_GreaterThan",">"},
{"op_LessThan","<"},
{"op_GreaterThanOrEqual",">="},
{"op_LessThanOrEqual","<="},   {"op_MultiplicationAssignment","*="},
{"op_SubtractionAssignment","-="},
{"op_ExclusiveOrAssignment","^="},
{"op_LeftShiftAssignment","<<="},
{"op_ModulusAssignment","%="},
{"op_AdditionAssignment","+="},
{"op_BitwiseAndAssignment","&="},
{"op_BitwiseOrAssignment","|="},
{"op_DivisionAssignment","/="},{"op_Assign","="},
        };

        public static readonly Dictionary<string, string> UnaryOperators = new Dictionary<string, string>
        {
           {"op_Decrement","--"},
{"op_Increment","++"},
{"op_UnaryNegation","-"},
{"op_UnaryPlus","+"},
{"op_OnesComplement","~"}
        };


        public static readonly Dictionary<string, string> EqualsOperators = new Dictionary<string, string>
        {
            {"op_Equality","=="},
{"op_Inequality","!="},

            // "==", "!=" // should overload Equal and NotEquals
        };

        public static readonly Dictionary<string, string> CmpOperators = new Dictionary<string, string>
        {
            {"op_GreaterThan",">"},
{"op_LessThan","<"},
{"op_GreaterThanOrEqual",">="},
{"op_LessThanOrEqual","<="},
        };

        public static readonly Dictionary<string, string> AssignOperators = new Dictionary<string, string>
        {
           {"op_Assign","="},

        };

        public static readonly Dictionary<string, string> AssignOpOperators = new Dictionary<string, string>
        {
            {"op_MultiplicationAssignment","*="},
{"op_SubtractionAssignment","-="},
{"op_ExclusiveOrAssignment","^="},
{"op_LeftShiftAssignment","<<="},
{"op_ModulusAssignment","%="},
{"op_AdditionAssignment","+="},
{"op_BitwiseAndAssignment","&="},
{"op_BitwiseOrAssignment","|="},
{"op_DivisionAssignment","/="},
        };

        static Dictionary<string, string> _map = new Dictionary<string, string>{
{"op_Implicit",""},
{"op_explicit",""},

            //Binary Operators
            {"op_Addition","+"},
{"op_Subtraction","-"},
{"op_Multiply","*"},
{"op_Division","/"},
{"op_Modulus","%"},
{"op_ExclusiveOr","^"},
{"op_BitwiseAnd","&"},
{"op_BitwiseOr","|"},
{"op_LogicalAnd","&&"},
{"op_LogicalOr","||"},
{"op_LeftShift","<<"},
{"op_RightShift",">>"},
{"op_Equality","=="},
{"op_GreaterThan",">"},
{"op_LessThan","<"},
{"op_Inequality","!="},
{"op_GreaterThanOrEqual",">="},
{"op_LessThanOrEqual","<="},


{"op_SignedRightShift",""},
{"op_UnsignedRightShift",""},

            //Assignment Operators
            {"op_Assign","="},
{"op_MultiplicationAssignment","*="},
{"op_SubtractionAssignment","-="},
{"op_ExclusiveOrAssignment","^="},
{"op_LeftShiftAssignment","<<="},
{"op_ModulusAssignment","%="},
{"op_AdditionAssignment","+="},
{"op_BitwiseAndAssignment","&="},
{"op_BitwiseOrAssignment","|="},
{"op_DivisionAssignment","/="},

{"op_Comma",","},

            //Unary Operators
            {"op_Decrement","--"},
{"op_Increment","++"},
{"op_UnaryNegation","-"},
{"op_UnaryPlus","+"},
{"op_OnesComplement","~"}
};

        public static void Go(OutputWriter writer, OperatorDeclarationSyntax method)
        {
            var methodSymbol = (IMethodSymbol)TypeProcessor.GetDeclaredSymbol(method);
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

            if (BinaryOperators.ContainsKey(actualMethodName))
            {
                methodName = "opBinary";
                var typeSymbolParam0 = TypeProcessor.GetTypeInfo(method.ParameterList.Parameters[0].Type);

                var typeSymbolParent = (methodSymbol.ContainingType);

                if (typeSymbolParam0.Type != typeSymbolParent)
                    methodName += "Right";
            }

            if (UnaryOperators.ContainsKey(actualMethodName))
                methodName = "opUnary";

            if (EqualsOperators.ContainsKey(actualMethodName))
                methodName = "opEquals";

            if (CmpOperators.ContainsKey(actualMethodName))
                methodName = "opCmp";

            if (AssignOperators.ContainsKey(actualMethodName))
                methodName = "opAssign";

            if (AssignOpOperators.ContainsKey(actualMethodName))
                methodName = "opOpAssign"; // need to remove = from the name


            var paramType = method.ParameterList.Parameters[0];

            if (method.ParameterList.Parameters.Count == 2)
            {
                if (methodName.EndsWith("Right"))
                    paramType = method.ParameterList.Parameters[0];
                else
                    paramType = method.ParameterList.Parameters[1];
            }

            var token = method.OperatorToken.Text;

            var methodBody = "";

            var temp = new TempWriter();

            foreach (var statement in method.Body.Statements)
                Core.Write(temp, statement);

            TriviaProcessor.ProcessTrivias(temp, method.Body.DescendantTrivia());

            methodBody = temp.ToString();


            if (methodName == "opOpAssign")
                token = token.Substring(0, 1);

            //We are going to have to rewrite this bit later ... for now all overloads are called directly
        /*    if (methodName == "opBinary")
            {

                writer.WriteLine("public final " + returnType + " " + methodName +
                String.Format(
                    " (string _op) ({0} other)\r\n\tif(_op==\"{2}\")\r\n\t{{ \r\n\t\treturn {1}(this,other); \r\n\t}}\r\n\r\n",
                    TypeProcessor.ConvertType(paramType.Type), actualMethodName, token));

                //Add Assignment operator if it doesn't exist
                if (!methodSymbol.ContainingType.GetMembers(AssignOpOperators.FirstOrDefault(k => k.Value == token + "=").Key).Any())
                {
                    writer.WriteLine("public final " + returnType + " opOpAssign" +
                String.Format(
                    " (string _op) ({0} other)\r\n\tif(_op==\"{2}\")\r\n\t{{ \r\n\t\treturn {1}(this,other); \r\n\t}}\r\n\r\n",
                    TypeProcessor.ConvertType(paramType.Type), actualMethodName, token));
                }
            }
            else if (methodName == "opUnary")//TODO unary operators are mostly going to be direct methodCalls
            {

                writer.WriteLine("public final " + returnType + " " + methodName +
                    String.Format(
                        " (string _op) ()\r\n\tif(_op==\"{2}\")\r\n\t{{ \r\n\t\treturn {1}(this); \r\n\t}}\r\n\r\n",
                        TypeProcessor.ConvertType(paramType.Type), actualMethodName, token));
                //				writer.WriteLine ("public final ref " + returnType + " " + methodName +
                //					String.Format (
                //						" (string _op) ()\r\n\tif(_op==\"{2}\")\r\n\t{{ \r\n\t\t{3}\r\n\t}}\r\n\r\n",
                //TypeProcessor.ConvertType (paramType.Type), actualMethodName, token, methodBody.Replace(method.ParameterList.Parameters[0].Identifier.ValueText,"this")));
            }
            else
            {
                writer.WriteLine("public final " + returnType + " " + methodName +
                    String.Format(
                        " (string _op) ({0} other)\r\n\tif(_op==\"{2}\")\r\n\t{{ \r\n\t\treturn {1}(this); \r\n\t}}\r\n\r\n",
                        TypeProcessor.ConvertType(paramType.Type), actualMethodName, token));
            }*/

            var @params = method.ParameterList.Parameters;



            writer.WriteLine("public static " + returnType + " " + actualMethodName + WriteMethod.GetParameterListAsString(method.ParameterList.Parameters));




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