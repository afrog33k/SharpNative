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
using Microsoft.CodeAnalysis.CSharp.Syntax;

#endregion

namespace SharpNative.Compiler.DlangAst
{
    public class InvocationRewriter : CSharpSyntaxRewriter
    {
        private readonly SemanticModel _semanticModel;

        public InvocationRewriter(SemanticModel semanticModel)
        {
            _semanticModel = semanticModel;
        }

        public static ArgumentSyntax[] ProcessArguments(SyntaxNode context, IMethodSymbol method,
            Func<ArgumentSyntax, int, bool> isArgumentArray, Func<ArgumentSyntax, int, string> getArgumentName,
            params ArgumentSyntax[] args)
        {
            //			var isExported = method.IsExported();

            // The number of arguments may differ from the number of parameters, due to default parameters and the params keyword.
            // This queue holds all the remaining arguments, and the matching of parameters to arguments happens in sequence, 
            // consuming from the queue.
            var remainingArguments = new Queue<ArgumentSyntax>(args);

            var argumentsByName = args
                .Select((x, i) => new {Name = getArgumentName(x, i), Argument = x})
                .Where(x => x.Name != null)
                .ToDictionary(x => x.Name, x => x.Argument);
            if (argumentsByName.Any())
            {
                var newArguments = new List<ArgumentSyntax>();
                foreach (var parameter in method.Parameters)
                {
                    if (!parameter.HasExplicitDefaultValue && !argumentsByName.ContainsKey(parameter.Name))
                        newArguments.Add(remainingArguments.Dequeue());
                    else
                    {
                        if (!argumentsByName.ContainsKey(parameter.Name))
                        {
                            var str = parameter.ExplicitDefaultValue.ToString();
                            if (parameter.ExplicitDefaultValue is string)
                                str = "\"" + str + "\"";
                            var argumentSyntax = (SyntaxFactory.ParseArgumentList(str).Arguments[0]);
                            newArguments.Add(argumentSyntax); // as ArgumentSyntax);
                            //							}
                        }
                        else
                        {
                            var argument = argumentsByName[parameter.Name];
                            newArguments.Add(argument);
                        }
                    }
                }
                remainingArguments = new Queue<ArgumentSyntax>(newArguments);
            }

            var arguments = new List<ArgumentSyntax>();
            foreach (var parameter in method.Parameters)
            {
                    arguments.Add(remainingArguments.Dequeue());
            }
            return arguments.ToArray();
        }

        public override SyntaxNode Visit(SyntaxNode node)
        {
            if (node is InvocationExpressionSyntax) //Fix for default arguments
            {
                var invocationExpression = node as InvocationExpressionSyntax;

                if (invocationExpression.ArgumentList != null &&
                    invocationExpression.ArgumentList.Arguments.Any(j => j.NameColon != null))
                {
                    var symbolInfo = _semanticModel.GetSymbolInfo(invocationExpression);
                    var expressionSymbol = _semanticModel.GetSymbolInfo(invocationExpression.Expression);

                    var symbol = symbolInfo.Symbol ?? symbolInfo.CandidateSymbols.FirstOrDefault(); // Resolution error

                    var methodSymbol = symbol.OriginalDefinition.As<IMethodSymbol>().UnReduce();

                    var arguments = ProcessArguments(
                        invocationExpression,
                        methodSymbol,
                        (x, i) =>
                            TypeProcessor.GetTypeInfo(invocationExpression.ArgumentList.Arguments[i].Expression)
                                .ConvertedType is IArrayTypeSymbol,
                        (x, i) =>
                            invocationExpression.ArgumentList.Arguments[i].NameColon == null
                                ? null
                                : invocationExpression.ArgumentList.Arguments[i].NameColon.Name.ToString(),
                        invocationExpression.ArgumentList.Arguments.ToArray()
                        ).ToList();

                    invocationExpression =
                        invocationExpression.WithArgumentList(
                            SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(arguments)));

                    return base.Visit(invocationExpression);
                }
            }
            return base.Visit(node);
        }
    }
}