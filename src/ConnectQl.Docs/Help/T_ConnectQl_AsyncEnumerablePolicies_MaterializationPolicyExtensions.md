# MaterializationPolicyExtensions Class
 

The materialization policy extensions.


## Inheritance Hierarchy
<a href="http://msdn2.microsoft.com/en-us/library/e5kfa45b" target="_blank">System.Object</a><br />&nbsp;&nbsp;ConnectQl.AsyncEnumerablePolicies.MaterializationPolicyExtensions<br />
**Namespace:**&nbsp;<a href="N_ConnectQl_AsyncEnumerablePolicies">ConnectQl.AsyncEnumerablePolicies</a><br />**Assembly:**&nbsp;ConnectQl (in ConnectQl.dll) Version: 100.0.1-prerelease

## Syntax

**C#**<br />
``` C#
public static class MaterializationPolicyExtensions
```

The MaterializationPolicyExtensions type exposes the following members.


## Methods
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_ConnectQl_AsyncEnumerablePolicies_MaterializationPolicyExtensions_CreateAsyncEnumerable__1">CreateAsyncEnumerable(T)(IMaterializationPolicy, IEnumerable(T))</a></td><td>
Creates an asynchronous enumerable from an <a href="http://msdn2.microsoft.com/en-us/library/9eekhta0" target="_blank">IEnumerable(T)</a>.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_ConnectQl_AsyncEnumerablePolicies_MaterializationPolicyExtensions_CreateAsyncEnumerable__1_1">CreateAsyncEnumerable(T)(IMaterializationPolicy, Func(Task(IAsyncEnumerable(T))))</a></td><td>
Creates an <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable(T)</a> from a generator.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_ConnectQl_AsyncEnumerablePolicies_MaterializationPolicyExtensions_CreateAsyncEnumerable__2_4">CreateAsyncEnumerable(T, TState)(IMaterializationPolicy, Func(TState, Task(IEnumerable(T))), Action(TState))</a></td><td>
Creates an <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable(T)</a> from a generator.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_ConnectQl_AsyncEnumerablePolicies_MaterializationPolicyExtensions_CreateAsyncEnumerable__2_5">CreateAsyncEnumerable(T, TState)(IMaterializationPolicy, Func(TState, Task(T)), Action(TState))</a></td><td>
Creates an <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable(T)</a> from a generator.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_ConnectQl_AsyncEnumerablePolicies_MaterializationPolicyExtensions_CreateAsyncEnumerable__2">CreateAsyncEnumerable(T, TState)(IMaterializationPolicy, Func(Task(TState)), Func(TState, Task(IEnumerable(T))), Action(TState))</a></td><td>
Creates an <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable(T)</a> from a generator.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_ConnectQl_AsyncEnumerablePolicies_MaterializationPolicyExtensions_CreateAsyncEnumerable__2_1">CreateAsyncEnumerable(T, TState)(IMaterializationPolicy, Func(Task(TState)), Func(TState, Task(T)), Action(TState))</a></td><td>
Creates an <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable(T)</a> from a generator.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_ConnectQl_AsyncEnumerablePolicies_MaterializationPolicyExtensions_CreateAsyncEnumerable__2_2">CreateAsyncEnumerable(T, TState)(IMaterializationPolicy, Func(TState), Func(TState, Task(IEnumerable(T))), Action(TState))</a></td><td>
Creates an <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable(T)</a> from a generator.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_ConnectQl_AsyncEnumerablePolicies_MaterializationPolicyExtensions_CreateAsyncEnumerable__2_3">CreateAsyncEnumerable(T, TState)(IMaterializationPolicy, Func(TState), Func(TState, Task(T)), Action(TState))</a></td><td>
Creates an <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable(T)</a> from a generator.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_ConnectQl_AsyncEnumerablePolicies_MaterializationPolicyExtensions_CreateAsyncEnumerableAndRunOnce__1">CreateAsyncEnumerableAndRunOnce(T)(IMaterializationPolicy, Func(Task(IEnumerable(T))))</a></td><td>
Creates an <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable(T)</a> by calling *itemGenerator* once.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_ConnectQl_AsyncEnumerablePolicies_MaterializationPolicyExtensions_CreateAsyncEnumerableAndRunOnce__1_1">CreateAsyncEnumerableAndRunOnce(T)(IMaterializationPolicy, Func(Task(IEnumerable(T))), Action)</a></td><td>
Creates an <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable(T)</a> by calling *itemGenerator* once.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_ConnectQl_AsyncEnumerablePolicies_MaterializationPolicyExtensions_CreateAsyncEnumerableAndRunOnce__2">CreateAsyncEnumerableAndRunOnce(T, TState)(IMaterializationPolicy, Func(Task(TState)), Func(TState, IEnumerable(T)), Action(TState))</a></td><td>
Creates an <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable(T)</a> by calling *itemGenerator* once.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_ConnectQl_AsyncEnumerablePolicies_MaterializationPolicyExtensions_CreateAsyncEnumerableAndRunOnce__2_1">CreateAsyncEnumerableAndRunOnce(T, TState)(IMaterializationPolicy, Func(Task(TState)), Func(TState, Task(IEnumerable(T))), Action(TState))</a></td><td>
Creates an <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable(T)</a> by calling *itemGenerator* once.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_ConnectQl_AsyncEnumerablePolicies_MaterializationPolicyExtensions_CreateEmptyAsyncEnumerable__1">CreateEmptyAsyncEnumerable(T)</a></td><td>
Creates an empty async enumerable.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_ConnectQl_AsyncEnumerablePolicies_MaterializationPolicyExtensions_MaterializeAsync__1">MaterializeAsync(T)</a></td><td>
Retrieves all elements from the <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable(T)</a> from the source and stores them in a persistent <a href="T_ConnectQl_AsyncEnumerables_IAsyncReadOnlyCollection_1">IAsyncReadOnlyCollection(T)</a>. This means that it can be enumerated multiple times without having to access the source over and over again.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_ConnectQl_AsyncEnumerablePolicies_MaterializationPolicyExtensions_ToAsyncEnumerable__1">ToAsyncEnumerable(T)</a></td><td>
Converts an enumerable to an <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable(T)</a>.</td></tr></table>&nbsp;
<a href="#materializationpolicyextensions-class">Back to Top</a>

## See Also


#### Reference
<a href="N_ConnectQl_AsyncEnumerablePolicies">ConnectQl.AsyncEnumerablePolicies Namespace</a><br />