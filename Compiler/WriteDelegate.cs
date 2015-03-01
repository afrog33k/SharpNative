// /*
//   SharpNative - C# to D Transpiler
//   (C) 2014 Irio Systems 
// */

#region Imports

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

#endregion

namespace SharpNative.Compiler
{
    internal static class WriteDelegate
    {
        /*
template Action(T) {
	alias void delegate(T obj) Action;
}

		*/


        public static void Go(OutputWriter outputWriter = null)
        {
            var partials = Context.Instance.DelegatePartials;
           
            var first = partials.First();
            bool fileExists = false;
            foreach (var @partial in partials)
            {
                Context.Instance.Namespace = @partial.Symbol.ContainingNamespace.FullName();
                Context.Instance.Type = @partial.Symbol;
                WriteOneDelegate(outputWriter, @partial, fileExists);
                fileExists = true;
            }
        }

        private static void WriteOneDelegate(OutputWriter outputWriter, Context.DelegateSyntaxAndSymbol first, bool fileExists)
        {
            Context.Instance.Namespace = first.Symbol.ContainingNamespace.FullName();
            Context.Instance.Type = first.Symbol;
            TypeProcessor.ClearUsedTypes();
            var mynamespace = Context.Instance.Type.ContainingNamespace.FullName().RemoveFromEndOfString(".Namespace");
            // + "." + TypeState.Instance.TypeName;

            var myUsingDirective = SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(mynamespace));
            var SystemUsingDirective = SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System"));
            // Required as certain functions like boxing are in this namespace
            Context.Instance.UsingDeclarations =
                first.Syntax.Parent.DescendantNodes().OfType<UsingDirectiveSyntax>().ToArray()
                    .Union(new[]
                    {
                        myUsingDirective, SystemUsingDirective
                    }).ToArray();
            OutputWriter writer = null;

            using (
                writer =
                    outputWriter == null
                        ? new OutputWriter(Context.Instance.Namespace, Context.Instance.TypeName)
                        : new TempWriter())
            {

                if (outputWriter != null)
                {
                    writer.WriteLine();
                    writer.Indent = outputWriter.Indent + 2;
                    writer.WriteIndent();
                }

                writer.FileExists = fileExists;

                WriteBcl.Go(writer);

                WriteStandardIncludes.Go(writer);

                //Look for generic arguments 
              
                {
                    List<TypeParameterSyntax> genericArgs = new List<TypeParameterSyntax>();
                    if (first.Syntax.TypeParameterList != null)
                        genericArgs = first.Syntax.TypeParameterList.Parameters.ToList();

                    var name = WriteType.TypeName(Context.Instance.Type, false); //Context.Instance.TypeName;

                    if (genericArgs.Count > 0)
                    {
                        name = "template " + name;
                        name += ("(");
                        name += (string.Join(" , ", genericArgs.Select(o => o)));
                        name += (")");

                        writer.WriteLine(name);

                        writer.OpenBrace();

                        writer.WriteLine("alias __Delegate!(" + TypeProcessor.ConvertType(first.Syntax.ReturnType) + " delegate" +
                                         WriteMethod.GetParameterListAsString(first.Syntax.ParameterList.Parameters) + ") " +
                                         WriteType.TypeName(Context.Instance.Type, false) + ";");

                        writer.CloseBrace();
                    }
                    else
                    {
                        //Non-generic
                        writer.WriteLine("alias __Delegate!(" + TypeProcessor.ConvertType(first.Syntax.ReturnType) + " delegate" +
                                         WriteMethod.GetParameterListAsString(first.Syntax.ParameterList.Parameters) + ") " +
                                         WriteType.TypeName(Context.Instance.Type, false) + ";");
                    }
                }

                if (outputWriter != null)
                    outputWriter.WriteLine(writer.ToString());
            }
        }
    }
}