module System.NObject;
import System.Namespace;
import System.Reflection.Namespace;

public class NObject //: ICloneable //Inheriting from ICloneable gives symbol multiply defined!
 {


	/*
    public static int[] __id = [1]; // For now no interfaces

    public int[] __get_id()
    {
        return __id;
    }

    public static bool __implements(int interfaceid=0)()
    {
        Console.WriteLine("in NObject impl");
        //if(interfaceid==0)
        //    return true;
        return __implements_impl(interfaceid);
    }

    public static bool __implements_impl(int interfaceid)
    {
        return false;
    }*/

    public override string  toString() // Needed to  use built-in functions
    {
        return std.conv.to!string(ToString().Text);
    }

	public String ToString()
	{
		return GetType().FullName;
	}

	public Type GetType()
	{
		return __TypeOf!(typeof(this));

	}

//	public Type opCast()
//	{
//		return GetType();
//	}

	public int GetHashCode()
	{//TODO... put a real hash code in here
		return cast(int)this.toHash;
	}

  

	public bool Equals(NObject obj) 
	{
        //return false;
        //throw "error";
		return obj == this;
	}

    public bool Equals(int x, int y) 
    {
        return (x==y);
    }

    //public  override bool opEquals(Object rhs)
    //{
    //    Console.WriteLine("tramp");
    //    return this.Equals((cast(NObject)rhs));
    //}
//	extern (C) Object _d_newclass (ClassInfo info);

/*    template shallow_copy (T : Object)
    {
        T shallow_copy (T value)
        {
            if (value is null)
                return null;

            void *copy = _d_newclass (value.classinfo);
            size_t size = value.classinfo.init.length;

            copy [0 .. size] = (cast (void *) value) [0 .. size];
            return cast (T) copy;
        }
    }*/

    /** Clobbers an object with the contents of another object of the same
*  class. Clobbering an arbitrary object is very risky so only use the
*  function inside of a dup() method in a class after creating the
*  target duplicate. For example,
*     class Foo {
*       int test;
*       int some_other_state;
*       this(int t) { test = t; }
*       Foo dup() {
*         Foo res = new Foo(test);
*         clobber(res, this);
*         return res;
*       }
*     }
*     ...
*     Foo x = new Foo(10);
*     ...
*     Foo y = x.dup;
*/
void clobber(NObject dest, NObject source) {
 ClassInfo ci = source.classinfo;
 if (ci !is dest.classinfo)
   throw new Exception("Cannot clobber subclasses or superclasses");
 void* s = cast(void*)source;
 void* d = cast(void*)dest;
 size_t start = NObject.classinfo.init.length;
 d[start .. ci.init.length] = s[start .. ci.init.length];
}

    public NObject ICloneable_Clone()
    {
    	//if (this is null)
     //           return null;

     //       void *copy = cast(void*) new (this.classinfo);
     //       size_t size = this.classinfo.init.length;

     //       copy [0 .. size] = (cast (void *) this) [0 .. size];
       
 		NObject copy = new NObject();
        clobber(copy, this);
       return cast (NObject) copy;
    }


    //String opBinary(string op)(String rhs)
    //{
    //    static if (op == "+") 
    //        return  new String(ToString().text ~ rhs.text);    
      
    //}
	
}