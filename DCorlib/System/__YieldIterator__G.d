module System.__YieldIterator__G;


import System.Namespace;
import System.Collections.Generic.Namespace;
import System.Collections.Namespace;

class __YieldIterator__G(T) : NObject ,IEnumerable__G!(T) ,IEnumerator__G!(T)
{

    private T __prop_Current;
    public T Current(IEnumerator__G!(T) __ig=null) { return __prop_Current;}
    public  T Current(T value,IEnumerator__G!(T) __ig=null) {__prop_Current = value;return value;}

    public abstract IEnumerator__G!(T) GetEnumerator(IEnumerable__G!(T) __j = null);

    public abstract bool MoveNext(System.Collections.Namespace.IEnumerator __j = null);

    System.Collections.Namespace.IEnumerator GetEnumerator(System.Collections.Namespace.IEnumerable __j = null)
    {
		return cast(System.Collections.Namespace.IEnumerator) GetEnumerator();
    }

    public void Dispose(IDisposable __j = null)
    {
    }

    NObject Current(System.Collections.Namespace.IEnumerator __ig=null) 
    {
		{
			return cast(NObject) BOX!(T)(Current(cast(IEnumerator__G!(T)) null));
        }

    }


    public void Reset(System.Collections.Namespace.IEnumerator __j = null)
    {
		throw  new NotImplementedException();
    }

	/*public override String ToString()
	{
		return GetType().FullName;
	}

	public override Type GetType()
	{
		return __TypeOf!(typeof(this));
	}*/
}