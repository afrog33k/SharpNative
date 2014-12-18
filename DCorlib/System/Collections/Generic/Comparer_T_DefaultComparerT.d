module System.Collections.Generic.Comparer_T_DefaultComparerT;


import System.Namespace;
import System.Collections.Generic.Namespace;

 class Comparer_T_DefaultComparerT(T) :  Comparer_T!(T)
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
     IComparable  i = cast( IComparable )(x);
    if (i !is null)
    {

      return (i.IComparable_CompareTo(y));
    
}
    i=cast( IComparable )(y);
    if (i !is null)
    {

      return (-i.IComparable_CompareTo(x));
    
}
    throw  new ArgumentException( (new String ("At least one argument has to implement IComparable interface")));
  
}

};