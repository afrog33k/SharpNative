using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CSharp;
using Microsoft.CSharp.RuntimeBinder;
using SharpNative.Compiler;

namespace CsNativeVisual
{
    public class MainWindowViewModel : NotificationViewModel
    {
        public MainWindow Window;
        private static CSharpCodeProvider codeProvider = new CSharpCodeProvider();
        public static ICodeCompiler Icc = codeProvider.CreateCompiler();

        public static CompilerParameters Parameters = new CompilerParameters()
        {
            GenerateExecutable = true,

        };


        public void CompileAndRunCode(string name, string code, string slnPath = null)
        {
            //  try
            {


                CompilerErrors = string.Empty;
                var outputExeName = "Test" + DateTime.Now.Ticks + name + ".exe";
                var outputNativeName = "TestNative" + name + ".exe";

                //Structure: Generate File, Compile  and Run It

                //Call Mcs / Csc
                Window.LastCompiledExecutable = outputExeName;


                string testName = "Test";

                Parameters.OutputAssembly = TempDir + "/" + outputExeName;
                Parameters.CompilerOptions += " /unsafe";

                Window.LastCompiledExecutablePath = Parameters.OutputAssembly;

                CompilerResults results = Icc.CompileAssemblyFromSource(Parameters, code);

                bool hasError = false;
                if (results.Errors.Count > 0)
                {


                    foreach (CompilerError error in results.Errors)
                    {
                        CompilerErrors += "\n" + error;
                        if (!error.IsWarning && error.ErrorNumber!= "CS5001")
                            hasError = true;
                    }

                    if (hasError)
                    return;
                }

                //Invoke CRCC
                CallCompiler(outputExeName, slnPath);

            }
            //  catch (Exception ex)
            {

                //       CompilerErrors += "\n" + ex.Message + ex.StackTrace;
            }
        }


        public void CallCompiler(string name, string slnPath = null)
        {

            CompileCode(name, new[] { SourceCode }, slnPath);
            //TODO: Make this call  the different compilers i.e. TestHelloWorld_GCC.exe etc...

        }

        private static string[] GetFiles(string sourceFolder, string filters, SearchOption searchOption)
        {
            return filters.Split('|').SelectMany(filter => Directory.GetFiles(sourceFolder, filter, searchOption)).ToArray();
        }


        public void CompileCode(string testName, IEnumerable<string> cSharp, string slnPath = null)
        {

            try
            {
                DateTime csnAtiveStartTime = DateTime.Now;

                if (Directory.Exists(TempDir))
                {
//                    foreach (var existing in GetFiles(TempDir, "*.d", SearchOption.AllDirectories))
//                    {
//                        File.SetAttributes(existing, FileAttributes.Normal); //clear read only flag so we can delete it
//                        File.Delete(existing);
//                    }
//                    foreach (var existing in GetFiles(TempDir, "*.obj", SearchOption.AllDirectories))
//                    {
//                        File.SetAttributes(existing, FileAttributes.Normal); //clear read only flag so we can delete it
//                        File.Delete(existing);
//                    }

                    // Directory.Delete(TempDir,true);
                }

                Console.WriteLine("Parsing into " + TempDir);


                if (slnPath == null)
                {



                    Program.Go(TempDir, new String[] { }, Window.LastCompiledExecutablePath, cSharp, testName);




                }
                else
                {
                    //Workspaces dont seem to work on mono :(
                    if (Path.GetExtension(slnPath)==".csproj")
                    {
                        Driver.CompileProject(TempDir, slnPath, null, new string[] { "MONOTOUCH" }, null);

                    }
                    else
                    {
                        Driver.CompileSolution(TempDir, slnPath, null, new string[] { }, null);

                    }
                    /*  //TODO: find a better solution to this ... use monodevelop / xbuild ?
                      var files = Directory.GetFiles(Path.GetDirectoryName(slnPath), "*.cs", SearchOption.AllDirectories)//.Union(Directory.GetFiles(TempDir, "*.h", SearchOption.AllDirectories))
                                                                                                                         //						.Where(o=>Path.GetFileName(o)!="AssemblyInfo.cs")
                          .Where(o => File.Exists(o))
                              .OrderBy(o => o)
                              .Select(o => File.ReadAllText(o))
                          .ToList();

                      //				var compilation = CSharpCompilation.Create (testName, files.Select (o => CSharpSyntaxTree.ParseText (o)), new MetadataReference[] 
                      //					{
                      //					new MetadataImageReference (AssemblyMetadata.CreateFromImageStream (new FileStream (typeof(object).Assembly.Location, FileMode.Open, FileAccess.Read))),
                      //					new MetadataImageReference (AssemblyMetadata.CreateFromImageStream (new FileStream (typeof(Enumerable).Assembly.Location, FileMode.Open, FileAccess.Read))), // Linq
                      //					new MetadataImageReference (AssemblyMetadata.CreateFromImageStream (new FileStream (typeof(RuntimeBinderException).Assembly.Location, FileMode.Open, FileAccess.Read))),
                      //					new MetadataImageReference (AssemblyMetadata.CreateFromImageStream (new FileStream (typeof(Microsoft.CSharp.CSharpCodeProvider).Assembly.Location, FileMode.Open, FileAccess.Read))),
                      //						}, new CSharpCompilationOptions (OutputKind.ConsoleApplication, allowUnsafe: true));

                      Program.Go(TempDir, new String[] { }, Window.LastCompiledExecutablePath, files, testName);*/
                }



                Func<string, string> strip = i => Regex.Replace(i, "[\r\n \t]+", " ").Trim();


                //Copy BCL Files to output folder

                var filesFromDisk = Directory.GetFiles(TempDir, "*.d", SearchOption.AllDirectories)
                .OrderBy(o => o)
                    .ToList();



                FileList = filesFromDisk.Select(f => new FileItem()
                {
                    Name = Path.GetFileName(f),
                    Location = f
                }).ToList();

                

                string first = filesFromDisk.FirstOrDefault();

                if (first != null)
					OutputCode = FileExtensions.ReadFile(first);

                CompilerErrors += ("\nCsNative took " + (DateTime.Now - csnAtiveStartTime).TotalMilliseconds + "ms\n");

            }
            catch (AggregateException agex)
            {
                Environment.ExitCode = 1;

                Exception ex = agex;
                while (ex is AggregateException)
                    ex = ex.InnerException;

                CompilerErrors += ("\nException: " + ex + "\n");
                //				Console.WriteLine("\nException:");
                //				Console.WriteLine(ex);
            }
            catch (Exception ex)
            {
                //				Environment.ExitCode = 1;

                CompilerErrors += ("\nException: " + ex + "\n");

                //				Console.WriteLine("\nException:");
                //				Console.WriteLine(ex);
            }
        }

        private string _sourceCode;
        public string SourceCode
        {
            get { return _sourceCode; }
            set
            {
                if (_sourceCode != value)
                {

                    _sourceCode = value;

                    RecompileSource();
                }
            }
        }

        public void RecompileSource(string slnPath=null)
        {

            OutputCode = "";
            ILCode = "";
            Recompile(slnPath);
            Changed(() => SourceCode);
        }


        private string _ilCode;
        public string ILCode
        {
            get { return _ilCode; }
            set
            {
                if (_ilCode != value)
                {
                    _ilCode = value;
                    Window.Dispatcher.InvokeAsync(() =>
                    {
                        //                        Window.IL.Text = value;
                    });
                    Changed(() => ILCode);
                }
            }
        }


        private List<FileItem> _fileList;
        public List<FileItem> FileList
        {
            get { return _fileList; }
            set
            {
                if (_fileList != value)
                {

                    _fileList = value;
                    Window.Dispatcher.Invoke(() =>
                         {
                             if (OutputCode != Window.Output.Text) //Seems there was a loop here
                             Window.CppFileList.ItemStringFormat = "";   
                                Window.CppFileList.ItemsSource = value;
                              Window.CppFileList.SelectedItem = value.FirstOrDefault();

                    });
                    Changed(() => OutputCode);


                }
            }
        }


        private string _outputCode;
        public string OutputCode
        {
            get { return _outputCode; }
            set
            {
                if (_outputCode != value)
                {

                    _outputCode = value;
                    Window.Dispatcher.Invoke(() =>
                    {
                        if (OutputCode != Window.Output.Text) //Seems there was a loop here
                            Window.Output.Text = value;
                    });
                    Changed(() => OutputCode);


                }
            }
        }

        private string _compilerErrors;
        public static string TempDir;

        public string CompilerErrors
        {
            get { return _compilerErrors; }
            set
            {
                Window.Dispatcher.Invoke(() =>
                {
                    _compilerErrors = value;
                    Changed(() => CompilerErrors);
                });
            }
        }

        public List<string> OptimizationList { get; set; }



        public MainWindowViewModel()
        {


            OptimizationList = new List<string>();

        }

        public void Recompile(string slnPath=null)
        {

            CompileAndRunCode("HelloWorld", SourceCode,slnPath);

        }

    }
}