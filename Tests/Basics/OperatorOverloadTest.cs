// complex.cs
using System;

public class Complex
{
    public int Real; //real is keyword in dlang
    public int Imaginary;

    public Complex(int Real, int Imaginary)
    {
        this.Real = Real;
        this.Imaginary = Imaginary;
    }

    // Declare which operator to overload (+), the types 
    // that can be added (two Complex objects), and the 
    // return type (Complex):
    public static Complex operator +(Complex c1, Complex c2)
    {
        return new Complex(c1.Real + c2.Real, c1.Imaginary + c2.Imaginary);
    }
    // Override the ToString method  (won't work) to display an complex number in the suitable format:
    public  void Print()
    {
        Console.Write(Real);

        Console.Write(" + i");
        Console.Write(Imaginary);
        Console.Write("i");
        //   return(String.Format("{0} + {1}i", Real, Imaginary));

    }

    public static void Main()
    {
        Complex num1 = new Complex(2, 3);
        Complex num2 = new Complex(3, 4);

        // Add two Complex objects (num1 and num2) through the
        // overloaded plus operator:
        Complex sum = num1 + num2;

        // Print the numbers and the sum using the overriden ToString method:
        Console.WriteLine("First complex number:  ");
        num1.Print();
        Console.WriteLine("Second complex number: ");
        num2.Print();
        Console.WriteLine("The sum of the two numbers: ");
        sum.Print();

    }
}