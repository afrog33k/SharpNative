// /*
//   SharpNative - C# to D Transpiler
//   (C) 2014 Irio Systems 
// */

#region Imports

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

#endregion

namespace SharpNative.Compiler
{
    public static class NativeCompilationUtils
    {
        public static Options CompilerOptions = new LdcOptions();
        //        private class ClangOptions : Options
        //        {
        //            public ClangOptions()
        //            {
        //				if(Environment.OSVersion.Platform==PlatformID.Win32Windows)
        //				{
        //                	PathOfCompilerTools = @"C:\Program Files (x86)\LLVM\bin\";
        //                	CompilerExe = "clang++.exe";
        //                	OptimizationFlags = " -I $(WindowsSDK_IncludePath) -O3 -Wc++11-extensions -ftree-vectorize --param max-unroll-times=4 -march=native -ffast-math  "; // -flto does not work on windows
        //				}
        //				else if (Environment.OSVersion.Platform==PlatformID.Unix)
        //				{
        //					PathOfCompilerTools = @"/usr/bin/";
        //					CompilerExe = "clang++";
        //					OptimizationFlags = " -Wall -Wno-dynamic-class-memaccess -Wno-reorder -Wno-unused-variable -Wno-deprecated-register -Wno-overloaded-virtual -Wno-unused-variable -Wno-unused-function  -Wno-unused-private-field -Wno-mismatched-tags -std=c++11 -DNDEBUG -funroll-loops  -ftree-vectorize -march=native -ffast-math  -Ofast "; // -flto does not work on windows
        //
        //				}
        //            }
        //        }

        private class LdcOptions : Options
        {
            public LdcOptions()
            {
                PathOfCompilerTools = @"/usr/local/bin/";
                //				PathOfCompilerTools = @"/Projects/Experiments/SharpNative/iOS/thumb7-ios-ldc/bin"; // iOS

                //				CompilerExe = "ldmd2-0.15"; // lat	est version of ldm2
                //				CompilerExe = "ldmd2"; //Doesnt work with AsIs Test (sharplang)
                CompilerExe = "ldc"; //iOS

                //				CompilerExe = "dmd";//Fast compile

                //fannkuch(12) -noboundscheck -fPIC--> (25s)
                //				OptimizationFlags = " -noboundscheck -fPIC -m64 -O  -inline -release \"-I/Projects/Experiments/SharpNative/ldc2-0.15.0/import\" ";

                OptimizationFlags =
                    "  -m64 -O  -noboundscheck -O -fPIC  -inline -release \"-I/usr/local/Cellar/ldc/0.15.0/include/d\" ";
                //				OptimizationFlags = " -mtriple=thumbv7-apple-ios5.0.0 -disable-fp-elim -float-abi=softfp  \"-I/Projects/Experiments/SharpNative/iOS/thumb7-ios-ldc/import/\" "; //iOS

                //				OptimizationFlags = "  -inline -release -m64 -noboundscheck -O -fPIC \"-I/usr/local/Cellar/dmd/2.066.0/include/d2\" ";
                //								OptimizationFlags = "  -inline -release -m64  -O  \"-I/usr/local/Cellar/dmd/2.066.0/include/d2\" ";

                LinkerOptions = "";
                //-release -O -inline -noboundscheck
            }
        }

        private class DMDWindowsOptions : Options
        {
            public DMDWindowsOptions()
            {
                PathOfCompilerTools = @"C:\\D\\dmd2\\windows\\bin\\";
                //				CompilerExe = "ldmd2-0.15"; // lat	est version of ldm2
                //				CompilerExe = "ldmd2"; //Doesnt work with AsIs Test (sharplang)
                CompilerExe = "dmd";

                //fannkuch(12) -noboundscheck -fPIC--> (25s)
                //				OptimizationFlags = "  -m64 -O  -inline -release \"-I/Projects/Experiments/SharpNative/ldc2-0.15.0/import\" ";

                //				OptimizationFlags = "  -m64 -O  -inline -release \"-I/usr/local/Cellar/ldc/0.14.0/include/d\" ";
                OptimizationFlags = "  -inline -release -m64 -noboundscheck -O  \"-IC:\\D\\dmd2\\windows\\lib\" ";
                //								OptimizationFlags = "  -inline -release -m64  -O  \"-I/usr/local/Cellar/dmd/2.066.0/include/d2\" ";

                LinkerOptions = "";
                //-release -O -inline -noboundscheck
            }
        }


        //        private class WindowsClOptions : Options
        //        {
        //            public WindowsClOptions()
        //            {
        //                PathOfCompilerTools = @"C:\Program Files (x86)\Microsoft Visual Studio 12.0\VC\bin\";
        //                CompilerExe = "cl.exe";
        //                OptimizationFlags = "/MT /YX  /O2 /Zc:wchar_t /fp:precise /EHsc -D _CRT_SECURE_NO_WARNINGS";
        //                LinkerOptions = "";
        //                StartWithShell = true;
        //            }
        //
        //
        //        }

        public class Options
        {
            public string CompilerKind;
            public string PathOfCompilerTools;
            public string CompilerExe;
            public string OptimizationFlags;
            public bool StartWithShell;
            public string LinkerOptions;
        }

        public static void SetCompilerOptions(string compilerKind)
        {
            switch (compilerKind)
            {
                case "ldc":
                    CompilerOptions = new LdcOptions();
                    break;
                case "dmdwin":
                    CompilerOptions = new DMDWindowsOptions();
                    break;
                    //                case "clang":
                    //                    CompilerOptions = new ClangOptions();
                    //                    break;
                    //                case "msvc":
                    //                    CompilerOptions = new WindowsClOptions();
                    //                    break;
            }
        }

        public static string GetFullFileName(this string fileName)
        {
            var fileInfo = new FileInfo(fileName);

            return fileInfo.FullName;
        }

        public static void CompileAppToNativeExe(string[] fileNames, string applicationNativeExe)
        {
            List<string> files = new List<string>();
            foreach (var fileName in fileNames)
            {
                var fileInfo = new FileInfo(fileName);

                var file = fileInfo.FullName;
                if (!fileInfo.Exists)
                    throw new InvalidDataException(string.Format("Filename: {0} does not exist!", fileName));

                file = GetSafeFileOrPath(fileName);

                files.Add(file);
            }

            var pathToGpp = CompilerOptions.PathOfCompilerTools + CompilerOptions.CompilerExe;

            var commandLineFormat = "{0} " + CompilerOptions.OptimizationFlags + " {2} -of{1}";

            applicationNativeExe = GetSafeFileOrPath(applicationNativeExe);

            var arguments = String.Format(commandLineFormat, files.Aggregate((a, b) => a + " " + b),
                applicationNativeExe,
                CompilerOptions.LinkerOptions);
            string standardOutput;
            if (CompilerOptions.StartWithShell)
            {
                standardOutput =
                    "cmd.exe".ExecuteCommand(
                        "/c \"\"C:\\Program Files (x86)\\Microsoft Visual Studio 12.0\\VC\\bin\\vcvars32.bat\" \\ && \"" +
                        pathToGpp + "\" " + arguments + "\"", CompilerOptions.PathOfCompilerTools);
            }
            else
                standardOutput = pathToGpp.ExecuteCommand(arguments, CompilerOptions.PathOfCompilerTools);
            if (!String.IsNullOrWhiteSpace(standardOutput) && standardOutput.ToLower().Contains("error"))
                throw new InvalidOperationException(String.Format("Errors when compiling: {0}", standardOutput));
            // (CompilerOptions.PathOfCompilerTools + "strip").ExecuteCommand(applicationNativeExe,Path.GetDirectoryName(applicationNativeExe));
        }

        private static string GetSafeFileOrPath(string fileName)
        {
            if (fileName.Contains(" "))
                fileName = string.Format("\"{0}\"", fileName);
            return fileName;
        }
    }
}