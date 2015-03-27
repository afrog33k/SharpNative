// /*
//   SharpNative - C# to D Transpiler
//   (C) 2014 Irio Systems 
// */

#region Imports

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

#endregion

namespace SharpNative.Compiler
{
    internal static class WriteArrayCreationExpression
    {
        public static void Go(OutputWriter writer, ImplicitArrayCreationExpressionSyntax array)
        {
            var ti = TypeProcessor.GetTypeInfo(array);
            var t = ti.Type;
            if (ti.ConvertedType!=null && !(ti.ConvertedType == t)) // Alot of times we are using covariance here
            {
                t = ti.ConvertedType;
            }
            var ptr = !t.As<IArrayTypeSymbol>().ElementType.IsValueType; // ? "" : "";
            var elementType = t.As<IArrayTypeSymbol>().ElementType;
            var type = TypeProcessor.ConvertType(elementType);
            var typeString = "Array_T!(" + type + ")";

            var tempWriter = new TempWriter();
            tempWriter.Indent = writer.Indent;

            if (elementType.TypeKind == TypeKind.TypeParameter)
                tempWriter.Write(" __TypeNew!(" + typeString + ")([");
            else
            {
                tempWriter.Write("new " + typeString + "(");
            }



            //__ARRAY
            var variableDeclarationSyntax = array.Parent.Parent.Parent as VariableDeclarationSyntax;
           // var argumentSyntax = array.Parent as ArgumentSyntax;

            if (variableDeclarationSyntax != null)
            {
                var atype = variableDeclarationSyntax.Type;
                array.Initializer.WriteArrayInitializer(tempWriter, atype);
            }
           else
                array.Initializer.WriteArrayInitializer(tempWriter,t);


            tempWriter.Write(")");

            var tempString = tempWriter.ToString();

            var oldString = tempString;

            tempString = tempString.Replace("new " + typeString + "(__CC!(" + type + "[])(", "__ARRAY!(" + type + ")(");

            if (tempString != oldString)
                tempString = tempString.RemoveFromEndOfString(")");

            writer.Write(tempString);

            
        }

        public static void Go(OutputWriter writer, ArrayCreationExpressionSyntax array)
        {
            //TODO: does this info just get discarded ?
            //   if (array.Type.RankSpecifiers.Count > 1 || array.Type.RankSpecifiers.Single().Sizes.Count > 1)
            //     throw new Exception("Multi-dimensional arrays are not supported " + Utility.Descriptor(array));

            //  var rankExpression = array.Type.RankSpecifiers.FirstOrDefault().Sizes.Single();
//            IEnumerable<ExpressionSyntax> fullDimension =
//                array.Type.RankSpecifiers.Select(o => o.Sizes).SelectMany(j=>j);

            if (array.Initializer != null)
            {
                var aType = TypeProcessor.GetTypeInfo(array).Type as IArrayTypeSymbol;
                //                writer.Write("new ");
                //                writer.Write(TypeProcessor.ConvertType(array.Type.ElementType));
                //                writer.Write("[]{");

                //                var statement = " new (smGC) ";
                // writer.Write(" new ");
                var type = "";

                if (aType.Rank == 1) // Jagged
                {
                    var rCount = array.Type.RankSpecifiers.Count;

                    for (int i = 0; i < rCount; i++)
                    {
                        type += ("Array_T!(");

                        var typeInfo = TypeProcessor.GetTypeInfo(array.Type.ElementType);

                        if ((i + 1) == rCount)
                        {
                            var isPtr = typeInfo.Type != null && typeInfo.Type.IsValueType; // ? "" : "";
                            var typeString = TypeProcessor.ConvertType(typeInfo.Type) + " ";

                            // Ideally Escape analysis should take care of this, but for now all value types are on heap and ref types on stack

                            //writer.Write(typeString);
                            type += typeString;
                        }
                    }
                    for (int i = 0; i < rCount; i++)
                    {
                        if ((i > 0) && rCount > 1)
                            type = type;
                        type += (")");
                    }
                }
                else
                {
                    type += "Array_T!(" + TypeProcessor.ConvertType(array.Type.ElementType)
                        /* + Enumerable.Range (0, aType.Rank-1).Select (l => "[]").Aggregate ((a, b) => a + b).ToString () */ +
                    ")";

//                  var typeInfo = TypeProcessor.GetTypeInfo (array.Type.ElementType);
                }
//              if (type.ConvertedType.TypeKind == Microsoft.CodeAnalysis.TypeKind.TypeParameter)
//                  writer.Write (" __TypeNew!(" + type + ")([");
//              else
                //{
                writer.Write("new  " + type);

                writer.Write("(");
                //}
                bool first = true;

                array.Initializer.WriteArrayInitializer(writer, array.Type);

                writer.Write(")");
            }
            else
            {
                // int.TryParse(rankExpression != null ? rankExpression.ToString() : "0", out sizeInteger);

                //  writer.Write(" new ");
                var typeStr = "";

                var rCount = array.Type.RankSpecifiers.Count;
                for (int i = 0; i < rCount; i++)
                {
                    typeStr += ("Array_T!(");

                    var type = TypeProcessor.GetTypeInfo(array.Type.ElementType);

                    if ((i + 1) == rCount)
                    {
//                    if (type.Type != null && (type.Type.IsValueType == false))
//                            typeStr += ("");
                        // Ideally Escape analysis should take care of this, but for now all value types are on heap and ref types on stack

                        typeStr += (TypeProcessor.ConvertType(array.Type.ElementType));
                    }
                }
                for (int i = 0; i < rCount; i++)
                {
//                    if ((i > 0) && rCount > 1)
//                        typeStr+=("");
                    typeStr += (")");
                }

                writer.Write("new " + typeStr);
                writer.Write("(");

                array.Initializer.WriteArrayInitializer(writer, array.Type);

                writer.Write(")");

                //                writer.Write("new Array_T!(");
                //
                //              
                //
                //                var symbol =Program.GetModel(array).GetDeclaredSymbol(array.Type.ElementType);
                //
                //                var lsymbol = symbol as ILocalSymbol;
                //
                //                if (lsymbol != null && (lsymbol.Type.IsValueType == false))
                //                    writer.Write(""); // Ideally Escape analysis should take care of this, but for now all value types are on heap and ref types on stack
                //
                //                writer.Write(TypeProcessor.ConvertType(array.Type.ElementType));
                //                                writer.Write(">("+fullDimension+")");

                // Core.Write(writer, array.Type.RankSpecifiers.Single().Sizes.Single());
                // writer.Write(")");

                //  if (array.Initializer != null)
                //    throw new Exception("Initalizers along with array sizes are not supported - please use a size or an initializer " + Utility.Descriptor(array));

                //                writer.Write("new ");
                //                writer.Write(TypeProcessor.ConvertType(array.Type.ElementType));
                //                writer.Write("[]{");
                //                Core.Write(writer, array.Type.RankSpecifiers.Single().Sizes.Single());
                //                writer.Write("}");
            }
        }

        public static void Go(OutputWriter writer, InitializerExpressionSyntax method)
        {
            var array = TypeProcessor.GetTypeInfo(method).ConvertedType as IArrayTypeSymbol;

            //            //TODO: does this info just get discarded ?
            //               if (array.Type.RankSpecifiers.Count > 1 || array.Type.RankSpecifiers.Single().Sizes.Count > 1)
            //                 throw new Exception("Multi-dimensional arrays are not supported " + Utility.Descriptor(array));
            //
            //            var arrayInit = array as ArrayCreationExpressionSyntax;
            //            var rankExpression = array.Type.RankSpecifiers.FirstOrDefault().Sizes.Single();
            //            IEnumerable<ExpressionSyntax> fullDimension =
            //                array.Type.RankSpecifiers.Select(o => o.Sizes).SelectMany(j => j);
            //
            //
            //            if (array.Initializer != null)
            //            {
            //                //                writer.Write("new ");
            //                //                writer.Write(TypeProcessor.ConvertType(array.Type.ElementType));
            //                //                writer.Write("[]{");
            //
            //                writer.Write(" new ");
            //
            //                var rCount = array.Type.RankSpecifiers.Count;
            //                for (int i = 0; i < rCount; i++)
            //                {
            //                    writer.Write("Array_T<");
            //
            //                    var typeInfo = TypeProcessor.GetTypeInfo(array.Type.ElementType);
            //
            //                    if ((i + 1) == rCount)
            //                    {
            //                        if (typeInfo.Type != null && (typeInfo.Type.IsValueType == false))
            //                            writer.Write("");
            //                        // Ideally Escape analysis should take care of this, but for now all value types are on heap and ref types on stack
            //
            //                        writer.Write(TypeProcessor.ConvertType(array.Type.ElementType));
            //
            //
            //                    }
            //
            //                }
            //                for (int i = 0; i < rCount; i++)
            //                {
            //                    if ((i > 0) && rCount > 1)
            //                        writer.Write("");
            //                    writer.Write(">");
            //
            //
            //                }
            //
            //                writer.Write("(");
            //                writer.Write("{");
            //                bool first = true;
            //                foreach (var expression in array.Initializer.Expressions)
            //                {
            //                    if (first)
            //                        first = false;
            //                    else
            //                        writer.Write(", ");
            //
            //                    Core.Write(writer, expression);
            //                }
            //
            //                writer.Write("})");
            //            }
            //            else
            //            {
            //
            //
            //
            //
            //
            //
            //                // int.TryParse(rankExpression != null ? rankExpression.ToString() : "0", out sizeInteger);
            //
            //
            //                writer.Write(" new ");
            //
            //                var rCount = array.Type.RankSpecifiers.Count;
            //                for (int i = 0; i < rCount; i++)
            //                {
            //                    writer.Write("Array_T<");
            //
            //                    var type = TypeProcessor.GetTypeInfo(array.Type.ElementType);
            //
            //                    if ((i + 1) == rCount)
            //                    {
            //                        if (type.Type != null && (type.Type.IsValueType == false))
            //                            writer.Write("");
            //                        // Ideally Escape analysis should take care of this, but for now all value types are on heap and ref types on stack
            //
            //                        writer.Write(TypeProcessor.ConvertType(array.Type.ElementType));
            //
            //
            //                    }
            //
            //                }
            //                for (int i = 0; i < rCount; i++)
            //                {
            //                    if ((i > 0) && rCount > 1)
            //                        writer.Write("");
            //                    writer.Write(">");
            //
            //
            //                }
            //
            //
            //
            //
            //                writer.Write("(");
            //
            //                var first = true;
            //                foreach (var dimensions in fullDimension)
            //                {
            //                    if (dimensions is OmittedArraySizeExpressionSyntax)
            //                        continue;
            //                    if (first)
            //                        first = false;
            //                    else
            //                        writer.Write(" + ");
            //
            //                    Core.Write(writer, dimensions);
            //                }
            //
            //                writer.Write(")");
            //
            //                //                writer.Write("new Array_T<");
            //                //
            //                //              
            //                //
            //                //                var symbol =Program.GetModel(array).GetDeclaredSymbol(array.Type.ElementType);
            //                //
            //                //                var lsymbol = symbol as ILocalSymbol;
            //                //
            //                //                if (lsymbol != null && (lsymbol.Type.IsValueType == false))
            //                //                    writer.Write(""); // Ideally Escape analysis should take care of this, but for now all value types are on heap and ref types on stack
            //                //
            //                //                writer.Write(TypeProcessor.ConvertType(array.Type.ElementType));
            //                //                                writer.Write(">("+fullDimension+")");
            //
            //                // Core.Write(writer, array.Type.RankSpecifiers.Single().Sizes.Single());
            //                // writer.Write(")");
            //
            //                //  if (array.Initializer != null)
            //                //    throw new Exception("Initalizers along with array sizes are not supported - please use a size or an initializer " + Utility.Descriptor(array));
            //
            //
            //
            //                //                writer.Write("new ");
            //                //                writer.Write(TypeProcessor.ConvertType(array.Type.ElementType));
            //                //                writer.Write("[]{");
            //                //                Core.Write(writer, array.Type.RankSpecifiers.Single().Sizes.Single());
            //                //                writer.Write("}");
            //            }
        }
    }
}