// /*
//   SharpNative - C# to D Transpiler
//   (C) 2014 Irio Systems 
// */

#region Imports

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.MSBuild;

#endregion

namespace SharpNative.Compiler
{
    //TODO: Add support for xbuild ... how ?

    public static class Driver
    {
        public static void CompileProject(string outDir, string pathToSolution, string projects, string[] extraDefines,
            List<string> extraTranslations)
        {
            try
            {
                if (pathToSolution == null)
                    throw new Exception("/sln parameter not passed");
                var theproject = MSBuildWorkspace.Create().OpenProjectAsync(pathToSolution).Result;
                var projectsList = new[] { theproject };
                if (projects != null)
                    TrimList(projectsList, projects);
                if (extraDefines.Length > 0)
                {
                    projectsList =
                        projectsList.Select(
                            p =>
                                p.WithParseOptions(
                                    new CSharpParseOptions(
                                        preprocessorSymbols:
                                            p.ParseOptions.As<ParseOptions>()
                                                .PreprocessorSymbolNames.Concat(
                                                    extraDefines.Where(z => z.StartsWith("-") == false))
                                                .Except(
                                                    extraDefines.Where(z => z.StartsWith("-"))
                                                        .Select(z => z.Substring(1)))
                                                .ToArray()))).ToArray();
                }
                foreach (var project in projectsList)
                {
                    if (Verbose)
                        Console.WriteLine("Converting project " + project.Name + "...");
                    var sw = Stopwatch.StartNew();
                    Program.Go(project.GetCompilationAsync().Result, outDir, extraTranslations);
                    if (Verbose)
                        Console.WriteLine("Finished project " + project.Name + " in " + sw.Elapsed);
                }
                Environment.ExitCode = 0;
            }
            catch (AggregateException agex)
            {
                Environment.ExitCode = 1;

                Exception ex = agex;
                while (ex is AggregateException)
                    ex = ex.InnerException;

                Console.WriteLine("\nException:");
                Console.WriteLine(ex);
            }
            catch (Exception ex)
            {
                Environment.ExitCode = 1;

                Console.WriteLine("\nException:");
                Console.WriteLine(ex);
            }
        }

        public static void CompileFiles(string outDir, string pathToFiles, string[] extraDefines,
           List<string> extraTranslations)
        {
            try
            {
                if (pathToFiles == null)
                    throw new Exception("/source parameter not passed");

                var filenames = pathToFiles.Split(',');
                var _source = filenames.Select(o => FileExtensions.ReadFile(Path.GetFullPath(o))).ToArray();
                if (_source == null)
                {
                    Environment.ExitCode = 1;

                    //throw new ArgumentNullException("\nSource Files Cannot be read");
                    return;
                }

                if (!_source.Any() || _source.Any(l => l == "-1"))
                {
                    Environment.ExitCode = 1;

                    Console.WriteLine("\nSource File(s) Cannot be read");
                    return;
                }

                string applicationExe;


                string dlangNativeExe;


                dlangNativeExe = outputFilename + "_d.exe";

                if (outputFilename == null)
                {
                    outputFilename = Path.GetFileNameWithoutExtension(filenames[0]);
                    dlangNativeExe = outDir + "/" + outputFilename + "_d.exe";

                }

                applicationExe = outputFilename + ".exe";

                applicationExe.DeleteFile();
                Program.Go(outDir, extraTranslations, outDir + "/" + outputFilename + ".exe", _source, applicationExe);

                if (outputtype == "exe")
                {
                    if (Verbose)
                        Console.WriteLine("Generating native exe: " + dlangNativeExe);
                    dlangNativeExe.DeleteFile();


                    var bclDir = pathToDcorlib;

                    if (String.IsNullOrEmpty(bclDir))
                    {
                        bclDir = Directory.GetParent(
                            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location + "..\\..\\..\\..\\..\\")) +
                                 "\\SharpNative\\DCorlib";
                    }

                    var bclFiles = Directory.GetFiles(bclDir, "*.d", SearchOption.AllDirectories)
                        .OrderBy(o => o)
                        .ToList();

                    var start = DateTime.Now;

                    var compilerOptions = CreateOptions();


                    NativeCompilationUtils.SetCompilerOptions(compilerOptions);

                    var outputFiles = Directory.GetFiles(outDir, "*.d", SearchOption.AllDirectories);

                    NativeCompilationUtils.CompileAppToNativeExe(
                        (outputFiles).Where(j => j.EndsWith(".d")).Union(bclFiles).ToArray(), dlangNativeExe);

                    if (Verbose)
                        Console.WriteLine("\nCompiling to binary took " + (DateTime.Now - start).TotalMilliseconds +
                                          " ms\n");

                    if (!String.IsNullOrEmpty(RunWithArgs))
                    {
                        if (Verbose)
                            Console.WriteLine("Running app " + dlangNativeExe + " with arguments: " + RunWithArgs);

                        var output = dlangNativeExe.ExecuteCommand(RunWithArgs);

                        Console.WriteLine(output);
                    }

                }

                if (outputtype == "library")
                {

                }


                Environment.ExitCode = 0;
            }
            catch (AggregateException agex)
            {
                Environment.ExitCode = 1;

                Exception ex = agex;
                while (ex is AggregateException)
                    ex = ex.InnerException;

                Console.WriteLine("\nException:");
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Environment.ExitCode = 1;

                Console.WriteLine("\nException:");
                Console.WriteLine(ex.Message);
            }
        }


        public static void RunTests(string outDir, string pathToFiles, string[] extraDefines,
            List<string> extraTranslations)
        {
            var options = CreateOptions();
            try
            {



                if (pathToFiles == null)
                    throw new Exception("/source parameter not passed");


                var filenames = Directory.GetFiles(Path.GetFullPath(pathToFiles), "*.cs");

                Console.WriteLine("\r\r" + ("Running all tests in : " + pathToFiles) + "\r\r");
                Func<string, string> strip = i => Regex.Replace(i ?? "", "[\r\n \t]+", " ").Trim();
                int count = 0;
                int passCount = 0;

                NativeCompilationUtils.SetCompilerOptions(options);
                    var bclDir = pathToDcorlib;

                    if (String.IsNullOrEmpty(bclDir))
                    {
                        bclDir = Directory.GetParent(
                            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location + "..\\..\\..\\..\\..\\")) +
                                 "\\SharpNative\\DCorlib";
                    }

                    var bclFiles = Directory.GetFiles(bclDir, "*.d", SearchOption.AllDirectories)
                        .OrderBy(o => o)
                        .ToList();
                var failedTestNames = new List<string>();
                foreach (var filename in filenames)
                {


                    var _source = FileExtensions.ReadFile(Path.GetFullPath(filename));
                    if (_source == null)
                    {
                        Environment.ExitCode = 1;

                        //throw new ArgumentNullException("\nSource Files Cannot be read");
                        return;
                    }

                    if (_source == "-1")
                    {
                        Environment.ExitCode = 1;

                        Console.WriteLine("\nSource File Cannot be read");
                        return;
                    }

                    var shortName = Path.GetFileName(filename);
                    Console.WriteLine("-------------------------Running Test: " + shortName +
                                      "-------------------------");

                    string applicationExe;


                    string dlangNativeExe;

                    if (String.IsNullOrEmpty(outDir))
                    {
                        outDir = TempDir;
                    }

                    dlangNativeExe = outputFilename + "_d.exe";
                    applicationExe = outputFilename + ".exe";


                    if (outputFilename == null)
                    {
                        outputFilename = Path.GetFileNameWithoutExtension(filename);
                        dlangNativeExe = outDir + "/" + outputFilename + "_d.exe";
                        applicationExe = outDir + "/" + outputFilename + ".exe";
                    }


                    applicationExe.DeleteFile();
                    Program.Go(outDir, extraTranslations, applicationExe,
                        new string[] { FileExtensions.ReadFile(filename) },
                        outputFilename + ".exe");


                    if (Verbose)
                        Console.WriteLine("Generating native exe: " + dlangNativeExe);

                    dlangNativeExe.DeleteFile();


                   

                    var start = DateTime.Now;



                    var outputFiles = Directory.GetFiles(outDir, "*.d", SearchOption.AllDirectories);

                    NativeCompilationUtils.CompileAppToNativeExe(
                        (outputFiles).Where(j => j.EndsWith(".d")).Union(bclFiles).ToArray(), dlangNativeExe);


                    Console.WriteLine("\nCompiling to binary took " + (DateTime.Now - start).TotalMilliseconds +
                                      " ms\n");


                    var CSharpOutput = applicationExe.ExecuteCommand(RunWithArgs);
                    var CppOutput = dlangNativeExe.ExecuteCommand(RunWithArgs);
                    Console.WriteLine(CSharpOutput);
                    Console.WriteLine(CppOutput);

                    count++;

                    if (count % 20 == 0)
                    {
                        GC.Collect();
                    }

                    if (strip(CSharpOutput) == strip(CppOutput))
                    {
                        passCount++;
                        Console.WriteLine("-------------------------Test {0} Passed:-------------------------\n",
                            shortName);
                    }
                    else
                    {
                        Console.WriteLine(String.Format("Test {0} Failed:\n", shortName));
                        failedTestNames.Add(Path.GetFileNameWithoutExtension(shortName));
                    }





                }

                Console.WriteLine(
                      String.Format("Summary \nTotal:{0} \nPass Rate:{1} \nPassed: {2} \nFailed: {3} {4}\n",
                          filenames.Count(), (passCount * 100) / ((float)filenames.Count()), passCount,
                          filenames.Count() - passCount,
                          (filenames.Count() - passCount == 0)
                              ? ""
                              : failedTestNames.Aggregate((k, j) => k + " , " + j)));

                PrepareTempDir();

                Environment.ExitCode = 0;

            }
            catch (AggregateException agex)
            {
                Environment.ExitCode = 1;

                Exception ex = agex;
                while (ex is AggregateException)
                    ex = ex.InnerException;

                Console.WriteLine("\nException:");
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Environment.ExitCode = 1;

                Console.WriteLine("\nException:");
                Console.WriteLine(ex.Message);
            }
        }

        private static NativeCompilationUtils.Options CreateOptions()
        {
            if (String.IsNullOrEmpty(pathToDstdlib))
            {
                pathToDstdlib = "C:\\D\\dmd2\\windows\\lib";
            }


            if (String.IsNullOrEmpty(pathToDcompiler))
            {
                pathToDcompiler = "C:\\D\\dmd2\\windows\\bin\\dmd";
            }

            if (String.IsNullOrEmpty(compilerOptions))
            {
                compilerOptions = "-inline -release -m64 -O";
            }

            if (Verbose)
            {
                Console.WriteLine("Compiling using:");
                Console.WriteLine("compiler: " + pathToDcompiler + " " + compilerOptions);
                Console.WriteLine("dcorlib:" + pathToDcorlib);
                Console.WriteLine("stdlib:" + pathToDstdlib);
            }

            if (pathToDcompiler.Contains("ldc2") || pathToDcompiler.Contains("ldmd2"))
                compilerOptions += " -oq "; // Allows similar names in different paths



            NativeCompilationUtils.Options options = new NativeCompilationUtils.Options();
            options.PathOfCompilerTools = Path.GetDirectoryName(pathToDcompiler) + "/";
            options.CompilerExe = Path.GetFileName(pathToDcompiler);
            options.OptimizationFlags = string.Format("  {1} \"-I{0}\" ", pathToDstdlib, compilerOptions);
            options.LinkerOptions = "";


            return options;
        }

        public static void CompileSolution(string outDir, string pathToSolution, string projects, string[] extraDefines,
            List<string> extraTranslations)
        {
            try
            {
                if (pathToSolution == null)
                    throw new Exception("/sln parameter not passed");
                var solution = MSBuildWorkspace.Create().OpenSolutionAsync(pathToSolution).Result;
                var projectsList = solution.Projects.ToList();
                if (projects != null)
                    TrimList(projectsList, projects);
                if (extraDefines.Length > 0)
                {
                    projectsList =
                        projectsList.Select(
                            p =>
                                p.WithParseOptions(
                                    new CSharpParseOptions(
                                        preprocessorSymbols:
                                            p.ParseOptions.As<ParseOptions>()
                                                .PreprocessorSymbolNames.Concat(
                                                    extraDefines.Where(z => z.StartsWith("-") == false))
                                                .Except(
                                                    extraDefines.Where(z => z.StartsWith("-"))
                                                        .Select(z => z.Substring(1)))
                                                .ToArray()))).ToList();
                }
                foreach (var project in projectsList)
                {
                    Console.WriteLine("Converting project " + project.Name + "...");
                    var sw = Stopwatch.StartNew();
                    Program.Go(project.GetCompilationAsync().Result, outDir, extraTranslations);
                    Console.WriteLine("Finished project " + project.Name + " in " + sw.Elapsed);
                }
                Environment.ExitCode = 0;
            }
            catch (AggregateException agex)
            {
                Environment.ExitCode = 1;

                Exception ex = agex;
                while (ex is AggregateException)
                    ex = ex.InnerException;

                Console.WriteLine("\nException:");
                Console.WriteLine(ex);
            }
            catch (Exception ex)
            {
                Environment.ExitCode = 1;

                Console.WriteLine("\nException:");
                Console.WriteLine(ex);
            }
        }

        const string extraTranslation = "/extraTranslation:";

        const string @out = "/out:";

        const string of = "/of:";

        const string sln = "/sln:";

        const string source = "/source:";

        const string _outputtype = "/outputtype:";

        private static string outputtype = "";
        private static string compilerOptions;
        private static string outputFilename;

        const string dcorlib = "/dcorlib:";
        private const string _dstdlib = "/dstdlib:";
        private const string _compiler = "/compiler:";
        private const string _compileroptions = "/compileroptions:";
        private const string _config = "/config:";
        private const string _projects = "/projects:";
        private const string _define = "/define:";
        private const string _verbose = "/v";
        private const string _run = "/run";
        private const string _test = "/test:";



        static string pathToDcorlib = "";
        static string pathToDstdlib = "";
        static string pathToDcompiler = "";
        public static bool Verbose = false;
        private static string RunWithArgs;
        private static string pathToTestFolder = "";
        private static readonly string TempDir = Path.Combine(Path.GetTempPath(), "SharpNative");

        public static void Main(string[] args)
        {

            try
            {
                if (args.Length == 0)
                    Console.WriteLine("C# to Dlang Converter v{0}\nhttp://github.com/afrogeek/SharpNative.", Constants.Version);

                if (args.Length == 0 || args.Any(o => o == "-?" || o == "--help" || o == "/?"))
                {
                    //Print usage
                    Console.WriteLine(Constants.Usage);
                    return;
                }


                PrepareTempDir();

                var outDir = "";
                var extraTranslations = new List<string>();
                string pathToSolution = null;
                string pathToSourceFiles = null;

                string config = null;
                string projects = null;
                var extraDefines = new string[] { };

                foreach (var arg in args)
                {
                    if (arg.StartsWith(extraTranslation))
                        extraTranslations.AddRange(arg.Substring(extraTranslation.Length).Split(';').Select(File.ReadAllText));
                    else if (arg.StartsWith(@out))
                        outDir = arg.Substring(@out.Length);
                    else if (arg.StartsWith(sln))
                        pathToSolution = arg.Substring(sln.Length);
                    else if (arg.StartsWith(_test))
                        pathToTestFolder = arg.Substring(_test.Length);
                    else if (arg.StartsWith(source))
                        pathToSourceFiles = arg.Substring(source.Length, arg.Length - source.Length);
                    else if (arg.StartsWith(dcorlib))
                        pathToDcorlib = arg.Substring(dcorlib.Length);
                    else if (arg.StartsWith(_outputtype))
                        outputtype = arg.Substring(_outputtype.Length);
                    else if (arg.StartsWith(_dstdlib))
                        pathToDstdlib = arg.Substring(_dstdlib.Length);
                    else if (arg.StartsWith(_compiler))
                        pathToDcompiler = arg.Substring(_compiler.Length);
                    else if (arg.StartsWith(_compileroptions))
                        compilerOptions = arg.Substring(_compileroptions.Length);
                    else if (arg.StartsWith(of))
                        outputFilename = arg.Substring(of.Length);
                    else if (arg.StartsWith(_verbose))
                        Verbose = true;
                    else if (arg.StartsWith(_run))
                    {

                        RunWithArgs = arg.Substring(_run.Length) + " ";
                        // Console.WriteLine("Running with args " +  RunWithArgs);
                    }

                    else if (arg.StartsWith(_config))
                        config = arg.Substring(_config.Length);
                    else if (arg.StartsWith(_projects))
                        projects = arg.Substring(_projects.Length);
                    else if (arg.StartsWith(_define))
                        extraDefines = arg.Substring(_define.Length)
                            .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    else
                        throw new Exception("Invalid argument: " + arg);
                }

                if (!String.IsNullOrEmpty(pathToTestFolder))
                {
                    RunTests(outDir, pathToTestFolder, extraDefines, extraTranslations);
                    return;
                }

                outDir = Directory.GetCurrentDirectory();

                if (!String.IsNullOrEmpty(pathToSolution))
                {
                    CompileSolution(outDir, pathToSolution, projects, extraDefines, extraTranslations);
                    return;
                }

                if (!String.IsNullOrEmpty(pathToSourceFiles))
                {
                    CompileFiles(outDir, pathToSourceFiles, extraDefines, extraTranslations);
                    return;
                }






            }
            catch (AggregateException agex)
            {
                Environment.ExitCode = 1;

                Exception ex = agex;
                while (ex is AggregateException)
                    ex = ex.InnerException;

                Console.WriteLine("\nException:");
                Console.WriteLine(ex);
            }
            catch (Exception ex)
            {
                Environment.ExitCode = 1;

                Console.WriteLine("\nException:");
                Console.WriteLine(ex);
            }
        }

        private static void PrepareTempDir()
        {
            if (!Directory.Exists(TempDir))
                Directory.CreateDirectory(TempDir);

            FileExtensions.DeleteFilesByWildcards("*.*", TempDir);
        }

        private static void TrimList(IList<Project> projectsList, string projectsCsv)
        {
            var split = projectsCsv.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            for (var i = 0; i < projectsList.Count; i++)
            {
                var si = split.IndexOf(projectsList[i].Name);
                if (si != -1)
                    split.RemoveAt(si);
                else
                {
                    projectsList.RemoveAt(i);
                    i--;
                }
            }

            if (split.Count > 0)
                throw new Exception("Project(s) not found: " + string.Join(", ", split));
        }
    }
}