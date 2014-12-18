//Type inference is fully supported, both using the 'var' keyword and implicitly typed lambdas.
using System; 
using System.Text; 

public class Driver {
static    string F(int i) {
        return "F(int)";
    }

static    string F(string s) {
        return "F(sting)";
    }

 static   string F(Func<int> f) {
        return "F(Func<int>)";
    }

 static   string F(Func<string> f) {
        return "F(Func<string>)";
    }

    public static void Main() {
        var v1 = 1;
        var v2 = "x";

        var sb = new StringBuilder();
        sb.AppendLine(F(v1));
        sb.AppendLine(F(v2));
        sb.AppendLine(F(() => v1));
        sb.AppendLine(F(() => v2));
    
        Console.WriteLine(sb.ToString());
    }
}