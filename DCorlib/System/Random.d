module System.Random;

class Random
{

	import std.random;
	std.random.Random random;

	this() 
	{
		this(0);
	}

	this(int seed) //Need proper implementation
	{
		//		random = new std.random.Random();
	}

	public	int Next()
	{
		return uniform(cast(int)0, int.max, random);
	}

	public	double NextDouble()
	{
		return uniform(cast(double)0, cast(double)int.max,random);
	}
}