//https://msdn.microsoft.com/en-us/library/vstudio/65zzykke(v=vs.100).aspx
namespace UsingIterators
{
    class Program
    {
        static void Main()
        {
            // Using a simple iterator.
            ListClass listClass1 = new ListClass();

            foreach (int i in listClass1)
            {
                System.Console.Write(i + " ");
            }
            // Output: 0 1 2 3 4 5 6 7 8 9
            System.Console.WriteLine();


            // Using a named iterator.
            ListClass test = new ListClass();

            foreach (int n in test.SampleIterator(1, 10))
            {
                System.Console.Write(n + " ");
            }
            // Output: 1 2 3 4 5 6 7 8 9 10
            System.Console.WriteLine();


            // Using multiple yield statements.
            foreach (string element in new TestClass())
            {
                System.Console.Write(element);
            }
            // Output: With an iterator, more than one value can be returned.
            System.Console.WriteLine();

        }
    }

    class ListClass : System.Collections.IEnumerable
    {

        public System.Collections.IEnumerator GetEnumerator()
        {
            for (int i = 0; i < 10; i++)
            {
                yield return i;
            }
        }

        // Implementing the enumerable pattern
        public System.Collections.IEnumerable SampleIterator(int start, int end)
        {
            for (int i = start; i <= end; i++)
            {
                yield return i;
            }
        }
    }

    class TestClass : System.Collections.IEnumerable
    {
        public System.Collections.IEnumerator GetEnumerator()
        {
            yield return "With an iterator, ";
            yield return "more than one ";
            yield return "value can be returned";
            yield return ".";
        }
    }
}
