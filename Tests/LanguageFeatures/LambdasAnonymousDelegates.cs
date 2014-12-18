//Type inference is fully supported, both using the 'var' keyword and implicitly typed lambdas.
using System;

using System.Text;



public class Driver {

	public static int F(Func<int, int> f) {
		return f(1);
	}

	public static void Main() {
		var sb = new StringBuilder();
		sb.AppendLine("Implicitly typed: " + F(i => i + 1));
		sb.AppendLine("Explicitly typed: " + F((int i) => i + 1));
		sb.AppendLine("Block lambda: " + F(i => { return i + 1; }));
		sb.AppendLine("C#2 anonymous delegate: " + F(delegate(int i) { return i + 1; }));

		Console.WriteLine(sb.ToString());
	}
}