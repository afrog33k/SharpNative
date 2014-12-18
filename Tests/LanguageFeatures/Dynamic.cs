//Type inference is fully supported, both using the 'var' keyword and implicitly typed lambdas.
using System; 
using System.Text; 

public class Driver {
	public static void Main() {
		dynamic myDynamic = new object();

		myDynamic["property"] = "property value";

		var sb = new StringBuilder();
		sb.AppendLine((string)myDynamic.property);
    
        Console.WriteLine(sb.ToString());
    }
}