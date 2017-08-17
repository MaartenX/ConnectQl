# AsyncEnumerableExtensions.Zip(*TLeft*, *TRight*, *TResult*) Method 
 

Enumerates the two <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable(T)</a>s and calls a function on each pair.

**Namespace:**&nbsp;<a href="N_ConnectQl_AsyncEnumerables">ConnectQl.AsyncEnumerables</a><br />**Assembly:**&nbsp;ConnectQl (in ConnectQl.dll) Version: 100.0.1-prerelease

## Syntax

**C#**<br />
``` C#
public static IAsyncEnumerable<TResult> Zip<TLeft, TRight, TResult>(
	this IAsyncEnumerable<TLeft> left,
	IAsyncEnumerable<TRight> right,
	Func<TLeft, TRight, TResult> resultSelector
)

```


#### Parameters
&nbsp;<dl><dt>left</dt><dd>Type: <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">ConnectQl.AsyncEnumerables.IAsyncEnumerable</a>(*TLeft*)<br />The left.</dd><dt>right</dt><dd>Type: <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">ConnectQl.AsyncEnumerables.IAsyncEnumerable</a>(*TRight*)<br />The right.</dd><dt>resultSelector</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/bb534647" target="_blank">System.Func</a>(*TLeft*, *TRight*, *TResult*)<br />The result selector.</dd></dl>

#### Type Parameters
&nbsp;<dl><dt>TLeft</dt><dd>The type of the left enumerable.</dd><dt>TRight</dt><dd>The type of the right enumerable.</dd><dt>TResult</dt><dd>The type of the result.</dd></dl>

#### Return Value
Type: <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable</a>(*TResult*)<br />The <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable(T)</a>.

#### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable</a>(*TLeft*). When you use instance method syntax to call this method, omit the first parameter. For more information, see <a href="http://msdn.microsoft.com/en-us/library/bb384936.aspx">Extension Methods (Visual Basic)</a> or <a href="http://msdn.microsoft.com/en-us/library/bb383977.aspx">Extension Methods (C# Programming Guide)</a>.

## See Also


#### Reference
<a href="T_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions">AsyncEnumerableExtensions Class</a><br /><a href="N_ConnectQl_AsyncEnumerables">ConnectQl.AsyncEnumerables Namespace</a><br />