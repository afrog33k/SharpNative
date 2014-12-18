// /*
//   SharpNative - C# to D Transpiler
//   (C) 2014 Irio Systems 
// */

#region Imports

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

#endregion

namespace SharpNative.Compiler
{
    internal class LoopInfo
    {
        public LoopInfo(SyntaxNode loopSyntax)
        {
            if (!IsLoopSyntax(loopSyntax))
                throw new Exception("LoopInfo constructed on non-loop");

            Recurse(loopSyntax, false);
        }

        private bool HasContinue;
        private bool HasBreak;

        private void Recurse(SyntaxNode node, bool isInSwitch)
        {
            if (node is ContinueStatementSyntax)
                HasContinue = true;
            else if (node is BreakStatementSyntax && !isInSwitch)
                //ignore break statements in a switch, since they apply to breaking the switch and not the loop
                HasBreak = true;
            else
            {
                foreach (var child in node.ChildNodes())
                {
                    if (!IsLoopSyntax(child))
                        //any breaks or continues in child loops will belong to that loop, so we can skip recusing into them.
                        Recurse(child, isInSwitch || child is SwitchStatementSyntax);
                }
            }
        }


        private static bool IsLoopSyntax(SyntaxNode syntax)
        {
            return syntax is ForEachStatementSyntax
                   || syntax is ForStatementSyntax
                   || syntax is WhileStatementSyntax
                   || syntax is DoStatementSyntax;
        }
    }
}