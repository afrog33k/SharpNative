module System.Collections.Generic.IntEqualityComparer;


import System.Namespace;
import System.Collections.Generic.Namespace;

 class IntEqualityComparer :  EqualityComparer_T!(int)
{


 override public int GetHashCode(int obj) 
  {

    return (obj);
  
}

 override public bool Equals(int x, int y) 
  {

    return (x==y);
  
}

override public int IndexOf(Array_T!(int) array, int value, int startIndex, int endIndex) 
{

    for (int i = startIndex;i<endIndex;++i)
    {

        if (array[i]==value)//Array.UnsafeLoad!( int )(array, i)==value)
        {
          return (i);
		}
    }
    return (-1);
}

};