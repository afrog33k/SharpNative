module System.Collections.Generic.GenericComparer_T;


import System.Namespace;
import System.Collections.Generic.Namespace;

 class GenericComparer_T( T ) :  Comparer_T!(T)
{


 override public int Compare(T x, T y) 
  {

    if (x is null)
    {

      return ((y is null) ? (0) : (-1));
    
}
    if (y is null)
    {

      return (1);
    
}
    return (x.IComparable_T_CompareTo(y));
  
}

};