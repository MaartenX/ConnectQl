# AsyncEnumerableExtensions.AggregateAsync(*TSource*, *TAccumulate*) Method (IAsyncEnumerable(*TSource*), *TAccumulate*, Func(*TAccumulate*, *TSource*, *TAccumulate*))
 

Aggregates an <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable(T)</a>.

**Namespace:**&nbsp;<a href="N_ConnectQl_AsyncEnumerables">ConnectQl.AsyncEnumerables</a><br />**Assembly:**&nbsp;ConnectQl (in ConnectQl.dll) Version: 100.0.1-prerelease

## Syntax

**C#**<br />
``` C#
public static Task<TAccumulate> AggregateAsync<TSource, TAccumulate>(
	this IAsyncEnumerable<TSource> source,
	TAccumulate seed,
	Func<TAccumulate, TSource, TAccumulate> func
)

```


#### Parameters
&nbsp;<dl><dt>source</dt><dd>Type: <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">ConnectQl.AsyncEnumerables.IAsyncEnumerable</a>(*TSource*)<br />The source <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable(T)</a>.</dd><dt>seed</dt><dd>Type: *TAccumulate*<br />The initial item.</dd><dt>func</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/bb534647" target="_blank">System.Func</a>(*TAccumulate*, *TSource*, *TAccumulate*)<br />The aggregator.</dd></dl>

#### Type Parameters
&nbsp;<dl><dt>TSource</dt><dd>The type of the elements in the enumerable.</dd><dt>TAccumulate</dt><dd>The result type.</dd></dl>

#### Return Value
Type: <a href="http://msdn2.microsoft.com/en-us/library/dd321424" target="_blank">Task</a>(*TAccumulate*)<br />The result.

#### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable</a>(*TSource*). When you use instance method syntax to call this method, omit the first parameter. For more information, see <a href="http://msdn.microsoft.com/en-us/library/bb384936.aspx">Extension Methods (Visual Basic)</a> or <a href="http://msdn.microsoft.com/en-us/library/bb383977.aspx">Extension Methods (C# Programming Guide)</a>.

## See Also


#### Reference
<a href="T_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions">AsyncEnumerableExtensions Class</a><br /><a href="Overload_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions_AggregateAsync">AggregateAsync Overload</a><br /><a href="N_ConnectQl_AsyncEnumerables">ConnectQl.AsyncEnumerables Namespace</a><br />