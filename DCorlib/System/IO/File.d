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
}

