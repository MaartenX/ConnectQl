# MaterializationPolicyExtensions.CreateEmptyAsyncEnumerable(*T*) Method 
 

Creates an empty async enumerable.

**Namespace:**&nbsp;<a href="N_ConnectQl_AsyncEnumerablePolicies">ConnectQl.AsyncEnumerablePolicies</a><br />**Assembly:**&nbsp;ConnectQl (in ConnectQl.dll) Version: 100.0.1-prerelease

## Syntax

**C#**<br />
``` C#
public static IAsyncEnumerable<T> CreateEmptyAsyncEnumerable<T>(
	this IMaterializationPolicy policy
)

```


#### Parameters
&nbsp;<dl><dt>policy</dt><dd>Type: <a href="T_ConnectQl_AsyncEnumerablePolicies_IMaterializationPolicy">ConnectQl.AsyncEnumerablePolicies.IMaterializationPolicy</a><br />The policy.</dd></dl>

#### Type Parameters
&nbsp;<dl><dt>T</dt><dd>The type of the elements.</dd></dl>

#### Return Value
Type: <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable</a>(*T*)<br />The <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable(T)</a>.

#### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type <a href="T_ConnectQl_AsyncEnumerablePolicies_IMaterializationPolicy">IMaterializationPolicy</a>. When you use instance method syntax to call this method, omit the first parameter. For more information, see <a href="http://msdn.microsoft.com/en-us/library/bb384936.aspx">Extension Methods (Visual Basic)</a> or <a href="http://msdn.microsoft.com/en-us/library/bb383977.aspx">Extension Methods (C# Programming Guide)</a>.

## See Also


#### Reference
<a href="T_ConnectQl_AsyncEnumerablePolicies_MaterializationPolicyExtensions">MaterializationPolicyExtensions Class</a><br /><a href="N_ConnectQl_AsyncEnumerablePolicies">ConnectQl.AsyncEnumerablePolicies Namespace</a><br />