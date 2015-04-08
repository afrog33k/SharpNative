module System.ArgumentException;
import System.Namespace;
/**
* The exception thrown when one of the arguments provided to a method is not valid.
*/
class ArgumentException : NException {

	private static const E_ARGUMENT = "Value does not fall within the expected range.";

	private String paramName_;

	this() {
		super(E_ARGUMENT);
	}

	this(String message) {
		super(cast(string)message);
	}

	this(String message, String paramName) {
		super(cast(string)message);
		paramName_ = paramName;
	}

	final String paramName() {
		return paramName_;
	}

}
