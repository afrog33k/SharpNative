module System.Threading.Namespace;

import System.Namespace;

alias __Delegate!(void delegate()) ThreadStart;

alias __Delegate!(void delegate(NObject state)) WaitCallback;

///////////////////////////////////////////////////////////////////////////////
// Thread
///////////////////////////////////////////////////////////////////////////////


/**
* This class encapsulates all threading functionality for the D
* programming language.  As thread manipulation is a required facility
* for garbage collection, all user threads should derive from this
* class, and instances of this class should never be explicitly deleted.
* A new thread may be created using either derivation or composition, as
* in the following example.
*/
//class __Thread
//{
//    public import core.time; // for Duration
//    import core.exception : onOutOfMemoryError;
//    import core.thread: Mutex, ThreadException;
//    import core.sys.windows.windows;
//    ///////////////////////////////////////////////////////////////////////////
//    // Initialization
//    ///////////////////////////////////////////////////////////////////////////
//
//
//    /**
//    * Initializes a thread object which is associated with a static
//    * D function.
//    *
//    * Params:
//    *  fn = The thread function.
//    *  sz = The stack size for this thread.
//    *
//    * In:
//    *  fn must not be null.
//    */
//    this( void function() fn, size_t sz = 0 )
//    in
//    {
//        assert( fn );
//    }
//    body
//    {
//        this(sz);
//        m_fn   = fn;
//        m_call = Call.FN;
//        m_curr = &m_main;
//    }
//
//    public void abort() 
//    {
//        version(Windows)
//        {
//            ThreadExit(m_hndl);
//        }
//
//    }
//    /**
//    * Initializes a thread object which is associated with a dynamic
//    * D function.
//    *
//    * Params:
//    *  dg = The thread function.
//    *  sz = The stack size for this thread.
//    *
//    * In:
//    *  dg must not be null.
//    */
//    this( void delegate() dg, size_t sz = 0 )
//    in
//    {
//        assert( dg );
//    }
//    body
//    {
//        this(sz);
//        m_dg   = dg;
//        m_call = Call.DG;
//        m_curr = &m_main;
//    }
//
//
//    /**
//    * Cleans up any remaining resources used by this object.
//    */
//    ~this()
//    {
//        if( m_addr == m_addr.init )
//        {
//            return;
//        }
//
//        version( Windows )
//        {
//            m_addr = m_addr.init;
//            CloseHandle( m_hndl );
//            m_hndl = m_hndl.init;
//        }
//        else version( Posix )
//        {
//            pthread_detach( m_addr );
//            m_addr = m_addr.init;
//        }
//        version( OSX )
//        {
//            m_tmach = m_tmach.init;
//        }
//        rt_tlsgc_destroy( m_tlsgcdata );
//        m_tlsgcdata = null;
//    }
//
//
//    ///////////////////////////////////////////////////////////////////////////
//    // General Actions
//    ///////////////////////////////////////////////////////////////////////////
//
//
//    /**
//    * Starts the thread and invokes the function or delegate passed upon
//    * construction.
//    *
//    * In:
//    *  This routine may only be called once per thread instance.
//    *
//    * Throws:
//    *  ThreadException if the thread fails to start.
//    */
//    final Thread start() nothrow
//        in
//        {
//            assert( !next && !prev );
//        }
//    body
//    {
//        auto wasThreaded  = multiThreadedFlag;
//        multiThreadedFlag = true;
//        scope( failure )
//        {
//            if( !wasThreaded )
//                multiThreadedFlag = false;
//        }
//
//        version( Windows ) {} else
//            version( Posix )
//            {
//                pthread_attr_t  attr;
//
//                if( pthread_attr_init( &attr ) )
//                    onThreadError( "Error initializing thread attributes" );
//                if( m_sz && pthread_attr_setstacksize( &attr, m_sz ) )
//                    onThreadError( "Error initializing thread stack size" );
//            }
//
//        version( Windows )
//        {
//            // NOTE: If a thread is just executing DllMain()
//            //       while another thread is started here, it holds an OS internal
//            //       lock that serializes DllMain with CreateThread. As the code
//            //       might request a synchronization on slock (e.g. in thread_findByAddr()),
//            //       we cannot hold that lock while creating the thread without
//            //       creating a deadlock
//            //
//            // Solution: Create the thread in suspended state and then
//            //       add and resume it with slock acquired
//            assert(m_sz <= uint.max, "m_sz must be less than or equal to uint.max");
//            m_hndl = cast(HANDLE) _beginthreadex( null, cast(uint) m_sz, &thread_entryPoint, cast(void*) this, CREATE_SUSPENDED, &m_addr );
//            if( cast(size_t) m_hndl == 0 )
//                onThreadError( "Error creating thread" );
//        }
//
//        // NOTE: The starting thread must be added to the global thread list
//        //       here rather than within thread_entryPoint to prevent a race
//        //       with the main thread, which could finish and terminat the
//        //       app without ever knowing that it should have waited for this
//        //       starting thread.  In effect, not doing the add here risks
//        //       having thread being treated like a daemon thread.
//        slock.lock_nothrow();
//        scope(exit) slock.unlock_nothrow();
//        {
//            version( Windows )
//            {
//                if( ResumeThread( m_hndl ) == -1 )
//                    onThreadError( "Error resuming thread" );
//            }
//            else version( Posix )
//            {
//                // NOTE: This is also set to true by thread_entryPoint, but set it
//                //       here as well so the calling thread will see the isRunning
//                //       state immediately.
//                atomicStore!(MemoryOrder.raw)(m_isRunning, true);
//                scope( failure ) atomicStore!(MemoryOrder.raw)(m_isRunning, false);
//
//                version (Shared)
//                {
//                    import rt.sections;
//                    auto libs = pinLoadedLibraries();
//                    auto ps = cast(void**).malloc(2 * size_t.sizeof);
//                    if (ps is null) onOutOfMemoryError();
//                    ps[0] = cast(void*)this;
//                    ps[1] = cast(void*)libs;
//                    if( pthread_create( &m_addr, &attr, &thread_entryPoint, ps ) != 0 )
//                    {
//                        unpinLoadedLibraries(libs);
//                        .free(ps);
//                        onThreadError( "Error creating thread" );
//                    }
//                }
//                else
//                {
//                    if( pthread_create( &m_addr, &attr, &thread_entryPoint, cast(void*) this ) != 0 )
//                        onThreadError( "Error creating thread" );
//                }
//            }
//            version( OSX )
//            {
//                m_tmach = pthread_mach_thread_np( m_addr );
//                if( m_tmach == m_tmach.init )
//                    onThreadError( "Error creating thread" );
//            }
//
//            // NOTE: when creating threads from inside a DLL, DllMain(THREAD_ATTACH)
//            //       might be called before ResumeThread returns, but the dll
//            //       helper functions need to know whether the thread is created
//            //       from the runtime itself or from another DLL or the application
//            //       to just attach to it
//            //       as a consequence, the new Thread object is added before actual
//            //       creation of the thread. There should be no problem with the GC
//            //       calling thread_suspendAll, because of the slock synchronization
//            //
//            // VERIFY: does this actually also apply to other platforms?
//            add( this );
//            return this;
//        }
//    }
//
//    /**
//    * Waits for this thread to complete.  If the thread terminated as the
//    * result of an unhandled exception, this exception will be rethrown.
//    *
//    * Params:
//    *  rethrow = Rethrow any unhandled exception which may have caused this
//    *            thread to terminate.
//    *
//    * Throws:
//    *  ThreadException if the operation fails.
//    *  Any exception not handled by the joined thread.
//    *
//    * Returns:
//    *  Any exception not handled by this thread if rethrow = false, null
//    *  otherwise.
//    */
//    final Throwable join( bool rethrow = true )
//    {
//        version( Windows )
//        {
//            if( WaitForSingleObject( m_hndl, INFINITE ) != WAIT_OBJECT_0 )
//                throw new ThreadException( "Unable to join thread" );
//            // NOTE: m_addr must be cleared before m_hndl is closed to avoid
//            //       a race condition with isRunning. The operation is done
//            //       with atomicStore to prevent compiler reordering.
//            atomicStore!(MemoryOrder.raw)(*cast(shared)&m_addr, m_addr.init);
//            CloseHandle( m_hndl );
//            m_hndl = m_hndl.init;
//        }
//        else version( Posix )
//        {
//            if( pthread_join( m_addr, null ) != 0 )
//                throw new ThreadException( "Unable to join thread" );
//            // NOTE: pthread_join acts as a substitute for pthread_detach,
//            //       which is normally called by the dtor.  Setting m_addr
//            //       to zero ensures that pthread_detach will not be called
//            //       on object destruction.
//            m_addr = m_addr.init;
//        }
//        if( m_unhandled )
//        {
//            if( rethrow )
//                throw m_unhandled;
//            return m_unhandled;
//        }
//        return null;
//    }
//
//
//    ///////////////////////////////////////////////////////////////////////////
//    // General Properties
//    ///////////////////////////////////////////////////////////////////////////
//
//
//    /**
//    * Gets the user-readable label for this thread.
//    *
//    * Returns:
//    *  The name of this thread.
//    */
//    final @property string name()
//    {
//        synchronized( this )
//        {
//            return m_name;
//        }
//    }
//
//
//    /**
//    * Sets the user-readable label for this thread.
//    *
//    * Params:
//    *  val = The new name of this thread.
//    */
//    final @property void name( string val )
//    {
//        synchronized( this )
//        {
//            m_name = val;
//        }
//    }
//
//
//    /**
//    * Gets the daemon status for this thread.  While the runtime will wait for
//    * all normal threads to complete before tearing down the process, daemon
//    * threads are effectively ignored and thus will not prevent the process
//    * from terminating.  In effect, daemon threads will be terminated
//    * automatically by the OS when the process exits.
//    *
//    * Returns:
//    *  true if this is a daemon thread.
//    */
//    final @property bool isDaemon()
//    {
//        synchronized( this )
//        {
//            return m_isDaemon;
//        }
//    }
//
//
//    /**
//    * Sets the daemon status for this thread.  While the runtime will wait for
//    * all normal threads to complete before tearing down the process, daemon
//    * threads are effectively ignored and thus will not prevent the process
//    * from terminating.  In effect, daemon threads will be terminated
//    * automatically by the OS when the process exits.
//    *
//    * Params:
//    *  val = The new daemon status for this thread.
//    */
//    final @property void isDaemon( bool val )
//    {
//        synchronized( this )
//        {
//            m_isDaemon = val;
//        }
//    }
//
//
//    /**
//    * Tests whether this thread is running.
//    *
//    * Returns:
//    *  true if the thread is running, false if not.
//    */
//    final @property bool isRunning() nothrow
//    {
//        if( m_addr == m_addr.init )
//        {
//            return false;
//        }
//
//        version( Windows )
//        {
//            uint ecode = 0;
//            GetExitCodeThread( m_hndl, &ecode );
//            return ecode == STILL_ACTIVE;
//        }
//        else version( Posix )
//        {
//            return atomicLoad(m_isRunning);
//        }
//    }
//
//
//    ///////////////////////////////////////////////////////////////////////////
//    // Thread Priority Actions
//    ///////////////////////////////////////////////////////////////////////////
//
//
//    /**
//    * The minimum scheduling priority that may be set for a thread.  On
//    * systems where multiple scheduling policies are defined, this value
//    * represents the minimum valid priority for the scheduling policy of
//    * the process.
//    */
//    __gshared const int PRIORITY_MIN;
//
//
//    /**
//    * The maximum scheduling priority that may be set for a thread.  On
//    * systems where multiple scheduling policies are defined, this value
//    * represents the maximum valid priority for the scheduling policy of
//    * the process.
//    */
//    __gshared const int PRIORITY_MAX;
//
//
//    /**
//    * The default scheduling priority that is set for a thread.  On
//    * systems where multiple scheduling policies are defined, this value
//    * represents the default priority for the scheduling policy of
//    * the process.
//    */
//    __gshared const int PRIORITY_DEFAULT;
//
//
//    /**
//    * Gets the scheduling priority for the associated thread.
//    *
//    * Note: Getting the priority of a thread that already terminated
//    * might return the default priority.
//    *
//    * Returns:
//    *  The scheduling priority of this thread.
//    */
//    final @property int priority()
//    {
//        version( Windows )
//        {
//            return GetThreadPriority( m_hndl );
//        }
//        else version( Posix )
//        {
//            int         policy;
//            sched_param param;
//
//            if (auto err = pthread_getschedparam(m_addr, &policy, &param))
//            {
//                // ignore error if thread is not running => Bugzilla 8960
//                if (!atomicLoad(m_isRunning)) return PRIORITY_DEFAULT;
//                throw new ThreadException("Unable to get thread priority");
//            }
//            return param.sched_priority;
//        }
//    }
//
//
//    /**
//    * Sets the scheduling priority for the associated thread.
//    *
//    * Note: Setting the priority of a thread that already terminated
//    * might have no effect.
//    *
//    * Params:
//    *  val = The new scheduling priority of this thread.
//    */
//    final @property void priority( int val )
//    in
//    {
//        assert(val >= PRIORITY_MIN);
//        assert(val <= PRIORITY_MAX);
//    }
//    body
//    {
//        version( Windows )
//        {
//            if( !SetThreadPriority( m_hndl, val ) )
//                throw new ThreadException( "Unable to set thread priority" );
//        }
//        else version( Solaris )
//        {
//            // the pthread_setschedprio(3c) and pthread_setschedparam functions
//            // are broken for the default (TS / time sharing) scheduling class.
//            // instead, we use priocntl(2) which gives us the desired behavior.
//
//            // We hardcode the min and max priorities to the current value
//            // so this is a no-op for RT threads.
//            if (m_isRTClass)
//                return;
//
//            pcparms_t   pcparm;
//
//            pcparm.pc_cid = PC_CLNULL;
//            if (priocntl(idtype_t.P_LWPID, P_MYID, PC_GETPARMS, &pcparm) == -1)
//                throw new ThreadException( "Unable to get scheduling class" );
//
//            pri_t* clparms = cast(pri_t*)&pcparm.pc_clparms;
//
//            // clparms is filled in by the PC_GETPARMS call, only necessary
//            // to adjust the element that contains the thread priority
//            clparms[1] = cast(pri_t) val;
//
//            if (priocntl(idtype_t.P_LWPID, P_MYID, PC_SETPARMS, &pcparm) == -1)
//                throw new ThreadException( "Unable to set scheduling class" );
//        }
//        else version( Posix )
//        {
//            static if(__traits(compiles, pthread_setschedprio))
//            {
//                if (auto err = pthread_setschedprio(m_addr, val))
//                {
//                    // ignore error if thread is not running => Bugzilla 8960
//                    if (!atomicLoad(m_isRunning)) return;
//                    throw new ThreadException("Unable to set thread priority");
//                }
//            }
//            else
//            {
//                // NOTE: pthread_setschedprio is not implemented on OSX or FreeBSD, so use
//                //       the more complicated get/set sequence below.
//                int         policy;
//                sched_param param;
//
//                if (auto err = pthread_getschedparam(m_addr, &policy, &param))
//                {
//                    // ignore error if thread is not running => Bugzilla 8960
//                    if (!atomicLoad(m_isRunning)) return;
//                    throw new ThreadException("Unable to set thread priority");
//                }
//                param.sched_priority = val;
//                if (auto err = pthread_setschedparam(m_addr, policy, &param))
//                {
//                    // ignore error if thread is not running => Bugzilla 8960
//                    if (!atomicLoad(m_isRunning)) return;
//                    throw new ThreadException("Unable to set thread priority");
//                }
//            }
//        }
//    }
//
//    ///////////////////////////////////////////////////////////////////////////
//    // Actions on Calling Thread
//    ///////////////////////////////////////////////////////////////////////////
//
//
//    /**
//    * Suspends the calling thread for at least the supplied period.  This may
//    * result in multiple OS calls if period is greater than the maximum sleep
//    * duration supported by the operating system.
//    *
//    * Params:
//    *  val = The minimum duration the calling thread should be suspended.
//    *
//    * In:
//    *  period must be non-negative.
//    *
//    * Example:
//    * ------------------------------------------------------------------------
//    *
//    * Thread.sleep( dur!("msecs")( 50 ) );  // sleep for 50 milliseconds
//    * Thread.sleep( dur!("seconds")( 5 ) ); // sleep for 5 seconds
//    *
//    * ------------------------------------------------------------------------
//    */
//    static void sleep( Duration val ) nothrow
//        in
//        {
//            assert( !val.isNegative );
//        }
//    body
//    {
//        version( Windows )
//        {
//            auto maxSleepMillis = dur!("msecs")( uint.max - 1 );
//
//            // avoid a non-zero time to be round down to 0
//            if( val > dur!"msecs"( 0 ) && val < dur!"msecs"( 1 ) )
//                val = dur!"msecs"( 1 );
//
//            // NOTE: In instances where all other threads in the process have a
//            //       lower priority than the current thread, the current thread
//            //       will not yield with a sleep time of zero.  However, unlike
//            //       yield(), the user is not asking for a yield to occur but
//            //       only for execution to suspend for the requested interval.
//            //       Therefore, expected performance may not be met if a yield
//            //       is forced upon the user.
//            while( val > maxSleepMillis )
//            {
//                Sleep( cast(uint)
//                       maxSleepMillis.total!"msecs" );
//                val -= maxSleepMillis;
//            }
//            Sleep( cast(uint) val.total!"msecs" );
//        }
//        else version( Posix )
//        {
//            timespec tin  = void;
//            timespec tout = void;
//
//            val.split!("seconds", "nsecs")(tin.tv_sec, tin.tv_nsec);
//            if( val.total!"seconds" > tin.tv_sec.max )
//                tin.tv_sec  = tin.tv_sec.max;
//            while( true )
//            {
//                if( !nanosleep( &tin, &tout ) )
//                    return;
//                if( errno != EINTR )
//                    throw new ThreadError( "Unable to sleep for the specified duration" );
//                tin = tout;
//            }
//        }
//    }
//
//
//    /**
//    * Forces a context switch to occur away from the calling thread.
//    */
//    static void yield() nothrow
//    {
//        version( Windows )
//            SwitchToThread();
//        else version( Posix )
//            sched_yield();
//    }
//
//
//    ///////////////////////////////////////////////////////////////////////////
//    // Thread Accessors
//    ///////////////////////////////////////////////////////////////////////////
//
//    /**
//    * Provides a reference to the calling thread.
//    *
//    * Returns:
//    *  The thread object representing the calling thread.  The result of
//    *  deleting this object is undefined.  If the current thread is not
//    *  attached to the runtime, a null reference is returned.
//    */
//    static Thread getThis() nothrow
//    {
//        // NOTE: This function may not be called until thread_init has
//        //       completed.  See thread_suspendAll for more information
//        //       on why this might occur.
//        version( OSX )
//        {
//            return sm_this;
//        }
//        else version( Posix )
//        {
//            auto t = cast(Thread) pthread_getspecific( sm_this );
//            return t;
//        }
//        else
//        {
//            return sm_this;
//        }
//    }
//
//
//    /**
//    * Provides a list of all threads currently being tracked by the system.
//    *
//    * Returns:
//    *  An array containing references to all threads currently being
//    *  tracked by the system.  The result of deleting any contained
//    *  objects is undefined.
//    */
//    static Thread[] getAll()
//    {
//        synchronized( slock )
//        {
//            size_t   pos = 0;
//            Thread[] buf = new Thread[sm_tlen];
//
//            foreach( Thread t; Thread )
//            {
//                buf[pos++] = t;
//            }
//            return buf;
//        }
//    }
//
//
//    /**
//    * Operates on all threads currently being tracked by the system.  The
//    * result of deleting any Thread object is undefined.
//    *
//    * Params:
//    *  dg = The supplied code as a delegate.
//    *
//    * Returns:
//    *  Zero if all elemented are visited, nonzero if not.
//    */
//    static int opApply( scope int delegate( ref Thread ) dg )
//    {
//        synchronized( slock )
//        {
//            int ret = 0;
//
//            for( Thread t = sm_tbeg; t; t = t.next )
//            {
//                ret = dg( t );
//                if( ret )
//                    break;
//            }
//            return ret;
//        }
//    }
//
//
//    ///////////////////////////////////////////////////////////////////////////
//    // Static Initalizer
//    ///////////////////////////////////////////////////////////////////////////
//
//
//    /**
//    * This initializer is used to set thread constants.  All functional
//    * initialization occurs within thread_init().
//    */
//    shared static this()
//    {
//        version( Windows )
//        {
//            PRIORITY_MIN = THREAD_PRIORITY_IDLE;
//            PRIORITY_DEFAULT = THREAD_PRIORITY_NORMAL;
//            PRIORITY_MAX = THREAD_PRIORITY_TIME_CRITICAL;
//        }
//        else version( Solaris )
//        {
//            pcparms_t pcParms;
//            pcinfo_t pcInfo;
//
//            pcParms.pc_cid = PC_CLNULL;
//            if (priocntl(idtype_t.P_PID, P_MYID, PC_GETPARMS, &pcParms) == -1)
//                throw new ThreadException( "Unable to get scheduling class" );
//
//            pcInfo.pc_cid = pcParms.pc_cid;
//            // PC_GETCLINFO ignores the first two args, use dummy values
//            if (priocntl(idtype_t.P_PID, 0, PC_GETCLINFO, &pcInfo) == -1)
//                throw new ThreadException( "Unable to get scheduling class info" );
//
//            pri_t* clparms = cast(pri_t*)&pcParms.pc_clparms;
//            pri_t* clinfo = cast(pri_t*)&pcInfo.pc_clinfo;
//
//            if (pcInfo.pc_clname == "RT")
//            {
//                m_isRTClass = true;
//
//                // For RT class, just assume it can't be changed
//                PRIORITY_MAX = clparms[0];
//                PRIORITY_MIN = clparms[0];
//                PRIORITY_DEFAULT = clparms[0];
//            }
//            else
//            {
//                m_isRTClass = false;
//
//                // For all other scheduling classes, there are
//                // two key values -- uprilim and maxupri.
//                // maxupri is the maximum possible priority defined
//                // for the scheduling class, and valid priorities
//                // range are in [-maxupri, maxupri].
//                //
//                // However, uprilim is an upper limit that the
//                // current thread can set for the current scheduling
//                // class, which can be less than maxupri.  As such,
//                // use this value for PRIORITY_MAX since this is
//                // the effective maximum.
//
//                // uprilim
//                PRIORITY_MAX = clparms[0];
//
//                // maxupri
//                PRIORITY_MIN = -clinfo[0];
//
//                // by definition
//                PRIORITY_DEFAULT = 0;
//            }
//        }
//        else version( Posix )
//        {
//            int         policy;
//            sched_param param;
//            pthread_t   self = pthread_self();
//
//            int status = pthread_getschedparam( self, &policy, &param );
//            assert( status == 0 );
//
//            PRIORITY_MIN = sched_get_priority_min( policy );
//            assert( PRIORITY_MIN != -1 );
//
//            PRIORITY_DEFAULT = param.sched_priority;
//
//            PRIORITY_MAX = sched_get_priority_max( policy );
//            assert( PRIORITY_MAX != -1 );
//        }
//    }
//
//
//    ///////////////////////////////////////////////////////////////////////////
//    // Stuff That Should Go Away
//    ///////////////////////////////////////////////////////////////////////////
//
//
//private:
//    //
//    // Initializes a thread object which has no associated executable function.
//    // This is used for the main thread initialized in thread_init().
//    //
//    this(size_t sz = 0)
//    {
//        if (sz)
//        {
//            version (Posix)
//            {
//                // stack size must be a multiple of PAGESIZE
//                sz += PAGESIZE - 1;
//                sz -= sz % PAGESIZE;
//                // and at least PTHREAD_STACK_MIN
//                if (PTHREAD_STACK_MIN > sz)
//                    sz = PTHREAD_STACK_MIN;
//            }
//            m_sz = sz;
//        }
//        m_call = Call.NO;
//        m_curr = &m_main;
//    }
//
//
//    //
//    // Thread entry point.  Invokes the function or delegate passed on
//    // construction (if any).
//    //
//    final void run()
//    {
//        switch( m_call )
//        {
//            case Call.FN:
//                m_fn();
//                break;
//            case Call.DG:
//                m_dg();
//                break;
//            default:
//                break;
//        }
//    }
//
//
//private:
//    //
//    // The type of routine passed on thread construction.
//    //
//    enum Call
//    {
//        NO,
//            FN,
//            DG
//    }
//
//
//    //
//    // Standard types
//    //
//    version( Windows )
//    {
//        alias uint TLSKey;
//        alias uint ThreadAddr;
//    }
//    else version( Posix )
//    {
//        alias pthread_key_t TLSKey;
//        alias pthread_t     ThreadAddr;
//    }
//
//
//    //
//    // Local storage
//    //
//    version( OSX )
//    {
//        static Thread       sm_this;
//    }
//    else version( Posix )
//    {
//        // On Posix (excluding OSX), pthread_key_t is explicitly used to
//        // store and access thread reference. This is needed
//        // to avoid TLS access in signal handlers (malloc deadlock)
//        // when using shared libraries, see issue 11981.
//        __gshared pthread_key_t sm_this;
//    }
//    else
//    {
//        static Thread       sm_this;
//    }
//
//
//    //
//    // Main process thread
//    //
//    __gshared Thread    sm_main;
//
//    version (FreeBSD)
//    {
//        // set when suspend failed and should be retried, see Issue 13416
//        static shared bool sm_suspendagain;
//    }
//
//
//    //
//    // Standard thread data
//    //
//    version( Windows )
//    {
//        HANDLE          m_hndl;
//    }
//    else version( OSX )
//    {
//        mach_port_t     m_tmach;
//    }
//    ThreadAddr          m_addr;
//    Call                m_call;
//    string              m_name;
//    union
//    {
//        void function() m_fn;
//        void delegate() m_dg;
//    }
//    size_t              m_sz;
//    version( Posix )
//    {
//        shared bool     m_isRunning;
//    }
//    bool                m_isDaemon;
//    bool                m_isInCriticalRegion;
//    Throwable           m_unhandled;
//
//    version( Solaris )
//    {
//        __gshared immutable bool m_isRTClass;
//    }
//
//private:
//    ///////////////////////////////////////////////////////////////////////////
//    // Storage of Active Thread
//    ///////////////////////////////////////////////////////////////////////////
//
//
//    //
//    // Sets a thread-local reference to the current thread object.
//    //
//    static void setThis( Thread t )
//    {
//        version( OSX )
//        {
//            sm_this = t;
//        }
//        else version( Posix )
//        {
//            pthread_setspecific( sm_this, cast(void*) t );
//        }
//        else
//        {
//            sm_this = t;
//        }
//    }
//
//
//private:
//    ///////////////////////////////////////////////////////////////////////////
//    // Thread Context and GC Scanning Support
//    ///////////////////////////////////////////////////////////////////////////
//
//
//    final void pushContext( Context* c ) nothrow
//        in
//        {
//            assert( !c.within );
//        }
//    body
//    {
//        c.within = m_curr;
//        m_curr = c;
//    }
//
//
//    final void popContext() nothrow
//        in
//        {
//            assert( m_curr && m_curr.within );
//        }
//    body
//    {
//        Context* c = m_curr;
//        m_curr = c.within;
//        c.within = null;
//    }
//
//
//    final Context* topContext() nothrow
//        in
//        {
//            assert( m_curr );
//        }
//    body
//    {
//        return m_curr;
//    }
//
//
//    static struct Context
//    {
//        void*           bstack,
//            tstack;
//        Context*        within;
//        Context*        next,
//            prev;
//    }
//
//
//    Context             m_main;
//    Context*            m_curr;
//    bool                m_lock;
//    void*               m_tlsgcdata;
//
//    version( Windows )
//    {
//        version( X86 )
//        {
//            uint[8]         m_reg; // edi,esi,ebp,esp,ebx,edx,ecx,eax
//        }
//        else version( X86_64 )
//        {
//            ulong[16]       m_reg; // rdi,rsi,rbp,rsp,rbx,rdx,rcx,rax
//            // r8,r9,r10,r11,r12,r13,r14,r15
//        }
//        else
//        {
//            static assert(false, "Architecture not supported." );
//        }
//    }
//    else version( OSX )
//    {
//        version( X86 )
//        {
//            uint[8]         m_reg; // edi,esi,ebp,esp,ebx,edx,ecx,eax
//        }
//        else version( X86_64 )
//        {
//            ulong[16]       m_reg; // rdi,rsi,rbp,rsp,rbx,rdx,rcx,rax
//            // r8,r9,r10,r11,r12,r13,r14,r15
//        }
//        else
//        {
//            static assert(false, "Architecture not supported." );
//        }
//    }
//
//
//private:
//    ///////////////////////////////////////////////////////////////////////////
//    // GC Scanning Support
//    ///////////////////////////////////////////////////////////////////////////
//
//
//    // NOTE: The GC scanning process works like so:
//    //
//    //          1. Suspend all threads.
//    //          2. Scan the stacks of all suspended threads for roots.
//    //          3. Resume all threads.
//    //
//    //       Step 1 and 3 require a list of all threads in the system, while
//    //       step 2 requires a list of all thread stacks (each represented by
//    //       a Context struct).  Traditionally, there was one stack per thread
//    //       and the Context structs were not necessary.  However, Fibers have
//    //       changed things so that each thread has its own 'main' stack plus
//    //       an arbitrary number of nested stacks (normally referenced via
//    //       m_curr).  Also, there may be 'free-floating' stacks in the system,
//    //       which are Fibers that are not currently executing on any specific
//    //       thread but are still being processed and still contain valid
//    //       roots.
//    //
//    //       To support all of this, the Context struct has been created to
//    //       represent a stack range, and a global list of Context structs has
//    //       been added to enable scanning of these stack ranges.  The lifetime
//    //       (and presence in the Context list) of a thread's 'main' stack will
//    //       be equivalent to the thread's lifetime.  So the Ccontext will be
//    //       added to the list on thread entry, and removed from the list on
//    //       thread exit (which is essentially the same as the presence of a
//    //       Thread object in its own global list).  The lifetime of a Fiber's
//    //       context, however, will be tied to the lifetime of the Fiber object
//    //       itself, and Fibers are expected to add/remove their Context struct
//    //       on construction/deletion.
//
//
//    //
//    // All use of the global lists should synchronize on this lock.
//    //
//    @property static Mutex slock() nothrow
//    {
//        return cast(Mutex)_locks[0].ptr;
//    }
//
//    @property static Mutex criticalRegionLock() nothrow
//    {
//        return cast(Mutex)_locks[1].ptr;
//    }
//
//    __gshared byte[__traits(classInstanceSize, Mutex)][2] _locks;
//
//    static void initLocks()
//    {
//        foreach (ref lock; _locks)
//        {
//            lock[] = typeid(Mutex).init[];
//            (cast(Mutex)lock.ptr).__ctor();
//        }
//    }
//
//    static void termLocks()
//    {
//        foreach (ref lock; _locks)
//            (cast(Mutex)lock.ptr).__dtor();
//    }
//
//    __gshared Context*  sm_cbeg;
//
//    __gshared Thread    sm_tbeg;
//    __gshared size_t    sm_tlen;
//
//    //
//    // Used for ordering threads in the global thread list.
//    //
//    Thread              prev;
//    Thread              next;
//
//
//    ///////////////////////////////////////////////////////////////////////////
//    // Global Context List Operations
//    ///////////////////////////////////////////////////////////////////////////
//
//
//    //
//    // Add a context to the global context list.
//    //
//    static void add( Context* c ) nothrow
//        in
//        {
//            assert( c );
//            assert( !c.next && !c.prev );
//        }
//    body
//    {
//        // NOTE: This loop is necessary to avoid a race between newly created
//        //       threads and the GC.  If a collection starts between the time
//        //       Thread.start is called and the new thread calls Thread.add,
//        //       the thread will have its stack scanned without first having
//        //       been properly suspended.  Testing has shown this to sometimes
//        //       cause a deadlock.
//
//        while( true )
//        {
//            slock.lock_nothrow();
//            scope(exit) slock.unlock_nothrow();
//            {
//                if( !suspendDepth )
//                {
//                    if( sm_cbeg )
//                    {
//                        c.next = sm_cbeg;
//                        sm_cbeg.prev = c;
//                    }
//                    sm_cbeg = c;
//                    return;
//                }
//            }
//            yield();
//        }
//    }
//
//
//    //
//    // Remove a context from the global context list.
//    //
//    // This assumes slock being acquired. This isn't done here to
//    // avoid double locking when called from remove(Thread)
//    static void remove( Context* c ) nothrow
//        in
//        {
//            assert( c );
//            assert( c.next || c.prev );
//        }
//    body
//    {
//        if( c.prev )
//            c.prev.next = c.next;
//        if( c.next )
//            c.next.prev = c.prev;
//        if( sm_cbeg == c )
//            sm_cbeg = c.next;
//        // NOTE: Don't null out c.next or c.prev because opApply currently
//        //       follows c.next after removing a node.  This could be easily
//        //       addressed by simply returning the next node from this
//        //       function, however, a context should never be re-added to the
//        //       list anyway and having next and prev be non-null is a good way
//        //       to ensure that.
//    }
//
//
//    ///////////////////////////////////////////////////////////////////////////
//    // Global Thread List Operations
//    ///////////////////////////////////////////////////////////////////////////
//
//
//    //
//    // Add a thread to the global thread list.
//    //
//    static void add( Thread t ) nothrow
//        in
//        {
//            assert( t );
//            assert( !t.next && !t.prev );
//            assert( t.isRunning );
//        }
//    body
//    {
//        // NOTE: This loop is necessary to avoid a race between newly created
//        //       threads and the GC.  If a collection starts between the time
//        //       Thread.start is called and the new thread calls Thread.add,
//        //       the thread could manipulate global state while the collection
//        //       is running, and by being added to the thread list it could be
//        //       resumed by the GC when it was never suspended, which would
//        //       result in an exception thrown by the GC code.
//        //
//        //       An alternative would be to have Thread.start call Thread.add
//        //       for the new thread, but this may introduce its own problems,
//        //       since the thread object isn't entirely ready to be operated
//        //       on by the GC.  This could be fixed by tracking thread startup
//        //       status, but it's far easier to simply have Thread.add wait
//        //       for any running collection to stop before altering the thread
//        //       list.
//        //
//        //       After further testing, having add wait for a collect to end
//        //       proved to have its own problems (explained in Thread.start),
//        //       so add(Thread) is now being done in Thread.start.  This
//        //       reintroduced the deadlock issue mentioned in bugzilla 4890,
//        //       which appears to have been solved by doing this same wait
//        //       procedure in add(Context).  These comments will remain in
//        //       case other issues surface that require the startup state
//        //       tracking described above.
//
//        while( true )
//        {
//            slock.lock_nothrow();
//            scope(exit) slock.unlock_nothrow();
//            {
//                if( !suspendDepth )
//                {
//                    if( sm_tbeg )
//                    {
//                        t.next = sm_tbeg;
//                        sm_tbeg.prev = t;
//                    }
//                    sm_tbeg = t;
//                    ++sm_tlen;
//                    return;
//                }
//            }
//            yield();
//        }
//    }
//
//
//    //
//    // Remove a thread from the global thread list.
//    //
//    static void remove( Thread t ) nothrow
//        in
//        {
//            assert( t );
//            assert( t.next || t.prev );
//        }
//    body
//    {
//        slock.lock_nothrow();
//        {
//            // NOTE: When a thread is removed from the global thread list its
//            //       main context is invalid and should be removed as well.
//            //       It is possible that t.m_curr could reference more
//            //       than just the main context if the thread exited abnormally
//            //       (if it was terminated), but we must assume that the user
//            //       retains a reference to them and that they may be re-used
//            //       elsewhere.  Therefore, it is the responsibility of any
//            //       object that creates contexts to clean them up properly
//            //       when it is done with them.
//            remove( &t.m_main );
//
//            if( t.prev )
//                t.prev.next = t.next;
//            if( t.next )
//                t.next.prev = t.prev;
//            if( sm_tbeg is t )
//                sm_tbeg = t.next;
//            --sm_tlen;
//        }
//        // NOTE: Don't null out t.next or t.prev because opApply currently
//        //       follows t.next after removing a node.  This could be easily
//        //       addressed by simply returning the next node from this
//        //       function, however, a thread should never be re-added to the
//        //       list anyway and having next and prev be non-null is a good way
//        //       to ensure that.
//        slock.unlock_nothrow();
//    }
//}

class ThreadStateException : NException
{

}

extern (Windows)
{
int  TerminateThread(void* hThread,
					 uint    dwExitCode
						 );
extern (Windows) void* GetCurrentThread();
}

class __KillableThread : core.thread.Thread {

	bool __isAlive = false;
	bool __isStarted = false;

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
		version(Windows)
		{
			TerminateThread(m_thisThread,0);

			//super.abort();
		}
		version(POSIX)
		{
		pthread_cancel(m_thisThread);
		}
	}

	private void run() {
		__isAlive = true;
		__isStarted = true;
		version(POSIX)
		{
		m_thisThread = pthread_self();
		}
		version(Windows)
		{
		m_thisThread= GetCurrentThread();
			//std.stdio.writeln(cast(long)m_thisThread);
		}
		m_fn();
		__isAlive = false;
	}

	version(POSIX)
	{
		private pthread_t m_thisThread;
	}
	version(Windows)
	{
		private void* m_thisThread;
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
		if(__athread.__isStarted)
		{
			throw new ThreadStateException();
		}

		__athread.start();
		
	}

	void Join()
	{
		__athread.join();
		__athread.__isAlive = false;
	}
	
	

	bool IsAlive() @property
	{
		return __athread.isRunning();
	}

	void Abort()
	{
		
		//doesnt exist in D
		__athread.abort();
		__athread.__isAlive = false;
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