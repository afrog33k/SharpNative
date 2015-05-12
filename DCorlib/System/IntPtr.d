module System.IntPtr;

import System.Namespace;

class IntPtr:Boxed!(int*)
{
	int* m_value;
	public __gshared const  IntPtr Zero = new IntPtr(0);

	this(long pointer)
	{

		m_value = cast(int*)pointer;
	}

	static IntPtr op_Explicit(long pointer)
	{

		return new IntPtr(pointer);
	}



	/*class __Boxed_:Boxed!(IntPtr)
	{
	this(ref IntPtr ptr)
	{
	super(ptr);
	}
	}*/

}
