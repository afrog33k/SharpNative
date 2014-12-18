using System;

class Program
{
    static void Main()
    {
		Test(name: "Perl", size: 5);
		Test(name: "Dot", size: -1);
		Test(6, "Net");
		Test(7, name: "Google");

	 	AddWidget("root", 320, 240, "First");
     	AddWidget("root", text: "Origin");
        AddWidget("root", 500);
        AddWidget("root", text: "Footer", y: 400);
    }

    static void Test(int size, string name)
    {
		Console.WriteLine("Size = {0}, Name = {1}", size, name);
    }

    static void AddWidget(string parent, float x = 0, float y = 0, string text = "Default")
    {
        Console.WriteLine("parent = {0}, x = {1}, y = {2}, text = {3}", parent, x, y, text);
    }
 
}