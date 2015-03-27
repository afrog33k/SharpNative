module System.Threading.Namespace;

import System.Namespace;

alias __Delegate!(void delegate()) ThreadStart;

alias __Delegate!(void delegate(NObject state)) WaitCallback;

class __KillableThread : core.thread.Thread {
	this(void function() fn) {
		m_fn = __ToDelegate(fn);
		super(&run);
	}

	this(void delegate() fn) {
		m_fn = fn;
		super(&run);
	}

	public void abort() 
	{
		version(POSIX)
		{
		pthread_cancel(m_thisThread);
		}
	}

	private void run() {
		version(POSIX)
		{
		m_thisThread = pthread_self();
		}
		m_fn();
	}

	version(POSIX)
	{
		private pthread_t m_thisThread;
	}

	private void delegate() m_fn;
}

class Thread : NObject
{
	//actual thread
	__KillableThread __athread;

	this(ThreadStart aThreadStarter)
	{
		__athread =  new __KillableThread(aThreadStarter.Function);
	}

	void Start()
	{
		__athread.start();
	}

	void Join()
	{
		__athread.join();
	}

	void Abort()
	{
		//doesnt exist in D
		__athread.abort();
	}

	static void Sleep(int ms)
	{
		import core.time;
		core.thread.Thread.sleep(dur!("msecs")( ms ));
	}
}

//Temporary based on https://github.com/rumbu13/sharp
private import
    core.atomic;

struct Interlocked
{
    @disable this();
    alias MemoryBarrier = atomicFence;

    static int Exchange(ref shared int location, int value)
    {
        atomicStore(location, value);
        return atomicLoad(location);
    }

    static long Exchange(ref shared long location, long value)
    {
        atomicStore(location, value);
        return atomicLoad(location);
    }

    static float Exchange(ref shared float location, float value)
    {
        atomicStore(location, value);
        return atomicLoad(location);
    }

    static double Exchange(ref shared double location, double value)
    {
        atomicStore(location, value);
        return atomicLoad(location);
    }

    static void* Exchange(ref shared void* location, void* value)
    {
        shared size_t loc = cast(shared size_t)location;
        atomicStore(loc, cast(size_t)value);
        location = cast(shared void*)atomicLoad(loc);
        return cast(void*)atomicLoad(location);
    }

    static T Exchange(T)(ref shared T location, T value) if (is(T == class))
    {
        shared size_t loc = cast(shared size_t)cast(shared void*)location;
        atomicStore(loc, cast(size_t)cast(void*)value);
        location = cast(shared T)cast(shared void*)atomicLoad(loc);
        return cast(T)atomicLoad(location);
    }

    static int CompareExchange(ref shared int location, int value, int comparand)
    {
        cas(&location, comparand, value);
        return location;
    }

    static long CompareExchange(ref shared long location, long value, long comparand)
    {
        cas(&location, comparand, value);
        return location;
    }

    static double CompareExchange(ref shared double location, double value, double comparand)
    {
        cas(&location, comparand, value);
        return location;
    }

    static float CompareExchange(ref shared float location, float value, float comparand)
    {
        cas(&location, comparand, value);
        return location;
    }

    static void* CompareExchange(ref shared void* location, shared void* value, shared void* comparand)
    {
        cas(&location, comparand, value);
        return cast(void*)location;
    }

    static T CompareExchange(T)(ref  T location,  T value,  T comparand) if (is(T == class))
    {
        cas(cast(shared(T)*)&location, cast(shared(T))comparand, cast(shared(T))value);
        return cast(T)(location);
    }

    static int Add(ref shared int location, int value)
    {
        return atomicOp!("+=")(location, value);
    }

    static long Add(ref shared long location, int value)
    {
        return atomicOp!("+=")(location, value);
    }

    static int Increment(ref shared int location)
    {
        return atomicOp!("+=")(location, 1);
    }

    static int Decrement(ref shared int location)
    {
        return atomicOp!("-=")(location, 1);
    }

    static long Increment(ref shared long location)
    {
        return atomicOp!("+=")(location, 1L);
    }

    static long Decrement(ref shared long location)
    {
        return atomicOp!("-=")(location, 1L);
    }

    alias Read = atomicLoad!(MemoryOrder.seq, long);
}