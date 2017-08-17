# MaterializationPolicyExtensions.CreateAsyncEnumerable(*T*, *TState*) Method (IMaterializationPolicy, Func(*TState*, Task(IEnumerable(*T*))), Action(*TState*))
 

Creates an <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable(T)</a> from a generator.

**Namespace:**&nbsp;<a href="N_ConnectQl_AsyncEnumerablePolicies">ConnectQl.AsyncEnumerablePolicies</a><br />**Assembly:**&nbsp;ConnectQl (in ConnectQl.dll) Version: 100.0.1-prerelease

## Syntax

**C#**<br />
``` C#
public static IAsyncEnumerable<T> CreateAsyncEnumerable<T, TState>(
	this IMaterializationPolicy policy,
	Func<TState, Task<IEnumerable<T>>> generateItems,
	Action<TState> dispose = null
)
where TState : new()

```


#### Parameters
&nbsp;<dl><dt>policy</dt><dd>Type: <a href="T_ConnectQl_AsyncEnumerablePolicies_IMaterializationPolicy">ConnectQl.AsyncEnumerablePolicies.IMaterializationPolicy</a><br />The policy.</dd><dt>generateItems</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/bb549151" target="_blank">System.Func</a>(*TState*, <a href="http://msdn2.microsoft.com/en-us/library/dd321424" target="_blank">Task</a>(<a href="http://msdn2.microsoft.com/en-us/library/9eekhta0" target="_blank">IEnumerable</a>(*T*)))<br />Returns a batch of items or `null` when no more batches are available.</dd><dt>dispose (Optional)</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/018hxwa8" target="_blank">System.Action</a>(*TState*)<br />Function to call when disposing the generator.</dd></dl>

#### Type Parameters
&nbsp;<dl><dt>T</dt><dd>The type of the items.</dd><dt>TState</dt><dd>The type of the enumerator state.</dd></dl>

#### Return Value
Type: <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable</a>(*T*)<br />The <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable(T)</a>.

#### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type <a href="T_ConnectQl_AsyncEnumerablePolicies_IMaterializationPolicy">IMaterializationPolicy</a>. When you use instance method syntax to call this method, omit the first parameter. For more information, see <a href="http://msdn.microsoft.com/en-us/library/bb384936.aspx">Extension Methods (Visual Basic)</a> or <a href="http://msdn.microsoft.com/en-us/library/bb383977.aspx">Extension Methods (C# Programming Guide)</a>.

## See Also


#### Reference
<a href="T_ConnectQl_AsyncEnumerablePolicies_MaterializationPolicyExtensions">MaterializationPolicyExtensions Class</a><br /><a href="Overload_ConnectQl_AsyncEnumerablePolicies_MaterializationPolicyExtensions_CreateAsyncEnumerable">CreateAsyncEnumerable Overload</a><br /><a href="N_ConnectQl_AsyncEnumerablePolicies">ConnectQl.AsyncEnumerablePolicies Namespace</a><br />