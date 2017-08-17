# AsyncEnumerableExtensions.PreSortedJoin(*TLeft*, *TRight*, *TKey*, *TResult*) Method 
 

Joins the two <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable(T)</a>s on a key. When no item is found that matches an item in *left*, *resultSelector* is called with the left item and the default for *TRight*. Both <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable(T)</a>s must be sorted by the keys before calling this method.

**Namespace:**&nbsp;<a href="N_ConnectQl_AsyncEnumerables">ConnectQl.AsyncEnumerables</a><br />**Assembly:**&nbsp;ConnectQl (in ConnectQl.dll) Version: 100.0.1-prerelease

## Syntax

**C#**<br />
``` C#
public static IAsyncEnumerable<TResult> PreSortedJoin<TLeft, TRight, TKey, TResult>(
	this IAsyncEnumerable<TLeft> left,
	IAsyncReadOnlyCollection<TRight> right,
	Func<TLeft, TKey> leftKeySelector,
	ExpressionType joinOperator,
	Func<TRight, TKey> rightKeySelector,
	Func<TLeft, TRight, bool> resultFilter,
	Func<TLeft, TRight, TResult> resultSelector,
	IComparer<TKey> comparer = null
)

```


#### Parameters
&nbsp;<dl><dt>left</dt><dd>Type: <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">ConnectQl.AsyncEnumerables.IAsyncEnumerable</a>(*TLeft*)<br />The left.</dd><dt>right</dt><dd>Type: <a href="T_ConnectQl_AsyncEnumerables_IAsyncReadOnlyCollection_1">ConnectQl.AsyncEnumerables.IAsyncReadOnlyCollection</a>(*TRight*)<br />The right.</dd><dt>leftKeySelector</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/bb549151" target="_blank">System.Func</a>(*TLeft*, *TKey*)<br />The left key selector.</dd><dt>joinOperator</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/bb361179" target="_blank">System.Linq.Expressions.ExpressionType</a><br />The join operator.</dd><dt>rightKeySelector</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/bb549151" target="_blank">System.Func</a>(*TRight*, *TKey*)<br />The right key selector.</dd><dt>resultFilter</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/bb534647" target="_blank">System.Func</a>(*TLeft*, *TRight*, <a href="http://msdn2.microsoft.com/en-us/library/a28wyd50" target="_blank">Boolean</a>)<br />The result filter.</dd><dt>resultSelector</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/bb534647" target="_blank">System.Func</a>(*TLeft*, *TRight*, *TResult*)<br />The result selector.</dd><dt>comparer (Optional)</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/8ehhxeaf" target="_blank">System.Collections.Generic.IComparer</a>(*TKey*)<br />The key comparer.</dd></dl>

#### Type Parameters
&nbsp;<dl><dt>TLeft</dt><dd>The type of the left item.</dd><dt>TRight</dt><dd>The type of the right item.</dd><dt>TKey</dt><dd>The type of the key to join on.</dd><dt>TResult</dt><dd>The type of the result.</dd></dl>

#### Return Value
Type: <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable</a>(*TResult*)<br />The <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable(T)</a>.

#### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable</a>(*TLeft*). When you use instance method syntax to call this method, omit the first parameter. For more information, see <a href="http://msdn.microsoft.com/en-us/library/bb384936.aspx">Extension Methods (Visual Basic)</a> or <a href="http://msdn.microsoft.com/en-us/library/bb383977.aspx">Extension Methods (C# Programming Guide)</a>.

## See Also


#### Reference
<a href="T_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions">AsyncEnumerableExtensions Class</a><br /><a href="N_ConnectQl_AsyncEnumerables">ConnectQl.AsyncEnumerables Namespace</a><br />