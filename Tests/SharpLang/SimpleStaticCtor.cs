public static class Program
{
    public class Test1
    {

        public static string Message = "yo";

        static Test1()
        {
            System.Console.WriteLine("Test1: Static ctor");
        }

        public static void Method1()
        {
            System.Console.WriteLine("Test1: Static method");
        }
    }

    public class Test2
    {
        static Test2()
        {

            System.Console.WriteLine("Test2: Static ctor");
        }
    }
    
    public static void Main()
    {
        //Force static initialization order
        var test1 = new Test1();
        var test2 = new Test2();
        Test1.Method1();

          //C# and D have different order of static initialization ... D initializes all statics at once, C# does on demand (Mono)
       // Test1.Method1();
        //var test2 = new Test2();

    }
}