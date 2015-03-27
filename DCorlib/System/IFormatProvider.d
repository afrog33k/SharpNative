module System.IFormatProvider;
import System.Namespace;

public interface IFormatProvider
{
	// Interface does not need to be marked with the serializable attribute

	public NObject GetFormat(Type formatType, IFormatProvider __j = null);
}