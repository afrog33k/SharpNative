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
    internal static class WriteAssignmentExpression
    {
        public static void Go(OutputWriter writer, AssignmentExpressionSyntax expression)
        {
            WriteBinaryExpression.WriteIt(writer,expression.OperatorToken,expression.Right,expression.Left);
        }
    }
}