module System.Comparison__G;

import System.Namespace;

template Comparison__G(T)
{
	alias __Delegate!(int delegate(T x, T y)) Comparison__G;
}
