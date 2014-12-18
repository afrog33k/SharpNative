using System;

namespace Blargh
{
    abstract class TopLevel
    {
        public abstract void AbstractMethod();
        public abstract string AbstractProperty { get; }

        public virtual void VirtualMethod()
        {
            Console.WriteLine("TopLevel::VirtualMethod");
        }
        public virtual string VirtualProperty
        {
            get
            {
                return "TopLevel::VirtualProperty";
            }
        }

        public override string ToString()
        {
            return string.Empty;
        }
    }

    class Derived : TopLevel
    {
        public override void AbstractMethod()
        {
            Console.WriteLine("Derived::AbstractMethod");
        }

        public override string AbstractProperty
        {
            get { return "Derived::AbstractProperty"; }
        }

        public override void VirtualMethod()
        {
            base.VirtualMethod();
            Console.WriteLine("Derived::VirtualMethod");
        }

        public override string VirtualProperty
        {
            get
            {
                return base.VirtualProperty + "Derived:VirtualProperty";
            }
        }
        public override string ToString()
        {
            return "DerivedToString";
        }

         public static void Main()
    {
        Derived derived = new Derived();
        derived.AbstractMethod();
        Console.WriteLine(derived.AbstractProperty);
        Console.WriteLine(derived.VirtualProperty);
        Console.WriteLine(derived);

        TopLevel abs = (TopLevel)derived;

        abs.AbstractMethod();
        Console.WriteLine(abs.AbstractProperty);
        Console.WriteLine(abs.VirtualProperty);
        Console.WriteLine(abs);


    }
    }


}