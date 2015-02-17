module System.Collections.Generic.KeyValuePair_TKey_TValue;


import System.Namespace;
import System.Collections.Generic.Namespace;

 struct KeyValuePair_TKey_TValue( TKey , TValue )
{

  private TKey key = cast(TKey) null;
  private TValue value = cast(TValue) null;
  

public    TKey  Key() @property  {

    {

      return (key);
    
}
  
}

public   void Key( TKey  value ) @property  {

    {

      this.key=value;
    
}
  
}

  

public    TValue  Value() @property  {

    {

      return (value);
    
}
  
}

public   void Value( TValue  value ) @property  {

    {

      this.value=value;
    
}
  
}


public String ToString() 
  {

    return ( (new String ("["))+((this.Key !is null) ? (Key.ToString()) : (String.Empty))+ (new String (", "))+((this.Value !is null) ? (Value.ToString()) : (String.Empty))+ new String ("]"));
  
}


public this(TKey key, TValue value)
  {

    this.key=key;
    this.value=value;
  
}

};