# ISerializer.WriteAsync(*T*) Method 
 

Writes the values to the stream.

**Namespace:**&nbsp;<a href="N_ConnectQl_AsyncEnumerablePolicies">ConnectQl.AsyncEnumerablePolicies</a><br />**Assembly:**&nbsp;ConnectQl (in ConnectQl.dll) Version: 100.0.1-prerelease

## Syntax

**C#**<br />
``` C#
Task<long> WriteAsync<T>(
	Stream stream,
	IEnumerable<T> value
)

```


#### Parameters
&nbsp;<dl><dt>stream</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/8f86tw9e" target="_blank">System.IO.Stream</a><br />The stream to write to.</dd><dt>value</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/9eekhta0" target="_blank">System.Collections.Generic.IEnumerable</a>(*T*)<br />The items to write.</dd></dl>

#### Type Parameters
&nbsp;<dl><dt>T</dt><dd>The type of the items.</dd></dl>

#### Return Value
Type: <a href="http://msdn2.microsoft.com/en-us/library/dd321424" target="_blank">Task</a>(<a href="http://msdn2.microsoft.com/en-us/library/6yy583ek" target="_blank">Int64</a>)<br />The number of items written.

## See Also


#### Reference
<a href="T_ConnectQl_AsyncEnumerablePolicies_ISerializer">ISerializer Interface</a><br /><a href="N_ConnectQl_AsyncEnumerablePolicies">ConnectQl.AsyncEnumerablePolicies Namespace</a><br />