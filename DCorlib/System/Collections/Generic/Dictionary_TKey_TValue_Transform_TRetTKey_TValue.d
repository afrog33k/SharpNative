module System.Collections.Generic.Dictionary_TKey_TValue_Transform_TRetTKey_TValue;


import System.Namespace;
import System.Collections.Generic.Namespace;

template Dictionary_TKey_TValue_Transform_TRetTKey_TValue( TRet )
{

alias Delegate!(TRet delegate(TKey key, TValue value) ) Dictionary_TKey_TValue_Transform_TRetTKey_TValue;

}
