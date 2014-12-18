using System;
using System.Collections.Generic;

namespace Blargh
{
    public static class Utilities
    {
        public static void SomeFunction()
        {
            var queue = new Queue<int>(10);
            queue.Enqueue(4);
            queue.Enqueue(2);
            Console.WriteLine(queue.Dequeue());
            queue.Clear();
    
            var list = new List<string>(3);
            list.Add("Three");
            list.RemoveAt(0);
            list.Insert(4, "Seven");

            var stack = new Stack<int>();
            stack.Push(9);
            stack.Push(3);
            Math.Max(stack.Pop(), stack.Pop());
        }
    }
}