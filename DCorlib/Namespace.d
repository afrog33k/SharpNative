module System.Threading.Namespace;

import System.Namespace;

alias __Delegate!(void delegate()) ThreadStart;


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