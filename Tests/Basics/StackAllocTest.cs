//https://msdn.microsoft.com/en-us/library/cx9s2sy4.aspx
class Test
{
    static unsafe void Main()
    {
        const int arraySize = 20;
        int* fib = stackalloc int[arraySize];
        int* p = fib;
        // The sequence begins with 1, 1.
        *p++ = *p++ = 1;
        for (int i = 2; i < arraySize; ++i, ++p)
        {
            // Sum the previous two numbers.
            *p = p[-1] + p[-2];
        }
        for (int i = 0; i < arraySize; ++i)
        {
            Console.WriteLine(fib[i]);
        }
    }
}