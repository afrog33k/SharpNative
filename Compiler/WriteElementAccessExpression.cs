// /*
//   SharpNative - C# to D Transpiler
//   (C) 2014 Irio Systems 
// */

#region Imports

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

#endregion

namespace SharpNative.Compiler
{
    internal static class WriteElementAccessExpression
    {
        public static void Go(OutputWriter writer, ElementAccessExpressionSyntax expression)
        {
            var type = TypeProcessor.GetTypeInfo(expression.Expression).Type;
            var typeStr = TypeProcessor.GenericTypeName(type);
            var additionalParam = "";
           var symbol =  TypeProcessor.GetSymbolInfo(expression); //This could be null
            if (symbol.Symbol != null)
            {
                var methodSymbol = symbol.Symbol as IPropertySymbol;
                //Lets find out if this is an interface implementation
                if (methodSymbol != null)
                {
                    IEnumerable<ISymbol> interfaceMethods =
                        methodSymbol.ContainingType.AllInterfaces.SelectMany(
                            u =>
                                u.GetMembers(methodSymbol.Name));

                    interfaceMethods =
                        interfaceMethods.Where(
                            o => Equals(methodSymbol.ContainingType.FindImplementationForInterfaceMember(o), methodSymbol));

                    if (interfaceMethods.Any())
                    {
                        //Lets  get the best match
                        var interfaceMethod = interfaceMethods.FirstOrDefault();
                        additionalParam = "cast("+ TypeProcessor.ConvertType(interfaceMethod.ContainingType.ConstructedFrom)+")null";
                    }
                }
            }

            //type.GetMembers("this[]")
            //Todo if we are using unsafe / fast mode, just use array->Data()[i] should bypass bounds check and indirection also should be as fast as pure c++ arrays
            //though fixed syntax could fix this too ??
            //            writer.Write("(*");

            Core.Write(writer, expression.Expression);

            if (type.SpecialType == SpecialType.System_Array)
//            writer.Write(")");
                writer.Write(".Items["); //TODO test this thoroughly

            else
                writer.Write("["); //TODO test this thoroughly

            var first = true;
            foreach (var argument in expression.ArgumentList.Arguments)
            {
                if (first)
                    first = false;
                else
                    writer.Write(", ");

                Core.Write(writer, argument.Expression);
            }
            if(additionalParam!="")
                writer.Write("," + additionalParam);
            writer.Write("]");
        }
    }
}