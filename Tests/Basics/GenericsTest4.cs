//Basic constraints test
using System;

namespace CSharpPlayground
{
    public class Vector2D
    {
        public int X { get; set; }
        public int Y { get; set; }
    }

    class Program
    {
        public static T Add<T>(T a, T b)
            where T : Vector2D, new()
        {
            T newVector = new T();
            newVector.X = a.X + b.X;
            newVector.Y = a.Y + b.Y;

            return newVector;
        }

        static void Main(string[] args)
        {
            Vector2D a = new Vector2D () {
                X = 1,
                Y = 2   
            };

            Vector2D b = new Vector2D () {
                X = 10,
                Y = 11
            };

            Vector2D c = Add(a, b);
            Console.WriteLine(c.X + ", " + c.Y);
        }
    }
}
