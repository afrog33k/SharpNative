module System.Collections.Generic.IEnumerable__G;


import System.Namespace;
import System.Collections.Namespace;
import System.Collections.Generic.Namespace;

interface IEnumerable__G(T) :  System.Collections.Namespace.IEnumerable
{

public IEnumerator__G!(T) GetEnumerator(IEnumerable__G!(T) k = null);

};