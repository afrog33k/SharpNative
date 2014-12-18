module System.Collections.Generic.List_T_EnumeratorT;


import System.Namespace;
import System.Collections.Generic.Namespace;
import System.Collections.Namespace;

 class List_T_EnumeratorT(T) :  IEnumerator_T!(T) ,  IDisposable
{

  List_T!(T) l = cast(List_T!(T)) null;
  int next;
  int ver;
  T current = cast(T) null;

public void IDisposable_Dispose() 
  {

  
}

public bool IEnumerator_MoveNext() 
  {

     List_T!(T)  list = l;
    if ((cast(long)this.next)<(cast(long)list._size)&&this.ver==list._version)
    {

      this.current=list._items[this.next++];
      return (true);
    
}
    if (this.ver!=l._version)
    {

      throw  new InvalidOperationException( (new String ("Collection was modified; enumeration operation may not execute.")));
    
}
    this.next=-1;
    return (false);
  
}
  

public    T  IEnumerator_T_Current() @property  {

    {

      return (current);
    
}
  
}


void IEnumerator_Reset() 
  {

    if (this.ver!=l._version)
    {

      throw  new InvalidOperationException( (new String ("Collection was modified; enumeration operation may not execute.")));
    
}
    this.next=0;
    this.current=cast(T)null;
  
}
  

   NObject  IEnumerator_Current() @property  {

    {

      if (this.ver!=l._version)
      {

        throw  new InvalidOperationException( (new String ("Collection was modified; enumeration operation may not execute.")));
      
}
      if (this.next<=0)
      {

        throw  new InvalidOperationException();
      
}
      return BOX!(T)(current);
    
}
  
}



public this(List_T!(T) l)
  {

    //this();
    this.l=l;
    this.ver=l._version;
  
}

};