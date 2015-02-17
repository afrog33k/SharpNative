using System;
using System.Runtime.InteropServices;

//Need to Add and fix StructLayout is concerned
struct SimpleStruct2
{
	int a,b,c;
	double r;
	float t;
	ulong x;
}

struct SimpleStruct
{
	SimpleStruct2 B;
	int a;
	//string b; //This would make this struct managed ... :P
	long c;
	byte f;
}
class MainClass
    {
        // unsafe not required for primitive types 
       unsafe static void Main()
        {
            Console.WriteLine("The size of short is {0}.", sizeof(short));
            Console.WriteLine("The size of int is {0}.", sizeof(int));
            Console.WriteLine("The size of long is {0}.", sizeof(long));
            Console.WriteLine("The size of SimpleStruct is {0}.", sizeof(SimpleStruct));
            
        }
    }