# AsyncEnumerableExtensions.SortAsync(*TSource*) Method 
 

Sorts the <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable(T)</a> by using the *comparison*.

**Namespace:**&nbsp;<a href="N_ConnectQl_AsyncEnumerables">ConnectQl.AsyncEnumerables</a><br />**Assembly:**&nbsp;ConnectQl (in ConnectQl.dll) Version: 100.0.1-prerelease

## Syntax

**C#**<br />
``` C#
public static Task<IAsyncReadOnlyCollection<TSource>> SortAsync<TSource>(
	this IAsyncEnumerable<TSource> source,
	Comparison<TSource> comparison
)

```


#### Parameters
&nbsp;<dl><dt>source</dt><dd>Type: <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">ConnectQl.AsyncEnumerables.IAsyncEnumerable</a>(*TSource*)<br />A sequence of values to sort.</dd><dt>comparison</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/tfakywbh" target="_blank">System.Comparison</a>(*TSource*)<br />The comparison to use when sorting.</dd></dl>

#### Type Parameters
&nbsp;<dl><dt>TSource</dt><dd>The type of the elements of *source*.</dd></dl>

#### Return Value
Type: <a href="http://msdn2.microsoft.com/en-us/library/dd321424" target="_blank">Task</a>(<a href="T_ConnectQl_AsyncEnumerables_IAsyncReadOnlyCollection_1">IAsyncReadOnlyCollection</a>(*TSource*))<br />The sorted <a href="T_ConnectQl_AsyncEnumerables_IAsyncReadOnlyCollection_1">IAsyncReadOnlyCollection(T)</a>.

#### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable</a>(*TSource*). When you use instance method syntax to call this method, omit the first parameter. For more information, see <a href="http://msdn.microsoft.com/en-us/library/bb384936.aspx">Extension Methods (Visual Basic)</a> or <a href="http://msdn.microsoft.com/en-us/library/bb383977.aspx">Extension Methods (C# Programming Guide)</a>.

## See Also


#### Reference
<a href="T_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions">AsyncEnumerableExtensions Class</a><br /><a href="N_ConnectQl_AsyncEnumerables">ConnectQl.AsyncEnumerables Namespace</a><br />