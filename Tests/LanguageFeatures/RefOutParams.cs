//Type inference is fully supported, both using the 'var' keyword and implicitly typed lambdas.
using System;

using System.Text;


public class Driver {
    static void F(ref int i) {
        i++;
    }

    public static void Main() {
        int i = 1;
        var sb = new StringBuilder();
        sb.AppendLine("Before: i = " + i);
        F(ref i);
        sb.AppendLine("After: i = " + i);

        Console.WriteLine(sb.ToString());
    }
}