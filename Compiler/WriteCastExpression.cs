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
    internal static class WriteCastExpression
    {
        public static void Go(OutputWriter writer, CastExpressionSyntax expression)
        {
            var symbol = TypeProcessor.GetSymbolInfo(expression);

            var castingFrom = TypeProcessor.GetTypeInfo(expression.Expression).Type ??
                              TypeProcessor.GetTypeInfo(expression).Type;

            var srcTypeDlang = TypeProcessor.ConvertType(castingFrom);
            var destType = TypeProcessor.GetTypeInfo(expression.Type).Type;
            var destTypeDlang = TypeProcessor.TryConvertType(expression.Type);

//            if (destTypeDlang == srcTypeDlang) // Looks like roslyn lies sometimes ... so lets cast anyway
//                Core.Write(writer, expression.Expression);
//            else
                //if (symbol.Symbol != null)// && srcTypeCpp != "int" && srcTypeCpp != "System.String" && srcTypeCpp != "bool")
            {
//                if (castingFrom != destType) // Check for implicit first then explicit

                bool useType = true;

                //We should start with exact converters and then move to more generic convertors i.e. base class or integers which are implicitly convertible
                var correctConverter = destType.GetImplicitCoversionOp(destType, castingFrom, true);
                //                            initializerType.Type.GetMembers("op_Implicit").OfType<IMethodSymbol>().FirstOrDefault(h => h.ReturnType == initializerType.Type && h.Parameters[0].Type == initializerType.ConvertedType);

                if (correctConverter == null)
                {
                    useType = false;
                    correctConverter =
                        castingFrom.GetImplicitCoversionOp(destType, castingFrom, true);
                    //.GetMembers("op_Implicit").OfType<IMethodSymbol>().FirstOrDefault(h => h.ReturnType == initializerType.Type && h.Parameters[0].Type == initializerType.ConvertedType);
                }

                if (correctConverter != null)
                {
                    //                            Core.Write(writer, expression.Left);
                    //                            writer.Write(" = ");
                    if (correctConverter.ReturnType != destType)
                        writer.Write("Cast!(" + destTypeDlang + (destType.IsValueType ? "" : "") + ")(");

                    if (castingFrom.SpecialType == SpecialType.System_Decimal && destType.IsPrimitiveInteger())
                    {
                       
                    }
                    else
                    {
                        if (useType)
                        {
                            writer.Write(TypeProcessor.ConvertType(destType) + "." + "op_Implicit_" +
                                         TypeProcessor.ConvertType(correctConverter.ReturnType, false, true, false).Replace(".","_"));
                        }
                        else
                        {
                            writer.Write(TypeProcessor.ConvertType(castingFrom) + "." + "op_Implicit_" +
                                         TypeProcessor.ConvertType(correctConverter.ReturnType, false, true, false).Replace(".", "_"));
                        }
                    }
                    writer.Write("(");
                    Core.Write(writer, expression.Expression);
                    writer.Write(")");

                    if (correctConverter.ReturnType != destType)
                        writer.Write(")");

                    return;
                }

                useType = true;

                //We should start with exact converters and then move to more generic convertors i.e. base class or integers which are implicitly convertible
                correctConverter = destType.GetExplictCoversionOp(destType, castingFrom, true);
                //                            initializerType.Type.GetMembers("op_Implicit").OfType<IMethodSymbol>().FirstOrDefault(h => h.ReturnType == initializerType.Type && h.Parameters[0].Type == initializerType.ConvertedType);

                if (correctConverter == null)
                {
                    useType = false;
                    correctConverter =
                        castingFrom.GetExplictCoversionOp(destType, castingFrom, true);
                    //.GetMembers("op_Implicit").OfType<IMethodSymbol>().FirstOrDefault(h => h.ReturnType == initializerType.Type && h.Parameters[0].Type == initializerType.ConvertedType);
                }

                if (correctConverter != null)
                {
                    var iscast = correctConverter.ReturnType != destType;
                    if (iscast)
                        writer.Write("Cast!(" + destTypeDlang  + ")(");

                    //                            Core.Write(writer, expression.Left);
                    //                            writer.Write(" = ");

                    if (castingFrom.SpecialType == SpecialType.System_Decimal && destType.IsPrimitiveInteger())
                    {

                    }
                    else
                    {
                         if (useType)
                        writer.Write(TypeProcessor.ConvertType(destType) + "." + "op_Explicit");
                    else
                        writer.Write(TypeProcessor.ConvertType(castingFrom) + "." + "op_Explicit");
                    }
                   
                    writer.Write("(");
                    Core.Write(writer, expression.Expression);
                    writer.Write(")");
                    if(iscast)
                        writer.Write(")");
                    return;
                }

                if (TypeProcessor.IsPrimitiveType(srcTypeDlang) && TypeProcessor.IsPrimitiveType(destTypeDlang))
                {//primitives can be directly cast
                    writer.Write("cast(" + destTypeDlang +  ")");
                    Core.Write(writer, expression.Expression);
                }

                else
                {
                    var convertedType = TypeProcessor.GetTypeInfo(expression).Type;
                    var type = TypeProcessor.GetTypeInfo(expression.Expression).Type;

                    if (type.IsValueType && !convertedType.IsValueType)
                    {
                        //We have to box then cast if not Object
                        if (destType.Name != "Object")
                            writer.Write("Cast!(" + destTypeDlang + ")(");
                        //Box
                        writer.Write("BOX!(" + TypeProcessor.ConvertType(type) + ")(");
                        //When passing an argument by ref or out, leave off the .Value suffix
//                    writer.Write(" >(");
                        Core.Write(writer, expression.Expression);
                        writer.Write(")");
                        if (destType.Name != "Object")
                            writer.Write(")");

                    }
                    else if (!type.IsValueType && convertedType.IsValueType)
                    {
                        //UnBox
                        //					writer.Write("(Cast!( Boxed!(" + TypeProcessor.ConvertType(convertedType) + ") )(");
                        //                    Core.Write(writer, expression.Expression);
                        //					writer.Write(")).Value");

                        writer.Write("UNBOX!(" + TypeProcessor.ConvertType(convertedType) + ")(");
                        Core.Write(writer, expression.Expression);
                        writer.Write(")");
                    }
                    else if (type.IsValueType && convertedType.IsValueType)
                    {
                        if (convertedType.TypeKind == TypeKind.Pointer)
                        {
                            //cannot use ascast here, its for boxed types and objects
                            writer.Write("cast(" + destTypeDlang + (destType.IsValueType ? "" : "") + ")(");
                            Core.Write(writer, expression.Expression);
                            writer.Write(")");
                        }
                        else
                        {
                            //cannot use ascast here, its for boxed types and objects
                            writer.Write("Cast!(" + destTypeDlang + (destType.IsValueType ? "" : "") + ")(");
                            Core.Write(writer, expression.Expression);
                            writer.Write(")");
                        }
                      
                    }
                    else
                    {
                        writer.Write("Cast!(");
                        writer.Write(destTypeDlang);
                        writer.Write(")(");
                        Core.Write(writer, expression.Expression);
                        writer.Write(")");
                    }
                }
            }
        }
    }
}