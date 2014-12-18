using System;

class Primes
{
  
  public override string  ToString()
    {
      return "I'm a little Prime Calc" + 45 + "Days of Pain";
    }
static int _internal = 8;
static int Integer
  {
  
  get{
    return _internal;
  }
  
  set{
    _internal = value;
  }

  }

 public static void Main()
    {
        Console.WriteLine("Prime numbers: ".Length + "hetto" + 45 + new Primes()); 
      
        
        object intBoxed = 9;
        
        Primes.Integer++;
        Console.WriteLine(Primes.Integer);
        if(Primes.Integer++==9)
          {
          Console.WriteLine(Primes.Integer);
          }
        
        if((int)intBoxed == 9)
          {
          Console.WriteLine("boxed a 9");
          }

    }

 
}