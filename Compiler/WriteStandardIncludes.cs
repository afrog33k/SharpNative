// /*
//   SharpNative - C# to D Transpiler
//   (C) 2014 Irio Systems 
// */

namespace SharpNative.Compiler
{
    public static class WriteStandardIncludes
    {
        public static string[] StandardIncludes =
        {
//            "iostream",
//            "thread",
//            "cwchar",
//            "stdio.h",
//            "memory",
            "System.Namespace"
//            "System/Console.h",
//            "System/Convert.h",
        };

//@"
//
//using namespace std;
//";


        public static void Go(OutputWriter writer)
        {
            foreach (var import in StandardIncludes)
                writer.AddInclude(import);
        }
    }
}