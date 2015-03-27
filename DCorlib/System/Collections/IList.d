module System.Collections.IList;


import System.Namespace;
import System.Collections.Namespace;

 interface IList :  ICollection ,  IEnumerable
{

  

public     abstract bool  IsFixedSize( IList k =null) @property;
  

public     abstract bool  IsReadOnly( IList k =null) @property;
  

public    abstract NObject opIndexAssign(NObject  value, int index, IList __k =null );
public    abstract NObject opIndex(int index, IList __k=null);


public int Add(NObject value, IList k =null);

public void Clear(IList k =null);

public bool Contains(NObject value, IList k =null);

public int IndexOf(NObject value, IList k =null);

public void Insert(int index, NObject value, IList k =null);

public void Remove(NObject value, IList k =null);

public void RemoveAt(int index, IList k =null);

};