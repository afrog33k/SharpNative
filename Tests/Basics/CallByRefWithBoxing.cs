public static class Program
{
    public static void Method1(ref object i, ref string str)
    {
        System.Console.WriteLine(i);
        System.Console.WriteLine(str);

        i = (int)i + 32;
        str = "Test2";
    }

    public static void Main()
    {
        object i = 8;
        string str = "Test1";

        System.Console.WriteLine(i);
        System.Console.WriteLine(str);
        Method1(ref i, ref str);
        System.Console.WriteLine(i);
        System.Console.WriteLine(str);
    }
}