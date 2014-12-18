//Saltarelle
// Inheritance is fully supported, including the possibility to override members.
using System;
 using System.Text; 

public class Base {
	public virtual string Method() {
		return "Base.Method";
	}

	public virtual string Property {
		get { return "Base.Property"; }
	}
}

public class Derived1 : Base {
	public override string Method() {
		return "Derived1.Method";
	}

	public override string Property {
		get { return "Derived1.Property"; }
	}
}

public class Derived2 : Base {
	public new string Method() {
		return "Derived2.Method";
	}

	public new string Property {
		get { return "Derived2.Property"; }
	}
}

public class Driver {
	public static void Main() {
		Derived1 d1 = new Derived1();
		Derived2 d2 = new Derived2();
		Base b2 = d2;

		var sb = new StringBuilder();
		sb.AppendLine("d1.Method() = " + d1.Method());
		sb.AppendLine("d1.Property = " + d1.Property);
		sb.AppendLine("d2.Method() = " + d2.Method());
		sb.AppendLine("d2.Property = " + d2.Property);
		sb.AppendLine("b2.Method() = " + b2.Method());
		sb.AppendLine("b2.Property = " + b2.Property);
		Console.WriteLine(sb.ToString());
	}
}