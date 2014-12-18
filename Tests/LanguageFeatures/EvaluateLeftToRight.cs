//Type inference is fully supported, both using the 'var' keyword and implicitly typed lambdas.
//This won't pass as I rearrange function calls to deal with named parameters ... wouldnt like to create temporary variables etc,
//not worth the overhead.

using System; 
using System.Text; 



public class Driver {
	private StringBuilder sb;

	void F(int a = 1, int b = 2, int c = 3, int d = 4, int e = 5, int f = 6, int g = 7) {}

	int F1() { sb.AppendLine("F1"); return 0; }
	int F2() { sb.AppendLine("F2"); return 0; }
	int F3() { sb.AppendLine("F3"); return 0; }
	int F4() { sb.AppendLine("F4"); return 0; }

	public void Main() {
		sb = new StringBuilder();

		F(d: F1(), g: F2(), f: F3(), b: F4());
    
        Console.WriteLine(sb.ToString());
    }
}