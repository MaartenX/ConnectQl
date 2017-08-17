# TemporaryFilePolicy Class
 

The temporary file policy.


## Inheritance Hierarchy
<a href="http://msdn2.microsoft.com/en-us/library/e5kfa45b" target="_blank">System.Object</a><br />&nbsp;&nbsp;ConnectQl.AsyncEnumerablePolicies.TemporaryFilePolicy<br />&nbsp;&nbsp;&nbsp;&nbsp;<a href="T_ConnectQl_Platform_AsyncEnumerablePolicies_JsonTemporaryFilePolicy">ConnectQl.Platform.AsyncEnumerablePolicies.JsonTemporaryFilePolicy</a><br />
**Namespace:**&nbsp;<a href="N_ConnectQl_AsyncEnumerablePolicies">ConnectQl.AsyncEnumerablePolicies</a><br />**Assembly:**&nbsp;ConnectQl (in ConnectQl.dll) Version: 100.0.1-prerelease

## Syntax

**C#**<br />
``` C#
public class TemporaryFilePolicy : IMaterializationPolicy
```

The TemporaryFilePolicy type exposes the following members.


## Constructors
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td><a href="M_ConnectQl_AsyncEnumerablePolicies_TemporaryFilePolicy__ctor">TemporaryFilePolicy</a></td><td>
Initializes a new instance of the TemporaryFilePolicy class.</td></tr></table>&nbsp;
<a href="#temporaryfilepolicy-class">Back to Top</a>

## Properties
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td><a href="P_ConnectQl_AsyncEnumerablePolicies_TemporaryFilePolicy_MaximumChunkSize">MaximumChunkSize</a></td><td>
Gets the maximum chunk size.</td></tr></table>&nbsp;
<a href="#temporaryfilepolicy-class">Back to Top</a>

## Methods
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_ConnectQl_AsyncEnumerablePolicies_TemporaryFilePolicy_CreateBuilder__1">CreateBuilder(T)</a></td><td>
Creates a builder that can be used to create an <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable(T)</a>.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="http://msdn2.microsoft.com/en-us/library/bsc2ak47" target="_blank">Equals</a></td><td>
Determines whether the specified object is equal to the current object.
 (Inherited from <a href="http://msdn2.microsoft.com/en-us/library/e5kfa45b" target="_blank">Object</a>.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td><a href="http://msdn2.microsoft.com/en-us/library/4k87zsw7" target="_blank">Finalize</a></td><td>
Allows an object to try to free resources and perform other cleanup operations before it is reclaimed by garbage collection.
 (Inherited from <a href="http://msdn2.microsoft.com/en-us/library/e5kfa45b" target="_blank">Object</a>.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="http://msdn2.microsoft.com/en-us/library/zdee4b3y" target="_blank">GetHashCode</a></td><td>
Serves as the default hash function.
 (Inherited from <a href="http://msdn2.microsoft.com/en-us/library/e5kfa45b" target="_blank">Object</a>.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="http://msdn2.microsoft.com/en-us/library/dfwy45w9" target="_blank">GetType</a></td><td>
Gets the <a href="http://msdn2.microsoft.com/en-us/library/42892f65" target="_blank">Type</a> of the current instance.
 (Inherited from <a href="http://msdn2.microsoft.com/en-us/library/e5kfa45b" target="_blank">Object</a>.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td><a href="http://msdn2.microsoft.com/en-us/library/57ctke0a" target="_blank">MemberwiseClone</a></td><td>
Creates a shallow copy of the current <a href="http://msdn2.microsoft.com/en-us/library/e5kfa45b" target="_blank">Object</a>.
 (Inherited from <a href="http://msdn2.microsoft.com/en-us/library/e5kfa45b" target="_blank">Object</a>.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_ConnectQl_AsyncEnumerablePolicies_TemporaryFilePolicy_RegisterTransform__1">RegisterTransform(T)</a></td><td>
Registers a transform that turns items into serializable objects.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_ConnectQl_AsyncEnumerablePolicies_TemporaryFilePolicy_SortAsync__1">SortAsync(T)</a></td><td>
Creates a new <a href="T_ConnectQl_AsyncEnumerables_IAsyncReadOnlyCollection_1">IAsyncReadOnlyCollection(T)</a> that contains the sorted elements of the <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable(T)</a>.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="http://msdn2.microsoft.com/en-us/library/7bxwbwt2" target="_blank">ToString</a></td><td>
Returns a string that represents the current object.
 (Inherited from <a href="http://msdn2.microsoft.com/en-us/library/e5kfa45b" target="_blank">Object</a>.)</td></tr></table>&nbsp;
<a href="#temporaryfilepolicy-class">Back to Top</a>

## Extension Methods
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_ConnectQl_AsyncEnumerablePolicies_MaterializationPolicyExtensions_CreateAsyncEnumerable__1">CreateAsyncEnumerable(T)(IEnumerable(T))</a></td><td>Overloaded.  
Creates an asynchronous enumerable from an <a href="http://msdn2.microsoft.com/en-us/library/9eekhta0" target="_blank">IEnumerable(T)</a>.
 (Defined by <a href="T_ConnectQl_AsyncEnumerablePolicies_MaterializationPolicyExtensions">MaterializationPolicyExtensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_ConnectQl_AsyncEnumerablePolicies_MaterializationPolicyExtensions_CreateAsyncEnumerable__1_1">CreateAsyncEnumerable(T)(Func(Task(IAsyncEnumerable(T))))</a></td><td>Overloaded.  
Creates an <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable(T)</a> from a generator.
 (Defined by <a href="T_ConnectQl_AsyncEnumerablePolicies_MaterializationPolicyExtensions">MaterializationPolicyExtensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_ConnectQl_AsyncEnumerablePolicies_MaterializationPolicyExtensions_CreateAsyncEnumerable__2_4">CreateAsyncEnumerable(T, TState)(Func(TState, Task(IEnumerable(T))), Action(TState))</a></td><td>Overloaded.  
Creates an <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable(T)</a> from a generator.
 (Defined by <a href="T_ConnectQl_AsyncEnumerablePolicies_MaterializationPolicyExtensions">MaterializationPolicyExtensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_ConnectQl_AsyncEnumerablePolicies_MaterializationPolicyExtensions_CreateAsyncEnumerable__2_5">CreateAsyncEnumerable(T, TState)(Func(TState, Task(T)), Action(TState))</a></td><td>Overloaded.  
Creates an <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable(T)</a> from a generator.
 (Defined by <a href="T_ConnectQl_AsyncEnumerablePolicies_MaterializationPolicyExtensions">MaterializationPolicyExtensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_ConnectQl_AsyncEnumerablePolicies_MaterializationPolicyExtensions_CreateAsyncEnumerable__2_2">CreateAsyncEnumerable(T, TState)(Func(TState), Func(TState, Task(IEnumerable(T))), Action(TState))</a></td><td>Overloaded.  
Creates an <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable(T)</a> from a generator.
 (Defined by <a href="T_ConnectQl_AsyncEnumerablePolicies_MaterializationPolicyExtensions">MaterializationPolicyExtensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_ConnectQl_AsyncEnumerablePolicies_MaterializationPolicyExtensions_CreateAsyncEnumerable__2_1">CreateAsyncEnumerable(T, TState)(Func(Task(TState)), Func(TState, Task(T)), Action(TState))</a></td><td>Overloaded.  
Creates an <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable(T)</a> from a generator.
 (Defined by <a href="T_ConnectQl_AsyncEnumerablePolicies_MaterializationPolicyExtensions">MaterializationPolicyExtensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_ConnectQl_AsyncEnumerablePolicies_MaterializationPolicyExtensions_CreateAsyncEnumerable__2_3">CreateAsyncEnumerable(T, TState)(Func(TState), Func(TState, Task(T)), Action(TState))</a></td><td>Overloaded.  
Creates an <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable(T)</a> from a generator.
 (Defined by <a href="T_ConnectQl_AsyncEnumerablePolicies_MaterializationPolicyExtensions">MaterializationPolicyExtensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_ConnectQl_AsyncEnumerablePolicies_MaterializationPolicyExtensions_CreateAsyncEnumerable__2">CreateAsyncEnumerable(T, TState)(Func(Task(TState)), Func(TState, Task(IEnumerable(T))), Action(TState))</a></td><td>Overloaded.  
Creates an <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable(T)</a> from a generator.
 (Defined by <a href="T_ConnectQl_AsyncEnumerablePolicies_MaterializationPolicyExtensions">MaterializationPolicyExtensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_ConnectQl_AsyncEnumerablePolicies_MaterializationPolicyExtensions_CreateAsyncEnumerableAndRunOnce__1">CreateAsyncEnumerableAndRunOnce(T)(Func(Task(IEnumerable(T))))</a></td><td>Overloaded.  
Creates an <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable(T)</a> by calling *itemGenerator* once.
 (Defined by <a href="T_ConnectQl_AsyncEnumerablePolicies_MaterializationPolicyExtensions">MaterializationPolicyExtensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_ConnectQl_AsyncEnumerablePolicies_MaterializationPolicyExtensions_CreateAsyncEnumerableAndRunOnce__1_1">CreateAsyncEnumerableAndRunOnce(T)(Func(Task(IEnumerable(T))), Action)</a></td><td>Overloaded.  
Creates an <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable(T)</a> by calling *itemGenerator* once.
 (Defined by <a href="T_ConnectQl_AsyncEnumerablePolicies_MaterializationPolicyExtensions">MaterializationPolicyExtensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_ConnectQl_AsyncEnumerablePolicies_MaterializationPolicyExtensions_CreateAsyncEnumerableAndRunOnce__2_1">CreateAsyncEnumerableAndRunOnce(T, TState)(Func(Task(TState)), Func(TState, Task(IEnumerable(T))), Action(TState))</a></td><td>Overloaded.  
Creates an <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable(T)</a> by calling *itemGenerator* once.
 (Defined by <a href="T_ConnectQl_AsyncEnumerablePolicies_MaterializationPolicyExtensions">MaterializationPolicyExtensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_ConnectQl_AsyncEnumerablePolicies_MaterializationPolicyExtensions_CreateAsyncEnumerableAndRunOnce__2">CreateAsyncEnumerableAndRunOnce(T, TState)(Func(Task(TState)), Func(TState, IEnumerable(T)), Action(TState))</a></td><td>Overloaded.  
Creates an <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable(T)</a> by calling *itemGenerator* once.
 (Defined by <a href="T_ConnectQl_AsyncEnumerablePolicies_MaterializationPolicyExtensions">MaterializationPolicyExtensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_ConnectQl_AsyncEnumerablePolicies_MaterializationPolicyExtensions_CreateEmptyAsyncEnumerable__1">CreateEmptyAsyncEnumerable(T)</a></td><td>
Creates an empty async enumerable.
 (Defined by <a href="T_ConnectQl_AsyncEnumerablePolicies_MaterializationPolicyExtensions">MaterializationPolicyExtensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_ConnectQl_AsyncEnumerablePolicies_MaterializationPolicyExtensions_MaterializeAsync__1">MaterializeAsync(T)</a></td><td>
Retrieves all elements from the <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable(T)</a> from the source and stores them in a persistent <a href="T_ConnectQl_AsyncEnumerables_IAsyncReadOnlyCollection_1">IAsyncReadOnlyCollection(T)</a>. This means that it can be enumerated multiple times without having to access the source over and over again.
 (Defined by <a href="T_ConnectQl_AsyncEnumerablePolicies_MaterializationPolicyExtensions">MaterializationPolicyExtensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_ConnectQl_AsyncEnumerablePolicies_MaterializationPolicyExtensions_ToAsyncEnumerable__1">ToAsyncEnumerable(T)</a></td><td>
Converts an enumerable to an <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable(T)</a>.
 (Defined by <a href="T_ConnectQl_AsyncEnumerablePolicies_MaterializationPolicyExtensions">MaterializationPolicyExtensions</a>.)</td></tr></table>&nbsp;
<a href="#temporaryfilepolicy-class">Back to Top</a>

## See Also


#### Reference
<a href="N_ConnectQl_AsyncEnumerablePolicies">ConnectQl.AsyncEnumerablePolicies Namespace</a><br />