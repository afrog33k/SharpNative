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
			if (isStringSwitch)// && method.Value.ToString ().Trim () == "null")
			{
				var value = Core.WriteString (method.Value, true, writer.Indent);
				if (value.Trim () == "null")
					writer.Write ("-1");
				else
				{
					Core.Write (writer, method.Value);
					writer.Write (isStringSwitch ? ".Hash" : "");
				}
			}
			else
			{
				//writer.WriteLine("case " + method.Value.ToString() +":");
				Core.Write (writer, method.Value);
			}
            writer.Write(":\r\n");
        }
    }
}