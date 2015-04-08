using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SharpNative.Compiler
{
    public class WriteStackArrayCreation
    {
        public static void Go(OutputWriter writer, StackAllocArrayCreationExpressionSyntax @as)
        {
            //import core.stdc.stdlib;
            //@as.Type;
            //auto ptr = cast(wchar*)alloca(wchar.sizeof * len);
            var arrayType = TypeProcessor.GetTypeInfo(@as.Type);
            var elementType = TypeProcessor.GetTypeInfo(((ArrayTypeSyntax) @as.Type).ElementType);
            var elementTypeName = TypeProcessor.ConvertType(elementType.Type);
            var size = Core.WriteString(((ArrayTypeSyntax)@as.Type).RankSpecifiers[0].Sizes[0]);
            writer.Write("cast("+ elementTypeName + "*) core.stdc.stdlib.alloca("+ elementTypeName + ".sizeof * "+ size + ")");

        }
    }
}