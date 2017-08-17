# ConnectQlFunctionsExtensions.AddWithoutSideEffects(*TResult*) Method (IConnectQlFunctions, String, Expression(Func(*TResult*)))
 

Adds a key/value pair of key'0 => function to the dictionary. The function should not have side effects, because it will be evaluated by IntelliSense.

**Namespace:**&nbsp;<a href="N_ConnectQl_Interfaces">ConnectQl.Interfaces</a><br />**Assembly:**&nbsp;ConnectQl (in ConnectQl.dll) Version: 100.0.1-prerelease

## Syntax

**C#**<br />
``` C#
public static IFunctionRegistration AddWithoutSideEffects<TResult>(
	this IConnectQlFunctions functions,
	string key,
	Expression<Func<TResult>> function
)

```


#### Parameters
&nbsp;<dl><dt>functions</dt><dd>Type: <a href="T_ConnectQl_Interfaces_IConnectQlFunctions">ConnectQl.Interfaces.IConnectQlFunctions</a><br />The functions to add the function to.</dd><dt>key</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/s1wwdcbf" target="_blank">System.String</a><br />The key.</dd><dt>function</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/bb335710" target="_blank">System.Linq.Expressions.Expression</a>(<a href="http://msdn2.microsoft.com/en-us/library/bb534960" target="_blank">Func</a>(*TResult*))<br />The function.</dd></dl>

#### Type Parameters
&nbsp;<dl><dt>TResult</dt><dd>The type of the function result.</dd></dl>

#### Return Value
Type: <a href="T_ConnectQl_Interfaces_IFunctionRegistration">IFunctionRegistration</a><br />The <a href="T_ConnectQl_Interfaces_IConnectQlFunctions">IConnectQlFunctions</a>.

#### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type <a href="T_ConnectQl_Interfaces_IConnectQlFunctions">IConnectQlFunctions</a>. When you use instance method syntax to call this method, omit the first parameter. For more information, see <a href="http://msdn.microsoft.com/en-us/library/bb384936.aspx">Extension Methods (Visual Basic)</a> or <a href="http://msdn.microsoft.com/en-us/library/bb383977.aspx">Extension Methods (C# Programming Guide)</a>.

## See Also


#### Reference
<a href="T_ConnectQl_Interfaces_ConnectQlFunctionsExtensions">ConnectQlFunctionsExtensions Class</a><br /><a href="Overload_ConnectQl_Interfaces_ConnectQlFunctionsExtensions_AddWithoutSideEffects">AddWithoutSideEffects Overload</a><br /><a href="N_ConnectQl_Interfaces">ConnectQl.Interfaces Namespace</a><br />