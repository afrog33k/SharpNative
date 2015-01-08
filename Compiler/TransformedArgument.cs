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
    public class TransformedArgument
    {
        //Either String will be populated, or Argument will be. Never both.
        public readonly string StringOpt;
        public readonly ArgumentSyntax ArgumentOpt;


        public TransformedArgument(ArgumentSyntax argument)
        {
            ArgumentOpt = argument;
        }

        public TransformedArgument(string str)
        {
            StringOpt = str;
        }

        public void Write(OutputWriter writer)
        {
            if (StringOpt != null)
                writer.Write(StringOpt);
            else
            {
                if (ArgumentOpt.NameColon != null)
                {
                    Core.Write(writer, ArgumentOpt.NameColon.Name);
                    writer.Write(" = ");
                }

                var symbol = TypeProcessor.GetSymbolInfo(ArgumentOpt.Expression);
                var type = TypeProcessor.GetTypeInfo(ArgumentOpt.Expression);

                if (symbol.Symbol != null && type.ConvertedType != null && symbol.Symbol.Kind == SymbolKind.Method &&
                    type.ConvertedType.TypeKind == TypeKind.Delegate)
                {
                    var typeString = TypeProcessor.ConvertType(type.ConvertedType);

                    var createNew = !(ArgumentOpt.Parent.Parent is ObjectCreationExpressionSyntax); //Ugly hack

                    if (createNew)
                    {
                        if (type.ConvertedType.TypeKind == TypeKind.TypeParameter)
                            writer.Write(" __TypeNew!(" + typeString + ")(");
                        else
                            writer.Write("new " + typeString + "(");
                    }

                    var isStatic = symbol.Symbol.IsStatic;
                    if (isStatic)
                        writer.Write("__ToDelegate(");
                    writer.Write("&");

                    Core.Write(writer, ArgumentOpt.Expression);
                    if (isStatic)
                        writer.Write(")");

                    if (createNew)
                        writer.Write(")");
                    return;
                }

                Core.Write(writer, ArgumentOpt.Expression);

            }
        }
    }
}