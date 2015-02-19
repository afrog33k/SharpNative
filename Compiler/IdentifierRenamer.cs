// /*
//   SharpNative - C# to D Transpiler
//   (C) 2014 Irio Systems 
// */

#region Imports

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

#endregion

namespace SharpNative.Compiler
{
    public class IdentifierRenamer : CSharpSyntaxRewriter
    {
        private SyntaxToken _renameFrom;
        private readonly SyntaxToken renameTo;

        public IdentifierRenamer(SyntaxToken renameFrom, SyntaxToken renameTo)
        {
            _renameFrom = renameFrom;
            this.renameTo = renameTo;
        }

        public static CSharpSyntaxNode RenameIdentifier(CSharpSyntaxNode node, SyntaxToken renameFrom,
            SyntaxToken renameTo)
        {
            return (CSharpSyntaxNode)node.Accept(new IdentifierRenamer(renameFrom, renameTo));
        }

        public override SyntaxNode VisitIdentifierName(IdentifierNameSyntax node)
        {
            if (node.Identifier.ToString() == _renameFrom.ToString())
                return SyntaxFactory.IdentifierName(renameTo);

            return base.VisitIdentifierName(node);
        }

        public override SyntaxNode VisitForEachStatement(ForEachStatementSyntax node)
        {
            if (node.Identifier.ToString() == _renameFrom.ToString())
                node = node.WithIdentifier(renameTo);

            return base.VisitForEachStatement(node);
        }

        public override SyntaxNode VisitVariableDeclarator(VariableDeclaratorSyntax node)
        {
            if (node.Identifier.ToString() == _renameFrom.ToString())
                node = node.WithIdentifier(renameTo);

            return base.VisitVariableDeclarator(node);
        }
    }
}