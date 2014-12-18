//Type inference is fully supported, both using the 'var' keyword and implicitly typed lambdas.
using System;

using System.Text;


public class GenericClass<T> {
    public string F() {
        return typeof(T).FullName;
    }
}

public class Driver {
    public static string F<T>() {
        return typeof(T).FullName;
    }

    public static void Main() {
        var sb = new StringBuilder();
        sb.AppendLine("new GenericClass<Driver>().F() = " + new GenericClass<Driver>().F());;
        sb.AppendLine("F<Driver>() = " + F<Driver>());

        Console.WriteLine(sb.ToString());
    }
}