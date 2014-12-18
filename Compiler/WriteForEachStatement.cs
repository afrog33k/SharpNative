// /*
//   SharpNative - C# to D Transpiler
//   (C) 2014 Irio Systems 
// */

#region Imports

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
            //  if (types.Type is IArrayTypeSymbol)
//            {
            //It's faster to "while" through arrays than "for" through them
            //   writer.WriteOpenBrace();
            writer.WriteLine("");

//                writer.WriteIndent();

            var typeinfo = TypeProcessor.GetTypeInfo(foreachStatement.Expression);

//                var isPtr = typeinfo.Type != null && typeinfo.Type.IsValueType ? "" : "";
            var typeString = TypeProcessor.ConvertType(foreachStatement.Type) + " ";

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

                //				writer.CloseBrace ();
                foreachCount++;
            }
//				writer.Write(string.Format("foreach ({1}; ", typeString, WriteIdentifierName.TransformIdentifier(foreachStatement.Identifier.ValueText)));
//                writer.Write(")\r\n");
//              
//                writer.OpenBrace();
//
//                writer.WriteIndent();
//
//                Core.WriteStatementAsBlock(writer, foreachStatement.Statement, false);
//
//                writer.CloseBrace();

            //    writer.WriteCloseBrace();
//            }
//            else if (typeStr == "System.Collections.Generic.List<>"
//                //|| typeStr == "System.Collections.Generic.Dictionary<,>" 
//                || typeStr == "System.Collections.Generic.Dictionary<,>.KeyCollection"
//                || typeStr == "System.Collections.Generic.Dictionary<,>.ValueCollection")
//            {
//                //It's faster to "while" over a list's iterator than to "for" through it
//                writer.WriteOpenBrace();
//                info.WritePreLoop(writer);
//
//                writer.WriteIndent();
//                writer.Write("val __foreachiterator = ");
//                Core.Write(writer, foreachStatement.Expression);
//                writer.Write(".iterator();\r\n");
//
//
//                writer.WriteLine("while (__foreachiterator.hasNext())");
//                writer.WriteOpenBrace();
//
//                writer.WriteIndent();
//                writer.Write("val ");
//                writer.Write(WriteIdentifierName.TransformIdentifier(foreachStatement.Identifier.ValueText));
//                writer.Write(" = __foreachiterator.next();\r\n");
//
//                info.WriteLoopOpening(writer);
//                Core.WriteStatementAsBlock(writer, foreachStatement.Statement, false);
//                info.WriteLoopClosing(writer);
//                writer.WriteCloseBrace();
//
//                info.WritePostLoop(writer);
//                writer.WriteCloseBrace();
//            }
//            else
//            {
//
//                info.WritePreLoop(writer);
//                writer.WriteIndent();
//                writer.Write("for (");
//                writer.Write(WriteIdentifierName.TransformIdentifier(foreachStatement.Identifier.ValueText));
//                writer.Write(" = ");
//                Core.Write(writer, foreachStatement.Expression);
//                writer.Write(")\r\n");
//                writer.WriteOpenBrace();
//                info.WriteLoopOpening(writer);
//                Core.WriteStatementAsBlock(writer, foreachStatement.Statement, false);
//                info.WriteLoopClosing(writer);
//                writer.WriteCloseBrace();
//                info.WritePostLoop(writer);
//            }
        }
    }
}