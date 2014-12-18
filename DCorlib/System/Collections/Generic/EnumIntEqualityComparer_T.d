module System.Collections.Generic.EnumIntEqualityComparer_T;


import System.Namespace;
import System.Collections.Generic.Namespace;

 class EnumIntEqualityComparer_T( T ) :  EqualityComparer_T!(T)
{


 override public int GetHashCode(T obj) 
  {

    return (Array_T.UnsafeMov!( T, int )(obj));
  
}

 override public bool Equals(T x, T y) 
  {

    return (Array_T.UnsafeMov!( T, int )(x)==Array_T.UnsafeMov!( T, int )(y));
  
}

 override public int IndexOf(Array_T!(T) array, T value, int startIndex, int endIndex) 
  {

    int v = Array_T.UnsafeMov!( T, int )(value);
     Array_T!(int)  a = Array_T.UnsafeMov!( Array_T!(T), Array_T!(int) )(array);
    for (int i = startIndex;i<endIndex;++i)
      {

                if (Array_T.UnsafeLoad!( int )(a, i)==v)
        {

          return (i);
        
}
      }
      return (-1);
    
}

};