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
		return _S(aString);
	}

	String GetText(wstring aString)
	{
		return _S(aString);
	}
}