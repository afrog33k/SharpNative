module System.__Internal.Namespace;
//alias System.__Internal.Reflection Reflection;
import System.Namespace;
import System.Reflection.Internal;


//Have to put these here to prevent cycles
public class __ReflectionInfo
{
	static this()
	{
		//Should move these to reflection info though
		__TypeOf!(System.NObject.NObject)("System.Object");
		__TypeOf!(int)("System.Int32");
		__TypeOf!(NException)("System.Exception");
	}
}