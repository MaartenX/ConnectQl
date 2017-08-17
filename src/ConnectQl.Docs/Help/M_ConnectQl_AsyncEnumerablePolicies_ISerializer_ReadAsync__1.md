# ISerializer.ReadAsync(*T*) Method 
 

Reads items from the stream.

**Namespace:**&nbsp;<a href="N_ConnectQl_AsyncEnumerablePolicies">ConnectQl.AsyncEnumerablePolicies</a><br />**Assembly:**&nbsp;ConnectQl (in ConnectQl.dll) Version: 100.0.1-prerelease

## Syntax

**C#**<br />
``` C#
Task<IEnumerable<T>> ReadAsync<T>(
	Stream stream,
	long count
)

```


#### Parameters
&nbsp;<dl><dt>stream</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/8f86tw9e" target="_blank">System.IO.Stream</a><br />The stream.</dd><dt>count</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/6yy583ek" target="_blank">System.Int64</a><br />The number of items to read.</dd></dl>

#### Type Parameters
&nbsp;<dl><dt>T</dt><dd>The type of the items.</dd></dl>

#### Return Value
Type: <a href="http://msdn2.microsoft.com/en-us/library/dd321424" target="_blank">Task</a>(<a href="http://msdn2.microsoft.com/en-us/library/9eekhta0" target="_blank">IEnumerable</a>(*T*))<br />The items.

## See Also


#### Reference
<a href="T_ConnectQl_AsyncEnumerablePolicies_ISerializer">ISerializer Interface</a><br /><a href="N_ConnectQl_AsyncEnumerablePolicies">ConnectQl.AsyncEnumerablePolicies Namespace</a><br />