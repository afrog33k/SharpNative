using System;
using System.IO;

namespace Blargh
{
    public static class Utilities
    {
        public static void Main()
        {
            Console.WriteLine("Before try");
            try
            {
                Console.WriteLine("In try");
                }
                catch (IOException ex)
                {
                    Console.WriteLine("In catch 1");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("In catch 2");
                }
                finally
                {
                    Console.WriteLine("In finally");
            }

            try
            {
                Console.WriteLine("Try in parameterless catch");
            }
            catch
            {
                Console.WriteLine("In parameterless catch");
            }

            throw new InvalidOperationException("err");
        }

    }
}