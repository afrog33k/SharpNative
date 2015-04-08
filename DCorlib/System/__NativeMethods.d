module System.__NativeMethods;


int System_DateTime_gettimeofday(int *time ,int *tzp)
{
	struct timeval {
		long      tv_sec;     /* seconds */
		long tv_usec;    /* microseconds */
	};

	struct timezone {
		int tz_minuteswest;     /* minutes west of Greenwich */
		int tz_dsttime;         /* type of DST correction */
	};

	version(Windows)
	{
		import core.sys.windows.windows;
		// MSVC defines this in winsock2.h!?
		

		int gettimeofday(timeval * tp, timezone * tz)
		{
			// Note: some broken versions only have 8 trailing zero's, the correct epoch has 9 trailing zero's
			enum EPOCH =  11644473600000000UL;

			SYSTEMTIME  system_time;
			FILETIME    file_time;
			ulong    time;

			GetSystemTime( &system_time );
			SystemTimeToFileTime( &system_time, &file_time );
			time =  (cast(ulong)file_time.dwLowDateTime )      ;
			time += (cast(ulong)file_time.dwHighDateTime) << 32;

			tp.tv_sec  = cast(long) ((time - EPOCH) / 10000000L);
			tp.tv_usec = cast(long) (system_time.wMilliseconds * 1000);
			return 0;
		}

		return gettimeofday(cast(timeval *)time,cast(timezone *)tzp);
	}
	else
	{
		extern (C) int gettimeofday(timeval * tp, timezone * tz);
		return gettimeofday(cast(timeval *)time,cast(timezone *)tzp);

	}
	return 0;
}

int System_DateTime_DaysInMonth(int year , int month)
{
	return 0;
}