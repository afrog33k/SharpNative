SharpNative
===========

A C# to Dlang Transpiler
Let C# Run Free and Fast

Why D?:
Initially this project was written with a C++11 backend, but due to slow compilation, adding and testing features 
was getting too long in the tooth. With D, it took less than 3 days to port over the code :) and DMD is blazing fast

Performance:
On average tests run about 2x .Net (without optimization)

Requirements:
-	Microsoft .Net 4.5.3
-	Windows 7 or Later (Compiler works on OSX too, the visual editor is not ready, yet)

Usage:
- For now some things are hardcoded in the compiler. (DMD should be in "C:\\D\\dmd2\\windows\\bin\\")
	You can change that though by editing DMDWindowsOptions in  NativeCompilationUtils.cs
	options will be added to specify paths and link against different frameworks.

Feature List (Incomplete):
	
-	What works: Moved to Dlang … c++ was a headache
-	Basic PInvoke
-	Arrays including initializers
-	Fields/ Properties/Methods with correct hiding semantics
-	Properties are better implemented
-	String
-	Int/Double/Bool
-	Classes and Polymorphism … we follow C# model
-	Some benchmarks - basic linpack, fannkuch, nbody
-	Modules/Namespaces
-	Enum - no enum.Parse support yet though
-	Iterators are as .Net use enumerators etc … switching arrays to use simple for loop though for performance improvement
-	Constructors/Overloads/Base Class calls
-	Static Variables/Members/Properties
-	Basic System.Math, more implementations required though
-	Extension Methods
-	Operator Overloading
-	Indexers
-	Anonymous Classes
-	Generics … All current test cases work
-	Boxed structs and interface casting for them
-	Inner Classes in the form of OuterClass_InnerClass
-	Static Constructors
-	Explicit Interfaces … current fix is not so pretty though … i.e. IEnumerator.MoveNext becomes IEnumerator.IEnumerator_MoveNext (this allows implementing methods with same name, differently)
-	Implicit and Explicit Cast Operators
-	String switch … dlang supports this natively :)
-	String.Format .. though implementation is very basic
-	C# multi dimensional arrays work correctly (even with multi dim syntax :) )… mostly … look at multi test from CrossNet
-	Delegates work including multicast (Native delegates through P/Invoke work too)
-	Events work as expected … though a bit slower than C#(mono)

What Doesn’t Work: (Also Incomplete)
-	Object initializers in lambdas/loops/fields do very weird things (tm), we need to wrap them in functions (lambda captures)
-	Right now compiler does not do any optimizations like final, in, out @pure etc …
-	Unboxing to Wrong Type doesn't throw exceptions
-	Yield - Though an implementation using fibers exists
-	Async/Await - Working on an implementation
-	Query Expression Syntax for LINQ and general Expressions
-	Structs FieldLayout implementation is currently wrong ... to mimic C# a new design is needed.
-	Lots of other minor issues not yet documented.

