using System;
interface SS
{
	int X
    {
        get; 
        
        set;
    }
     void DisplayX();

}

struct SimpleStruct:SS
{
    private int xval;
    public int X
    {
        get 
        {
            return xval;
        }
        set 
        {
            if (value < 100)
                xval = value;
        }
    }
    public void DisplayX()
    {
        Console.WriteLine("The stored value is: {0}", xval);
    }
}

class SimpleClass:SS
{
    private int xval;
    public int X
    {
        get 
        { 
            return xval;
        }
        set 
        {
            if (value < 100)
                xval = value;
        }
    }
    public void DisplayX()
    {
        Console.WriteLine("The stored value is: {0}", xval);
    }
}

class TestClass
{
    public static void Main()
    {
        SimpleStruct ss = new SimpleStruct();
        ss.X = 5;
        ss.DisplayX();

        SS si = (SS) ss;
        si.X = 90;
        si.DisplayX();
        
        ss =  (SimpleStruct) si ;
        ss.X = 5;
        ss.DisplayX();
        
        SimpleClass ss1 = new SimpleClass();
        ss1.X = 5;
        ss1.DisplayX();
        
        try {
        ss1 =  (SimpleClass) si ;
        ss1.X = 5;
        ss1.DisplayX();
        } 
        catch(Exception)
        {
        	Console.WriteLine("Failed to cast");
        }
        

    }
}