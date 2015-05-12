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

#endregion

namespace SharpNative.Compiler
{
    public static class TriviaProcessor
    {
        private static readonly ConcurrentHashSet<SyntaxTrivia> _triviaProcessed = new ConcurrentHashSet<SyntaxTrivia>();


        public static void WriteTrailingTrivia(OutputWriter writer, SyntaxNode node)
        {
            SyntaxTriviaList triviaList = node.GetTrailingTrivia();
            foreach (var trivia in triviaList)
            {
                if (trivia.Kind() == SyntaxKind.SingleLineCommentTrivia ||
                    trivia.Kind() == SyntaxKind.MultiLineCommentTrivia ||
                    trivia.Kind() == SyntaxKind.SingleLineDocumentationCommentTrivia ||
                    trivia.Kind() == SyntaxKind.MultiLineDocumentationCommentTrivia ||
                    trivia.Kind() == SyntaxKind.WhitespaceTrivia)
                {
                    if (trivia.Kind() == SyntaxKind.WhitespaceTrivia)
                    {
                        if (trivia.ToFullString().EndsWith("\n"))
                        {
                            writer.WriteLine();
                            writer.WriteIndent();
                        }
                    }
                    else
                        writer.WriteLine(trivia.ToFullString());
                }
            }
        }

        public static void WriteLeadingTrivia(OutputWriter writer, SyntaxNode node)
        {
            var triviaList = node.GetLeadingTrivia();
            foreach (var trivia in triviaList)
            {
                if (trivia.Kind() == SyntaxKind.SingleLineCommentTrivia ||
                    trivia.Kind() == SyntaxKind.MultiLineCommentTrivia ||
                    trivia.Kind() == SyntaxKind.SingleLineDocumentationCommentTrivia ||
                    trivia.Kind() == SyntaxKind.MultiLineDocumentationCommentTrivia ||
                    trivia.Kind() == SyntaxKind.WhitespaceTrivia)
                {
                    if (trivia.Kind() == SyntaxKind.WhitespaceTrivia)
                    {
                        if (trivia.ToFullString().EndsWith("\n"))
                        {
                            writer.WriteLine();
                            writer.WriteIndent();
                        }
                    }
                    else
                        writer.WriteLine(trivia.ToFullString());
                }
            }
        }

        public static void ProcessNode(OutputWriter writer, SyntaxNode node)
        {
            ProcessTrivias(writer, node.GetLeadingTrivia());
        }

        public static void ProcessTrivias(OutputWriter writer, IEnumerable<SyntaxTrivia> trivias)
        {
            bool literalCode = false;
            //if we encounter a #if SharpNative, we set this to true, which indicates that the next DisabledTextTrivia should be written as pure code.   

            foreach (var trivia in trivias)
            {
                if (_triviaProcessed.Add(trivia)) //ensure we don't look at the same trivia multiple times
                {
                    if (trivia.RawKind == (decimal)SyntaxKind.IfDirectiveTrivia)
                        literalCode |= GetConditions(trivia, "#if ").Contains("SharpNative");
                    else if (trivia.RawKind == (decimal)SyntaxKind.DisabledTextTrivia && literalCode)
                    {
                        writer.Write(trivia.ToString());
                        literalCode = false;
                    }
                }
            }
        }

        private static string[] GetConditions(SyntaxTrivia trivia, string lineStart)
        {
            var str = trivia.ToString().Trim().RemoveFromStartOfString("#if ").Trim();

            int i = str.IndexOf("//");
            if (i != -1)
                str = str.Substring(0, i).Trim();

            i = str.IndexOf("/*");
            if (i != -1)
                str = str.Substring(0, i).Trim();

            return str.Split("|& ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        ///     Remove nodes that are to be omitted due to an #if !SharpNative or the else of an #if SharpNative
        /// </summary>
        /// <param name="tree"></param>
        /// <returns></returns>
        public static IEnumerable<SyntaxNode> DoNotWrite(SyntaxTree tree)
        {
            var triviaProcessed = new ConcurrentHashSet<SyntaxTrivia>();

            var skipCount = 0;
            //set to 1 if we encounter a #if !SharpNative directive (while it's 0).  Incremented for each #if that's started inside of that, and decremented for each #endif
            var elseCount = 0;
            //set to 1 if we encounter an #if SharpNative directive (while it's 0).  Incremented for each #if that's started inside of that, and decremented for each #endif

            var ret = new List<SyntaxNode>();

            Action<SyntaxNodeOrToken> recurse = null;
            recurse = node =>
            {
                Action<SyntaxTrivia> doTrivia = trivia =>
                {
                    if (!triviaProcessed.Add(trivia))
                        return;

                    if (trivia.RawKind == (decimal)SyntaxKind.EndIfDirectiveTrivia)
                    {
                        if (skipCount > 0)
                            skipCount--;
                        if (elseCount > 0)
                            elseCount--;
                    }
                    else if (trivia.RawKind == (decimal)SyntaxKind.IfDirectiveTrivia)
                    {
                        if (skipCount > 0)
                            skipCount++;
                        if (elseCount > 0)
                            elseCount++;

                        var cond = GetConditions(trivia, "#if ");

                        if (cond.Contains("!SharpNative") && skipCount == 0)
                            skipCount = 1;
                        else if (cond.Contains("SharpNative") && elseCount == 0)
                            elseCount = 1;
                    }
                    else if (trivia.RawKind == (decimal)SyntaxKind.ElseDirectiveTrivia)
                    {
                        if (elseCount == 1)
                        {
                            skipCount = 1;
                            elseCount = 0;
                        }
                    }
                };

                foreach (var trivia in node.GetLeadingTrivia())
                    doTrivia(trivia);

                if (skipCount > 0 && node.IsNode)
                    ret.Add(node.AsNode());

                foreach (var child in node.ChildNodesAndTokens())
                    recurse(child);

                foreach (var trivia in node.GetTrailingTrivia())
                    doTrivia(trivia);
            };

            var root = tree.GetRoot();
            recurse(root);

            return ret;
        }
    }
}