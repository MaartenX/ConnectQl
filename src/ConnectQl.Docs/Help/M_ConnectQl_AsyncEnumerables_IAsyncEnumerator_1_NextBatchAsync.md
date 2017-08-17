# IAsyncEnumerator(*T*).NextBatchAsync Method 
 

Advances the enumerator to the next batch of elements of the collection.

**Namespace:**&nbsp;<a href="N_ConnectQl_AsyncEnumerables">ConnectQl.AsyncEnumerables</a><br />**Assembly:**&nbsp;ConnectQl (in ConnectQl.dll) Version: 100.0.1-prerelease

## Syntax

**C#**<br />
``` C#
Task<bool> NextBatchAsync()
```


#### Return Value
Type: <a href="http://msdn2.microsoft.com/en-us/library/dd321424" target="_blank">Task</a>(<a href="http://msdn2.microsoft.com/en-us/library/a28wyd50" target="_blank">Boolean</a>)<br />True if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.

## Exceptions
&nbsp;<table><tr><th>Exception</th><th>Condition</th></tr><tr><td><a href="http://msdn2.microsoft.com/en-us/library/2asft85a" target="_blank">InvalidOperationException</a></td><td>The collection was modified after the enumerator was created.</td></tr></table>

## See Also


#### Reference
<a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerator_1">IAsyncEnumerator(T) Interface</a><br /><a href="N_ConnectQl_AsyncEnumerables">ConnectQl.AsyncEnumerables Namespace</a><br />