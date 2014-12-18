module System.Collections.Hashtable_Entry;


import System.Namespace;
import System.Collections.Namespace;

 class Hashtable_Entry :  NObject
{
  public NObject key = cast(NObject) null;
  public NObject value = cast(NObject) null;
  public Hashtable_Entry next = cast(Hashtable_Entry) null;


public this(NObject key, NObject value,  ref Hashtable_Entry n)
  {
    this.key=key;
    this.value=value;
    this.next=n;
  }

};