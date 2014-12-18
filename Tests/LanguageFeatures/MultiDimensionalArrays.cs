//Type inference is fully supported, both using the 'var' keyword and implicitly typed lambdas.
using System; 
using System.Text; 


public class Driver {
	public double[,] Identity {
		get {
			return new double[,] { { 1, 0, 0 }, { 0, 1, 0 }, { 0, 0, 1 } };
		}
	}

	public double[,] Multiply(double[,] a, double[,] b) {
		var result = new double[3,3];
		for (int i = 0; i < 3; i++) {
			for (int j = 0; j < 3; j++) {
				for (int k = 0; k < 3; k++) {
					result[i, j] += a[i, k] * b[k, j];
				}
			}
		}
		return result;
	}

	public void Main() {
		var m1 = new double[,] { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 8, 9 } };
		var m2 = new double[,] { { 10, 11, 12 }, { 13, 14, 15 }, { 16, 17, 18 } };
		var i = Identity;
		var result = Multiply(Multiply(m1, i), m2);

		var s = result[0, 0] + ", " + result[0, 1] + ", " + result[0, 2] + "\n" +
		        result[1, 0] + ", " + result[1, 1] + ", " + result[1, 2] + "\n" +
		        result[2, 0] + ", " + result[2, 1] + ", " + result[2, 2] + "\n";

    
        Console.WriteLine(s);
    }
}