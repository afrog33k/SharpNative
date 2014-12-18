//Type inference is fully supported, both using the 'var' keyword and implicitly typed lambdas.
using System;

using System.Text;


public class Driver {
	private static Action manualEventDelegate;

	public static event Action AutoEvent;

	public static event Action ManualEvent {
		add { manualEventDelegate += value; }
		remove { manualEventDelegate -= value; }
	}

	public static void Main() {
		var sb = new StringBuilder();

		AutoEvent += () => sb.AppendLine("Auto event");
		ManualEvent += () => sb.AppendLine("Manual event");

		AutoEvent();
		manualEventDelegate();

		Console.WriteLine(sb.ToString());
	}
}