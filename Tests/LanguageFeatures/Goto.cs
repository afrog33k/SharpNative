//Type inference is fully supported, both using the 'var' keyword and implicitly typed lambdas.
using System; 
using System.Text; 

public class Driver {

	public static void Main() {
		var sb = new StringBuilder();

		int a = 0;
		lbl1:
		sb.AppendLine("lbl1");
		if (a == 0)
			goto lbl2;
		else if (a == 1)
			goto lbl3;
		else
			goto lbl4;
		lbl2:
		sb.AppendLine("lbl2");
		goto lbl3;
		lbl3:
		a = 2;
		sb.AppendLine("lbl3");
		goto lbl1;
		lbl4:
		sb.AppendLine("lbl4");
    
        Console.WriteLine(sb.ToString());
    }
}