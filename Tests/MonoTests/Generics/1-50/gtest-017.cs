// Compiler options: -r:gtest-017-lib.dll

public class X
{
	public static void Foo (Stack stack)
	{
		stack.Hello<string> ("Hello World");
	}

	public static void Main ()
	{
		Stack stack = new Stack ();
		Foo (stack);
	}
}

// Compiler options: -t:library

public class Stack
{
	public Stack ()
	{ }

	public void Hello<T> (T t)
	{ }
}
