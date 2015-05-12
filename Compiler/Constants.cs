namespace SharpNative.Compiler
{
    public static class Constants
    {
		
        public static string Version = "0.1.6";
        public const string Usage = @"
Usage:
    SharpNative.exe  /sln:<path to solution file> [options]  -- To compile a solution/project
	SharpNative.exe  /source:""<comma separated paths to C# files>"" /of:outputname [options]   -- To compile C# file(s)
    SharpNative.exe  /test:<path to test folder> [options]   -- To compile and test files in folder
	
Options available:

    /output:
        Choose the output type: 
            source  -- source code in dlang
            library -- dlang library
            exe     -- binary

    /run:""arguments""
        Run program with arguments   

	/dcorlib:
		Path to DCorlib

	/dstdlib:
		Path to phobos / druntime to use for compiling

	/compiler:
		Path to D compiler to use e.g. (/usr/bin/ldc)
	
	/compileroptions:""options""
		Options to pass to the Compiler chosen.
		
	/out:<output directory>
		Directory to write d files to.  If not specified, output will be written to the current working directory.

	/config:<configuration>
		The configuration within the passed solution file to use. (Debug, Release, etc.)

	/projects:<comma-delimited list of project names>
		If you don't want to convert all projects in the passed solution, you can provide a list of project names.  Only the projects named here will be converted.

	/define:<symbol>
		Adds extra pre-processor #define symbols to add to the project before building.
";
    }
}

