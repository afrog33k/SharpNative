set PATH=C:\D\dmd2\windows\\bin;C:\Program Files (x86)\Windows Kits\8.1\\\bin;%PATH%

echo System\Collections\Generic\Comparer_T.d >Debug\DCorlib.build.rsp
echo System\Collections\Generic\Comparer_T_DefaultComparerT.d >>Debug\DCorlib.build.rsp
echo System\Collections\Generic\DefaultComparer_T.d >>Debug\DCorlib.build.rsp
echo System\Collections\Generic\Dictionary_TKey_TValue.d >>Debug\DCorlib.build.rsp
echo System\Collections\Generic\Dictionary_TKey_TValue_EnumeratorTKey_TValue.d >>Debug\DCorlib.build.rsp
echo System\Collections\Generic\Dictionary_TKey_TValue_KeyCollectionTKey_TValue.d >>Debug\DCorlib.build.rsp
echo System\Collections\Generic\Dictionary_TKey_TValue_KeyCollection_EnumeratorTKey_TValue.d >>Debug\DCorlib.build.rsp
echo System\Collections\Generic\Dictionary_TKey_TValue_ShimEnumeratorTKey_TValue.d >>Debug\DCorlib.build.rsp
echo System\Collections\Generic\Dictionary_TKey_TValue_Transform_TRetTKey_TValue.d >>Debug\DCorlib.build.rsp
echo System\Collections\Generic\Dictionary_TKey_TValue_ValueCollectionTKey_TValue.d >>Debug\DCorlib.build.rsp
echo System\Collections\Generic\Dictionary_TKey_TValue_ValueCollection_EnumeratorTKey_TValue.d >>Debug\DCorlib.build.rsp
echo System\Collections\Generic\EnumIntEqualityComparer_T.d >>Debug\DCorlib.build.rsp
echo System\Collections\Generic\EqualityComparer_T.d >>Debug\DCorlib.build.rsp
echo System\Collections\Generic\GenericComparer_T.d >>Debug\DCorlib.build.rsp
echo System\Collections\Generic\GenericEqualityComparer_T.d >>Debug\DCorlib.build.rsp
echo System\Collections\Generic\ICollection_T.d >>Debug\DCorlib.build.rsp
echo System\Collections\Generic\IComparer_T.d >>Debug\DCorlib.build.rsp
echo System\Collections\Generic\IDictionary_TKey_TValue.d >>Debug\DCorlib.build.rsp
echo System\Collections\Generic\IEnumerable_T.d >>Debug\DCorlib.build.rsp
echo System\Collections\Generic\IEnumerator_T.d >>Debug\DCorlib.build.rsp
echo System\Collections\Generic\IEqualityComparer_T.d >>Debug\DCorlib.build.rsp
echo System\Collections\Generic\IList_T.d >>Debug\DCorlib.build.rsp
echo System\Collections\Generic\IntEqualityComparer.d >>Debug\DCorlib.build.rsp
echo System\Collections\Generic\InternalStringComparer.d >>Debug\DCorlib.build.rsp
echo System\Collections\Generic\KeyNotFoundException.d >>Debug\DCorlib.build.rsp
echo System\Collections\Generic\KeyValuePair_TKey_TValue.d >>Debug\DCorlib.build.rsp
echo System\Collections\Generic\Link.d >>Debug\DCorlib.build.rsp
echo System\Collections\Generic\List_T.d >>Debug\DCorlib.build.rsp
echo System\Collections\Generic\List_T_EnumeratorT.d >>Debug\DCorlib.build.rsp
echo System\Collections\Generic\System.Collections.Generic.Namespace.d >>Debug\DCorlib.build.rsp
echo System\Collections\Collection.d >>Debug\DCorlib.build.rsp
echo System\Collections\Collections.d >>Debug\DCorlib.build.rsp
echo System\Collections\Hashtable.d >>Debug\DCorlib.build.rsp
echo System\Collections\Hashtable_Entry.d >>Debug\DCorlib.build.rsp
echo System\Collections\Hashtable_EnumeratorType.d >>Debug\DCorlib.build.rsp
echo System\Collections\Hashtable_HashtableEnumerator.d >>Debug\DCorlib.build.rsp
echo System\Collections\Hashtable_KeyCollection.d >>Debug\DCorlib.build.rsp
echo System\Collections\Hashtable_ValueCollection.d >>Debug\DCorlib.build.rsp
echo System\Collections\ICollection.d >>Debug\DCorlib.build.rsp
echo System\Collections\IDictionary.d >>Debug\DCorlib.build.rsp
echo System\Collections\IEnumerable.d >>Debug\DCorlib.build.rsp
echo System\Collections\IEnumerator.d >>Debug\DCorlib.build.rsp
echo System\Collections\IEqualityComparer.d >>Debug\DCorlib.build.rsp
echo System\Collections\IList.d >>Debug\DCorlib.build.rsp
echo System\Collections\SortedList.d >>Debug\DCorlib.build.rsp
echo System\IO\File.d >>Debug\DCorlib.build.rsp
echo System\Runtime\InteropServices\InteropServices.d >>Debug\DCorlib.build.rsp
echo System\Text\IFormatProvider.d >>Debug\DCorlib.build.rsp
echo System\Text\StringBuilder.d >>Debug\DCorlib.build.rsp
echo System\Text\Text.d >>Debug\DCorlib.build.rsp
echo System\Array.d >>Debug\DCorlib.build.rsp
echo System\Array_T.d >>Debug\DCorlib.build.rsp
echo System\Boolean.d >>Debug\DCorlib.build.rsp
echo System\Char.d >>Debug\DCorlib.build.rsp
echo System\Console.d >>Debug\DCorlib.build.rsp
echo System\Double.d >>Debug\DCorlib.build.rsp
echo System\EmptyArray_T.d >>Debug\DCorlib.build.rsp
echo System\Environment.d >>Debug\DCorlib.build.rsp
echo System\ICloneable.d >>Debug\DCorlib.build.rsp
echo System\Int32.d >>Debug\DCorlib.build.rsp
echo System\Math.d >>Debug\DCorlib.build.rsp
echo System\NObject.d >>Debug\DCorlib.build.rsp
echo System\Single.d >>Debug\DCorlib.build.rsp
echo System\String.d >>Debug\DCorlib.build.rsp
echo System\System.d >>Debug\DCorlib.build.rsp

"C:\Program Files (x86)\VisualD\pipedmd.exe" dmd -lib -g -debug -X -Xf"Debug\DCorlib.json" -deps="Debug\DCorlib.dep" -of"Debug\DCorlib.lib" -map "Debug\DCorlib.map" -L/NOMAP @Debug\DCorlib.build.rsp
if errorlevel 1 goto reportError
if not exist "Debug\DCorlib.lib" (echo "Debug\DCorlib.lib" not created! && goto reportError)

goto noError

:reportError
echo Building Debug\DCorlib.lib failed!

:noError
