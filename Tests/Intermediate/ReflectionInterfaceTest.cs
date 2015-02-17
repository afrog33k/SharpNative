// ==++==
//
//   
//    Copyright (c) 2006 Microsoft Corporation.  All rights reserved.
//   
//    The use and distribution terms for this software are contained in the file
//    named license.txt, which can be found in the root of this distribution.
//    By using this software in any fashion, you are agreeing to be bound by the
//    terms of this license.
//   
//    You must not remove this notice, or any other, from this software.
//   
//
// ==--==
//Modified

using System;
using System.Reflection;

public interface IFoo
{
	
} 

public interface XFoo
{
	
} 

public class FooClass : IFoo
{

}

public struct FooStruct : IFoo,XFoo
{

}

public class GenClass<T> where T : IFoo
{
	public static IFoo ConvertToConstraint(T t)
	{
        	return t;
	}
}

public struct GenStruct<T> where T : IFoo
{
	public static IFoo ConvertToConstraint(T t)
	{
        	return t;
    	}
}
public class Test
{


 public static bool MyInterfaceFilter(Type typeObj,Object criteriaObj)
    {
        if(typeObj.ToString() == criteriaObj.ToString())
            return true;
        else 
            return false;
    }

	public static int counter = 0;
	public static bool result = true;
	public static void Eval(bool exp)
	{
		counter++;
		if (!exp)
		{
			result = exp;
			Console.WriteLine("Test Failed at location: " + counter);
		}
	
	}
	
	public static int Main()
	{
		Eval(GenClass<FooClass>.ConvertToConstraint(new FooClass()).GetType().Equals(typeof(FooClass)));
		Eval(GenClass<FooStruct>.ConvertToConstraint(new FooStruct()).GetType().Equals(typeof(FooStruct)));

		Eval(GenStruct<FooClass>.ConvertToConstraint(new FooClass()).GetType().Equals(typeof(FooClass)));
		Eval(GenStruct<FooStruct>.ConvertToConstraint(new FooStruct()).GetType().Equals(typeof(FooStruct)));

TypeFilter myFilter = new TypeFilter(MyInterfaceFilter); 
 
		Console.WriteLine(typeof(FooClass).FindInterfaces(myFilter,typeof(XFoo))); 
foreach(var iface in typeof(FooStruct).GetInterfaces())
{
	Console.WriteLine(iface);
}
Console.WriteLine(typeof(FooStruct).GetInterfaces());   
Console.WriteLine(typeof(FooStruct).IsInterface); 
Console.WriteLine(typeof(XFoo).IsInterface);  
		if (result)
		{
			Console.WriteLine("Test Passed");
			return 0; 
		}
		else
		{
			Console.WriteLine("Test Failed");
			return 1;
		}
	}
		
}

