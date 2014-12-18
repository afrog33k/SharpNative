//Type inference is fully supported, both using the 'var' keyword and implicitly typed lambdas.
using System; 
using System.Text; 

public class Driver {
	public class Nested {
		public string GetTypeName() {
			return GetType().FullName;
		}
	}

	public void Main() {
		var sb = new StringBuilder();
		sb.AppendLine(new Nested().GetTypeName());
    
        Console.WriteLine(sb.ToString());
    }
}