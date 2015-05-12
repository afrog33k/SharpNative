module System.Environment;
import System.Namespace;
import std.stdio;

public class Environment : NObject
{


public static string NewLine() @property
 {

		return (Platform == PlatformID.Unix) ? "\n" : "\r\n";			
 }

 public static int TickCount() @property
 {
 		return 0;//tk;
 }

 private static OperatingSystem os = null;

  static  PlatformID Platform() @property
  {
  		//extern
  		return PlatformID.init;
  }
 //{
	//		[MethodImplAttribute(MethodImplOptions.InternalCall)]
	//		get;
	//	}

		//[MethodImplAttribute(MethodImplOptions.InternalCall)]
		 static  String GetOSVersionString()
		 {
		 	//extern
		 	return String.Empty;
		 }

		public static OperatingSystem OSVersion() @property 
		{
			
				if (os is null) {
					Version v = Version.CreateFromString(GetOSVersionString());
					PlatformID p = Platform;
					os = new OperatingSystem(p, v);
				}
				return os;
			
		}

		 static bool IsRunningOnWindows() @property 
		 {
			
				return Platform != PlatformID.Unix;
			
		}

		 static int ExitCode;

	this()
	{
		// Constructor code
	}
	
}

