using System;
using System.Diagnostics;
using System.Linq;

class Program
{
    static void Main(string[] args)
    {
        string getEnv = Environment.GetEnvironmentVariable("VSCodeProject");
        Console.WriteLine("Startup: " + Environment.CurrentDirectory+ " Args: " + getEnv);
//         Environment.CurrentDirectory = @"D:\temp";
// Open the file "example.txt".
	// ... It must be in the same directory as the .exe file.
	    Process.Start("/usr/bin/xbuild" , getEnv);
        
        Environment.Exit(0);
//         Console.WriteLine("After:" + Environment.CurrentDirectory);
    }
}