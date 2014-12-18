//Type inference is fully supported, both using the 'var' keyword and implicitly typed lambdas.
using System;

using System.Text;
using System.Collections.Generic;



public class Driver {

	public static void Main() {
		var list = new List<string> { "Value 1", "Value 2", "Value 3" };

		var sb = new StringBuilder();
		foreach (var s in list) {
			sb.AppendLine(s);
		}

		Console.WriteLine(sb.ToString());
	}
}