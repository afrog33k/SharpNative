using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SharpNative.Compiler
{
    public class WriteYieldStatement
    {
        public static void Go(OutputWriter writer, YieldStatementSyntax yieldStatementSyntax)
        {
            
            if (yieldStatementSyntax.ReturnOrBreakKeyword.IsKind(SyntaxKind.ReturnKeyword))
            {
				writer.WriteLine("__iter.yieldReturn(BOX({0}));", Core.WriteString(yieldStatementSyntax.Expression));
            }
            else
            {
                writer.WriteLine("__iter.yieldBreak();");
            }

        }
    }
}