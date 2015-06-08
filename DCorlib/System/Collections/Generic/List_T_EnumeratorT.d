module System.Collections.Generic.List__G_EnumeratorT;


import System.Namespace;
import System.Collections.Generic.Namespace;
import System.Collections.Namespace;

 class List__G_EnumeratorT(T) :  IEnumerator_T!(T) ,  IDisposable
{

  List__G!(T) l = cast(List__G!(T)) null;
  int next;
  int ver;
  T current = cast(T) null;

public void IDisposable_Dispose() 
  {

  
}

public bool IEnumerator_MoveNext() 
  {

     List__G!(T)  list = l;
    if ((cast(long)this.next)<(cast(long)list._size)&&this.ver==list._version)
    {

      this.current=list._items[this.next++];
      return (true);
    
}
    if (this.ver!=l._version)
    {

      throw  new InvalidOperationException( ( String ("Collection was modified; enumeration operation may not execute.")));
    
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

      throw  new InvalidOperationException( ( String ("Collection was modified; enumeration operation may not execute.")));
    
}
    this.next=0;
    this.current=cast(T)null;
  
}
  

   NObject  IEnumerator_Current() @property  {

    {

      if (this.ver!=l._version)
      {

        throw  new InvalidOperationException( ( String ("Collection was modified; enumeration operation may not execute.")));
      
}
      if (this.next<=0)
      {

        throw  new InvalidOperationException();
      
}
      return BOX!(T)(current);
    
}
  
}



public this(List__G!(T) l)
  {

    //this();
    this.l=l;
    this.ver=l._version;
  
}

};