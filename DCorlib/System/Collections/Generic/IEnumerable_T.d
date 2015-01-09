module System.Collections.Generic.IEnumerable_T;


import System.Namespace;
import System.Collections.Namespace;
import System.Collections.Generic.Namespace;

interface IEnumerable_T(T) :  System.Collections.Namespace.IEnumerable
{


public IEnumerator_T!(T) GetEnumerator(IEnumerable_T!(T) k = null);

};