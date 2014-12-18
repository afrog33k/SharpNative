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
                // params parameters require special handling .. not for D
//				if (parameter.IsParams)
//				{
//					// If there's only one argument for the params parameter, it could be either an array containing
//					// the the params arguments, or just a single argument.
//					if (remainingArguments.Count == 1)
//					{
//						var argument = remainingArguments.Peek();
//						var argIndex = args.Length - remainingArguments.Count;
//
//						if (isArgumentArray(argument, argIndex))
//						{
//							// If it's an array and exported, we just pass it as is, since the argument must ultimately be an array.
//							//							if (isExported)
//							//							{
//							//								arguments.Add(argument);
//							//							}
//							// If it's NOT exported, then we simply pass them as ordinary arguments, since that's how this "params"
//							// concept works over there (it's not abstracted into an array).
//							//							else
//							{
//								//								foreach (var element in ((ArraySy)argument).Elements)
//								{
//									//									arguments.Add(element);
//								}
//							}
//							remainingArguments.Dequeue();
//							continue;
//						}
//					}
                // If exported, then add all the rest of the arguments as an array
                //					if (isExported)
                //					{
                //						arguments.Add(MakeArray(Js.Array(remainingArguments.ToArray()), (IArrayTypeSymbol)parameter.Type));
                //					}
                // Otherwise, add all the rest of the arguments as ordinary arguments per the comment earlier about non exported types.
                //else
//					{
//						while (remainingArguments.Any())
//							arguments.Add(remainingArguments.Dequeue());
//					}
//				}
//				else 
                if (!remainingArguments.Any())
                {
                    // If not exported, then it's a C# to Javascript transfer, and in Javascript land, default arguments are 
                    // always undefined. Thus we don't want to add default arguments for non-exported methods.
                    //					if (isExported)
                    {
                        //						if (parameter.GetAttributes().Any(x => Equals(x.AttributeClass, Context.Instance.CallerMemberNameAttribute)))
                        //						{
                        //							var value = context.GetContainingMemberName();
                        //							arguments.Add(Js.Literal(value ?? parameter.ExplicitDefaultValue));
                        //						}
                        //						else
                        //						{
                        //							arguments.Add(Js.Literal(parameter.ExplicitDefaultValue));
                        //						}
                    }
                }
                else
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