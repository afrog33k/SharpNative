module System.Predicate__G;


import System.Namespace;

template Predicate__G(T)
{
	alias __Delegate!(bool delegate(T obj)) Predicate__G;
}
