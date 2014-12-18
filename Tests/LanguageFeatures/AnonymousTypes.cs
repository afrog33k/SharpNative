//Type inference is fully supported, both using the 'var' keyword and implicitly typed lambdas.
using System; 
using System.Text; 


public class Driver {
  
    public static void Main() {
      var v = new { i = 1, s = "x" };
        var sb = new StringBuilder();
        sb.AppendLine("v.i = " + v.i);
        sb.AppendLine("v.s = " + v.s);
    
        Console.WriteLine(sb.ToString());
    }
}