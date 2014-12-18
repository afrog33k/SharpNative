module System.EmptyArray_T;


import System.Namespace;

 class EmptyArray_T( T ) :  NObject
{

  public static Array_T!(T) Value;
  static this()
  {

    Value =  new Array_T!(T )(0);

  
}

};