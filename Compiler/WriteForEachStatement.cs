// /*
//   SharpNative - C# to D Transpiler
//   (C) 2014 Irio Systems 
// */

#region Imports

using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

#endregion

namespace SharpNative.Compiler
{
    internal static class WriteForEachStatement
    {

        public static void Go(OutputWriter writer, ForEachStatementSyntax foreachStatement)
        {
            var info = new LoopInfo(foreachStatement);

            var types = TypeProcessor.GetTypeInfo(foreachStatement.Expression);
            var typeStr = TypeProcessor.GenericTypeName(types.Type);
            writer.WriteLine("");

           
            //   writer.WriteOpenBrace();

//                writer.WriteIndent();

            var typeinfo = TypeProcessor.GetTypeInfo(foreachStatement.Expression);

//                var isPtr = typeinfo.Type != null && typeinfo.Type.IsValueType ? "" : "";
            var typeString = TypeProcessor.ConvertType(foreachStatement.Type) + " ";

            var foreachCount = Context.Instance.ForeachCount++;
            var isListT = types.Type.OriginalDefinition == Context.ListT;
            var isArray = types.Type is IArrayTypeSymbol;
            if (isArray || isListT)
            {//Lets just for through the array, iterators are slow ... really slow
                var forIter = "__for" + foreachCount;
                var forArray = "__varfor" + foreachCount;

                var temp = new TempWriter();

                Core.Write(temp, foreachStatement.Expression);

                var expressiono = temp.ToString();

               // writer.WriteIndent();
                writer.WriteLine("auto {0} = {1};", forArray, expressiono);
                writer.WriteLine("for (int {0}=0;{0} < {2}.{3}; {0}++)", forIter, //Special case to support iterating "params" array
                    WriteIdentifierName.TransformIdentifier(foreachStatement.Identifier.Text), forArray, isListT?"Count" :"length");
               
                writer.OpenBrace();
                writer.WriteLine("auto {0} = {1}[{2}];", WriteIdentifierName.TransformIdentifier(foreachStatement.Identifier.Text), forArray, forIter);
                Core.WriteStatementAsBlock(writer, foreachStatement.Statement, false);
                writer.CloseBrace();
               

                return;
            }
            //It's faster to "while" through arrays than "for" through them

            var foreachIter = "__foreachIter" + foreachCount;

            if (typeinfo.Type.AllInterfaces.OfType<INamedTypeSymbol>().Any(j => j.MetadataName == "IEnumerable`1") ||
                typeinfo.Type.MetadataName == "IEnumerable`1")
            {

                var collections = SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System.Collections.Generic"));
               
               
                Context.Instance.UsingDeclarations = Context.Instance.UsingDeclarations
                    .Union(new[]
                {
                    collections
                }).ToArray();

                writer.WriteLine("//ForEach");
//				writer.OpenBrace ();
                writer.WriteIndent();
                writer.Write(string.Format("auto {0} = ", foreachIter));
                Core.Write(writer, foreachStatement.Expression);
                writer.Write(String.Format(".GetEnumerator(cast(IEnumerable__G!({0}))null);\r\n",typeString));
                writer.WriteLine(string.Format("while({0}.MoveNext())", foreachIter));
                writer.OpenBrace();

                writer.WriteLine(string.Format("{0}{1} = {2}.Current(cast(IEnumerator__G!({0}))null);", typeString,
                    WriteIdentifierName.TransformIdentifier(foreachStatement.Identifier.Text), foreachIter));

                Core.WriteStatementAsBlock(writer, foreachStatement.Statement, false);

                writer.CloseBrace();
                writer.WriteLine("");

//				writer.CloseBrace ();
                foreachCount++;
            }
            else
            {
                var collections = SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System.Collections"));
                Context.Instance.UsingDeclarations = Context.Instance.UsingDeclarations
                  .Union(new[]
                {
                    collections
                }).ToArray();

                writer.WriteLine("//ForEach");
                writer.WriteIndent();
                writer.Write(string.Format("auto {0} = ", foreachIter));
                Core.Write(writer, foreachStatement.Expression);
                writer.Write(".GetEnumerator();\r\n");
                writer.WriteLine(string.Format("while({0}.MoveNext())", foreachIter));
                writer.OpenBrace();

                writer.WriteLine(string.Format("{0}{1} = UNBOX!({0})({2}.Current);", typeString,
                    WriteIdentifierName.TransformIdentifier(foreachStatement.Identifier.Text), foreachIter));

                Core.WriteStatementAsBlock(writer, foreachStatement.Statement, false);

                writer.CloseBrace();
                writer.WriteLine("");

                foreachCount++;
            }
        }
    }
}