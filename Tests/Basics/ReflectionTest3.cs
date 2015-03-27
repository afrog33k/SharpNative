//http://stackoverflow.com/questions/6608368/why-doesnt-reflection-set-a-property-in-a-struct
using System;
using System.Reflection;
class PriceClass {

        private int value;
        public int Value
        {
            get { return this.value; }
            set { this.value = value; }
        }           
    }


    struct PriceStruct
    {

        private int value;
        public int Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
    }
    public class Test {
    static void Main(string[] args)
    {
        PriceClass _priceClass = new PriceClass();
        Type type = typeof(PriceClass);
        PropertyInfo info = type.GetProperty("Value");
        info.SetValue(_priceClass, 32, null);
        Console.WriteLine(_priceClass.Value);

        PriceStruct _priceStruct = new PriceStruct();
        type = typeof(PriceStruct);
        info = type.GetProperty("Value");
        info.SetValue(_priceStruct, 32, null);
        Console.WriteLine(_priceStruct.Value);
        
         object _bpriceStruct = new PriceStruct(); //Box first
    type = typeof(PriceStruct);
    info = type.GetProperty("Value");
    info.SetValue(_bpriceStruct, 32, null);
    Console.WriteLine(((PriceStruct)_bpriceStruct).Value); //now unbox and get value

       // Debugger.Break();
    }
    }