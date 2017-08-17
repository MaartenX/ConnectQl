# IAsyncEnumerator(*T*) Interface
 

The async enumerator interface.

**Namespace:**&nbsp;<a href="N_ConnectQl_AsyncEnumerables">ConnectQl.AsyncEnumerables</a><br />**Assembly:**&nbsp;ConnectQl (in ConnectQl.dll) Version: 100.0.1-prerelease

## Syntax

**C#**<br />
``` C#
public interface IAsyncEnumerator<out T> : IDisposable

```


#### Type Parameters
&nbsp;<dl><dt>T</dt><dd>The type of the elements.</dd></dl>&nbsp;
The IAsyncEnumerator(T) type exposes the following members.


## Properties
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td><a href="P_ConnectQl_AsyncEnumerables_IAsyncEnumerator_1_Current">Current</a></td><td>
Gets the element in the collection at the current position of the enumerator.</td></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td><a href="P_ConnectQl_AsyncEnumerables_IAsyncEnumerator_1_IsSynchronous">IsSynchronous</a></td><td>
Gets a value indicating whether the enumerator is synchronous. When `false`, <a href="M_ConnectQl_AsyncEnumerables_IAsyncEnumerator_1_NextBatchAsync">NextBatchAsync()</a> must be called when <a href="M_ConnectQl_AsyncEnumerables_IAsyncEnumerator_1_MoveNext">MoveNext()</a> returns `false`.</td></tr></table>&nbsp;
<a href="#iasyncenumerator(*t*)-interface">Back to Top</a>

## Methods
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="http://msdn2.microsoft.com/en-us/library/es4s3w1d" target="_blank">Dispose</a></td><td>
Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
 (Inherited from <a href="http://msdn2.microsoft.com/en-us/library/aax125c9" target="_blank">IDisposable</a>.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_ConnectQl_AsyncEnumerables_IAsyncEnumerator_1_MoveNext">MoveNext</a></td><td>
Advances the enumerator to the next element of the collection.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_ConnectQl_AsyncEnumerables_IAsyncEnumerator_1_NextBatchAsync">NextBatchAsync</a></td><td>
Advances the enumerator to the next batch of elements of the collection.</td></tr></table>&nbsp;
<a href="#iasyncenumerator(*t*)-interface">Back to Top</a>

## See Also


#### Reference
<a href="N_ConnectQl_AsyncEnumerables">ConnectQl.AsyncEnumerables Namespace</a><br />