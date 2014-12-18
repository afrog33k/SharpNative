module System.Collections.Generic.Comparer_T;


import System.Namespace;
import System.Collections.Generic.Namespace;
import System.Collections.Namespace;
//import System.Runtime.InteropServices.Namespace;

 class Comparer_T( T ) :  NObject ,  IComparer_T!(T) ,  System.Collections.Namespace.IComparer
{

  static Comparer_T!(T) _default;

public  abstract int IComparer_T_Compare(T x, T y) ;
  

public static    Comparer_T!(T)  Default() @property  {

    {

      return (System.Collections.Generic.Namespace.Comparer._default);
    
}
  
}


int IComparer_Compare(NObject x, NObject y) 
  {

    if (x==y)
    {

      return (0);
    
}
    if (x is null)
    {

      return ((y is null) ? (0) : (-1));
    
}
    if (y is null)
    {

      return (1);
    
}
    if ((IsCast!( T )(x))&&(IsCast!( T )(y)))
    {

      return (IComparer_T_Compare(AsCast!( T )(x), AsCast!( T )(y)));
    
}
    throw  new ArgumentException();
  
}
  static this()
  {

    //_default = (new System.Type(classOf[IComparable_T!(T)])._Type_IsAssignableFrom(new System.Type(classOf[T]))) ? (AsCast!( System.Collections.Generic.Namespace.Comparer_T!(T) )(Activator.CreateInstance(new System.Type(classOf[GenericComparer_T]).MakeGenericType(new System.Type(classOf[T]))))) : ( new Comparer_T_DefaultComparerT());

  
}

};