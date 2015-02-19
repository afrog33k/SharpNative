// /*
//   SharpNative - C# to D Transpiler
//   (C) 2014 Irio Systems 
// */

namespace SharpNative.Compiler
{
    public class TempWriter : OutputWriter
    {
        public TempWriter()
            : base("", "", false)
        {
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}