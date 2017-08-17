# AsyncEnumerableExtensions.OrderBy(*TSource*, *TKey*) Method (IAsyncEnumerable(*TSource*), Func(*TSource*, *TKey*), IComparer(*TKey*))
 

Sorts the elements of a sequence in ascending order according to a key.

**Namespace:**&nbsp;<a href="N_ConnectQl_AsyncEnumerables">ConnectQl.AsyncEnumerables</a><br />**Assembly:**&nbsp;ConnectQl (in ConnectQl.dll) Version: 100.0.1-prerelease

## Syntax

**C#**<br />
``` C#
public static IOrderedAsyncEnumerable<TSource> OrderBy<TSource, TKey>(
	this IAsyncEnumerable<TSource> source,
	Func<TSource, TKey> keySelector,
	IComparer<TKey> comparer = null
)

```


#### Parameters
&nbsp;<dl><dt>source</dt><dd>Type: <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">ConnectQl.AsyncEnumerables.IAsyncEnumerable</a>(*TSource*)<br />A sequence of values to order.</dd><dt>keySelector</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/bb549151" target="_blank">System.Func</a>(*TSource*, *TKey*)<br />A function to extract a key from an element.</dd><dt>comparer (Optional)</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/8ehhxeaf" target="_blank">System.Collections.Generic.IComparer</a>(*TKey*)<br />An <a href="http://msdn2.microsoft.com/en-us/library/8ehhxeaf" target="_blank">IComparer(T)</a> to compare keys.</dd></dl>

#### Type Parameters
&nbsp;<dl><dt>TSource</dt><dd>The type of the elements of *source*.</dd><dt>TKey</dt><dd>The type of the key returned by *keySelector*.</dd></dl>

#### Return Value
Type: <a href="T_ConnectQl_AsyncEnumerables_IOrderedAsyncEnumerable_1">IOrderedAsyncEnumerable</a>(*TSource*)<br />An <a href="T_ConnectQl_AsyncEnumerables_IOrderedAsyncEnumerable_1">IOrderedAsyncEnumerable(TElement)</a> whose elements are sorted according to a key..

#### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable</a>(*TSource*). When you use instance method syntax to call this method, omit the first parameter. For more information, see <a href="http://msdn.microsoft.com/en-us/library/bb384936.aspx">Extension Methods (Visual Basic)</a> or <a href="http://msdn.microsoft.com/en-us/library/bb383977.aspx">Extension Methods (C# Programming Guide)</a>.

## See Also


#### Reference
<a href="T_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions">AsyncEnumerableExtensions Class</a><br /><a href="Overload_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions_OrderBy">OrderBy Overload</a><br /><a href="N_ConnectQl_AsyncEnumerables">ConnectQl.AsyncEnumerables Namespace</a><br />