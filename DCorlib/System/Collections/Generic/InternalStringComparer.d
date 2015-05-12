module System.Collections.Generic.InternalStringComparer;


import System.Namespace;
import System.Collections.Generic.Namespace;

 class InternalStringComparer :  EqualityComparer__G!(String)
{

	alias GetHashCode = EqualityComparer__G!(String).GetHashCode;

  public int GetHashCode(String obj) 
  {

    if (obj is  null)
    {

      return (0);
    
}
    return (obj.GetHashCode());
  
}

  public bool Equals(String x, String y) 
  {

    if (x is  null)
    {

      return (y is null);
    
}
    if (AsCast!( System.Namespace.NObject )(x)==AsCast!( System.Namespace.NObject )(y))
    {

      return (true);
    
}
    return (x.IEquatable_T_Equals(y));
  
}

 override public int IndexOf(Array_T!(String) array, String value, int startIndex, int endIndex) 
  {

  /*  for (int i = startIndex;i<endIndex;++i)
      {

                if (Array_T.UnsafeLoad!( String )(array, i)==value)
        {

          return (i);
        
}
      }*/
      return (-1);
    
}

};