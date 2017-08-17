# AsyncEnumerableExtensions.OuterApply(*TLeft*, *TRight*, *TResult*) Method 
 

Applies a function to all elements and combines the results, when no results are returned, the result selector is called with the default value for *TRight*.

**Namespace:**&nbsp;<a href="N_ConnectQl_AsyncEnumerables">ConnectQl.AsyncEnumerables</a><br />**Assembly:**&nbsp;ConnectQl (in ConnectQl.dll) Version: 100.0.1-prerelease

## Syntax

**C#**<br />
``` C#
public static IAsyncEnumerable<TResult> OuterApply<TLeft, TRight, TResult>(
	this IAsyncEnumerable<TLeft> source,
	Func<TLeft, IAsyncEnumerable<TRight>> function,
	Func<TLeft, TRight, TResult> resultSelector
)

```


#### Parameters
&nbsp;<dl><dt>source</dt><dd>Type: <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">ConnectQl.AsyncEnumerables.IAsyncEnumerable</a>(*TLeft*)<br />The source.</dd><dt>function</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/bb549151" target="_blank">System.Func</a>(*TLeft*, <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable</a>(*TRight*))<br />The function.</dd><dt>resultSelector</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/bb534647" target="_blank">System.Func</a>(*TLeft*, *TRight*, *TResult*)<br />The result selector.</dd></dl>

#### Type Parameters
&nbsp;<dl><dt>TLeft</dt><dd>The type of the left items.</dd><dt>TRight</dt><dd>The type of the right items.</dd><dt>TResult</dt><dd>The type of the result items.</dd></dl>

#### Return Value
Type: <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable</a>(*TResult*)<br />The <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable(T)</a>.

#### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable</a>(*TLeft*). When you use instance method syntax to call this method, omit the first parameter. For more information, see <a href="http://msdn.microsoft.com/en-us/library/bb384936.aspx">Extension Methods (Visual Basic)</a> or <a href="http://msdn.microsoft.com/en-us/library/bb383977.aspx">Extension Methods (C# Programming Guide)</a>.

## See Also


#### Reference
<a href="T_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions">AsyncEnumerableExtensions Class</a><br /><a href="N_ConnectQl_AsyncEnumerables">ConnectQl.AsyncEnumerables Namespace</a><br />