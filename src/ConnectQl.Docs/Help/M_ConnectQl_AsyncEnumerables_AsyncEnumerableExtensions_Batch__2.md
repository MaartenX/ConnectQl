# AsyncEnumerableExtensions.Batch(*TSource*, *TValue*) Method (IAsyncEnumerable(*TSource*), Int32, Func(*TSource*, *TValue*), IComparer(*TValue*))
 

Splits the asynchronous enumerable into batches of at most *batchSize* elements that have the same value.

**Namespace:**&nbsp;<a href="N_ConnectQl_AsyncEnumerables">ConnectQl.AsyncEnumerables</a><br />**Assembly:**&nbsp;ConnectQl (in ConnectQl.dll) Version: 100.0.1-prerelease

## Syntax

**C#**<br />
``` C#
public static IAsyncEnumerable<IAsyncEnumerable<TSource>> Batch<TSource, TValue>(
	this IAsyncEnumerable<TSource> source,
	int batchSize,
	Func<TSource, TValue> valueSelector,
	IComparer<TValue> comparer = null
)

```


#### Parameters
&nbsp;<dl><dt>source</dt><dd>Type: <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">ConnectQl.AsyncEnumerables.IAsyncEnumerable</a>(*TSource*)<br />The source.</dd><dt>batchSize</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/td2s409d" target="_blank">System.Int32</a><br />The batch size.</dd><dt>valueSelector</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/bb549151" target="_blank">System.Func</a>(*TSource*, *TValue*)<br />A function that returns the value that should be equal over a batch.</dd><dt>comparer (Optional)</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/8ehhxeaf" target="_blank">System.Collections.Generic.IComparer</a>(*TValue*)<br />Compares values of batches.</dd></dl>

#### Type Parameters
&nbsp;<dl><dt>TSource</dt><dd>The type of the elements.</dd><dt>TValue</dt><dd>The type of the values that must be identical.</dd></dl>

#### Return Value
Type: <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable</a>(<a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable</a>(*TSource*))<br />The groupings.

#### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable</a>(*TSource*). When you use instance method syntax to call this method, omit the first parameter. For more information, see <a href="http://msdn.microsoft.com/en-us/library/bb384936.aspx">Extension Methods (Visual Basic)</a> or <a href="http://msdn.microsoft.com/en-us/library/bb383977.aspx">Extension Methods (C# Programming Guide)</a>.

## See Also


#### Reference
<a href="T_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions">AsyncEnumerableExtensions Class</a><br /><a href="Overload_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions_Batch">Batch Overload</a><br /><a href="N_ConnectQl_AsyncEnumerables">ConnectQl.AsyncEnumerables Namespace</a><br />