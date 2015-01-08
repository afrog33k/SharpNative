//using System;
//Mac OSX Specific
// namespace CSharpPlayground
// {

	// using System;
	// using System.Runtime.InteropServices;
	// class PlatformInvokeTest01
	// {
		// public static int MASK(int x,int BYTEX) 
		// {
			// return	(x&(0xff<<8*BYTEX))>>(8*BYTEX);
		// }




		// static	int gestaltSystemVersionMajor     = 1937339185;// MASK_STRING("sys1"); /* The major system version number; in 10.4.17 this would be the decimal value 10 */
		// static	int gestaltSystemVersionMinor     = 1937339186;// MASK_STRING("sys2"); /* The minor system version number; in 10.4.17 this would be the decimal value 4 */
		// static	int gestaltSystemVersionBugFix    =1937339187;// MASK_STRING("sys3"); /* The bug fix system version number; in 10.4.17 this would be the decimal value 17 */

		// const string CoreServicesLib= "System/Library/Frameworks/CoreServices.framework/CoreServices";
		// [DllImport(CoreServicesLib)]
		// private static extern int Gestalt(int ostype,[Out] out int response);

		// static void Main()
		// {


		
			// int majorVersion = 0;
			// int minorVersion =0;
			// int bugFixVersion = 0;
			// Gestalt(gestaltSystemVersionMajor, out majorVersion);
			// Gestalt(gestaltSystemVersionMinor, out minorVersion);
			// Gestalt(gestaltSystemVersionBugFix, out bugFixVersion);

			// Console.WriteLine(String.Format("Running on Mac OSX {0} {1} {2}", majorVersion, minorVersion, bugFixVersion));

		// }
	// }
// }


/*
//Windows Specific
using System;
using System.Runtime.InteropServices;    

class Class1
{
	const string User32Lib = "user32.dll";
    [DllImport(User32Lib, CharSet=CharSet.Auto)]
    static extern int MessageBoxW(IntPtr hWnd, String text, String caption, int options);

    [STAThread]
    static void Main()
    {
        MessageBoxW(IntPtr.Zero, "Text", "Caption", 0);
    }
}
*/
// PInvokeTest.cs
using System;
using System.Runtime.InteropServices;

class PlatformInvokeTest
{
    [DllImport("msvcrt.dll")]
    public static extern int puts(string c);
    [DllImport("msvcrt.dll")]
    internal static extern int _flushall();

    public static void Main() 
    {
        puts("Test");
        _flushall();
    }
}