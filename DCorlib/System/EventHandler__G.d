module System.EventHandler__G;

import System.Namespace;

template EventHandler__G(TEventArgs)
{
	alias __Delegate!(void delegate(NObject sender, TEventArgs e)) EventHandler__G;
}