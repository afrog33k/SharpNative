using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using Microsoft.CSharp;
using SharpNative;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using Microsoft.CSharp.RuntimeBinder;
using System.Linq;
using SharpNative.Compiler;


namespace VisualCompiler
{
    public class MainWindowViewModel 
    {
		public MyDocument Window;
		public static string TempDir;

        private static CSharpCodeProvider codeProvider = new CSharpCodeProvider();


        public static CompilerParameters parameters = new CompilerParameters()
        {
            GenerateExecutable = true,
			GenerateInMemory =true
			//IncludeDebugInformation =false,

        };

      
		public void CompileAndRunCode(string name, string code, string slnPath=null)
        {
			CompilerErrors = string.Empty;
			var outputExeName = "Test" +DateTime.Now.Ticks + name + ".exe";
			var outputNativeName = "TestNative" + name + ".exe";
			string testName = "Test";
			MyDocument.LastCompiledExecutable = outputExeName;
			MyDocument.LastCompiledExecutablePath = TempDir + "/" + outputExeName;//parameters.OutputAssembly;
			Console.WriteLine (MyDocument.LastCompiledExecutablePath);
            try
            {


           
            //Structure: Generate File, Compile  and Run It

            //Call Mcs / Csc
           
//            parameters.OutputAssembly = outputExeName;
//					try
//					{
//				parameters.OutputAssembly = TempDir + "\\" + outputExeName;
//				parameters.CompilerOptions = " /unsafe";
//
//
//				CompilerResults results = codeProvider.CompileAssemblyFromSource(parameters, code);
//			
//            if (results.Errors.Count > 0)
//            {
//                
//
//                foreach (var error in results.Errors)
//                {
//                    CompilerErrors += "\n" + error;
//                }
//                
//
//                return;
//            }
//					}catch(Exception ex) {
//					}


          
            }
            catch (Exception ex)
            {

                CompilerErrors += "\n" + ex.Message + ex.StackTrace;
            }

			//Invoke CsNative
			CallCompiler(outputExeName, slnPath);

        }


		public  void CallCompiler(string name, string slnPath = null)
		{

			CompileCode(name, new[] { SourceCode }, slnPath);
			//TODO: Make this call  the different compilers i.e. TestHelloWorld_GCC.exe etc...

		}
			

		private static string[] GetFiles(string sourceFolder, string filters, SearchOption searchOption)
		{
			return filters.Split('|').SelectMany(filter => Directory.GetFiles(sourceFolder, filter, searchOption)).ToArray();
		}


		public  void CompileCode(string testName, IEnumerable<string> cSharp, string slnPath=null)
		{

			try
			{
			DateTime csnAtiveStartTime = DateTime.Now;

			if (Directory.Exists(TempDir))
			{
				foreach (var existing in GetFiles(TempDir, "*.d|*.o|*.exe", SearchOption.AllDirectories))
				{
					File.SetAttributes(existing, FileAttributes.Normal); //clear read only flag so we can delete it
					File.Delete(existing);
				}
			}

			Console.WriteLine("Parsing into " + TempDir);


			if (slnPath == null) {
		
				
			
					Program.Go (TempDir, new String[] { }, MyDocument.LastCompiledExecutablePath,cSharp,testName);
				



			} else {

				//Workspaces dont seem to work on mono :(

				//Driver.CompileSolution (TempDir, slnPath, null, new string[]{}, null);
				//TODO: find a better solution to this ... use monodevelop / xbuild ?
				var files = Directory.GetFiles(Path.GetDirectoryName(slnPath), "*.cs", SearchOption.AllDirectories)//.Union(Directory.GetFiles(TempDir, "*.h", SearchOption.AllDirectories))
//						.Where(o=>Path.GetFileName(o)!="AssemblyInfo.cs")
						.Where(o=>File.Exists(o))
						.OrderBy(o => o)
						.Select(o=>Extensions.ReadFile(o))
					.ToList();

//				var compilation = CSharpCompilation.Create (testName, files.Select (o => CSharpSyntaxTree.ParseText (o)), new MetadataReference[] 
//					{
//					new MetadataImageReference (AssemblyMetadata.CreateFromImageStream (new FileStream (typeof(object).Assembly.Location, FileMode.Open, FileAccess.Read))),
//					new MetadataImageReference (AssemblyMetadata.CreateFromImageStream (new FileStream (typeof(Enumerable).Assembly.Location, FileMode.Open, FileAccess.Read))), // Linq
//					new MetadataImageReference (AssemblyMetadata.CreateFromImageStream (new FileStream (typeof(RuntimeBinderException).Assembly.Location, FileMode.Open, FileAccess.Read))),
//					new MetadataImageReference (AssemblyMetadata.CreateFromImageStream (new FileStream (typeof(Microsoft.CSharp.CSharpCodeProvider).Assembly.Location, FileMode.Open, FileAccess.Read))),
//						}, new CSharpCompilationOptions (OutputKind.ConsoleApplication, allowUnsafe: true));

					Program.Go ( TempDir, new String[] { }, MyDocument.LastCompiledExecutablePath, files, testName);

			}


			Func<string, string> strip = i => Regex.Replace (i, "[\r\n \t]+", " ").Trim ();


			//Copy BCL Files to output folder

			var filesFromDisk = Directory.GetFiles(TempDir, "*.d", SearchOption.AllDirectories)//.Union(Directory.GetFiles(TempDir, "*.h", SearchOption.AllDirectories))
				.Where(o => Path.GetFileName(o) != "Constructors.d")
				// .Select(File.ReadAllText)
				//.Select(strip)                
				.OrderBy(o => o)
				.ToList();



			FileList = filesFromDisk.Select(f => new FileItem()
				{
					Name = Path.GetFileName(f),
					Location = f
				}).ToList();
			string first = filesFromDisk.FirstOrDefault();

			if (first != null)
					OutputCode = Extensions.ReadFile(first);


			CompilerErrors += ("\nCsNative took " + (DateTime.Now - csnAtiveStartTime).TotalMilliseconds + "ms\n");

			}
			catch (AggregateException agex)
			{
				Environment.ExitCode = 1;

				Exception ex = agex;
				while (ex is AggregateException)
					ex = ex.InnerException;

				CompilerErrors += ("\nException: "  + ex + "\n");
//				Console.WriteLine("\nException:");
//				Console.WriteLine(ex);
			}
			catch (Exception ex)
			{
//				Environment.ExitCode = 1;

				CompilerErrors += ("\nException: "  + ex + "\n");

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
              //  if (_sourceCode != value)
                {
                   
                    _sourceCode = value;
                 
                    RecompileSource();
                }
            }
        }

        public void RecompileSource()
        {
           
            OutputCode = "";
            ILCode = "";
            Recompile();
//            Changed(() => SourceCode);
        }

		public void RecompileSolution(string slnPath)
		{

			OutputCode = "";
			ILCode = "";
			Recompile(slnPath);
			//            Changed(() => SourceCode);
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
					Window.InvokeOnMainThread(() =>
                    {
//							Window.IntermediateText.TextStorage.DeleteRange(new MonoMac.Foundation.NSRange(0,Window.IntermediateText.TextStorage.Length));
//							Window.IntermediateText.TextStorage.Insert(new MonoMac.Foundation.NSAttributedString(value),0);
                    });
//                    Changed(() => ILCode);
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
					Window.InvokeOnMainThread(() =>
                    {
							if (OutputCode != Window.CPPTextEditor.Value) //Seems there was a loop here
								Window.CPPTextEditor.Value = value;
							Window.CPPTextEditor.NeedsDisplay = true;
                      });
//                        Changed(() => OutputCode);
                    

                }
            }
        }

        private string _compilerErrors;
        public string CompilerErrors
        {
            get { return _compilerErrors; }
            set
            {
				Window.BeginInvokeOnMainThread(() =>
                {
                    _compilerErrors = value;
						if(Window.ConsoleText.Value!=value)
							Window.ConsoleText.Value =value;
//                    Changed(() => CompilerErrors);
                });
            }
        }

		static	List<FileItem> fileList;
		public List<FileItem> FileList {
			get {
				return fileList;
			}
			set {

					fileList = value;

				Window.BeginInvokeOnMainThread(() =>
					{


						Window.CppFileList.DataSource = new CppFileDataSource(fileList);
					

//						if(Window.CppFileList.!=value)
//							Window.ConsoleText.Value =value;
						//                    Changed(() => CompilerErrors);
					});

			}
		}

     



        public MainWindowViewModel( )
        {


           
          
        }

		public void Recompile(string slnPath = null)
        {

			CompileAndRunCode("HelloWorld",SourceCode, slnPath);

        }

    }
}