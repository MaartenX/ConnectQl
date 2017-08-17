# AsyncEnumerableExtensions.Select(*TSource*, *TResult*) Method (IAsyncEnumerable(*TSource*), Func(*TSource*, *TResult*))
 

Projects each element of a sequence into a new form.

**Namespace:**&nbsp;<a href="N_ConnectQl_AsyncEnumerables">ConnectQl.AsyncEnumerables</a><br />**Assembly:**&nbsp;ConnectQl (in ConnectQl.dll) Version: 100.0.1-prerelease

## Syntax

**C#**<br />
``` C#
public static IAsyncEnumerable<TResult> Select<TSource, TResult>(
	this IAsyncEnumerable<TSource> source,
	Func<TSource, TResult> selector
)

```


#### Parameters
&nbsp;<dl><dt>source</dt><dd>Type: <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">ConnectQl.AsyncEnumerables.IAsyncEnumerable</a>(*TSource*)<br />A sequence of values to invoke a transform function on.</dd><dt>selector</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/bb549151" target="_blank">System.Func</a>(*TSource*, *TResult*)<br />A transform function to apply to each element.</dd></dl>

#### Type Parameters
&nbsp;<dl><dt>TSource</dt><dd>The type of the elements of *source*.</dd><dt>TResult</dt><dd>The type of the value returned by *selector*.</dd></dl>

#### Return Value
Type: <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable</a>(*TResult*)<br />An <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable(T)</a> whose elements are the result of invoking the transform function on each element of *source*.

#### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable</a>(*TSource*). When you use instance method syntax to call this method, omit the first parameter. For more information, see <a href="http://msdn.microsoft.com/en-us/library/bb384936.aspx">Extension Methods (Visual Basic)</a> or <a href="http://msdn.microsoft.com/en-us/library/bb383977.aspx">Extension Methods (C# Programming Guide)</a>.

## See Also


#### Reference
<a href="T_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions">AsyncEnumerableExtensions Class</a><br /><a href="Overload_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions_Select">Select Overload</a><br /><a href="N_ConnectQl_AsyncEnumerables">ConnectQl.AsyncEnumerables Namespace</a><br />