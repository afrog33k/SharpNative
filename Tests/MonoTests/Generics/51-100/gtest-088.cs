using System;

public struct KeyValuePair<K,V>
{
	public KeyValuePair (K k, V v)
	{ }

	public KeyValuePair (K k)
	{ }
}

class X
{
	public static void Main ()
	{
		// new KeyValuePair<int,long> (); cannot run
		var s =new KeyValuePair<int,long> ();
	}
}
