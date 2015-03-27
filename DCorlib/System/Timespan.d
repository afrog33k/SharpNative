module System.TimeSpan;
import std.conv;

import System.Namespace;



      static struct TimeSpan
      {
        enum __staticFieldTuple = __Tuple!("TicksPerMillisecond","TicksPerSecond","TicksPerMinute","TicksPerHour","TicksPerDay","Zero","MaxValue","MinValue","MillisecondsPerTick","SecondsPerTick","MinutesPerTick","HoursPerTick","DaysPerTick","MaxMilliSeconds","MinMilliSeconds");//Reflection Support
          public long m_ticks = 0;
          public __gshared const long TicksPerMillisecond = cast(long )cast(long)10000;
          public __gshared const long TicksPerSecond = cast(long )TimeSpan.TicksPerMillisecond*cast(long)1000;
          public __gshared const long TicksPerMinute = cast(long )TimeSpan.TicksPerSecond*cast(long)60;
          public __gshared const long TicksPerHour = cast(long )TimeSpan.TicksPerMinute*cast(long)60;
          public __gshared const long TicksPerDay = cast(long )TimeSpan.TicksPerHour*cast(long)24;
          public __gshared TimeSpan Zero;
          public __gshared TimeSpan MaxValue;
          public __gshared TimeSpan MinValue;
          private __gshared const double MillisecondsPerTick = cast(double )1.0/TimeSpan.TicksPerMillisecond;
          private __gshared const double SecondsPerTick = cast(double )1.0/TimeSpan.TicksPerSecond;
          private __gshared const double MinutesPerTick = cast(double )1.0/TimeSpan.TicksPerMinute;
          private __gshared const double HoursPerTick = cast(double )1.0/TimeSpan.TicksPerHour;
          private __gshared const double DaysPerTick = cast(double )1.0/TimeSpan.TicksPerDay;
          private __gshared const long MaxMilliSeconds = cast(long )System.Namespace.Int64.MaxValue/TimeSpan.TicksPerMillisecond;
          private __gshared const long MinMilliSeconds = cast(long )System.Namespace.Int64.MinValue/TimeSpan.TicksPerMillisecond;

          public long Ticks() 
          {
                          {
                return this.m_ticks;
              }

          }


          public int Days() 
          {
                          {
                return cast(int)(this.m_ticks/TimeSpan.TicksPerDay);
              }

          }


          public int Hours() 
          {
                          {
                return cast(int)((this.m_ticks/TimeSpan.TicksPerHour)%cast(long)24);
              }

          }


          public int Milliseconds() 
          {
                          {
                return cast(int)((this.m_ticks/TimeSpan.TicksPerMillisecond)%cast(long)1000);
              }

          }


          public int Minutes() 
          {
                          {
                return cast(int)((this.m_ticks/TimeSpan.TicksPerMinute)%cast(long)60);
              }

          }


          public int Seconds() 
          {
                          {
                return cast(int)((this.m_ticks/TimeSpan.TicksPerSecond)%cast(long)60);
              }

          }


          public double TotalDays() 
          {
                          {
                return (cast(double)this.m_ticks)*TimeSpan.DaysPerTick;
              }

          }


          public double TotalHours() 
          {
                          {
                return cast(double)this.m_ticks*TimeSpan.HoursPerTick;
              }

          }


          public double TotalMilliseconds() 
          {
                          {
                double temp = cast(double)this.m_ticks*TimeSpan.MillisecondsPerTick;
                if(temp>TimeSpan.MaxMilliSeconds)
                {
                  return cast(double)TimeSpan.MaxMilliSeconds;
                }
                if(temp<TimeSpan.MinMilliSeconds)
                {
                  return cast(double)TimeSpan.MinMilliSeconds;
                }
                return temp;
              }

          }


          public double TotalMinutes() 
          {
                          {
                return cast(double)this.m_ticks*TimeSpan.MinutesPerTick;
              }

          }


          public double TotalSeconds() 
          {
                          {
                return cast(double)this.m_ticks*TimeSpan.SecondsPerTick;
              }

          }


          public TimeSpan Add(TimeSpan ts)
          {
            return   TimeSpan(this.m_ticks+ts.m_ticks);
          }

          public static int Compare(TimeSpan t1, TimeSpan t2)
          {
            //Extern (Internal) Method Call
            //return CsRoot_Primes_TimeSpan_Compare(t1 ,t2);
			  return 0;
          }

          public int CompareTo(NObject value)
          {
            //Extern (Internal) Method Call
           // return CsRoot_Primes_TimeSpan_CompareTo(value);
			  return 0;
          }

          public TimeSpan Duration()
          {
            return   TimeSpan((this.m_ticks>=cast(long)0) ? (this.m_ticks) : (-this.m_ticks));
          }

          public bool Equals(NObject value)
          {
            throw  new System.Namespace.NotImplementedException();
          }

          public static bool Equals(TimeSpan t1, TimeSpan t2)
          {
            //Extern (Internal) Method Call
            //return CsRoot_Primes_TimeSpan_Equals(t1 ,t2);
			  return false;
          }

          public TimeSpan Negate()
          {
            return   TimeSpan(-this.m_ticks);
          }

          public TimeSpan Subtract(TimeSpan ts)
          {
            return   TimeSpan(this.m_ticks-ts.m_ticks);
          }

          public static TimeSpan FromTicks(long val)
          {
            return   TimeSpan(val);
          }

          public String ToString()
          {
            throw  new System.Namespace.NotImplementedException();
          }

          public final TimeSpan  opUnary (string _op) ()
	if(_op=="-")
	{ 
		return op_UnaryNegation(this); 
	}


          public static TimeSpan  op_UnaryNegation(TimeSpan t)
          {
            return   TimeSpan(-t.m_ticks);
          }

          public final TimeSpan  opBinary (string _op) (TimeSpan other)
	if(_op=="-")
	{ 
		return op_Subtraction(this,other); 
	}


          public final TimeSpan  opOpAssign (string _op) (TimeSpan other)
	if(_op=="-")
	{ 
		return op_Subtraction(this,other); 
	}


          public static TimeSpan  op_Subtraction(TimeSpan t1, TimeSpan t2)
          {
            return   TimeSpan(t1.m_ticks-t2.m_ticks);
          }

          public final TimeSpan  opUnary (string _op) ()
	if(_op=="+")
	{ 
		return op_UnaryPlus(this); 
	}


          public static TimeSpan  op_UnaryPlus(TimeSpan t)
          {
            return t;
          }

          public final TimeSpan  opBinary (string _op) (TimeSpan other)
	if(_op=="+")
	{ 
		return op_Addition(this,other); 
	}


          public final TimeSpan  opOpAssign (string _op) (TimeSpan other)
	if(_op=="+")
	{ 
		return op_Addition(this,other); 
	}


          public static TimeSpan  op_Addition(TimeSpan t1, TimeSpan t2)
          {
            return   TimeSpan(t1.m_ticks+t2.m_ticks);
          }

          public final bool  opEquals (string _op) (TimeSpan other)
	if(_op=="==")
	{ 
		return op_Equality(this); 
	}


          public static bool  op_Equality(TimeSpan t1, TimeSpan t2)
          {
            return t1.m_ticks==t2.m_ticks;
          }

          public final bool  opEquals (string _op) (TimeSpan other)
	if(_op=="!=")
	{ 
		return op_Inequality(this); 
	}


          public static bool  op_Inequality(TimeSpan t1, TimeSpan t2)
          {
            return t1.m_ticks!=t2.m_ticks;
          }

          public final bool  opCmp (string _op) (TimeSpan other)
	if(_op=="<")
	{ 
		return op_LessThan(this); 
	}


          public static bool  op_LessThan(TimeSpan t1, TimeSpan t2)
          {
            return t1.m_ticks<t2.m_ticks;
          }

          public final bool  opCmp (string _op) (TimeSpan other)
	if(_op=="<=")
	{ 
		return op_LessThanOrEqual(this); 
	}


          public static bool  op_LessThanOrEqual(TimeSpan t1, TimeSpan t2)
          {
            return t1.m_ticks<=t2.m_ticks;
          }

          public final bool  opCmp (string _op) (TimeSpan other)
	if(_op==">")
	{ 
		return op_GreaterThan(this); 
	}


          public static bool  op_GreaterThan(TimeSpan t1, TimeSpan t2)
          {
            return t1.m_ticks>t2.m_ticks;
          }

          public final bool  opCmp (string _op) (TimeSpan other)
	if(_op==">=")
	{ 
		return op_GreaterThanOrEqual(this); 
	}


          public static bool  op_GreaterThanOrEqual(TimeSpan t1, TimeSpan t2)
          {
            return t1.m_ticks>=t2.m_ticks;
          }

          public  void __init(long ticks)
          {
            this.m_ticks=ticks;
          }

          public  void __init(int hours, int minutes, int seconds)
          {
          }

          public  void __init(int days, int hours, int minutes, int seconds)
          {
          }

          public  void __init(int days, int hours, int minutes, int seconds, int milliseconds)
          {
          }
          void __init(){}//default xtor
          static TimeSpan opCall(U...)(U args_)
          {
             TimeSpan s;
            s.__init(args_);
            return s;
          }

          static this()
          {
            Zero =   TimeSpan(cast(long)0);

            MaxValue =   TimeSpan(System.Namespace.Int64.MaxValue);

            MinValue =   TimeSpan(System.Namespace.Int64.MinValue);

          }

        public static class __Boxed_ : Boxed!(TimeSpan)
        {
          import std.traits;

          this()
          {
            super(TimeSpan.init);
          }
          public override String ToString()
          {
            return __Value.ToString();
          }

          this(ref TimeSpan value)
          {
            super(value);
          }

          U opCast(U)()
          if(is(U:TimeSpan))
          {
            return __Value;
          }

          U opCast(U)()
          if(!is(U:TimeSpan))
          {
            return this;
          }

          auto opDispatch(string op, Args...)(Args args)
          {
            enum name = op;
            return __traits(getMember, __Value, name)(args);
          }

          public override Type GetType()
          {
            return __Value.GetType();
          }
        }

        public __Boxed_ __Get_Boxed()
        {
          return new __Boxed_(this);
        }
        alias __Get_Boxed this;

        public Type GetType()
        {
          return __TypeOf!(typeof(this));
        }
      }