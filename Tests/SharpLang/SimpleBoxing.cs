public static class Program
{
    public struct Test
    {
        public string A;
        public int B;
    }
    
    public static void Main()
    {
        Test i = new Test();
        i.A = "abcd";
        i.B = 32;
        object o = i;
        Test j = (Test)o;

        System.Console.WriteLine(j.A);
        System.Console.WriteLine(j.B);

        int i2 = 48;
        System.Console.WriteLine((int)(object)i2);
        
        try 
        {
        	System.Console.WriteLine((long)(object)i2);
        }catch(System.InvalidCastException ex)
        {
        	System.Console.WriteLine("invalidcast"); 
        }
    }
}