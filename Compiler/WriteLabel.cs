// /*
//   SharpNative - C# to D Transpiler
//   (C) 2014 Irio Systems 
// */

#region Imports

using Microsoft.CodeAnalysis.CSharp.Syntax;

#endregion

namespace SharpNative.Compiler
{
    internal static class WriteLabel
    {
        public static void Go(OutputWriter writer, LabeledStatementSyntax method)
        {
            writer.WriteLine(method.Identifier + ":");
            Core.Write(writer, method.Statement);
        }

        public static void Go(OutputWriter writer, CaseSwitchLabelSyntax method, bool isStringSwitch = false)
        {
            writer.Write("case ");
            //writer.WriteLine("case " + method.Value.ToString() +":");
            Core.Write(writer, method.Value);
            writer.Write(isStringSwitch ? ".Text" : "");
            writer.Write(":\r\n");
        }
    }
}