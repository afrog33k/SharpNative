module System.Action__G;

import System.Namespace;


template Action__G( T )
{
	alias __Delegate!(void delegate(T arg) ) Action__G;
}
