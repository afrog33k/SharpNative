//Type inference is fully supported, both using the 'var' keyword and implicitly typed lambdas.
using System; 
using System.Text; 

public class C {
	private string message;

	public C(int i) {
		message = "Constructed using int";
	}

	public C(string s) {
		message = "Constructed using string";
	}

	public string Message { get { return message; } }
}


public class Driver 
{

	public string F(int i) {
		return "F(int)";
	}

	public string F(string s) {
		return "F(string)";
	}
  
    public static void Main() {
     var sb = new StringBuilder();
		sb.AppendLine("Constructing with int: " + new C(1).Message);
		sb.AppendLine("Constructing with string: " + new C("x").Message);
    
        Console.WriteLine(sb.ToString());
    }
}