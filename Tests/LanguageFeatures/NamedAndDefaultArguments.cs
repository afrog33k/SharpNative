//Type inference is fully supported, both using the 'var' keyword and implicitly typed lambdas.
using System;

using System.Text;




public class Driver {

	public static string F(int a, int b, int c, string d = "default value") {
		return "a = " + a + ", b = " + b + ", c = " + c + ", d = " + d;
	}

	public static void Main() {
		var sb = new StringBuilder();

		sb.AppendLine(F(12, c: 34, b: 56));

		Console.WriteLine(sb.ToString());
	}
}