// /*
//   SharpNative - C# to D Transpiler
//   (C) 2014 Irio Systems 
// */

#region Imports

using Microsoft.CodeAnalysis.CSharp.Syntax;

#endregion

namespace SharpNative.Compiler
{
    internal static class WriteUnsafeStatement
    {
        public static void Go(OutputWriter writer, UnsafeStatementSyntax checkedExpressionSyntax)
        {
            writer.WriteLine("//Unsafe");
            Core.Write(writer, checkedExpressionSyntax.Block);
        }
    }

    internal static class WriteChecked
    {
        public static void Go(OutputWriter writer, CheckedStatementSyntax checkedExpressionSyntax)
        {
            //TODO: implement real checked syntax ... this just ignores it
            Core.Write(writer, checkedExpressionSyntax.Block);
        }

        public static void Go(OutputWriter writer, CheckedExpressionSyntax checkedExpressionSyntax)
        {
            Core.Write(writer, checkedExpressionSyntax.Expression);
//			var block = checkedExpressionSyntax.
//				if (writeBraces)
//					writer.WriteOpenBrace();

//				foreach (var statement in block.Statements)
//					Write(writer, statement);

//				TriviaProcessor.ProcessTrivias(writer, block.DescendantTrivia());

//				if (writeBraces)
//					writer.WriteCloseBrace();
        }
    }
}