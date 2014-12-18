//Structs work but there is an issue with structs in arrays
//Also Array code is too verbose and alot of unneccesary code is generated, can we partially decompile such
//code using some kind of instrinsic probably from ILSPY ?

public static class Program
{
    public class Test
    {
        public int A;
        public Test B;
    }

    static int indent = 0;
    public static void PrintStruct(Test aTest)
    {
        indent += 8;

        System.Console.WriteLine(aTest.A);

        for (int i = 0; i < indent; i++)
            System.Console.Write(" ");

        if (aTest.B != null)
            PrintStruct(aTest.B);


    }
    static int GetNumber()
    {
        return 10;
    }
    public static void Main()
    {
        var struct3 = new Test();
        struct3.A = 32;

        PrintStruct(new Test()
        {
            A = new Test() { A = new Test() { A = 786 }.A }.A,
            B = new Test()
            {
                A = 67,
                B = new Test() { A = 98, B = new Test() { A = GetNumber() } }
            }
        });

        System.Console.WriteLine("\n");
        var struct1 = new Test { A = 132 };

        var struct2 = new Test { A = 116 };

        System.Console.WriteLine(struct1.A);
        System.Console.WriteLine(struct2.A);


        var testArray = new Test[]
        {
            new Test { A = 325 },
            new Test { A = 165 },
        };

        System.Console.WriteLine(testArray[1].A);

        var test = testArray[0];
        System.Console.WriteLine(test.A);
    }
}