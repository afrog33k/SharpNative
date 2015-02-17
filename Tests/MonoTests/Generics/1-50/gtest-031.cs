// Compiler options: -r:gtest-031-lib.dll

public class X
{
	public static void Test (Bar<int,string> bar)
	{
		bar.Hello ("Test");
		bar.Test (7, "Hello");
	}

	public static void Main ()
	{ }
}

// Compiler options: -t:library

public class Foo<T>
{
	public void Hello (T t)
	{ }
}

public class Bar<T,U> : Foo<U>
{
	public void Test (T t, U u)
	{ }
}
