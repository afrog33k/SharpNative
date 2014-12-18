//Type inference is fully supported, both using the 'var' keyword and implicitly typed lambdas.
using System; 
using System.Text; 

public class C {
	private StringBuilder _sb;

	public C(StringBuilder sb) {
		_sb = sb;
	}

	public IEnumerable<int> GetEnumerable(int n) {
		try {
			for (int i = 0; i < n; i++) {
				_sb.AppendLine("yielding " + i);
				yield return i;
			}
		}
		finally {
			_sb.AppendLine("in finally");
		}
	}
}


public class Driver {

	public void Main() {
		var sb = new StringBuilder();
		int n = 0;
		foreach (var i in new C(sb).GetEnumerable(5)) {
			sb.AppendLine("got " + i);
			if (++n == 2)
				break;
		}
    
        Console.WriteLine(sb.ToString());
    }
}