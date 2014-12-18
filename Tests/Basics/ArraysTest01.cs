public static class Program
{

    public static void modifyArray(string[] anArray)
    {
        anArray[0] = "good";
    }

    public static void modifyArray2(double[] anArray)
    {
        anArray[2] = 100.7;
    }

    public static void Main()
    {
        var testArray = new[] { "a", "b" };

        System.Console.WriteLine(testArray.Length);
        System.Console.WriteLine(testArray[0]);
        System.Console.WriteLine(testArray[1]);

        testArray[1] = "c";
        System.Console.WriteLine(testArray[1]);

        modifyArray(testArray);
        System.Console.WriteLine(testArray[0]);

        var testArray2 = new[] { 1.002, 15.6, 560 };
        System.Console.WriteLine(testArray2.Length);
        testArray2[1] = 89.5;
        System.Console.WriteLine(testArray2[1]);

        modifyArray2(testArray2);
        System.Console.WriteLine(testArray2[2]);

    }
}