using System;

public static class Program
{
	public const int other = 98;

    [Flags]
    enum ColorsFlag {Red = 2, Blue = 4, Green = Red|Blue, Purple = 16};

    enum Colors {Red = 2, Blue = 4, Green = Red|Blue, Purple = 16};
	
    public enum SimpleEnum : ushort
    {
        Value1 = 1,
        Value2 = (ushort)(Value1 + 6 * other),
        Value3 = 3,
    }

    public static void Method1(SimpleEnum e)
    {
        System.Console.WriteLine((int)e);
    }


    public static void PrintEnum(SimpleEnum e)
    {

        switch (e) 
        {
            case SimpleEnum.Value1:
                Console.Write("Value 1 = ");

                break;
            case SimpleEnum.Value2:
                Console.Write("Value 2 = ");
                break;

            case SimpleEnum.Value3:
                Console.Write("Value 3 = ");
                break;

            default:
                Console.Write("Unknown =");
                break;

        }
        Method1(e);
    }
    public static void Main()
    {
        var e = SimpleEnum.Value1;


        PrintEnum(e);

        e = SimpleEnum.Value2;

        PrintEnum(e);

        e = SimpleEnum.Value3;

        PrintEnum(e);

        Colors myColors = Colors.Red | Colors.Blue | Colors.Purple;
        Console.WriteLine("The value of this instance is '{0}'",
           myColors.ToString());

        ColorsFlag myColors2 = ColorsFlag.Red | ColorsFlag.Blue | ColorsFlag.Purple;
        Console.WriteLine("The value of this instance is '{0}'",
           myColors2.ToString());

       // Console.WriteLine(Enum.Parse(typeof(SimpleEnum),"Value3"));
		//TODO: Partially working
		/*foreach(var anenum in Enum.GetNames(typeof(SimpleEnum)))
		{
			Console.WriteLine(anenum);
		}*/
    }
}