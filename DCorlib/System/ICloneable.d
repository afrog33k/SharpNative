module System.ICloneable;
import System.Namespace;
import std.conv;

public interface ICloneable 
{
		NObject Clone(ICloneable j=null);
}
