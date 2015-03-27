module System.__PrimitiveExtensions;

//Comparison operators
int opCmp(wstring a, wstring b)
{
	if(a==b)
		return 0;
	else if(a>b)
		return 1;
	else if(a < b)
		return -1;

	return 0;
}