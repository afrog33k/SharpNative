//Type inference is fully supported, both using the 'var' keyword and implicitly typed lambdas.
using System; 
using System.Text; 



public class Driver 
{
	public static string F(int i) {
		return "F(int)";
	}

	public static string F(string s) {
		return "F(string)";
	}

	public static void Main() {
		var sb = new StringBuilder();
		sb.AppendLine(F(0));
		sb.AppendLine(F("X"));
        Console.WriteLine(sb.ToString());
    }
}