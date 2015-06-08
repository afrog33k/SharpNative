module System.Locale;

import System.Namespace;

//Locale Helper
struct Locale
{
	String GetText(String aString)
	{
		return aString;
	}

	String GetText(string aString)
	{
		return new String(aString);
	}

	String GetText(wstring aString)
	{
		return new String(aString);
	}
}