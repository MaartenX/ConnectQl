# IOrderedAsyncEnumerable(*TElement*).CreateOrderedAsyncEnumerable(*TKey*) Method 
 

Performs a subsequent ordering on the elements of an <a href="T_ConnectQl_AsyncEnumerables_IOrderedAsyncEnumerable_1">IOrderedAsyncEnumerable(TElement)</a> according to a key.

**Namespace:**&nbsp;<a href="N_ConnectQl_AsyncEnumerables">ConnectQl.AsyncEnumerables</a><br />**Assembly:**&nbsp;ConnectQl (in ConnectQl.dll) Version: 100.0.1-prerelease

## Syntax

**C#**<br />
``` C#
IOrderedAsyncEnumerable<TElement> CreateOrderedAsyncEnumerable<TKey>(
	Func<TElement, TKey> keySelector,
	IComparer<TKey> comparer,
	bool descending
)

```


#### Parameters
&nbsp;<dl><dt>keySelector</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/bb549151" target="_blank">System.Func</a>(<a href="T_ConnectQl_AsyncEnumerables_IOrderedAsyncEnumerable_1">*TElement*</a>, *TKey*)<br />The <a href="http://msdn2.microsoft.com/en-us/library/bb549151" target="_blank">Func(T, TResult)</a> used to extract the key for each element.</dd><dt>comparer</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/8ehhxeaf" target="_blank">System.Collections.Generic.IComparer</a>(*TKey*)<br />The <a href="http://msdn2.microsoft.com/en-us/library/8ehhxeaf" target="_blank">IComparer(T)</a> used to compare keys for placement in the returned sequence.</dd><dt>descending</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/a28wyd50" target="_blank">System.Boolean</a><br />`true` to sort the elements in descending order; `false` to sort the elements in ascending order.</dd></dl>

#### Type Parameters
&nbsp;<dl><dt>TKey</dt><dd>The type of the key produced by *keySelector*.</dd></dl>

#### Return Value
Type: <a href="T_ConnectQl_AsyncEnumerables_IOrderedAsyncEnumerable_1">IOrderedAsyncEnumerable</a>(<a href="T_ConnectQl_AsyncEnumerables_IOrderedAsyncEnumerable_1">*TElement*</a>)<br />An <a href="T_ConnectQl_AsyncEnumerables_IOrderedAsyncEnumerable_1">IOrderedAsyncEnumerable(TElement)</a> whose elements are sorted according to a key.

## See Also


#### Reference
<a href="T_ConnectQl_AsyncEnumerables_IOrderedAsyncEnumerable_1">IOrderedAsyncEnumerable(TElement) Interface</a><br /><a href="N_ConnectQl_AsyncEnumerables">ConnectQl.AsyncEnumerables Namespace</a><br />