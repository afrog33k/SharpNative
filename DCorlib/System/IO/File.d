module System.IO.File;
import System.Namespace;
import std.stdio;
import std.string;
import std.file;
import std.conv;

//Basic file support based on Phobos

class File : NObject
{
	public static String ReadAllText(String filename)
	{
		return new String(std.file.readText(to!(string)(filename.Text)));
	}

	public static void WriteAllText(String filename, String text)
	{
		
		std.file.write(to!(string)(filename.Text), cast(char[])cast(wchar[])text.Text);
	}

	public static void WriteAllBytes(String filename, Array_T!(ubyte) bytes)
	{
		std.file.write(to!(string)(filename.Text), bytes.Items);
	}

	public static Array_T!(ubyte) ReadAllBytes(String filename)
	{
		return new Array_T!(ubyte)(__CC!(ubyte[])(cast(ubyte[])std.file.read(to!(string)(filename.Text))));
	}
}

