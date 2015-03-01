// /*
//   SharpNative - C# to D Transpiler
//   (C) 2014 Irio Systems 
// */

#region Imports

using Microsoft.CodeAnalysis.CSharp.Syntax;

#endregion

namespace SharpNative.Compiler
{
    internal static class WriteDefaultExpression
    {
        public static void Go(OutputWriter writer, DefaultExpressionSyntax node)
        {
			var type = TypeProcessor.GetTypeInfo(node.Type).Type;
           
			writer.Write("__Default!("+TypeProcessor.ConvertType(type)+")");
        }
    }
}