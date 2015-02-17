using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using CsNativeVisual.Views.Dialogs;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;
using MahApps.Metro.Controls;
using Ookii.Dialogs.Wpf;
using SharpNative.Compiler;
using Color = System.Drawing.Color;
using Timer = System.Timers.Timer;

namespace CsNativeVisual
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {



        public MainWindow()
        {


            InitializeComponent();

            MainWindowViewModel.TempDir = Path.Combine(Path.GetTempPath(), "SharpNative");

            if (!Directory.Exists(MainWindowViewModel.TempDir))
            {
                Directory.CreateDirectory(MainWindowViewModel.TempDir);
            }


            //            Dispatcher.Invoke(() => Console.SetOut(new ControlWriter(Errors)));

            CompilerUtils.DeleteFilesByWildcards("Test*.exe", MainWindowViewModel.TempDir);

            ViewModel.Window = this;


            TextEditor.Text = ViewModel.SourceCode;
            TextEditor.TextArea.TextEntering += textEditor_TextArea_TextEntering;
            TextEditor.TextArea.TextEntered += textEditor_TextArea_TextEntered;
            TextEditor.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.CSharp.CSharpIndentationStrategy(TextEditor.Options);
            foldingStrategy = new BraceFoldingStrategy();

            DispatcherTimer foldingUpdateTimer = new DispatcherTimer();
            foldingUpdateTimer.Interval = TimeSpan.FromSeconds(2);
            foldingUpdateTimer.Tick += foldingUpdateTimer_Tick;
            foldingUpdateTimer.Start();


            if (foldingManager == null)
                foldingManager = FoldingManager.Install(TextEditor.TextArea);
            foldingStrategy.CreateNewFoldings(TextEditor.Document);

            TextEditor.Text = VisualCompilerConstants.InitialCode;
            CppFileList.SelectionChanged += (sender, args) =>
            {
                var list = (ListBox) sender;

                if (list.SelectedItem == null) 
                    return;
                
                var fileItem = (FileItem) list.SelectedItem;
				Output.Text = FileExtensions.ReadFile(fileItem.Location);
            };

        }



        public void Update()
        {
            ViewModel.RecompileSource();
        }

        private void TextEditor_OnTextChanged(object sender, EventArgs e)
        {
            if (DataContext != null)
            {

                ViewModel.SourceCode = TextEditor.Text;

            }
        }


        private void Output_OnTextChanged(object sender, EventArgs e)
        {
           
            if (DataContext != null)
            {
               
                          ViewModel.OutputCode = Output.Text;
             
            }
        }




        CompletionWindow completionWindow;

        void textEditor_TextArea_TextEntered(object sender, TextCompositionEventArgs e)
        {
            //            if (e.Text == ".")
            //            {
            //                // open code completion after the user has pressed dot:
            //                completionWindow = new CompletionWindow(TextEditor.TextArea);
            //                // provide AvalonEdit with the data:
            //                IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
            //                data.Add(new MyCompletionData("Item1"));
            //                data.Add(new MyCompletionData("Item2"));
            //                data.Add(new MyCompletionData("Item3"));
            //                data.Add(new MyCompletionData("Another item"));
            //                completionWindow.Show();
            //                completionWindow.Closed += delegate
            //                {
            //                    completionWindow = null;
            //                };
            //            }
        }

        void textEditor_TextArea_TextEntering(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Length > 0 && completionWindow != null)
            {
                if (!char.IsLetterOrDigit(e.Text[0]))
                {
                    // Whenever a non-letter is typed while the completion window is open,
                    // insert the currently selected element.
                    completionWindow.CompletionList.RequestInsertion(e);
                }
            }
            // do not set e.Handled=true - we still want to insert the character that was typed
        }


        private readonly string[] DlangKeywords =
        {
           "import","version", "module"

        };

        private readonly string[] ILOperatorKeywords =
        {
            "add", "mul", "div", "sub", "branch"
        };

        #region Folding
        FoldingManager foldingManager;
        BraceFoldingStrategy foldingStrategy;



        void foldingUpdateTimer_Tick(object sender, EventArgs e)
        {
            if (foldingStrategy != null)
            {
                foldingStrategy.CreateNewFoldings(TextEditor.Document);
            }
        }
        #endregion

        private void RunCSharpButton_Click(object sender, RoutedEventArgs e)
        {
           
                this.ViewModel.CompilerErrors = String.Empty;
            ThreadPool.QueueUserWorkItem((h) => CompileCSharp(true));
        }

        private void CompileCSharp(bool clearoutput)
        {
            try
            {
                CSharpOutput = "";
             
                //TextWriter originalConsoleOutput = Console.Out;
                //StringWriter writer = new StringWriter();
               // Console.SetOut(writer);

                //                AppDomain appDomain = AppDomain.CreateDomain("Loading Domain");
                //
                //
                //
                                var start = Environment.TickCount;
                //
                //                this.Execute(LastCompiledExecutablePath);
                //                var end = Environment.TickCount - start;
                //
                //                AppDomain.Unload(appDomain);


                var output = (LastCompiledExecutablePath).ExecuteCommand();
                //(" -c \"/usr/bin/mono --attach=disable --gc=sgen " + LastCompiledExecutablePath + "\"", "");
                Console.WriteLine(output);

                var end = Environment.TickCount - start;

              //  Console.SetOut(originalConsoleOutput);
                this.ViewModel.CompilerErrors +=  String.Format("CS time: {0} ms\n", end);

                string result = output;
                CSharpOutput = result;
                this.ViewModel.CompilerErrors += (CSharpOutput);
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
            // load the bytes and run Main() using reflection
            // working with bytes is useful if the assembly doesn't come from disk
            byte[] bytes = File.ReadAllBytes(exeName);
            Assembly assembly = Assembly.Load(bytes);
            MethodInfo main = assembly.EntryPoint;
            main.Invoke(null, new object[] {});
        }



        public MainWindowViewModel ViewModel
        {
            get
            {
                MainWindowViewModel f = null;

                Dispatcher.Invoke(() =>
                {
                    f= (MainWindowViewModel) DataContext;
                });
                return f;
            }
        }

        private void RunCPPButton_Click(object sender, RoutedEventArgs e)
        {

           

                ViewModel.CompilerErrors = String.Empty;
            ThreadPool.QueueUserWorkItem((h) => CompileD());



        }

        public string LastCompiledExecutable;
        private void CompileD()
        {
           
             //   CppOutput = String.Empty;
//                var outputcpp = "OpenRuntime/" + LastCompiledExecutable.Replace(".exe", ".d");

             //   var fileInfo = new FileInfo(outputcpp);
               // outputcpp = fileInfo.FullName;
           

                var outputexe = MainWindowViewModel.TempDir + "/"+ LastCompiledExecutable.Replace(".exe", ".d").Replace(".d", "_d.exe");

                try
                {
                var start = DateTime.Now;
                //  var sb = new StringBuilder(ViewModel.OutputCode);

                var bclDir =
                         Directory.GetParent(
                            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location + "..\\..\\..\\..\\..\\")) +
                        "\\SharpNative\\DCorlib";
                    //sb.ToFile(outputcpp);
                   // @"C:\Program Files (x86)\Microsoft Visual Studio 12.0\VC\bin\vcvars32.bat".ExecuteCommand("",@"C:\Program Files (x86)\Microsoft Visual Studio 12.0\VC\bin\");
                    var bclFiles = Directory.GetFiles(bclDir, "*.d", SearchOption.AllDirectories)
                .OrderBy(o => o)
                .ToList();

                    NativeCompilationUtils.SetCompilerOptions("dmdwin");
                  //  NativeCompilationUtils.CompilerOptions.OptimizationFlags += " -I " + MainWindowViewModel.TempDir + " -I " + bclDir;
                    NativeCompilationUtils.CompileAppToNativeExe(((List<FileItem>)CppFileList.ItemsSource).Where(j=>j.Name.EndsWith(".d")).Select(k=>k.Location).Union(bclFiles).ToArray(), outputexe);


                ViewModel.CompilerErrors += ("\nCompiling to binary took " + (DateTime.Now - start).TotalMilliseconds + " ms\n");
                 start =DateTime.Now;

                    // Do not wait for the child process to exit before
                    // reading to the end of its redirected stream.
                    // p.WaitForExit();
                    // Read the output stream first and then wait.
                    string output = outputexe.ExecuteCommand("");

                    var end = DateTime.Now - start;

                    CppOutput = output;
                    Dispatcher.Invoke(() =>
                    {
                        ViewModel.CompilerErrors += String.Format("D time: {0} ms\n", end.TotalMilliseconds) + output ;
                    });
                }
                catch (Exception ex)
                {

                    Dispatcher.Invoke(() =>
                    {
                        ViewModel.CompilerErrors += ex.Message + "\nStackTrace: \n" +
                                                    ex.StackTrace;
                    });

                }
                finally
                {
//                    File.Delete(outputcpp);
                    if (File.Exists(outputexe))
                        File.Delete(outputexe);
                }
           
        }


        private void TestButton_Click(object sender, RoutedEventArgs e)
        {
            Func<string, string> strip = i => Regex.Replace(i, "[\r\n \t]+", "").Trim();

            if (ResetStatus != null)
                ResetStatus.Stop();
          ThreadPool.QueueUserWorkItem((h) =>
          {
              CompileCSharp(false);
              CompileD();
         
                if (strip(CSharpOutput??"") == strip(CppOutput??""))
                {
                    Dispatcher.Invoke(() =>
                    {
                        TestStatus.Content = "PASSED";
                        TestStatus.Background =
                            new SolidColorBrush(System.Windows.Media.Color.FromRgb(Color.GreenYellow.R,
                                Color.GreenYellow.G, Color.GreenYellow.B));

                        ViewModel.CompilerErrors = String.Format("Test Passed:\n\nCSharpOutput:\n{0}DlangOutPut:\n{1}",
                            CSharpOutput, CppOutput);
                    });
                }
                else
                {
                    Dispatcher.Invoke(() =>
                    {
                        TestStatus.Content = "FAILED";
                        TestStatus.Background =
                            new SolidColorBrush(System.Windows.Media.Color.FromRgb(Color.Red.R, Color.Red.G, Color.Red.B));
                        ViewModel.CompilerErrors = "Test Failed\n" + ViewModel.CompilerErrors;
                    });
                }
                if (ResetStatus == null)
                {
                    ResetStatus = new Timer(2000);
                    ResetStatus.AutoReset = false;
                    ResetStatus.Elapsed += (o, args) => Dispatcher.Invoke(() =>
                    {
                        TestStatus.Content = "TEST STATUS";
                        TestStatus.Background =
                            new SolidColorBrush(System.Windows.Media.Color.FromRgb(Color.White.R, Color.White.G,
                                Color.White.B));
                    });
                }
                ResetStatus.Start();
          });
        }

        private Timer ResetStatus;


        private void OnShowCompilerOptions(object sender, RoutedEventArgs e)
        {
          /*  var optionsWindow = new CompilerOptionsWindow();
            optionsWindow.Owner = this;
            optionsWindow.ShowDialog();
            if (!optionsWindow.ViewModel.Accepted)
                return;
            ViewModel.OptimizationList.Clear();
            ViewModel.OptimizationList.AddRange(optionsWindow.ViewModel.Capabilities);*/

        }

        public static string BaseDir = "X:/Experiments/Roslyn/CsNative/";

        public static string BCLDir = BaseDir + "Runtime/D/";

        public static string TestsDir = BaseDir + "Tests/";

        private string CurrentSourceFile = null;
        private void OnFileOpen(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.DefaultExt = ".cs";
            dialog.Filter = "CSharp Source Code (.cs)|*.cs";
            //  var folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Tests\\");
            //  dialog.InitialDirectory = folder;
            dialog.InitialDirectory = (TestsDir.Replace("/","\\"));

            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                CurrentSourceFile = dialog.FileName;
                CSharpFileName.Content=CurrentSourceFile;
				TextEditor.Text = FileExtensions.ReadFile(CurrentSourceFile);
            }
            else
            {
                
            }

           
        }

        private Timer SaveStatus;
        public string LastCompiledExecutablePath;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (SaveStatus != null)
                SaveStatus.Stop();

            var list = CppFileList;

            if (list.SelectedItem == null)
                return;
            try
            {
                var fileItem = (FileItem)list.SelectedItem;
               File.SetAttributes(fileItem.Location, FileAttributes.Normal); //clear read only flag so we can delete it
               File.WriteAllText(fileItem.Location, Output.Text);
               File.SetAttributes(fileItem.Location, FileAttributes.ReadOnly);
                Dispatcher.Invoke(() =>
                {
                    SaveButton.Content = "Saved";
                    SaveButton.Background =
                        new SolidColorBrush(System.Windows.Media.Color.FromRgb(Color.GreenYellow.R,
                            Color.GreenYellow.G, Color.GreenYellow.B));


                });
            }
            catch (Exception ex)
            {

             

                Dispatcher.Invoke(() =>
                {
                    SaveButton.Content = "Failed";
                    SaveButton.Background =
                        new SolidColorBrush(System.Windows.Media.Color.FromRgb(Color.Red.R,
                            Color.Red.G, Color.Red.B));

                    ViewModel.CompilerErrors = String.Format(ex.Message + "\n" + ex.StackTrace);
                });
            }
           

            if (SaveStatus == null)
            {
                SaveStatus = new Timer(2000);
                SaveStatus.AutoReset = false;
                SaveStatus.Elapsed += (o, args) => Dispatcher.Invoke(() =>
                {
                    SaveButton.Content = "Save";
                    SaveButton.Background =
                        new SolidColorBrush(System.Windows.Media.Color.FromRgb(Color.White.R, Color.White.G,
                            Color.White.B));
                });
            }
            SaveStatus.Start();
        }

        private void SaveButton_Test_Click(object sender, RoutedEventArgs e)
        {
            if (SaveStatus != null)
                SaveStatus.Stop();

          

            if (CurrentSourceFile == null)
                return;
            try
            {
               
                File.SetAttributes(CurrentSourceFile, FileAttributes.Normal); //clear read only flag so we can delete it
                File.WriteAllText(CurrentSourceFile, TextEditor.Text);
                File.SetAttributes(CurrentSourceFile, FileAttributes.ReadOnly);
                Dispatcher.Invoke(() =>
                {
                    SaveButton_Test.Content = "Saved";
                    SaveButton_Test.Background =
                        new SolidColorBrush(System.Windows.Media.Color.FromRgb(Color.GreenYellow.R,
                            Color.GreenYellow.G, Color.GreenYellow.B));


                });
            }
            catch (Exception ex)
            {



                Dispatcher.Invoke(() =>
                {
                    SaveButton_Test.Content = "Failed";
                    SaveButton_Test.Background =
                        new SolidColorBrush(System.Windows.Media.Color.FromRgb(Color.Red.R,
                            Color.Red.G, Color.Red.B));

                    ViewModel.CompilerErrors = String.Format(ex.Message + "\n" + ex.StackTrace);
                });
            }


            if (SaveStatus == null)
            {
                SaveStatus = new Timer(2000);
                SaveStatus.AutoReset = false;
                SaveStatus.Elapsed += (o, args) => Dispatcher.Invoke(() =>
                {
                    SaveButton_Test.Content = "Save";
                    SaveButton_Test.Background =
                        new SolidColorBrush(System.Windows.Media.Color.FromRgb(Color.White.R, Color.White.G,
                            Color.White.B));
                });
            }
            SaveStatus.Start();

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.DefaultExt = ".sln";
            dialog.Filter = "CSharp Solution (.sln)|*.sln |Csharp Project (.csproj)|*.csproj";
            //  var folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Tests\\");
            //  dialog.InitialDirectory = folder;
            dialog.InitialDirectory = (TestsDir.Replace("/", "\\"));

            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                CurrentSourceFile = dialog.FileName;
              //  TextEditor.Text = File.ReadAllText(CurrentSourceFile);
                ViewModel.Recompile(CurrentSourceFile);
            }
            else
            {

            }

        }

        private void RunAllTests(object sender, RoutedEventArgs e)
        {
            


            VistaFolderBrowserDialog dialog = new VistaFolderBrowserDialog();
            dialog.Description = "Please select a folder.";
            dialog.UseDescriptionForTitle = true; // This applies to the Vista style dialog only, not the old dialog.
          
           

            if ((bool)dialog.ShowDialog(this))
            {
              
          
                var filename = dialog.SelectedPath; // will switch to .Url later
                                                   //				var newText = File.ReadAllText (filename);
                                                   //				CSharpFilename.StringValue = Path.GetFileName (filename);
                                                   //				CSharpTextEditor.Replace (new NSRange (0, CSharpTextEditor.Value.Length), newText);
                                                   //				ViewModel.SourceCode = newText;
                                                   //				ViewModel.RecompileSource ();

                Func<string, string> strip = i => Regex.Replace(i ?? "", "[\r\n \t]+", " ").Trim();

                ViewModel.CompilerErrors = "\r\r" + ("Running all tests in : " + filename) + "\r\r";
                int passCount = 0;
                //			if (ResetStatus != null)
                //				ResetStatus.Stop();
                List<string> failedTestNames = new List<string>();
                ThreadPool.QueueUserWorkItem((h) =>
                    {

                        var allTests = Directory.EnumerateFiles(filename).Where(u => Path.GetExtension(u) == ".cs");
                        var count = 0;
                        foreach (var file in allTests)
                        {

                            var shortName = Path.GetFileName(file);
                            Console.WriteLine("-------------------------Running Test: " + shortName + "-------------------------");
                            ViewModel.CompilerErrors += "-------------------------Running Test: " + shortName + "-------------------------";

                            var text = FileExtensions.ReadFile(file);

                            if (text == "-1")
                                break;

                            Dispatcher.Invoke(() =>
                            {
                                CurrentSourceFile = file;
                                  TextEditor.Text = text;
//                                ViewModel.Recompile(CurrentSourceFile);

//                                CSharpFilename.StringValue = Path.GetFileName(file);

//                                CSharpTextEditor.Value = Extensions.ReadFile(file);
                            });

                            CSharpOutput = "Z..)";
                            CppOutput = "A..)";
                            //							ViewModel.CompileAndRunCode(shortName,File.ReadAllText(file));
                            CompileCSharp(false);
                            CompileD();
                            count++;

                            if (count % 20 == 0)
                            {
                                GC.Collect();
                            }

                            if (strip(CSharpOutput) == strip(CppOutput))
                            {
                                passCount++;
                                Dispatcher.Invoke(() =>
                                {
                                   

                                    ViewModel.CompilerErrors += String.Format("-------------------------Test {0} Passed:-------------------------\n", shortName);
                                });
                            }
                            else
                            {
                                Dispatcher.Invoke(() =>
                                {
                               
                                    ViewModel.CompilerErrors += String.Format("Test {0} Failed:\n", shortName);
                                    failedTestNames.Add(Path.GetFileNameWithoutExtension(shortName));

                                });
                            }
                           
                        }

                        ViewModel.CompilerErrors += String.Format("Summary \nTotal:{0} \nPass Rate:{1} \nPassed: {2} \nFailed: {3} {4}\n", allTests.Count(), (passCount * 100) / ((float)allTests.Count()), passCount, allTests.Count() - passCount, (allTests.Count() - passCount == 0) ? "" : failedTestNames.Aggregate((k, j) => k + " , " + j));

                    });



            }
        }
    }
}

