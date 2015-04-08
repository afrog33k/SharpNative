module System.__YieldAsyncSupport;

import core.thread;
import System.Collections.Generic.Namespace;
import System.Collections.Namespace;
class __IteratorBlock(TSource) : Fiber,  
	IEnumerable__G!(TSource),
	IEnumerator__G!(TSource) 
{
	bool started = false;
	void delegate(__IteratorBlock!TSource) func;

	this(void function(__IteratorBlock!TSource) func)
	{
		//_threadId = Thread.getThis();
		aborted = false;
		this.func = __ToDelegate(func);
		super(&run);
	}

	this(void delegate(__IteratorBlock!TSource) func)
	{
		//_threadId = Thread.getThis();
		aborted = false;
		this.func = func;
		super(&run);
	}


	private void run()
	{
		//Console.WriteLine("starting ...");
		try {
			func(this);
		}catch(Exception ex)
		{
		}
	}
	private void ensureStarted()
	{
		if(!started)
		{

			call();
			started = true;
		}
	}
	// Member 'front' must be a function due to DMD Issue #5403
	private TSource _front;
	@property TSource front()
	{

		ensureStarted();
		return _front;
	}
	void popFront()
	{
		if(aborted)
			return;

		ensureStarted();
		if (state == Fiber.State.HOLD)
			call();
	}
	@property bool empty()
	{
		if(aborted)
			return true;

		ensureStarted();
		return state == Fiber.State.TERM;
	}


	__IteratorBlock!(TSource) clone() 
	{
		return new __IteratorBlock!(TSource)(func);
	}


	public IEnumerator__G!(TSource) GetEnumerator(IEnumerable__G!(TSource) k = null)
	{

		/*	if (Thread.getThis() == _threadId && ! _enumeratorCreated) 
		{
		_enumeratorCreated = true;
		return  cast(IEnumerator__G!(TSource))(this);
		}*/

		__IteratorBlock!(TSource) cloned = clone();
		return cloned;
		//		return this;
	}

	public IEnumerator GetEnumerator(IEnumerable j =null)
	{
		return cast(IEnumerator)GetEnumerator(cast(IEnumerable__G!(TSource))null);
	}



	public NObject  Current(IEnumerator j=null) @property
	{
		return BOX(Current(cast(IEnumerator__G!(TSource))null));
	}

	// IEnumerator
	public void Reset(IEnumerator j=null) {
		throw  new InvalidOperationException();
  	}

	public bool MoveNext(IEnumerator j=null)
	{
		//Console.WriteLine("MoveNext");
		if(!started)
		{
			ensureStarted();
			//Console.WriteLine("ensureStarted");
			static if(is(T==class) || is(T==interface))
			{
				if(front!=null)
					return true;
			}
			else
			{
				return true;
			}
		}
		else
		{
			//Console.WriteLine("aborted||state == Fiber.State.TERM");
			if(aborted||state == Fiber.State.TERM)
				return false;

			popFront();
			if(!empty)

				return true;
			else
				return false;
		}

		return false;
	}
	//	
	public TSource  Current(IEnumerator__G!(TSource) k = null)   @property
	{
		return front();
	}

	bool aborted = false;
	void yield(TSource elem)
	{
		_front = elem;
		Fiber.yield();
	}

	void yieldReturn(TSource returnValue) 
	{
		//	Console.Write("yielding ");
		//	Console.WriteLine(returnValue);
		_front = returnValue;
		try
		{
			Fiber.yield();
		}catch(Exception ex)
		{
			Console.WriteLine(ex);
		}
	}

	void yieldBreak() 
	{
		aborted = true;
		Fiber.yield();
	}

	void Dispose(IDisposable j = null)
	{

	}
}