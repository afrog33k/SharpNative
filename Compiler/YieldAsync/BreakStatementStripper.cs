using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SharpNative.Compiler.YieldAsync
{
    public class BreakStatementStripper : CSharpSyntaxRewriter
    {
        public static SyntaxNode StripStatements(CSharpSyntaxNode root)
        {
            return root.Accept(new BreakStatementStripper());
        }

        public override SyntaxNode VisitBreakStatement(BreakStatementSyntax node)
        {
            return SyntaxFactory.EmptyStatement();
        }

        public override SyntaxNode VisitForEachStatement(ForEachStatementSyntax node)
        {
            return node;
        }

        public override SyntaxNode VisitWhileStatement(WhileStatementSyntax node)
        {
            return node;
        }

        public override SyntaxNode VisitForStatement(ForStatementSyntax node)
        {
            return node;
        }

        public override SyntaxNode VisitDoStatement(DoStatementSyntax node)
        {
            return node;
        }

        public override SyntaxNode VisitSwitchStatement(SwitchStatementSyntax node)
        {
            return node;
        }
    }
}