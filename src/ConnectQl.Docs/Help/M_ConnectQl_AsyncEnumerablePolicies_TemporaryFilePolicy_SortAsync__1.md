# TemporaryFilePolicy.SortAsync(*T*) Method 
 

Creates a new <a href="T_ConnectQl_AsyncEnumerables_IAsyncReadOnlyCollection_1">IAsyncReadOnlyCollection(T)</a> that contains the sorted elements of the <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable(T)</a>.

**Namespace:**&nbsp;<a href="N_ConnectQl_AsyncEnumerablePolicies">ConnectQl.AsyncEnumerablePolicies</a><br />**Assembly:**&nbsp;ConnectQl (in ConnectQl.dll) Version: 100.0.1-prerelease

## Syntax

**C#**<br />
``` C#
public Task<IAsyncReadOnlyCollection<T>> SortAsync<T>(
	IAsyncEnumerable<T> source,
	Comparison<T> comparison
)

```


#### Parameters
&nbsp;<dl><dt>source</dt><dd>Type: <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">ConnectQl.AsyncEnumerables.IAsyncEnumerable</a>(*T*)<br />The <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable(T)</a> to sort.</dd><dt>comparison</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/tfakywbh" target="_blank">System.Comparison</a>(*T*)<br />The comparison to use while sorting.</dd></dl>

#### Type Parameters
&nbsp;<dl><dt>T</dt><dd>The type of the items.</dd></dl>

#### Return Value
Type: <a href="http://msdn2.microsoft.com/en-us/library/dd321424" target="_blank">Task</a>(<a href="T_ConnectQl_AsyncEnumerables_IAsyncReadOnlyCollection_1">IAsyncReadOnlyCollection</a>(*T*))<br />The sorted <a href="T_ConnectQl_AsyncEnumerables_IAsyncReadOnlyCollection_1">IAsyncReadOnlyCollection(T)</a>.

#### Implements
<a href="M_ConnectQl_AsyncEnumerablePolicies_IMaterializationPolicy_SortAsync__1">IMaterializationPolicy.SortAsync(T)(IAsyncEnumerable(T), Comparison(T))</a><br />

## See Also


#### Reference
<a href="T_ConnectQl_AsyncEnumerablePolicies_TemporaryFilePolicy">TemporaryFilePolicy Class</a><br /><a href="N_ConnectQl_AsyncEnumerablePolicies">ConnectQl.AsyncEnumerablePolicies Namespace</a><br />