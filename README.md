What is SharpNative ?
===================

SharpNative is a tool that generates **Native Code** (D, Soon C++11, Java and Swift) from **C#** leveraging Microsoft Roslyn to generate almost hand-written code the the target languages.

The idea is to maximize the cross-platform capabilities of C# without being tied to any vendor or platform. The Emphasis here is on Performance and Readability of the generated D Code. Comments are preserved as well.

---

###**Supported Output Languages**
The Compiler in its current state only supports D as the output language. (This is due to DMD being an extremely fast compiler, so testing features is fun)

---

###**Performance** -- *These are very unscientific*

The following are tests taken from CrossNet (one of the first C# to Native compiler efforts)

**Machine:**

-- Macbook Pro Retina (Mid 2012)

-- 2.6Ghz Intel Core i7

-- 16GB  1600 MHz DDR3

Some benchmarks on my Parallels Windows 8 VM: (3GB Ram, 3 Cores) using DMD  with options  `-inline -release -m64 -O`
and .Net in release mode


|Type Of Test | C# Time (ms) |     D Time (ms)   |  Speed Ratio (C#/D) |
|-------------|:----------:|:---|------:|
|NSieveTest| 18859  |  5450 | 3.46x |
|MatrixTest(MultiDimensional)| 12359  |    22606   |   0.56x |
|MatrixTest(Jagged)| 10156  | 2580 |    3.98x |
|GC Test| 10657   | 57288 |    0.19x |
|Unsafe Test| 32375    | 4752 |    6.81x |
|HeapSort Test| 8671     | 3906 |    2.21x |
|Average |      |  |    **2.87x** |

Due to the produced binaries being native and better optimizations in the DMD, the generated binaries are generally much faster than their C# counterparts. Except when Garbage Collection is concerned, the D GC is much slower than that of .Net (Maybe we can port it to D). Also the current multidimensional array implementation seems lacking in performance.

----------
Example of Generated Code

C#:

 ```c++
using System;

class Primes
{
 	public static void Main()
    {
        var len = 1000000; // This is a comment
        var primes = AddPrimes(len);
        Console.Write(primes);
    }

    private static int AddPrimes(int len)
    {
        var primes = 0;
        for (var i = 2; i < len; i++)
        {
            if (i%2 == 0)
                continue;
            var isPrime = true;
            for (var j = 2; j*j <= i; j++)
            {
                if (i%j == 0)
                {
                    isPrime = false;
                    break;
                }
            }
            if (isPrime)
                primes++;
        }
        return primes;
    }
    
}
```
D:

```c++
module CsRoot.Primes;
import System.Namespace;
import CsRoot.Namespace;

class Primes : NObject
{

    public static void Main()
    {
      int len = 1000000;
      // This is a comment
      int primes = AddPrimes(len);
      Console.Write(primes);
    }

    final static int AddPrimes(int len)
    {
      int primes = 0;
      
      for (int i = 2;i<len;i++)
      {
        if(i%2==0)
        {
          continue;
        }
        bool isPrime = true;
        
        for (int j = 2;j*j<=i;j++)
        {
          if(i%j==0)
          {
            isPrime=false;
            break;
          }
        }
        if(isPrime)
        {
          primes++;
        }
      }
      return primes;
    }

  public override String ToString()
  {
    return GetType().FullName;
  }

  public override Type GetType()
  {
    return __TypeOf!(typeof(this));
  }
}
```
---

###**Documentation** 
Unfortunately this is all the documentation the transpiler has at the moment.

----------
**Feature List (Incomplete):**
-	What works: 
-	Basic PInvoke including marshalling
-	Arrays including initializers
-	Fields/ Properties/Methods with correct hiding semantics
-	Properties
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
-	Explicit Interfaces
-	Implicit and Explicit Cast Operators
-	String switch … dlang supports this natively :)
-	String.Format .. though implementation is very basic
-	C# multi dimensional arrays work correctly (even with multi dim syntax :) )
-	Delegates work including multicast (Native delegates through P/Invoke work too)
-	Events work as expected … though a bit slower than C#(mono)
-	Object initializers work as a chain of lambda expressions
-	Generic Virtual Methods
-	Basic Reflection of Methods, Fields and Properties, IndexerProperties are not yet supported as are Generic Methods ... for now.
-	Iterators and Yield using code from WootzJS, a fiber implementation exists but it crashes on DMD 2.066 Windows 64-bit


----------

###**Requirements -- Testing** 
-- Microsoft .Net 4.0 / Mono 3.6 and above.

-- Windows 7 or Later ( The CLI Interface works on Linux and OSX)

-- A Working D Installation  (LDC,DMD or GDC) 

###**Requirements -- Development** 
All requirements mentioned above and:

-- Visual Studio 2013 or above

-- Visual D 

###**Usage**
If you are using the GUI interface(windows) note that DMD should be installed in `"C:\\D\\dmd2\\windows\\bin\\"` 
For the CLI interface the driver can be invoked in the following manner:


    mono ./SharpNative.exe /compiler:pathtodcompiler /dcorlib:/**pathtodcorlib** /source:"pathtosourcefile" /outputtype:exe /dstdlib:pathtophobos /compileroptions:"compileroptions"

**where**:

-- **pathtodcompiler** is the path to a d compiler e.g. `/usr/local/bin/ldc` on mac osx

-- **pathtodcorlib** is the path to the included basic corlib e.g. `/Projects/SharpNative/DCorlib`

-- **pathtosourcefile** is the path to the test source file in C#

--**pathtophobos** is the location of phobos in your installation e.g. `/usr/local/Cellar/ldc/0.15.0/include/d`

--**compileroptions** are the compiler options to pass to dmd/ldc/gdc e.g. `-inline -release -m64 -O5 -oq`


**What Doesn’t Work: (Also Incomplete)**
-	Right now compiler does not do any optimizations like final, in, out @pure etc …
-	Async/Await - Working on an implementation
-	Query Expression Syntax for LINQ and Expression Trees
-	Structs FieldLayout implementation is currently wrong ... to mimic C# a new design is needed.

