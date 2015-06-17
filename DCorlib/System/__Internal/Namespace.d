module System.__Internal.Namespace;
//alias System.__Internal.Reflection Reflection;
import System.Namespace;
import System.Reflection.Internal;
//import System.__Internal.hashmap;


//Have to put these here to prevent cycles
public class __ReflectionInfo
{
	static this()
	{
		import System.Reflection.Namespace;
		//Should move these to reflection info though
		__TypeOf!(Nullable__G!(__UNBOUND))("System.Nullable`1[T]");
		__TypeOf!(NObject)("System.Object")
			.__Method("ToString", new MethodInfo__G!(NObject,String function())(&NObject.ToString), MethodAttributes.Virtual | MethodAttributes.Public)
			.__Method("Equals", new MethodInfo__G!(NObject,bool function(NObject obj))(&NObject.Equals))
			.__Method("GetHashCode", new MethodInfo__G!(NObject,int function())(&NObject.GetHashCode))
			.__Method("GetType", new MethodInfo__G!(NObject,System.Namespace.Type function())(&NObject.GetType), MethodAttributes.Virtual | MethodAttributes.Public);
		__TypeOf!(int)("System.Int32");
		__TypeOf!(NException)("System.Exception");
	}
}
//ToString
//Equals
//GetHashCode
//GetType