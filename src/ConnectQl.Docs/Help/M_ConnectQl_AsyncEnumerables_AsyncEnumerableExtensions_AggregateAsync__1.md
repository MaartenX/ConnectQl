# AsyncEnumerableExtensions.AggregateAsync(*TSource*) Method (IAsyncEnumerable(*TSource*), Func(*TSource*, *TSource*, Task(*TSource*)))
 

Aggregates an <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable(T)</a>.

**Namespace:**&nbsp;<a href="N_ConnectQl_AsyncEnumerables">ConnectQl.AsyncEnumerables</a><br />**Assembly:**&nbsp;ConnectQl (in ConnectQl.dll) Version: 100.0.1-prerelease

## Syntax

**C#**<br />
``` C#
public static Task<TSource> AggregateAsync<TSource>(
	this IAsyncEnumerable<TSource> source,
	Func<TSource, TSource, Task<TSource>> func
)

```


#### Parameters
&nbsp;<dl><dt>source</dt><dd>Type: <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">ConnectQl.AsyncEnumerables.IAsyncEnumerable</a>(*TSource*)<br />The source <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable(T)</a>.</dd><dt>func</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/bb534647" target="_blank">System.Func</a>(*TSource*, *TSource*, <a href="http://msdn2.microsoft.com/en-us/library/dd321424" target="_blank">Task</a>(*TSource*))<br />The aggregator.</dd></dl>

#### Type Parameters
&nbsp;<dl><dt>TSource</dt><dd>The type of the elements in the enumerable.</dd></dl>

#### Return Value
Type: <a href="http://msdn2.microsoft.com/en-us/library/dd321424" target="_blank">Task</a>(*TSource*)<br />The result.

#### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable</a>(*TSource*). When you use instance method syntax to call this method, omit the first parameter. For more information, see <a href="http://msdn.microsoft.com/en-us/library/bb384936.aspx">Extension Methods (Visual Basic)</a> or <a href="http://msdn.microsoft.com/en-us/library/bb383977.aspx">Extension Methods (C# Programming Guide)</a>.

## See Also


#### Reference
<a href="T_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions">AsyncEnumerableExtensions Class</a><br /><a href="Overload_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions_AggregateAsync">AggregateAsync Overload</a><br /><a href="N_ConnectQl_AsyncEnumerables">ConnectQl.AsyncEnumerables Namespace</a><br />