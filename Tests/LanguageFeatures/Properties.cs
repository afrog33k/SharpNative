//Type inference is fully supported, both using the 'var' keyword and implicitly typed lambdas.
using System; 
using System.Text; 



public class Driver {
	public static int MyInt { get; set; }

	private static string myString;
	public  static string MyString { get { return myString; } set { myString = value; } }

	public static void Main() {
		MyInt = 3;
		MyString = "x";

		var sb = new StringBuilder();
		sb.AppendLine("MyInt = " + MyInt);
		sb.AppendLine("MyString = " + MyString);
    
        Console.WriteLine(sb.ToString());
    }
}