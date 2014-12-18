using System;
using System.Text;

namespace Blargh
{
    class Box
    {
        private float _width;
        public float Width
        {
            get { return _width; }
            set { _width = value; }
        }

        public float SetOnly
        {
            set { Console.WriteLine(value); }
        }

        public int GetOnly
        {
            get { return 4; }
        }
        
        public bool IsRectangular = true;
        public char[] Characters = new char[] { 'a', 'b' };
        public static StringBuilder StaticField = new StringBuilder();
        public const int ConstInt = 24;
        public static readonly int StaticReadonlyInt = 5;
        public const string WithQuoteMiddle = @""before""""after"";
        public const string WithQuoteStart = @""""""after"";
        public int MultipleOne, MultipleTwo;
        public readonly int ReadonlyInt = 3;
        public DateTime UninitializedDate;
        public DateTime? UninitializedNullableDate;
        public int? UnitializedNullableInt;
        public TimeSpan UninitializedTimeSpan;
        public static DateTime StaticUninitializedDate;
        public static int? StaticUnitializedNullableInt;
        public static TimeSpan StaticUninitializedTimeSpan;

        static Box()
        {
            Console.WriteLine(""cctor"");
        }

        public Box()
        {
            Console.WriteLine(""ctor"");
        }
    }
}