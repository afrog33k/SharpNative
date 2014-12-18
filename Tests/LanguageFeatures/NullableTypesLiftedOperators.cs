//Type inference is fully supported, both using the 'var' keyword and implicitly typed lambdas.
using System; 
using System.Text; 

public class Driver {
	public void Main() {
		int? i = null;

		var sb = new StringBuilder();
		sb.AppendLine("i = " + i);
		sb.AppendLine("i + 1 = " + (i + 1));
		sb.AppendLine("i.HasValue = " + i.HasValue);
    
        Console.WriteLine(sb.ToString());
    }
}