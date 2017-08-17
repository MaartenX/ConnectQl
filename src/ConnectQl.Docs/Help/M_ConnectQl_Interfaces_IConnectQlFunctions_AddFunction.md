# IConnectQlFunctions.AddFunction Method 
 

Adds a key/value pair of key'1 => function to the dictionary.

**Namespace:**&nbsp;<a href="N_ConnectQl_Interfaces">ConnectQl.Interfaces</a><br />**Assembly:**&nbsp;ConnectQl (in ConnectQl.dll) Version: 100.0.1-prerelease

## Syntax

**C#**<br />
``` C#
IConnectQlFunctions AddFunction(
	string name,
	IFunctionDescriptor function
)
```


#### Parameters
&nbsp;<dl><dt>name</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/s1wwdcbf" target="_blank">System.String</a><br />The name.</dd><dt>function</dt><dd>Type: <a href="T_ConnectQl_Interfaces_IFunctionDescriptor">ConnectQl.Interfaces.IFunctionDescriptor</a><br />The function.</dd></dl>

#### Return Value
Type: <a href="T_ConnectQl_Interfaces_IConnectQlFunctions">IConnectQlFunctions</a><br />The <a href="T_ConnectQl_Interfaces_IConnectQlFunctions">IConnectQlFunctions</a>.

## Exceptions
&nbsp;<table><tr><th>Exception</th><th>Condition</th></tr><tr><td><a href="http://msdn2.microsoft.com/en-us/library/3w1b3114" target="_blank">ArgumentException</a></td><td>Thrown when a lambda with the specified number of parameters is already in the dictionary.</td></tr></table>

## See Also


#### Reference
<a href="T_ConnectQl_Interfaces_IConnectQlFunctions">IConnectQlFunctions Interface</a><br /><a href="N_ConnectQl_Interfaces">ConnectQl.Interfaces Namespace</a><br />