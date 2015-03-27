module System.IComparable__G;

import System.Namespace;


interface IComparable__G(T)
{
	// Interface does not need to be marked with the serializable attribute
	// Compares this object to another object, returning an integer that
	// indicates the relationship. An implementation of this method must return
	// a value less than zero if this is less than object, zero
	// if this is equal to object, or a value greater than zero
	// if this is greater than object.
	// 

	public int CompareTo(T other, IComparable__G!(T) __j = null);
}