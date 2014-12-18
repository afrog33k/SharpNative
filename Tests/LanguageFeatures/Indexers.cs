//Type inference is fully supported, both using the 'var' keyword and implicitly typed lambdas.
using System;

using System.Text;


public class C {
	public string this[int i] {
		get { return "Retrieved index " + i; }
		set {}
	}
}

public class Driver {
	public static void Main() {
		var c = new C();

		var sb = new StringBuilder();
		sb.AppendLine(c[13]);

		Console.WriteLine(sb.ToString());
	}
}