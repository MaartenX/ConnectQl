# IAsyncReadOnlyCollection(*T*) Interface
 

The MaterializedAsyncEnumerable interface.

**Namespace:**&nbsp;<a href="N_ConnectQl_AsyncEnumerables">ConnectQl.AsyncEnumerables</a><br />**Assembly:**&nbsp;ConnectQl (in ConnectQl.dll) Version: 100.0.1-prerelease

## Syntax

**C#**<br />
``` C#
public interface IAsyncReadOnlyCollection<out T> : IAsyncEnumerable<T>, 
	IAsyncEnumerable, IDisposable

```


#### Type Parameters
&nbsp;<dl><dt>T</dt><dd>The type of the elements.</dd></dl>&nbsp;
The IAsyncReadOnlyCollection(T) type exposes the following members.


## Properties
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td><a href="P_ConnectQl_AsyncEnumerables_IAsyncReadOnlyCollection_1_Count">Count</a></td><td>
Gets the number of elements in the enumerable.</td></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td><a href="P_ConnectQl_AsyncEnumerables_IAsyncEnumerable_Policy">Policy</a></td><td>
Gets the materialization policy.
 (Inherited from <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable">IAsyncEnumerable</a>.)</td></tr></table>&nbsp;
<a href="#iasyncreadonlycollection(*t*)-interface">Back to Top</a>

## Methods
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="http://msdn2.microsoft.com/en-us/library/es4s3w1d" target="_blank">Dispose</a></td><td>
Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
 (Inherited from <a href="http://msdn2.microsoft.com/en-us/library/aax125c9" target="_blank">IDisposable</a>.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1_GetAsyncEnumerator">GetAsyncEnumerator()</a></td><td>
Gets an enumerator that returns batches of elements.
 (Inherited from <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable(T)</a>.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_ConnectQl_AsyncEnumerables_IAsyncReadOnlyCollection_1_GetAsyncEnumerator">GetAsyncEnumerator(Int64)</a></td><td>
Gets an enumerator that returns batches of elements and starts at the offset.</td></tr></table>&nbsp;
<a href="#iasyncreadonlycollection(*t*)-interface">Back to Top</a>

## Extension Methods
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions_AfterElement__1">AfterElement(T)</a></td><td>
Performs an action before the <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable(T)</a> is enumerated.
 (Defined by <a href="T_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions">AsyncEnumerableExtensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions_AfterLastElement__1">AfterLastElement(T)</a></td><td>
Performs an action when the <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable(T)</a> is enumerated.
 (Defined by <a href="T_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions">AsyncEnumerableExtensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions_AggregateAsync__1">AggregateAsync(T)(Func(T, T, Task(T)))</a></td><td>Overloaded.  
Aggregates an <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable(T)</a>.
 (Defined by <a href="T_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions">AsyncEnumerableExtensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions_AggregateAsync__1_1">AggregateAsync(T)(Func(T, T, T))</a></td><td>Overloaded.  
Aggregates an <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable(T)</a>.
 (Defined by <a href="T_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions">AsyncEnumerableExtensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions_AggregateAsync__2_1">AggregateAsync(T, TAccumulate)(TAccumulate, Func(TAccumulate, T, TAccumulate))</a></td><td>Overloaded.  
Aggregates an <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable(T)</a>.
 (Defined by <a href="T_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions">AsyncEnumerableExtensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions_AggregateAsync__2">AggregateAsync(T, TAccumulate)(TAccumulate, Func(TAccumulate, T, Task(TAccumulate)))</a></td><td>Overloaded.  
Aggregates an <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable(T)</a>.
 (Defined by <a href="T_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions">AsyncEnumerableExtensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions_ApplyEnumerableFunction__2">ApplyEnumerableFunction(T, TResult)</a></td><td>
The apply enumerable function.
 (Defined by <a href="T_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions">AsyncEnumerableExtensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions_Batch__1">Batch(T)(Int64)</a></td><td>Overloaded.  
Splits the asynchronous enumerable into batches of *batchSize* elements.
 (Defined by <a href="T_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions">AsyncEnumerableExtensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions_Batch__2">Batch(T, TValue)(Int32, Func(T, TValue), IComparer(TValue))</a></td><td>Overloaded.  
Splits the asynchronous enumerable into batches of at most *batchSize* elements that have the same value.
 (Defined by <a href="T_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions">AsyncEnumerableExtensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions_BeforeFirstElement__1">BeforeFirstElement(T)</a></td><td>
Performs an action before the <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable(T)</a> is enumerated.
 (Defined by <a href="T_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions">AsyncEnumerableExtensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions_Convert__1">Convert(T)</a></td><td>
Converts the <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable">IAsyncEnumerable</a> to a typed <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable(T)</a>.
 (Defined by <a href="T_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions">AsyncEnumerableExtensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions_CountAsync__1">CountAsync(T)()</a></td><td>Overloaded.  
The count async.
 (Defined by <a href="T_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions">AsyncEnumerableExtensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions_CountAsync__1_1">CountAsync(T)(Func(T, Boolean))</a></td><td>Overloaded.  
Counts the number of items in the <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable(T)</a>.
 (Defined by <a href="T_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions">AsyncEnumerableExtensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions_CrossApply__3">CrossApply(T, TRight, TResult)</a></td><td>
Applies a function to all elements and combines the results.
 (Defined by <a href="T_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions">AsyncEnumerableExtensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions_CrossJoin__3">CrossJoin(T, TRight, TResult)</a></td><td>
Performs a cross join between the two <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable(T)</a>s.
 (Defined by <a href="T_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions">AsyncEnumerableExtensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions_Distinct__1">Distinct(T)()</a></td><td>Overloaded.  
The distinct.
 (Defined by <a href="T_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions">AsyncEnumerableExtensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions_Distinct__1_1">Distinct(T)(IComparer(T))</a></td><td>Overloaded.  
Groups the asynchronous enumerable.
 (Defined by <a href="T_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions">AsyncEnumerableExtensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions_FirstAsync__1">FirstAsync(T)()</a></td><td>Overloaded.  
Gets the first element.
 (Defined by <a href="T_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions">AsyncEnumerableExtensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions_FirstAsync__1_1">FirstAsync(T)(Func(T, Boolean))</a></td><td>Overloaded.  
Returns the first item.
 (Defined by <a href="T_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions">AsyncEnumerableExtensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions_FirstOrDefaultAsync__1">FirstOrDefaultAsync(T)()</a></td><td>Overloaded.  
The first or default async.
 (Defined by <a href="T_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions">AsyncEnumerableExtensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions_FirstOrDefaultAsync__1_1">FirstOrDefaultAsync(T)(Func(T, Boolean))</a></td><td>Overloaded.  
Returns the first item for which the *condition* is `true`.
 (Defined by <a href="T_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions">AsyncEnumerableExtensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions_ForEachAsync__1">ForEachAsync(T)(Action(T))</a></td><td>Overloaded.  
Executes an action for all items in the <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable(T)</a>.
 (Defined by <a href="T_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions">AsyncEnumerableExtensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions_ForEachAsync__1_1">ForEachAsync(T)(Func(T, Task))</a></td><td>Overloaded.  
Executes an action for all items in the <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable(T)</a>.
 (Defined by <a href="T_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions">AsyncEnumerableExtensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions_ForEachBatchAsync__1">ForEachBatchAsync(T)(Action(IEnumerable(T)))</a></td><td>Overloaded.  
Executes an action for all items in the <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable(T)</a>.
 (Defined by <a href="T_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions">AsyncEnumerableExtensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions_ForEachBatchAsync__1_1">ForEachBatchAsync(T)(Func(IEnumerable(T), Task))</a></td><td>Overloaded.  
Executes an action for all items in the <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable(T)</a>.
 (Defined by <a href="T_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions">AsyncEnumerableExtensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions_GetElementType">GetElementType</a></td><td>
Gets the element type for the <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable">IAsyncEnumerable</a>.
 (Defined by <a href="T_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions">AsyncEnumerableExtensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions_GroupBy__2">GroupBy(T, TKey)</a></td><td>
Groups the asynchronous enumerable.
 (Defined by <a href="T_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions">AsyncEnumerableExtensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions_Join__4">Join(T, TRight, TKey, TResult)(IAsyncEnumerable(TRight), Func(T, TKey), Func(TRight, TKey), Func(T, TRight, TResult), IComparer(TKey))</a></td><td>Overloaded.  
Joins the two <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable(T)</a>s on a key. When no item is found that matches an item in *left*, *resultSelector* is called with the left item and the default for *TRight*.
 (Defined by <a href="T_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions">AsyncEnumerableExtensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions_Join__4_1">Join(T, TRight, TKey, TResult)(IAsyncEnumerable(TRight), Func(T, TKey), ExpressionType, Func(TRight, TKey), Func(T, TRight, Boolean), Func(T, TRight, TResult), IComparer(TKey))</a></td><td>Overloaded.  
Joins the two <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable(T)</a>s on a key. When no item is found that matches an item in *left*, *resultSelector* is called with the left item and the default for *TRight*.
 (Defined by <a href="T_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions">AsyncEnumerableExtensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions_LastAsync__1">LastAsync(T)()</a></td><td>Overloaded.  
The last async.
 (Defined by <a href="T_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions">AsyncEnumerableExtensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions_LastAsync__1_1">LastAsync(T)(Func(T, Boolean))</a></td><td>Overloaded.  
The last async.
 (Defined by <a href="T_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions">AsyncEnumerableExtensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions_LastOrDefaultAsync__1">LastOrDefaultAsync(T)()</a></td><td>Overloaded.  
The last or default async.
 (Defined by <a href="T_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions">AsyncEnumerableExtensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions_LastOrDefaultAsync__1_1">LastOrDefaultAsync(T)(Func(T, Boolean))</a></td><td>Overloaded.  
The last or default async.
 (Defined by <a href="T_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions">AsyncEnumerableExtensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions_LeftJoin__4">LeftJoin(T, TRight, TKey, TResult)</a></td><td>
Joins the two <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable(T)</a>s on a key. When no item is found that matches an item in *left*, *resultSelector* is called with the left item and the default for *TRight*. When no element can be matched with an element of *left*, the *resultSelector* is called with the left element and `default(*TRight*)`.
 (Defined by <a href="T_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions">AsyncEnumerableExtensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions_MaterializeAsync__1">MaterializeAsync(T)</a></td><td>
Retrieves all elements from the <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable(T)</a> from the source and stores them in a persistent <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable(T)</a>. This means that it can be enumerated multiple times without having to access the source over and over again.
 (Defined by <a href="T_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions">AsyncEnumerableExtensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions_MaxAsync__1">MaxAsync(T)()</a></td><td>Overloaded.  
The max async.
 (Defined by <a href="T_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions">AsyncEnumerableExtensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions_MaxAsync__1_1">MaxAsync(T)(IComparer(T))</a></td><td>Overloaded.  
The max async.
 (Defined by <a href="T_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions">AsyncEnumerableExtensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions_MinAsync__1">MinAsync(T)()</a></td><td>Overloaded.  
The min async.
 (Defined by <a href="T_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions">AsyncEnumerableExtensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions_MinAsync__1_1">MinAsync(T)(IComparer(T))</a></td><td>Overloaded.  
The min async.
 (Defined by <a href="T_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions">AsyncEnumerableExtensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions_OrderBy__1">OrderBy(T)(IEnumerable(IOrderByExpression))</a></td><td>Overloaded.  
Orders the <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable(T)</a> by the *orderByExpressions*.
 (Defined by <a href="T_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions">AsyncEnumerableExtensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions_OrderBy__2">OrderBy(T, TKey)(Func(T, TKey), IComparer(TKey))</a></td><td>Overloaded.  
Sorts the elements of a sequence in ascending order according to a key.
 (Defined by <a href="T_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions">AsyncEnumerableExtensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions_OrderByDescending__2">OrderByDescending(T, TKey)</a></td><td>
Sorts the elements of a sequence in descending order according to a key.
 (Defined by <a href="T_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions">AsyncEnumerableExtensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions_OuterApply__3">OuterApply(T, TRight, TResult)</a></td><td>
Applies a function to all elements and combines the results, when no results are returned, the result selector is called with the default value for *TRight*.
 (Defined by <a href="T_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions">AsyncEnumerableExtensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions_PreSortedJoin__4">PreSortedJoin(T, TRight, TKey, TResult)</a></td><td>
Joins the two <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable(T)</a>s on a key. When no item is found that matches an item in *left*, *resultSelector* is called with the left item and the default for *TRight*. Both <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable(T)</a>s must be sorted by the keys before calling this method.
 (Defined by <a href="T_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions">AsyncEnumerableExtensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions_PreSortedLeftJoin__4">PreSortedLeftJoin(T, TRight, TKey, TResult)</a></td><td>
Joins the two <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable(T)</a>s on a key. When no item is found that matches an item in *left*, *resultSelector* is called with the left item and the default for *TRight*. Both <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable(T)</a>s must be sorted by the keys before calling this method.
 (Defined by <a href="T_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions">AsyncEnumerableExtensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions_Select__2">Select(T, TResult)(Func(T, Task(TResult)))</a></td><td>Overloaded.  
Projects each element of a sequence into a new form.
 (Defined by <a href="T_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions">AsyncEnumerableExtensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions_Select__2_1">Select(T, TResult)(Func(T, TResult))</a></td><td>Overloaded.  
Projects each element of a sequence into a new form.
 (Defined by <a href="T_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions">AsyncEnumerableExtensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions_Skip__1">Skip(T)</a></td><td>
The skip.
 (Defined by <a href="T_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions">AsyncEnumerableExtensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions_SortAsync__1">SortAsync(T)</a></td><td>
Sorts the <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable(T)</a> by using the *comparison*.
 (Defined by <a href="T_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions">AsyncEnumerableExtensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions_Take__1">Take(T)(Int64)</a></td><td>Overloaded.  
The take.
 (Defined by <a href="T_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions">AsyncEnumerableExtensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions_Take__1_1">Take(T)(Nullable(Int64))</a></td><td>Overloaded.  
Takes the number of items of the <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable(T)</a>. When *count* is `null`, all items are returned.
 (Defined by <a href="T_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions">AsyncEnumerableExtensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions_ToArrayAsync__1">ToArrayAsync(T)</a></td><td>
Converts the enumerable to an array.
 (Defined by <a href="T_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions">AsyncEnumerableExtensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions_Union__1">Union(T)</a></td><td>
Produces the set union of two sequences by using a specified <a href="http://msdn2.microsoft.com/en-us/library/8ehhxeaf" target="_blank">IComparer(T)</a>. The order of the rows is not preserved. Uses the materialization policy of the first sequence.
 (Defined by <a href="T_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions">AsyncEnumerableExtensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions_Where__1">Where(T)</a></td><td>
Filters a sequence of values based on a predicate.
 (Defined by <a href="T_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions">AsyncEnumerableExtensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions_Zip__3">Zip(T, TRight, TResult)</a></td><td>
Enumerates the two <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable(T)</a>s and calls a function on each pair.
 (Defined by <a href="T_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions">AsyncEnumerableExtensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions_ZipAll__3">ZipAll(T, TRight, TResult)</a></td><td>
Enumerates the two <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable(T)</a>s and calls a function on each pair. When *right* has less elements than *left*, they are padded with the default value of *TRight*.
 (Defined by <a href="T_ConnectQl_AsyncEnumerables_AsyncEnumerableExtensions">AsyncEnumerableExtensions</a>.)</td></tr></table>&nbsp;
<a href="#iasyncreadonlycollection(*t*)-interface">Back to Top</a>

## See Also


#### Reference
<a href="N_ConnectQl_AsyncEnumerables">ConnectQl.AsyncEnumerables Namespace</a><br />