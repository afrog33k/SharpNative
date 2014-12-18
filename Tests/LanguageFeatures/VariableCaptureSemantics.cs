//Type inference is fully supported, both using the 'var' keyword and implicitly typed lambdas.
using System;

using System.Text;
using System.Collections.Generic;




public class Driver {

	public static void Main() 
	{
		var actions = new List<Action>();

		var sb = new StringBuilder();
		for (int i = 0; i < 5; i++) {
			int i2 = i;
			actions.Add(() => sb.AppendLine(i2.ToString()));
		}

		for (int i = 0; i < actions.Count; i++)
			actions[i]();

		Console.WriteLine(sb.ToString());
	}
}