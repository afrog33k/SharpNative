using System;

class StringFormatTest01
{

 

   
    public static void Main()
    {
 		var floatVal = 1.023f;
 		var doubleVal = 1.24234;
        // Console.WriteLine(String.Format("P0: = {0}, P1:= {1}", 1.023f, 1.2423423423423)); // need to fix number formatting in D to follow .net
     	Console.WriteLine(String.Format("P0: = {0}, P1:= {1}", floatVal+1, doubleVal+2));   
    }
}