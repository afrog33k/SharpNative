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
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.MSBuild;

#endregion

namespace SharpNative.Compiler
{
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
                var projectsList = new[] {theproject};
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

        public static void Main(string[] args)
        {
            try
            {
                Console.WriteLine(
                    "C# to Dlang Converter\nSee http://github.com/afrogeek/SharpNative for full info and documentation.\n\n");

                if (args.Length == 0 || args.Any(o => o == "-?" || o == "--help" || o == "/?"))
                {
                    //Print usage
                    Console.WriteLine(
                        @"

Usage:
    SharpNative.exe  /sln:<path to solution file> [options] 


Options available:

	/out:<output directory>
		Directory to write d files to.  If not specified, output will be written to the current working directory.

	/config:<configuration>
		The configuration within the passed solution file to use. (Debug, Release, etc.)

	/projects:<comma-delimited list of project names>
		If you don't want to convert all projects in the passed solution, you can provide a list of project names.  Only the projects named here will be converted.

	/extraTranslation:<path to xml file>
		Defines extra conversion parameters for use with this project.  See Translations.xml for examples.

	/define:<symbol>
		Adds extra pre-processor #define symbols to add to the project before building.
");
                    return;
                }

                var outDir = Directory.GetCurrentDirectory();
                var extraTranslations = new List<string>();
                string pathToSolution = null;
                string config = null;
                string projects = null;
                var extraDefines = new string[] {};

                foreach (var arg in args)
                {
                    if (arg.StartsWith("/extraTranslation:"))
                        extraTranslations.AddRange(arg.Substring(18).Split(';').Select(File.ReadAllText));
                    else if (arg.StartsWith("/out:"))
                        outDir = arg.Substring(5);
                    else if (arg.StartsWith("/sln:"))
                        pathToSolution = arg.Substring(5);
                    else if (arg.StartsWith("/config:"))
                        config = arg.Substring(8);
                    else if (arg.StartsWith("/projects:"))
                        projects = arg.Substring(10);
                    else if (arg.StartsWith("/define:"))
                        extraDefines = arg.Substring(8).Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
                    else
                        throw new Exception("Invalid argument: " + arg);
                }

                CompileSolution(outDir, pathToSolution, projects, extraDefines, extraTranslations);
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

        private static void TrimList(IList<Project> projectsList, string projectsCsv)
        {
            var split = projectsCsv.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).ToList();

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