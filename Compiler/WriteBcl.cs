// /*
//   SharpNative - C# to D Transpiler
//   (C) 2014 Irio Systems 
// */

namespace SharpNative.Compiler
{
    public static class WriteBcl
    {
//        public static string SimpleBcl = File.ReadAllText(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/Runtime/System/System.d") ;

        public static void Go(OutputWriter writer)
        {
//            writer.WriteLine(SimpleBcl);
        }

        public static string SimpleBcl { get; set; }
    }
}