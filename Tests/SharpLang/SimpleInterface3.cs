public static class Program
{
    public interface ITest<T>
    {
        T A();
        
        string Name{get;set;}
        string this[int index]
    	{
        	get;
        	set;
    	}
        
        void SayIt();
    }

    public class Test1 : ITest<string>, ITest<int>, ITest<long>
    {
    	public string Name{get;set;}
    	
    	public void SayIt()
    		{
    		}
    	void ITest<long>.SayIt()
    		{
    			System.Console.WriteLine("hello");
    		}
      public  string A()
        {
            return "Test4";
        }
        
        int ITest<int>.A()
        {
            return 89;
        }

 		long ITest<long>.A()
        {
            return 89;
        }        
       string ITest<int>.this[int index]
        	{
        		get
        		{
					return index.ToString();
        		}
      			set
      			{
      			}
        	}

      public string this[int index]
        	{
        		get
        		{
					return (index+8).ToString();
        		}
      			set
      			{
      			}
        	}
    }

    public static void Main()
    {
        ITest<string> test1 = new Test1();

        System.Console.WriteLine(test1.A());
        
        System.Console.WriteLine(test1[89]);
        
        ITest<int> test2 = new Test1();
         
		System.Console.WriteLine(test2.A());
		 System.Console.WriteLine(test2[89]);
    }
}