//Type inference is fully supported, both using the 'var' keyword and implicitly typed lambdas.
using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;


public class Driver {

	public static void Main() {
		int[] arr1 = new[] { 1, 2, 3 };
		int[] arr2 = new[] { 1, 2, 3 };

		var query = arr1.SelectMany (i => arr2, (i, j) => new {
	i,
	j
}).Where (_ => _.i >= _.j).Select (_1 => new {
	_1,
	k = _1.i + _1.j
}).Select (_2 => "i = " + _2._1.i + ", j = " + _2._1.j + ", k = " + _2.k);

		var sb = new StringBuilder();
		foreach (var x in query)
			sb.AppendLine(x);

		Console.WriteLine(sb.ToString());
	}
}