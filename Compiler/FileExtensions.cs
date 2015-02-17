using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace SharpNative.Compiler
{
    public static class FileExtensions
    {

        public static void DeleteFilesByWildcards(string pattern, string path = "./")
        {
            var files = Directory.GetFiles(path, pattern);
            foreach (var file in files)
            {
                try
                {
                    File.Delete(file);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception: " + ex);
                }
            }
        }

        private static int _bufferSize = 16384;

        public static string ReadFile(string filename)
        {
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                using (FileStream fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read))

                using (StreamReader streamReader = new StreamReader(fileStream))
                {

                    stringBuilder.Append(streamReader.ReadToEnd());

                }

                return stringBuilder.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Cannot read file: " + filename );
                return "-1";
            }
        }
        public static string ExecuteCommand(this string pathToExe, string arguments = "", string workingDirectory = "")
        {
            if (Driver.Verbose)
            {
                Console.WriteLine("Running:" + pathToExe + " " + arguments);
            }

            if (workingDirectory == "")
            {
                workingDirectory = Path.GetDirectoryName(pathToExe);
            }

            var process = new Process
            {
                StartInfo =
                {

                    FileName = pathToExe,
                    Arguments = arguments,
                    WorkingDirectory = workingDirectory,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden | ProcessWindowStyle.Minimized,
                    UseShellExecute = false
                }
            };

            StringBuilder output = new StringBuilder();
            StringBuilder error = new StringBuilder();

            using (AutoResetEvent outputWaitHandle = new AutoResetEvent(false))
            using (AutoResetEvent errorWaitHandle = new AutoResetEvent(false))
            {
                process.OutputDataReceived += (sender, e) =>
                {
                    if (e.Data == null)
                    {
                        try
                        {
 outputWaitHandle.Set();
                        }
                        catch (Exception)
                        {
                            
                            
                        }
                       
                    }
                    else
                    {
                        output.AppendLine(e.Data);
                    }
                };
                process.ErrorDataReceived += (sender, e) =>
                {
                    if (e.Data == null)
                    {
                        try
                        {
                            errorWaitHandle.Set();
                        }catch(Exception)
                        { }
                    }
                    else
                    {
                        error.AppendLine(e.Data);
                    }
                };

                var start = DateTime.Now;
               
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                //240 seconds ... for some tests .. i.e. benchmarks
                if (process.WaitForExit(240000) &&
                    outputWaitHandle.WaitOne(240000) &&
                    errorWaitHandle.WaitOne(240000))
                {
                    // Process completed. Check process.ExitCode here.
                    var standardOutput = output.ToString();
                    var standardError = error.ToString();

                    var end = DateTime.Now - start;
                    if (Driver.Verbose)
                        Console.WriteLine("Process took " + end.TotalMilliseconds + " ms");
                    return String.IsNullOrWhiteSpace(standardOutput)
                        ? standardError
                        : String.IsNullOrWhiteSpace(standardError)
                            ? standardOutput
                            : standardOutput + Environment.NewLine + standardError;
                }
                else
                {
                    // Timed out.
                    return "Process terminated immaturely";
                }
            }
        }



        public static void DeleteFile(this string fileName)
        {
            try
            {
                if (File.Exists(fileName))
                    File.Delete(fileName);
            }
            catch (Exception)
            {
            }
        }

        public static void ToFile(this string text, string fileName)
        {
            File.WriteAllText(fileName, text);
        }

        public static void ToFile(this StringBuilder text, string fileName)
        {
            File.WriteAllText(fileName, text.ToString());
        }



    }
}
