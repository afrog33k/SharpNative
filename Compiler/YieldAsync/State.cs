using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SharpNative.Compiler.YieldAsync
{
    public class State
    {
        public int Index { get; private set; }
        public List<StatementSyntax> Statements { get; set; }
        public State NextState { get; set; }
        public State BreakState { get; set; }
        public bool IsClosed { get; set; }
        public Action<State> Germ { get; set; }

        public State(StateGenerator generator, bool isFakeState = false)
        {
            Statements = new List<StatementSyntax>();
            if (!isFakeState)
            {
                Index = generator.states.Count;
                generator.states.Add(this);
            }
            else
            {
                Index = -1;
            }
        }

        public void Add(StatementSyntax statement)
        {
            Statements.Add(statement);
        }

        public override string ToString()
        {
            return Index + ": " + string.Join(" ", Statements);
        }
    }
}