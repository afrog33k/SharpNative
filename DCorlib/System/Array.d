module System.Array;
import System.Namespace;

class Array : NObject
{
	public	int Length() @property
	{
		return -1;
	}

	int Rank() @property
	{
		return -1;
	}

	int GetLength(int dimension=0)
	{

		return -1;

	}

	int GetUpperBound(int dimension=0)
	{

	return -1;

	}


	int GetLowerBound(int dimension=0)
	{
	return -1;

	}

	public static void Reverse(U)(Array_T!(U) array)
	{
		array.Reverse();
	}
}

