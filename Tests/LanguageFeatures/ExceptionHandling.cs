//Type inference is fully supported, both using the 'var' keyword and implicitly typed lambdas.
using System;

using System.Text;


public class MyException : Exception {
	public MyException(string message) : base(message) {
	}
}


public class Driver {

	public static void Main() {
		var sb = new StringBuilder();

		try {
			throw new Exception("message 1");
		}
		catch (MyException ex) {
			sb.AppendLine("Caught MyException: " + ex.Message);
		}
		catch (Exception ex) {
			sb.AppendLine("Caught Exception: " + ex.Message);
		}

		try {
			throw new MyException("message 2");
		}
		catch (MyException ex) {
			sb.AppendLine("Caught MyException: " + ex.Message);
		}
		catch (Exception ex) {
			sb.AppendLine("Caught Exception: " + ex.Message);
		}

//this causes an error as this exception has already been captured
//		catch (Exception ex) {
//			sb.AppendLine("Caught Exception: " + ex.Message);
//		}

		Console.WriteLine(sb.ToString());
	}
}