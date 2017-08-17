# AsyncEnumerableExtensions.Union(*TSource*) Method 
 

Produces the set union of two sequences by using a specified <a href="http://msdn2.microsoft.com/en-us/library/8ehhxeaf" target="_blank">IComparer(T)</a>. The order of the rows is not preserved. Uses the materialization policy of the first sequence.

**Namespace:**&nbsp;<a href="N_ConnectQl_AsyncEnumerables">ConnectQl.AsyncEnumerables</a><br />**Assembly:**&nbsp;ConnectQl (in ConnectQl.dll) Version: 100.0.1-prerelease

## Syntax

**C#**<br />
``` C#
public static IAsyncEnumerable<TSource> Union<TSource>(
	this IAsyncEnumerable<TSource> first,
	IAsyncEnumerable<TSource> second,
	IComparer<TSource> comparer = null
)

```


#### Parameters
&nbsp;<dl><dt>first</dt><dd>Type: <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">ConnectQl.AsyncEnumerables.IAsyncEnumerable</a>(*TSource*)<br />An <a href="T_ConnectQl_AsyncEnumerables_IAsyncReadOnlyCollection_1">IAsyncReadOnlyCollection(T)</a> whose distinct elements form the first set for the union.</dd><dt>second</dt><dd>Type: <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">ConnectQl.AsyncEnumerables.IAsyncEnumerable</a>(*TSource*)<br />An <a href="T_ConnectQl_AsyncEnumerables_IAsyncReadOnlyCollection_1">IAsyncReadOnlyCollection(T)</a> whose distinct elements form the second set for the union.</dd><dt>comparer (Optional)</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/8ehhxeaf" target="_blank">System.Collections.Generic.IComparer</a>(*TSource*)<br />The <a href="http://msdn2.microsoft.com/en-us/library/8ehhxeaf" target="_blank">IComparer(T)</a> to compare values.</dd></dl>

#### Type Parameters
&nbsp;<dl><dt>TSource</dt><dd>The type of the elements of the input sequences.</dd></dl>

#### Return Value
Type: <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable</a>(*TSource*)<br />The <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable(T)</a>.

#### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable</a>(*TSource*). When you use instance method syntax to call this method, omit the first parameter. For more information, see <a href="http://msdn.microsoft.com/en-us/library/bb384936.aspx">Extension Methods (Visual Basic)</a> or <a href="http://msdn.microsoft.com/en-us/library/bb383977.aspx">Extension Methods (C# Programming Guide)</a>.

## See Also


#### Reference
<a href="T_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions">AsyncEnumerableExtensions Class</a><br /><a href="N_ConnectQl_AsyncEnumerables">ConnectQl.AsyncEnumerables Namespace</a><br />