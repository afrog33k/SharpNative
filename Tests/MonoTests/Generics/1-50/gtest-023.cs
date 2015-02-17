using System;

class Foo<T>
{
	public void Hello ()
	{ 
		Console.WriteLine(this.GetType() + "...Hello");
	}

	public void World (T t)
	{
		Console.WriteLine(this.GetType() + "...World");
		Hello ();
	}
}

//
// This is some kind of a `recursive' declaration:
//
// Note that we're using the class we're currently defining (Bar)
// as argument of its parent.
//
// Is is important to run the resulting executable since this is
// both a test for the compiler and the runtime.
//

class Bar : Foo<Bar>
{
	public void Test () 
	{
		Hello ();
		World (this);
	}
}

class X
{
	public static void Main ()
	{ 
		var aBar = new Bar();
		aBar.Test();
	}
}
