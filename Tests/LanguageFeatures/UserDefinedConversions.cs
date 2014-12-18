//Type inference is fully supported, both using the 'var' keyword and implicitly typed lambdas.
using System;

using System.Text;


public class Value {
	public int i;

	private Value(int i) {
		this.i = i;
	}

	public static explicit operator Value(int i) {
		return new Value(i);
	}
}

public class Driver 
{
	public static void Main() {

		var sb = new StringBuilder();
		var v = (Value)13;
		sb.AppendLine("Result = " + v.i);

		Console.WriteLine(sb.ToString());
	}
}