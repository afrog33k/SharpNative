module System.Collections.Generic.KeyNotFoundException;


import System.Namespace;
//import System.Runtime.Serialization.Namespace;
import System.Collections.Generic.Namespace;

 class KeyNotFoundException :  NException//SystemException ,  System.Runtime.Serialization.Namespace.ISerializable
{



public this()
  {

    super(new String ("The given key was not present in the dictionary."));
  
}


public this(String message)
  {

    super(message);
  
}


public this(String message, NException innerException)
  {

    super(message, innerException);
  
}


//public this(System.Runtime.Serialization.Namespace.SerializationInfo info, System.Runtime.Serialization.Namespace.StreamingContext context)
//  {

//    super(info, context);
  
//}

};