using System;

namespace Blargh
{
    public static class Utilities
    {
        const int Four = Three + 1;
        const int Three = Two + One;
        const int Two = One + 1;
        const int One = 1;

	public static void Main()
	{
		Console.WriteLine(Four);
	}
    }
}