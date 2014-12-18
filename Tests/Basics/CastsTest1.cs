using System;

public static class Program
{

    public static void Main()
    {
       object aString = "hey there";
       var strings = (string) aString;
       Console.WriteLine(strings);



    }
}