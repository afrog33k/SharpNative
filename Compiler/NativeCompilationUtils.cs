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
        public static Options CompilerOptions = new LdcWindowsOptions();

        private class LdcWindowsOptions : Options
        {
            public LdcWindowsOptions()
            {
                PathOfCompilerTools = @"/usr/local/bin/";
                //              PathOfCompilerTools = @"/Projects/Experiments/SharpNative/iOS/thumb7-ios-ldc/bin"; // iOS

                //              CompilerExe = "ldmd2-0.15"; // lat  est version of ldm2
                //              CompilerExe = "ldmd2"; //Doesnt work with AsIs Test (sharplang)
                CompilerExe = "ldc"; //iOS

                //              CompilerExe = "dmd";//Fast compile

                //fannkuch(12) -noboundscheck -fPIC--> (25s)
                //              OptimizationFlags = " -noboundscheck -fPIC -m64 -O  -inline -release \"-I/Projects/Experiments/SharpNative/ldc2-0.15.0/import\" ";

                OptimizationFlags =
                    " -m64 -O  -noboundscheck -O -fPIC  -inline -release \"-I/usr/local/Cellar/ldc/0.15.0/include/d\" ";
                //              OptimizationFlags = " -mtriple=thumbv7-apple-ios5.0.0 -disable-fp-elim -float-abi=softfp  \"-I/Projects/Experiments/SharpNative/iOS/thumb7-ios-ldc/import/\" "; //iOS

                //              OptimizationFlags = "  -inline -release -m64 -noboundscheck -O -fPIC \"-I/usr/local/Cellar/dmd/2.066.0/include/d2\" ";
                //                              OptimizationFlags = "  -inline -release -m64  -O  \"-I/usr/local/Cellar/dmd/2.066.0/include/d2\" ";

                LinkerOptions = "";
                //-release -O -inline -noboundscheck
            }
        }

        private class DMDWindowsOptions : Options
        {
            public DMDWindowsOptions()
            {
                PathOfCompilerTools = @"C:\\D\\dmd2\\windows\\bin\\";
                //              CompilerExe = "ldmd2-0.15"; // lat  est version of ldm2
                //              CompilerExe = "ldmd2"; //Doesnt work with AsIs Test (sharplang)
                CompilerExe = "dmd";

                //fannkuch(12) -noboundscheck -fPIC--> (25s)
                //              OptimizationFlags = "  -m64 -O  -inline -release \"-I/Projects/Experiments/SharpNative/ldc2-0.15.0/import\" ";

                //              OptimizationFlags = "  -m64 -O  -inline -release \"-I/usr/local/Cellar/ldc/0.14.0/include/d\" ";
                //-w -wi -v
                OptimizationFlags = "  -inline -release -m64 -O  \"-IC:\\D\\dmd2\\windows\\lib\" ";
                //                              OptimizationFlags = "  -inline -release -m64  -O  \"-I/usr/local/Cellar/dmd/2.066.0/include/d2\" ";

                LinkerOptions = "";
                //-release -O -inline -noboundscheck
            }
        }

        private class DmdMacOptions : Options
        {
            public DmdMacOptions()
            {
                PathOfCompilerTools = @"/usr/local/bin/";

                CompilerExe = "dmd";
                //fannkuch(12) -noboundscheck -fPIC--> (25s)

                OptimizationFlags = "  -inline -release -m64  -O  \"-I/usr/local/Cellar/dmd/2.066.0/include/d2\" ";

                LinkerOptions = "";
                //-release -O -inline -noboundscheck
            }
        }

        private class LdciOSOptions : Options
        {
            public LdciOSOptions()
            {
                //              PathOfCompilerTools = @"/usr/local/bin/";
                //              CompilerExe = "ldc2-0.15";
                PathOfCompilerTools = @"/Projects/Experiments/CsNative/iOS/ldc/build/bin/"; // iOS
                CompilerExe = "ldc2"; //iOS

                //              CompilerExe = "ldmd2-0.15"; // lat  est version of ldm2
                //              CompilerExe = "ldmd2"; //Doesnt work with AsIs Test (sharplang)
                //              CompilerExe = "dmd";
                //                              OptimizationFlags = " -mtriple=thumbv7-apple-ios5.0.0 -disable-fp-elim -float-abi=softfp  \"-I/Projects/Experiments/CsNative/iOS/thumb7-ios-ldc/import/\" "; //iOS
                OptimizationFlags = " -march=arm  -O  -inline -release \"-I/Projects/Experiments/CsNative/iOS/thumb7-ios-ldc/import/\" ";
                //fannkuch(12) -noboundscheck -fPIC--> (25s)
                //              OptimizationFlags = " -noboundscheck -fPIC -m64 -O  -inline -release \"-I/Projects/Experiments/CsNative/ldc2-0.15.0/import\" ";


                //              OptimizationFlags = "  -m64 -O  -inline -release \"-I/usr/local/Cellar/ldc/0.15.0/include/d\" ";
                //              OptimizationFlags = "  -inline -release -m64 -noboundscheck -O -fPIC \"-I/usr/local/Cellar/dmd/2.066.0/include/d2\" ";
                //                              OptimizationFlags = "  -inline -release -m64  -O  \"-I/usr/local/Cellar/dmd/2.066.0/include/d2\" ";

                LinkerOptions = "";
                //-release -O -inline -noboundscheck
            }
        }


        private class LdcOptions : Options
        {
            public LdcOptions()
            {
                //              PathOfCompilerTools = @"/usr/local/bin/";
                //              CompilerExe = "ldc2-0.15";
                PathOfCompilerTools = @"/Projects/Experiments/CsNative/iOS/ldc/build/bin/"; // iOS
                CompilerExe = "ldc2"; //iOS

                //              CompilerExe = "ldmd2-0.15"; // lat  est version of ldm2
                //              CompilerExe = "ldmd2"; //Doesnt work with AsIs Test (sharplang)
                //              CompilerExe = "dmd";
                //                              OptimizationFlags = " -mtriple=thumbv7-apple-ios5.0.0 -disable-fp-elim -float-abi=softfp  \"-I/Projects/Experiments/CsNative/iOS/thumb7-ios-ldc/import/\" "; //iOS
                OptimizationFlags = " -march=arm  -O  -inline -release \"-I/Projects/Experiments/CsNative/iOS/thumb7-ios-ldc/import/\" ";
                //fannkuch(12) -noboundscheck -fPIC--> (25s)
                //              OptimizationFlags = " -noboundscheck -fPIC -m64 -O  -inline -release \"-I/Projects/Experiments/CsNative/ldc2-0.15.0/import\" ";


                //              OptimizationFlags = "  -m64 -O  -inline -release \"-I/usr/local/Cellar/ldc/0.15.0/include/d\" ";
                //              OptimizationFlags = "  -inline -release -m64 -noboundscheck -O -fPIC \"-I/usr/local/Cellar/dmd/2.066.0/include/d2\" ";
                //                              OptimizationFlags = "  -inline -release -m64  -O  \"-I/usr/local/Cellar/dmd/2.066.0/include/d2\" ";

                LinkerOptions = "";
                //-release -O -inline -noboundscheck
            }
        }

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
                case "ldcwin":
                    CompilerOptions = new LdcWindowsOptions();
                    break;
                case "dmdwin":
                    CompilerOptions = new DMDWindowsOptions();
                    break;
               
            }
        }

        public static void SetCompilerOptions(Options compiler)
        {

            CompilerOptions = compiler;
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

            var pathToCompiler = CompilerOptions.PathOfCompilerTools + CompilerOptions.CompilerExe;

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
                    pathToCompiler + "\" " + arguments + "\"", CompilerOptions.PathOfCompilerTools);
            }
            else
                standardOutput = pathToCompiler.ExecuteCommand(arguments, CompilerOptions.PathOfCompilerTools);
            if (!String.IsNullOrWhiteSpace(standardOutput) && standardOutput.ToLower().Contains("error"))
                throw new InvalidOperationException(String.Format("Errors when compiling: {0}", standardOutput));
            
        }

        private static string GetSafeFileOrPath(string fileName)
        {
            if (fileName.Contains(" "))
                fileName = string.Format("\"{0}\"", fileName);
            return fileName;
        }
    }
}