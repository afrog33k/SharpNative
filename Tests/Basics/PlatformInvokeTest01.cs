using System;

namespace CSharpPlayground
{

	using System;
	using System.Runtime.InteropServices;
	class PlatformInvokeTest01
	{
		public static int MASK(int x,int BYTEX) 
		{
			return	(x&(0xff<<8*BYTEX))>>(8*BYTEX);
		}




		static	int gestaltSystemVersionMajor     = 1937339185;// MASK_STRING("sys1"); /* The major system version number; in 10.4.17 this would be the decimal value 10 */
		static	int gestaltSystemVersionMinor     = 1937339186;// MASK_STRING("sys2"); /* The minor system version number; in 10.4.17 this would be the decimal value 4 */
		static	int gestaltSystemVersionBugFix    =1937339187;// MASK_STRING("sys3"); /* The bug fix system version number; in 10.4.17 this would be the decimal value 17 */

		const string CoreServicesLib= "System/Library/Frameworks/CoreServices.framework/CoreServices";
		[DllImport(CoreServicesLib)]
		private static extern int Gestalt(int ostype,[Out] out int response);

		static void Main()
		{


		
			// MessageBoxA(System.IntPtr.Zero, "Hello, world!", "Test", 64);
			int majorVersion = 0;
			int minorVersion =0;
			int bugFixVersion = 0;
			Gestalt(gestaltSystemVersionMajor, out majorVersion);
			Gestalt(gestaltSystemVersionMinor, out minorVersion);
			Gestalt(gestaltSystemVersionBugFix, out bugFixVersion);

			Console.WriteLine(String.Format("Running on Mac OSX {0} {1} {2}", majorVersion, minorVersion, bugFixVersion));

		}
	}
}