//Type inference is fully supported, both using the 'var' keyword and implicitly typed lambdas.
using System; 
using System.Text; 

public class Value {
	public int i;
	public Value(int i) {
		this.i = i;
	}

	public static Value operator+(Value v, int i) {
		return new Value(v.i + i);
	}
}

public class Driver {

	
  
    public static void Main() {
    var sb = new StringBuilder();
		var v = new Value(13);
		sb.AppendLine("Result = " + (v + 12).i);
    
        Console.WriteLine(sb.ToString());
    }
}