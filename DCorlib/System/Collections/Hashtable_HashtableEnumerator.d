module System.Collections.Hashtable_HashtableEnumerator;


import System.Namespace;
import System.Collections.Namespace;

 class Hashtable_HashtableEnumerator :  NObject ,  IEnumerator
{
  Hashtable ht = cast(Hashtable) null;
  Hashtable_Entry temp = cast(Hashtable_Entry) null;
  int index = -1;
  Hashtable_EnumeratorType returnType;// = cast(Hashtable_EnumeratorType) null;
  

public    NObject  Current(IEnumerator j=null) @property  
{
    {

      switch( returnType )
      {

        case Hashtable_EnumeratorType.DE:
          //return ((DictionaryEntry(temp.key, temp.value)));
          break;        

        case Hashtable_EnumeratorType.KEY:
          return (temp.key);
          break;        

        case Hashtable_EnumeratorType.VALUE:
          return (temp.value);
          break;

        default:
          break;
      }

      //return ((DictionaryEntry(temp.key, temp.value)));
      //Fix this
      return null;
    }
}


public bool MoveNext(IEnumerator j=null) 
  {
    startLoop:
    if (this.temp is null)
    {
      this.index++;
      //if (this.index<ht._numberOfBuckets)
      {
        //this.temp=ht._buckets[index];
      }
      //else
      {
        return (false);
      }
    }
    else
    {
      this.temp=temp.next;
    }
    if (this.temp is null)
    {
      goto startLoop;    }
    return (true);
  }

public void Reset(IEnumerator j=null) 
  {
    this.index=-1;
  }


public this(Hashtable hashtable, Hashtable_EnumeratorType cstype)
  {
    this.ht=hashtable;
    this.returnType=cstype;
  }

};