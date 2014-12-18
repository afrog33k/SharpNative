//#region Usings
//
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Windows.Documents;
//
//#endregion
//
//namespace CsNativeVisual
//{
//    public static class NativeCompilationUtils
//    {
//        public static Options CompilerOptions = new GccOptions();
//        private class ClangOptions : Options
//        {
//            public ClangOptions()
//            {
//                PathOfCompilerTools = @"C:\Program Files (x86)\LLVM\bin\";
//                CompilerExe = "clang++.exe";
//                OptimizationFlags = " -I $(WindowsSDK_IncludePath) -O3 -Wc++11-extensions -ftree-vectorize --param max-unroll-times=4 -march=native -ffast-math  "; // -flto does not work on windows
//            }
//        }
//
//        private class GccOptions : Options
//        {
//            public GccOptions()
//            {
//                PathOfCompilerTools = @"C:\TDM-GCC-32\bin\";
//                CompilerExe = "g++.exe";
//                OptimizationFlags = "-Ofast -fomit-frame-pointer -ffast-math -std=c++11 -static-libgcc -fpermissive";
//                LinkerOptions = "";
//            }
//        }
//
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
//
//        public class Options
//        {
//            public string CompilerKind;
//            public string PathOfCompilerTools;
//            public string CompilerExe;
//            public string OptimizationFlags;
//            public bool StartWithShell;
//            public string LinkerOptions;
//        }
//
//
//        public static void SetCompilerOptions(string compilerKind)
//        {
//            switch (compilerKind)
//            {
//                case "gcc":
//                    CompilerOptions = new GccOptions();
//                    break;
//                case "clang":
//                    CompilerOptions = new ClangOptions();
//                    break;
//                case "msvc":
//                    CompilerOptions = new WindowsClOptions();
//                    break;
//            }
//        }
//
//        public static string GetFullFileName(this string fileName)
//        {
//            var fileInfo = new FileInfo(fileName);
//
//            return fileInfo.FullName; 
//            
//        }
//
//        public static void CompileAppToNativeExe(string[] fileNames, string applicationNativeExe)
//        {
//
//            List<string> files = new List<string>();
//            foreach (var fileName in fileNames)
//            {
//                var fileInfo = new FileInfo(fileName);
//
//               var file = fileInfo.FullName;
//                if (!fileInfo.Exists)
//                {
//                    throw new InvalidDataException(string.Format("Filename: {0} does not exist!", fileName));
//                }
//
//                file = GetSafeFileOrPath(fileName);
//
//                files.Add(file);
//
//            }
//           
//
//
//            var pathToGpp = CompilerOptions.PathOfCompilerTools + CompilerOptions.CompilerExe;
//
//            var commandLineFormat = "{0} " + CompilerOptions.OptimizationFlags + " {2} -o {1}";
//
//           
//            applicationNativeExe = GetSafeFileOrPath(applicationNativeExe);
//
//            var arguments = String.Format(commandLineFormat, files.Aggregate((a,b)=> a + " " + b), applicationNativeExe,
//                CompilerOptions.LinkerOptions);
//            string standardOutput;
//            if (CompilerOptions.StartWithShell)
//            {
//               standardOutput= "cmd.exe".ExecuteCommand("/c \"\"C:\\Program Files (x86)\\Microsoft Visual Studio 12.0\\VC\\bin\\vcvars32.bat\" \\ && \"" + pathToGpp + "\" "+ arguments +"\"", CompilerOptions.PathOfCompilerTools);
//            }
//            else
//            standardOutput = pathToGpp.ExecuteCommand(arguments, CompilerOptions.PathOfCompilerTools);
//            if (!String.IsNullOrWhiteSpace(standardOutput) && standardOutput.Contains("error"))
//            {
//                throw new InvalidOperationException(String.Format("Errors when compiling: {0}", standardOutput));
//            }
//            else
//            {
//                //TODO: this is a temp fix for permissive casting
//
//            }
//           // (CompilerOptions.PathOfCompilerTools + "strip").ExecuteCommand(applicationNativeExe,Path.GetDirectoryName(applicationNativeExe));
//        }
//
//        private static string GetSafeFileOrPath(string fileName)
//        {
//            if (fileName.Contains(" "))
//                fileName = string.Format("\"{0}\"", fileName);
//            return fileName;
//        }
//    }
//}