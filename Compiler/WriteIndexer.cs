// /*
//   SharpNative - C# to D Transpiler
//   (C) 2014 Irio Systems 
// */

#region Imports

using Microsoft.CodeAnalysis.CSharp.Syntax;

#endregion

namespace SharpNative.Compiler
{
    internal static class WriteIndexer
    {
        public static void Go(OutputWriter writer, IndexerDeclarationSyntax property, bool isProxy = false)
        {
            WriteProperty.Go(writer, property, isProxy);
        }
    }
}