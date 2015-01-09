module System.Collections.IEnumerator;


import System.Namespace;
import System.Collections.Namespace;

 interface IEnumerator
{

public bool MoveNext(IEnumerator j=null);
  

public abstract NObject  Current(IEnumerator j=null) @property;

public void Reset(IEnumerator j=null);

};