using System;

class BoxingTest01
{

    public override string ToString()
    {
        return "BoxingTest01";
    }

    private static void Log(object data)
    {

        //This doesnt (Need proper isinst instruction)
        if (data is int)
        {
            var a = (int)data;
            Console.WriteLine("Integer:" + a.ToString());
        }

        else
            if (data is string)
            {
                var a = (string)data;
                Console.WriteLine("String:" + a.ToString());
            }
            else
                if (data is float)
                {
                    var a = (float)data;
                    Console.WriteLine("Float:" + a.ToString());
                }
                else
                    if (data is double)
                    {
                        var a = (double)data;
                        Console.WriteLine("Double:" + a.ToString());
                    }
                    else
                    {
                        
                        Console.WriteLine("unknown:" + data.ToString());
                    }

    }

    public static void Main()
    {
        int n = 500000;
        Log(n);
        Log("Hey");
        Log(1.023f);
        Log(1.53422);
        Log(new BoxingTest01() { });
        Console.WriteLine(1.023.ToString());
        Console.WriteLine(String.Format("P0: = {0}, P1:= {1}", 1.023f, 1.24234)); // TODO: fix number display 1.2423423423423
    }
}