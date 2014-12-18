//Type inference is fully supported, both using the 'var' keyword and implicitly typed lambdas.
using System;

using System.Text;
using System.Collections.Generic;


public class C {
    public string Property;
    public List<string> Collection;

    public C() {
        Collection = new List<string>();
    }
}

public class Driver {

    public static void Main() {
        var c = new C() { Property = "Property", Collection = { "Value 1", "Value 2", "Value 3" } };
        var sb = new StringBuilder();
        sb.AppendLine("c.Property = " + c.Property);
        sb.AppendLine("c.Collection = " + c.Collection);

        Console.WriteLine(sb.ToString());
    }
}