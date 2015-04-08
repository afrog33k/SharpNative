module System.Func__G;

import System.Namespace;

template Func__G( T , TResult )
{
	alias __Delegate!(TResult delegate(T arg) ) Func__G;
}