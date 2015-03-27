module System.Collections.Generic.EqualityComparer__G;


import System.Namespace;
import System.Collections.Namespace;
import System.Collections.Generic.Namespace;
//import CsRoot.Namespace;
//import System.Runtime.InteropServices.Namespace;

 class EqualityComparer__G( T ) :  NObject ,  System.Collections.Namespace.IEqualityComparer ,  IEqualityComparer__G!(T)
{

  static EqualityComparer__G!(T) _default;

public  abstract int GetHashCode(T obj,IEqualityComparer__G!(T) __j=null) ;

public  abstract bool Equals(T x, T y,IEqualityComparer__G!(T) __j=null) ;
  

public static    EqualityComparer__G!(T)  Default() @property  {

    {

      return (_default);
    
}
  
}


int GetHashCode(NObject obj,IEqualityComparer __j=null) 
  {

    if (obj is null)
    {
      return (0);
    }

   /* if (!((IsCast!( T )(obj))))
    {
      throw  new ArgumentException( (new String ("Argument is not compatible")),  (new String ("obj"))); 
    }

    return (IEqualityComparer_T_GetHashCode(AsCast!( T )(obj)));*/
    return 0;
  
}

bool Equals(NObject x, NObject y,IEqualityComparer __j=null) 
  {

    if (x==y)
    {

      return (true);
    
}
    if (x is null||y is null)
    {

      return (false);
    
}
   /* if (!((IsCast!( T )(x))))
    {

      throw  new ArgumentException( (new String ("Argument is not compatible")),  (new String ("x")));
    
}
    if (!((IsCast!( T )(y))))
    {

      throw  new ArgumentException( (new String ("Argument is not compatible")),  (new String ("y")));
    
}
    return (IEqualityComparer_T_Equals(AsCast!( T )(x), AsCast!( T )(y)));
  */
  return false;
}

public int IndexOf(Array_T!(T) array, T value, int startIndex, int endIndex) 
  {

    /*for (int i = startIndex;i<endIndex;++i)
      {

        if (IEqualityComparer_T_Equals(Array_T.UnsafeLoad!( T )(array, i), value))
        {
          return (i);        
        }
      }*/
      return (-1);
    
}
    static this()
    {

       /*Type  t = new System.Type(classOf[T]);
      if (t==new System.Type(classOf[String]))
      {

        _default=AsCast!( System.Collections.Generic.Namespace.EqualityComparer_T!(T) )(AsCast!( System.Namespace.NObject )( new InternalStringComparer()));
        return;
      
}
      if (t==new System.Type(classOf[int]))
      {

        _default=AsCast!( System.Collections.Generic.Namespace.EqualityComparer_T!(T) )(AsCast!( System.Namespace.NObject )( new IntEqualityComparer()));
        return;
      
}
      if (t._Type_IsEnum&&Enum.GetUnderlyingType(t)==new System.Type(classOf[int]))
      {

        _default= new EnumIntEqualityComparer_T!(T)();
        return;
      
}
      if (new System.Type(classOf[IEquatable_T!(T)])._Type_IsAssignableFrom(t))
      {

        _default=AsCast!( System.Collections.Generic.Namespace.EqualityComparer_T!(T) )(Activator.CreateInstance(new System.Type(classOf[GenericEqualityComparer_T]).MakeGenericType(t)));
      
}
      else*/
      {

        //_default= new DefaultComparer_T!(T)();
      
}
      _default = cast(EqualityComparer__G!(T)) null;

    
}

};