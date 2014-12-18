//Type inference is fully supported, both using the 'var' keyword and implicitly typed lambdas.
using System;

using System.Text;


public class MyDisposable : IDisposable {
	public bool Disposed = false;

	public void Dispose() {
		Disposed = true;
	}
}


public class Driver {

	public static void Main() {
		var d = new MyDisposable();
		using (d) {
			// Here we could do stuff.
		}

		var sb = new StringBuilder();
		sb.AppendLine("Disposed: " + d.Disposed);

		Console.WriteLine(sb.ToString());
	}
}