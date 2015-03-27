using System;
using System.Collections.Generic;

class ListTest
{
	static void Main()
	{
		Console.WriteLine("List Test ...");
		List<int> aList = new List<int>();
		int sum = 0;
		
		for(int i=0; i< 10000000;i++)
		{
			aList.Add(1+i);
			aList.Add(2+(i-2));
			aList.Add(3+(i+4));
		}

		foreach(var integer in aList)
		{
			sum += integer;
		}
		
		Console.WriteLine("Sum is: " + sum);
	}
}