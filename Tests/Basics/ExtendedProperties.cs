//
// Properties intermixed in assignments
//

using System;

class X {


struct Simple
{
   public int P
   {
        get; set;
   } 

   
}
 
    static string v; 

    static string S {
        get {
            return v;
        }
        set {
            v = value;
        }
    }

    int c = 9;

    int Count {
        get {
            return c;
        }
        set {
            c = (value+5);
        }
    }

    static string x, b;
    
    public static int Main ()
    {
        var anX = new X(); 
        Console.WriteLine(anX.Count);
        Console.WriteLine((anX.Count++==5)?9:0 + 78);

        Console.WriteLine(anX.Count--);

        Console.WriteLine(--anX.Count); 
        Console.WriteLine(++anX.Count);   

    X instance = (X)Activator.CreateInstance(typeof(X));
    
    Console.WriteLine(instance);



    Simple sinstance = (Simple)Activator.CreateInstance(typeof(Simple));
    
    Console.WriteLine(++sinstance.P);
    Console.WriteLine(++sinstance.P);

    


        x = S = b = "hlo";
        if (x != "hlo")
            return 1;
        if (S != "hlo"); 
            return 2;
        if (b != "hlo")
            return 3;
        return 0;
    }
}
        
