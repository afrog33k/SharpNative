
using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;
using System.Threading;
using System.IO;
using System.Collections;
using System.Reflection;
using System.Text;
using System.Diagnostics;
using System.Text.RegularExpressions;
using SharpNative.Compiler;

namespace VisualCompiler
{



	public partial class MyDocument : MonoMac.AppKit.NSDocument
	{
		public static string LastCompiledExecutablePath {
			get;
			set;
		}

		// Called when created from unmanaged code
		public MyDocument (IntPtr handle) : base (handle)
		{
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public MyDocument (NSCoder coder) : base (coder)
		{
		}

		public override void WindowControllerDidLoadNib (NSWindowController windowController)
		{
			base.WindowControllerDidLoadNib (windowController);
			
			// Add code to here after the controller has loaded the document window

//			NSView view = new NSView ();
//			var scrollView = new NSScrollView ();
//			var clipView = new NSClipView ();
//
//			var textView = new NSTextView ();
//			clipView.AddSubview (textView);
//
//			scrollView.AddSubview (clipView);
//
//			view.AddSubview (scrollView);
//
//			windowController.Window.ContentView.AddSubview (view);

			//Xcode assistant editor aint working, so I'm resorting to finding views manually

			/*var textViews = windowController.Window.ContentView.FindViews<NSTextView> ();
			var count = 0;

			//foreach(var view in textViews)
			//{
			//	view.InsertText (new NSString("View: " + count++));
			//}//0: cshapr
			//1: output
			//2:Intermediate
			//3://CPP

			CSharpTextBox = textViews [0];
			ConsoleTextBox = textViews [1];
			IntermediateTextBox = textViews [2];
			CPPTextBox = textViews [3];
*/

			windowController.Window.Title = "Visual Compiler Mac";

			CppFileList.UsesDataSource = true;

			MainWindowViewModel.TempDir = Path.Combine (Path.GetTempPath (), "CsNative");

			if (!Directory.Exists (MainWindowViewModel.TempDir)) {
				Directory.CreateDirectory (MainWindowViewModel.TempDir);
			}


			//            Dispatcher.Invoke(() => Console.SetOut(new ControlWriter(Errors)));

			CompilerUtils.DeleteFilesByWildcards ("Test*.exe", MainWindowViewModel.TempDir);


			CSharpTextEditor.TextStorage.TextStorageDidProcessEditing += (object sender, EventArgs e) => {
				ViewModel.SourceCode = CSharpTextEditor.Value;
			};

			CPPTextEditor.TextStorage.TextStorageDidProcessEditing += (object sender, EventArgs e) => {
				ViewModel.OutputCode = CPPTextEditor.Value;
			};

			CSharpTextEditor.Value = VisualCompilerConstants.InitialCode;


			CPPRunButton.Activated += RunCPPButton_Click;

			OpenFileButton.Activated += OpenFile;

			CppFileList.Activated += SelectedFile;

			CSharpRunButton.Activated += RunCSharpButton_Click;

			TestButton.Activated += TestButton_Click;

			RunAllTests.Activated+= RunAllTests_Click;
	
		}


		string LastFolder;


		void RunAllTests_Click (object sender, EventArgs e)
		{
			var openPanel = new NSOpenPanel ();

			openPanel.CanChooseDirectories = true;
			openPanel.CanChooseFiles = false;
			openPanel.ReleasedWhenClosed = true;
			openPanel.Prompt = "Select Folder ... ";

			if (String.IsNullOrEmpty (LastFolder))
			{
				openPanel.DirectoryUrl = new NSUrl (TestsDir);
			}
			else
			{
				openPanel.DirectoryUrl = new NSUrl (LastFolder);
			}
			var result = openPanel.RunModal ();
			if (result == 1) {
				var filename = openPanel.Filename; // will switch to .Url later

				LastFolder = openPanel.Directory;

				Func<string, string> strip = i => Regex.Replace(i??"", "[\r\n \t]+", " ").Trim();

				ViewModel.CompilerErrors = "\r\r" + ("Running all tests in : " + filename) + "\r\r";
				int passCount = 0;
				//			if (ResetStatus != null)
				//				ResetStatus.Stop();
					List<string> failedTestNames = new List<string> ();
				ThreadPool.QueueUserWorkItem((h) =>
					{

					var allTests = Directory.EnumerateFiles(filename).Where(u=> Path.GetExtension(u)==".cs").ToList().CustomSort();
					int noRunTests = 0;
						foreach(var file in allTests)
						{

							var shortName = Path.GetFileName(file);
						Console.WriteLine("-------------------------Running Test: " + shortName+ "-------------------------");
						ViewModel.CompilerErrors += "-------------------------Running Test: " + shortName+ "-------------------------";
						var text = Extensions.ReadFile (file);

						if(text=="-1")
							break;

							InvokeOnMainThread(()=>{
								CSharpFilename.StringValue = Path.GetFileName (file);


								CSharpTextEditor.Value = text;
						
							});

						CSharpOutput ="Z..)";
						CppOutput ="A..)";
						noRunTests++;
//							ViewModel.CompileAndRunCode(shortName,File.ReadAllText(file));
						CompileCSharp(false);
						CompileD();


						if (strip(CSharpOutput) == strip(CppOutput))
						{
								passCount++;
							InvokeOnMainThread(() =>
								{
									//	TestStatus.Content = "PASSED";
									//TestStatus.Background =
									//	new SolidColorBrush(System.Windows.Media.Color.FromRgb(Color.GreenYellow.R,
									//		Color.GreenYellow.G, Color.GreenYellow.B));

								ViewModel.CompilerErrors += String.Format("-------------------------Test {0} Passed:-------------------------\n", shortName);
								});
							Console.WriteLine( String.Format("-------------------------Test {0} Passed:-------------------------\n", shortName));

						}
						else
						{
							InvokeOnMainThread(() =>
								{
									//								TestStatus.Content = "FAILED";
									//								TestStatus.Background =
									//									new SolidColorBrush(System.Windows.Media.Color.FromRgb(Color.Red.R, Color.Red.G, Color.Red.B));
//										ViewModel.CompilerErrors += String.Format("Test {2} Failed:\n\nCSharp Output:\n{0}\nD Output:\n{1}",
//											CSharpOutput, CppOutput, shortName);
										ViewModel.CompilerErrors += String.Format("Test {0} Failed:\n", shortName);
								Console.WriteLine( String.Format("-------------------------Test {0} Failed:-------------------------\n", shortName));

										failedTestNames.Add(Path.GetFileNameWithoutExtension(shortName));

								});
						}
						//					if (ResetStatus == null)
						//					{
						//						ResetStatus = new Timer(2000);
						//						ResetStatus.AutoReset = false;
						//						ResetStatus.Elapsed += (o, args) => Dispatcher.Invoke(() =>
						//							{
						//								TestStatus.Content = "TEST STATUS";
						//								TestStatus.Background =
						//									new SolidColorBrush(System.Windows.Media.Color.FromRgb(Color.White.R, Color.White.G,
						//										Color.White.B));
						//							});
						//					}
						//					ResetStatus.Start();
						}

					ViewModel.CompilerErrors += String.Format("Summary \nTotal:{0} \nPass Rate:{1} \nPassed: {2} \nFailed: {3} {4}\n", noRunTests, (passCount*100)/((float)noRunTests), passCount, noRunTests-passCount, (noRunTests-passCount ==0)?"":failedTestNames.Aggregate((k,j)=>k+" , "+j));

					});
					


			}
		}

		void SelectedFile (object sender, EventArgs e)
		{
			if (CppFileList.SelectedIndex != -1) 
			{

				var filename = ViewModel.FileList [(CppFileList.SelectedIndex)]; // will switch to .Url later
				if (File.Exists (filename.Location)) 
				{
					CPPFilename.StringValue = Path.GetFileName (filename.Location);
					CPPTextEditor.Value = Extensions.ReadFile (filename.Location);
				}
			}
		}

		void OpenFile (object sender, EventArgs e)
		{
			var openPanel = new NSOpenPanel ();

			openPanel.ReleasedWhenClosed = true;
			openPanel.Prompt = "Select File ... ";
			if (String.IsNullOrEmpty (LastFolder))
			{
				openPanel.DirectoryUrl = new NSUrl (TestsDir);
			}
			else
			{
				openPanel.DirectoryUrl = new NSUrl (LastFolder);
			}
			var result = openPanel.RunModal ();
			if (result == 1) {
				var filename = openPanel.Filename; // will switch to .Url later

				LastFolder = openPanel.Directory;// will switch to .Url later


				if (Path.GetExtension (filename) == ".cs") {
					var newText = Extensions.ReadFile (filename);
					CSharpFilename.StringValue = Path.GetFileName (filename);
					CSharpTextEditor.Replace (new NSRange (0, CSharpTextEditor.Value.Length), newText);
					ViewModel.SourceCode = newText;
					ThreadPool.QueueUserWorkItem ((h) => {

						ViewModel.RecompileSource ();
					});
				}
				else if ((Path.GetExtension (filename) == ".sln")||(Path.GetExtension (filename) == ".csproj")) 
				{
					var newText = Extensions.ReadFile (filename);
					CSharpFilename.StringValue = Path.GetFileName (filename);
					CSharpTextEditor.Replace (new NSRange (0, CSharpTextEditor.Value.Length), newText);
					ViewModel.SourceCode = newText;
					ThreadPool.QueueUserWorkItem ((h) => {
						ViewModel.RecompileSolution (filename);
					});

//					CompileSolution
				}
				else
					Console.WriteLine("Cannot Load Type " +  Path.GetExtension(filename));
			
			}
		}



		public void Update ()
		{
			ViewModel.RecompileSource ();
		}
			







		//		CompletionWindow completionWindow;



	

		private readonly string[] ILKeywords = {
			"LoadFunction","Call","Return","Assignment","BranchOperator","NewObject",
			"SetField","AlwaysBranch", "Label", "BinaryOperator","GetField", "CallVirtual"

		};

		private readonly string[] ILOperatorKeywords =
		{
			"add", "mul", "div", "sub", "branch"
		};

		#region Folding
//		FoldingManager foldingManager;
//		AbstractFoldingStrategy foldingStrategy;
//
//
//
//		void foldingUpdateTimer_Tick(object sender, EventArgs e)
//		{
//			if (foldingStrategy != null)
//			{
//				foldingStrategy.UpdateFoldings(foldingManager, TextEditor.Document);
//			}
//		}
		#endregion

		private void RunCSharpButton_Click(object sender, EventArgs args)
		{

			this.ViewModel.CompilerErrors = String.Empty;
			ThreadPool.QueueUserWorkItem((h) =>
				CompileCSharp (true));
		}

		private void CompileCSharp(bool clearoutput)
		{
			try
			{
				CSharpOutput = "";

//				TextWriter originalConsoleOutput = Console.Out;
			//	StringWriter writer = new StringWriter();
			//	Console.SetOut(writer);





				var start = Environment.TickCount;
//				Console.WriteLine("Running: " + LastCompiledExecutablePath);

//				this.Execute(LastCompiledExecutablePath);

				var output = ("/bin/bash").ExecuteCommand(" -c \"/usr/bin/mono -O=all --gc=sgen " + Driver.LastBuildPath +"\"","");
				//Console.WriteLine(output);

				var end = Environment.TickCount - start;

				this.ViewModel.CompilerErrors +=  String.Format("CS time: {0} ms\n{1}", end,output);
				//this.ViewModel.CompilerErrors +=  output;


//				Console.SetOut(originalConsoleOutput);
			//	string result = writer.ToString() + output;
				CSharpOutput = output;
				Console.WriteLine(output);
//				this.ViewModel.CompilerErrors += (CSharpOutput);
			}
			catch (Exception ex)
			{

				this.ViewModel.CompilerErrors += ex.Message + "\n" + ex.StackTrace;
			}
		}

		public string CSharpOutput;
		public string CppOutput;
		private IList _selectedCapabilities;

		/// <summary>
		/// This gets executed in the temporary appdomain.
		/// No error handling to simplify demo.
		/// </summary>
		public void Execute(string exeName)
		{
			AppDomain appDomain = AppDomain.CreateDomain(exeName.Replace("/",""));

			// load the bytes and run Main() using reflection
			// working with bytes is useful if the assembly doesn't come from disk
//			byte[] bytes = File.ReadAllBytes(exeName);
			Assembly assembly = Assembly.LoadFile(exeName);
			MethodInfo main = assembly.EntryPoint;
			main.Invoke(null, null);
			AppDomain.Unload(appDomain);
		}

		MainWindowViewModel f = null;

		public MainWindowViewModel ViewModel
		{
			get
			{

				if (f == null) {

					this.InvokeOnMainThread (() => {
						f = new MainWindowViewModel ()
						{
							Window = this 
						};
					});
				}
					return f;
				
			}
		}

		private void RunCPPButton_Click(object sender, EventArgs args)
		{



			ViewModel.CompilerErrors = String.Empty;
			ThreadPool.QueueUserWorkItem((h) =>
				CompileD ());



		}

		public static string BaseDir =  Path.GetFullPath (Assembly.GetExecutingAssembly().Location+ "/../../../../../../../");

		public static string BCLDir =  BaseDir+"/DCorlib/";

		public static string TestsDir =  BaseDir+"Tests/";

		public static string LastCompiledExecutable;
		private void CompileD()
		{


		
			if (!Directory.Exists (BCLDir)) {

				InvokeOnMainThread(() =>
					{
						ViewModel.CompilerErrors += String.Format("Please set correct BCL directory, current value \"{0}\" doesn't exist", BCLDir);
					});
				Console.WriteLine ("Please set correct BCL directory, current value {0} doesnt exist", BCLDir);
				return;
			}
				;


			//sb.ToFile(outputcpp);
			// @"C:\Program Files (x86)\Microsoft Visual Studio 12.0\VC\bin\vcvars32.bat".ExecuteCommand("",@"C:\Program Files (x86)\Microsoft Visual Studio 12.0\VC\bin\");
		

			var outputexe = MainWindowViewModel.TempDir + "/"+ LastCompiledExecutable.Replace(".exe", ".d").Replace(".d", "_d.exe");

			try
			{
				var start = DateTime.Now;
				//  var sb = new StringBuilder(ViewModel.OutputCode);

				//var bclDir = BCLDir;

					//Directory.GetParent(
					//	Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location + "..\\..\\..\\..\\CsNative")) +
					//"\\CsNative\\Runtime";
				//sb.ToFile(outputcpp);
				// @"C:\Program Files (x86)\Microsoft Visual Studio 12.0\VC\bin\vcvars32.bat".ExecuteCommand("",@"C:\Program Files (x86)\Microsoft Visual Studio 12.0\VC\bin\");
				var bclFiles = Directory.GetFiles(BCLDir, "*.d", SearchOption.AllDirectories)
					.OrderBy(o => o)
					.ToList();

				NativeCompilationUtils.SetCompilerOptions("ldcmac");
				NativeCompilationUtils.CompilerOptions.OptimizationFlags += " -I" + MainWindowViewModel.TempDir + " -I" + BCLDir;
				var fileList = ViewModel.FileList;
				var executableFiles = fileList.Where (j => j.Name.EndsWith (".d", StringComparison.Ordinal)).Select (k => k.Location);

				NativeCompilationUtils.CompileAppToNativeExe(executableFiles.Union(bclFiles).ToArray(), outputexe);


				ViewModel.CompilerErrors += ("\nCompiling to binary took " + (DateTime.Now - start).TotalMilliseconds + " ms\n");
				start =DateTime.Now;

				// Do not wait for the child process to exit before
				// reading to the end of its redirected stream.
				// p.WaitForExit();
				// Read the output stream first and then wait.
				string output = outputexe.ExecuteCommand("");

				var end = DateTime.Now - start;

				CppOutput = output;
				InvokeOnMainThread(() =>
					{
						ViewModel.CompilerErrors += String.Format("D time: {0} ms\n", end.TotalMilliseconds) + output ;
					Console.WriteLine(String.Format("D time: {0} ms\n", end.TotalMilliseconds) + output);
					});
			}
			catch (Exception ex)
			{

				InvokeOnMainThread(() =>
					{
						ViewModel.CompilerErrors += ex.Message + "\nStackTrace: \n" +
							ex.StackTrace;
					Console.WriteLine(ex.Message + "\nStackTrace: \n" +
						ex.StackTrace);
					});

			}
			finally
			{
				if (File.Exists(outputexe))
					File.Delete(outputexe);

			}

		}

		private void TestButton_Click(object sender, EventArgs e)
		{
			ViewModel.CompilerErrors = "";

			Func<string, string> strip = i => Regex.Replace(i, "[\r\n \t]+", " ").Trim();

//			if (ResetStatus != null)
//				ResetStatus.Stop();
			ThreadPool.QueueUserWorkItem((h) =>
				{
					CompileCSharp(false);
					CompileD();


					if (strip(CSharpOutput) == strip(CppOutput))
					{
						InvokeOnMainThread(() =>
							{
								//	TestStatus.Content = "PASSED";
								//TestStatus.Background =
								//	new SolidColorBrush(System.Windows.Media.Color.FromRgb(Color.GreenYellow.R,
								//		Color.GreenYellow.G, Color.GreenYellow.B));
						Console.WriteLine(String.Format("Test Passed:\n\nCSharp Output:\n{0}\nD Output:\n{1}",
							CSharpOutput, CppOutput));
								ViewModel.CompilerErrors = String.Format("Test Passed:\n\nCSharp Output:\n{0}\nD Output:\n{1}",
									CSharpOutput, CppOutput);
							});
					}
					else
					{
						InvokeOnMainThread(() =>
							{
//								TestStatus.Content = "FAILED";
//								TestStatus.Background =
//									new SolidColorBrush(System.Windows.Media.Color.FromRgb(Color.Red.R, Color.Red.G, Color.Red.B));
								ViewModel.CompilerErrors = "Test Failed\n" + ViewModel.CompilerErrors;
						Console.WriteLine(String.Format("Test Failed:\n\nCSharp Output:\n{0}\nD Output:\n{1}",
							CSharpOutput, CppOutput));
							});
					}
//					if (ResetStatus == null)
//					{
//						ResetStatus = new Timer(2000);
//						ResetStatus.AutoReset = false;
//						ResetStatus.Elapsed += (o, args) => Dispatcher.Invoke(() =>
//							{
//								TestStatus.Content = "TEST STATUS";
//								TestStatus.Background =
//									new SolidColorBrush(System.Windows.Media.Color.FromRgb(Color.White.R, Color.White.G,
//										Color.White.B));
//							});
//					}
//					ResetStatus.Start();
				});
		}

//		private Timer ResetStatus;


		private void OnShowCompilerOptions()
		{
//			var optionsWindow = new CompilerOptionsWindow();
//			optionsWindow.Owner = this;
//			optionsWindow.ShowDialog();
//			if (!optionsWindow.ViewModel.Accepted)
//				return;
//			ViewModel.OptimizationList.Clear();
//			ViewModel.OptimizationList.AddRange(optionsWindow.ViewModel.Capabilities);

		}

		private void OnFileOpen()
		{
//			Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
//			dialog.DefaultExt = ".cs";
//			dialog.Filter = "CSharp Source Code (.cs)|*.cs";
//			var folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Tests\\");
//			dialog.InitialDirectory = folder;
//
//
//			bool? result = dialog.ShowDialog();
//			if (result == true)
//			{
//				TextEditor.Text = File.ReadAllText(dialog.FileName);
//			}
//			else
//			{
//
//			}


		}

//		public NSTextView CSharpTextBox;
//		public NSTextView ConsoleTextBox;
//		public NSTextView IntermediateTextBox;
//		public NSTextView CPPTextBox;




		
		//
		// Save support:
		//    Override one of GetAsData, GetAsFileWrapper, or WriteToUrl.
		//
		
		// This method should store the contents of the document using the given typeName
		// on the return NSData value.
		public override NSData GetAsData (string documentType, out NSError outError)
		{
			outError = NSError.FromDomain (NSError.OsStatusErrorDomain, -4);
			return null;
		}
		
		//
		// Load support:
		//    Override one of ReadFromData, ReadFromFileWrapper or ReadFromUrl
		//
		public override bool ReadFromData (NSData data, string typeName, out NSError outError)
		{
			outError = NSError.FromDomain (NSError.OsStatusErrorDomain, -4);
			return false;
		}

		// If this returns the name of a NIB file instead of null, a NSDocumentController
		// is automatically created for you.
		public override string WindowNibName { 
			get {
				return "MyDocument";
			}
		}

	}
}

