module System.Collections.Generic.GenericEqualityComparer_T;


import System.Namespace;
import System.Collections.Generic.Namespace;

 class GenericEqualityComparer_T( T ) :  EqualityComparer_T!(T)
{


 override public int GetHashCode(T obj) 
  {

    if (obj is null)
    {

      return (0);
    
}
    return (obj.GetHashCode());
  
}

 override public bool Equals(T x, T y) 
  {

    if (x is null)
    {

      return (y is null);
    
}
    return (x.IEquatable_T_Equals(y));
  
}

};