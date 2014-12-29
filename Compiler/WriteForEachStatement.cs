// /*
//   SharpNative - C# to D Transpiler
//   (C) 2014 Irio Systems 
// */

#region Imports

using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

#endregion

namespace SharpNative.Compiler
{
    internal static class WriteForEachStatement
    {
        private static int foreachCount;

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


            if (types.Type is IArrayTypeSymbol)
            {//Lets just for through the array, iterators are slow ... really slow
                var forIter = "__for" + foreachCount;

                var temp = new TempWriter();

                Core.Write(temp, foreachStatement.Expression);

                var expressiono = temp.ToString();

                writer.WriteIndent();
                writer.WriteLine("for (int {0}=0;{0} < {2}.Length; {0}++)", forIter,
                    WriteIdentifierName.TransformIdentifier(foreachStatement.Identifier.ValueText), expressiono);
               
                writer.OpenBrace();
                writer.WriteLine("auto {0} = {1}[{2}];", WriteIdentifierName.TransformIdentifier(foreachStatement.Identifier.ValueText), expressiono, forIter);
                Core.WriteStatementAsBlock(writer, foreachStatement.Statement, false);
                writer.CloseBrace();
                foreachCount++;

                return;
            }
            //It's faster to "while" through arrays than "for" through them

            var foreachIter = "__foreachIter" + foreachCount;

            if (typeinfo.Type.AllInterfaces.OfType<INamedTypeSymbol>().Any(j => j.MetadataName == "IEnumerable`1") ||
                typeinfo.Type.MetadataName == "IEnumerable`1")
            {
                writer.WriteLine("//ForEach");
//				writer.OpenBrace ();
                writer.WriteIndent();
                writer.Write(string.Format("auto {0} = ", foreachIter));
                Core.Write(writer, foreachStatement.Expression);
                writer.Write(".IEnumerable_T_GetEnumerator();\r\n");
                writer.WriteLine(string.Format("while({0}.IEnumerator_MoveNext())", foreachIter));
                writer.OpenBrace();

                writer.WriteLine(string.Format("{0}{1} = {2}.IEnumerator_T_Current;", typeString,
                    WriteIdentifierName.TransformIdentifier(foreachStatement.Identifier.ValueText), foreachIter));

                Core.WriteStatementAsBlock(writer, foreachStatement.Statement, false);

                writer.CloseBrace();
                writer.WriteLine("");

//				writer.CloseBrace ();
                foreachCount++;
            }
            else
            {
                writer.WriteLine("//ForEach");
                writer.WriteIndent();
                writer.Write(string.Format("auto {0} = ", foreachIter));
                Core.Write(writer, foreachStatement.Expression);
                writer.Write(".IEnumerable_GetEnumerator();\r\n");
                writer.WriteLine(string.Format("while({0}.IEnumerator_MoveNext())", foreachIter));
                writer.OpenBrace();

                writer.WriteLine(string.Format("{0}{1} = UNBOX!({0})({2}.IEnumerator_Current);", typeString,
                    WriteIdentifierName.TransformIdentifier(foreachStatement.Identifier.ValueText), foreachIter));

                Core.WriteStatementAsBlock(writer, foreachStatement.Statement, false);

                writer.CloseBrace();
                writer.WriteLine("");

                foreachCount++;
            }
        }
    }
}