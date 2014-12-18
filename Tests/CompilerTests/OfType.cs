using System.Text;
using System.Linq;

namespace Blargh
{
    public class SomeClass
    {
        public SomeClass()
        {
            var a = new[] { 1, 2, 3 };
            var b = a.OfType<StringBuilder>().ToList();
        }
    }
}