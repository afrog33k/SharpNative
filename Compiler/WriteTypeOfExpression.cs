﻿// /*
//   SharpNative - C# to D Transpiler
//   (C) 2014 Irio Systems 
// */

#region Imports

using Microsoft.CodeAnalysis.CSharp.Syntax;

#endregion

namespace SharpNative.Compiler
{
    internal static class WriteTypeOfExpression
    {
        public static void Go(OutputWriter writer, TypeOfExpressionSyntax expression)
        {
            writer.Write("__TypeOf!(");
            TypeProcessor.AddUsedType(TypeProcessor.GetTypeInfo(expression.Type).Type);
            writer.Write(TypeProcessor.ConvertType(expression.Type));
            writer.Write(")");
        }
    }
}