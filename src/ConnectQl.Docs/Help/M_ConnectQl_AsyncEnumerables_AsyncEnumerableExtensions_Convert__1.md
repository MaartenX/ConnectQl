# AsyncEnumerableExtensions.Convert(*T*) Method 
 

Converts the <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable">IAsyncEnumerable</a> to a typed <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable(T)</a>.

**Namespace:**&nbsp;<a href="N_ConnectQl_AsyncEnumerables">ConnectQl.AsyncEnumerables</a><br />**Assembly:**&nbsp;ConnectQl (in ConnectQl.dll) Version: 100.0.1-prerelease

## Syntax

**C#**<br />
``` C#
public static IAsyncEnumerable<T> Convert<T>(
	this IAsyncEnumerable source
)

```


#### Parameters
&nbsp;<dl><dt>source</dt><dd>Type: <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable">ConnectQl.AsyncEnumerables.IAsyncEnumerable</a><br />The enumerable to convert.</dd></dl>

#### Type Parameters
&nbsp;<dl><dt>T</dt><dd>The type of the items to convert to.</dd></dl>

#### Return Value
Type: <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable</a>(*T*)<br />The <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable">IAsyncEnumerable</a>.

#### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable">IAsyncEnumerable</a>. When you use instance method syntax to call this method, omit the first parameter. For more information, see <a href="http://msdn.microsoft.com/en-us/library/bb384936.aspx">Extension Methods (Visual Basic)</a> or <a href="http://msdn.microsoft.com/en-us/library/bb383977.aspx">Extension Methods (C# Programming Guide)</a>.

## See Also


#### Reference
<a href="T_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions">AsyncEnumerableExtensions Class</a><br /><a href="N_ConnectQl_AsyncEnumerables">ConnectQl.AsyncEnumerables Namespace</a><br />