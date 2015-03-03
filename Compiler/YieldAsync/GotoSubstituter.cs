using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SharpNative.Compiler.YieldAsync
{
    public class GotoSubstituter : CSharpSyntaxRewriter
    {
        private Compilation compilation;
        private Dictionary<object, State> labelStates;
        private State currentState;

        public GotoSubstituter(Compilation compilation, Dictionary<object, State> labelStates)
        {
            this.compilation = compilation;
            this.labelStates = labelStates;
        }

        public override SyntaxNode VisitGotoStatement(GotoStatementSyntax node)
        {
            var label = node.Expression.ToString();
            if (label.StartsWith("_"))
                return node;

            return StateGenerator.ChangeState(labelStates[label]);
        }
    }
}