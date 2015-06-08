module System.Boolean;
import System.Namespace;

class Boolean : Boxed!bool
{

	// The true value. 
	// 
	public static const int True = 1; 

	// The false value.
	// 
	public static const int False = 0; 


	//
	// Internal Constants are real consts for performance.
	//

	// The internal string representation of true.
	// 
	public  __gshared  String TrueLiteral  = new String("True");

	// The internal string representation of false.
	// 
	public  __gshared  String FalseLiteral = new String("False");


	//
	// Public Constants
	//

	// The public string representation of true.
	// 
	public __gshared  String TrueString  = new String("True");

	// The public string representation of false.
	// 
	public __gshared  String FalseString = new String("False");

	//
	// Overriden Instance Methods
	//
	/*=================================GetHashCode==================================
	**Args:  None
	**Returns: 1 or 0 depending on whether this instance represents true or false.
	**Exceptions: None
	**Overriden From: Value
	==============================================================================*/
	// Provides a hash code for this instance.
	public override int GetHashCode() {
		return (__Value)?True:False;
	}

	/*===================================ToString===================================
	**Args: None
	**Returns:  "True" or "False" depending on the state of the boolean.
	**Exceptions: None.
	==============================================================================*/
	// Converts the boolean value of this instance to a String.
	public override String ToString() {
        if (false == __Value) {
			return FalseLiteral;
        }
        return TrueLiteral;
	}

	

	this(bool value = false)
    {
        this.__Value = value;
    }

	public override string toString() {
		return __Value.stringof;
	}

	public static String ToString(bool value)
	{
		if (false == value) {
			return FalseLiteral;
        }
        return TrueLiteral;
	}

	

	public override Type GetType()
	{
		return __TypeOf!(typeof(this));
	}

}

