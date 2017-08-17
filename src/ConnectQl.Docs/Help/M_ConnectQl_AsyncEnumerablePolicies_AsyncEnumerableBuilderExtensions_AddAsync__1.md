# AsyncEnumerableBuilderExtensions.AddAsync(*T*) Method 
 

Adds items to the <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable(T)</a>.

**Namespace:**&nbsp;<a href="N_ConnectQl_AsyncEnumerablePolicies">ConnectQl.AsyncEnumerablePolicies</a><br />**Assembly:**&nbsp;ConnectQl (in ConnectQl.dll) Version: 100.0.1-prerelease

## Syntax

**C#**<br />
``` C#
public static Task<IAsyncEnumerableBuilder<T>> AddAsync<T>(
	this IAsyncEnumerableBuilder<T> builder,
	IAsyncEnumerable<T> items
)

```


#### Parameters
&nbsp;<dl><dt>builder</dt><dd>Type: <a href="T_ConnectQl_AsyncEnumerablePolicies_IAsyncEnumerableBuilder_1">ConnectQl.AsyncEnumerablePolicies.IAsyncEnumerableBuilder</a>(*T*)<br />The builder.</dd><dt>items</dt><dd>Type: <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">ConnectQl.AsyncEnumerables.IAsyncEnumerable</a>(*T*)<br />The items to add.</dd></dl>

#### Type Parameters
&nbsp;<dl><dt>T</dt><dd>The type of the items to add.</dd></dl>

#### Return Value
Type: <a href="http://msdn2.microsoft.com/en-us/library/dd321424" target="_blank">Task</a>(<a href="T_ConnectQl_AsyncEnumerablePolicies_IAsyncEnumerableBuilder_1">IAsyncEnumerableBuilder</a>(*T*))<br />The <a href="http://msdn2.microsoft.com/en-us/library/dd235678" target="_blank">Task</a>.

#### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type <a href="T_ConnectQl_AsyncEnumerablePolicies_IAsyncEnumerableBuilder_1">IAsyncEnumerableBuilder</a>(*T*). When you use instance method syntax to call this method, omit the first parameter. For more information, see <a href="http://msdn.microsoft.com/en-us/library/bb384936.aspx">Extension Methods (Visual Basic)</a> or <a href="http://msdn.microsoft.com/en-us/library/bb383977.aspx">Extension Methods (C# Programming Guide)</a>.

## See Also


#### Reference
<a href="T_ConnectQl_AsyncEnumerablePolicies_AsyncEnumerableBuilderExtensions">AsyncEnumerableBuilderExtensions Class</a><br /><a href="N_ConnectQl_AsyncEnumerablePolicies">ConnectQl.AsyncEnumerablePolicies Namespace</a><br />