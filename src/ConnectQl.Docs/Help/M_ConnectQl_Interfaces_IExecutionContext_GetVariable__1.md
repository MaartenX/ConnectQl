# IExecutionContext.GetVariable(*T*) Method 
 

Gets the value for the specified variable.

**Namespace:**&nbsp;<a href="N_ConnectQl_Interfaces">ConnectQl.Interfaces</a><br />**Assembly:**&nbsp;ConnectQl (in ConnectQl.dll) Version: 100.0.1-prerelease

## Syntax

**C#**<br />
``` C#
T GetVariable<T>(
	string variable
)

```


#### Parameters
&nbsp;<dl><dt>variable</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/s1wwdcbf" target="_blank">System.String</a><br />The name of the variable, including the '@'.</dd></dl>

#### Type Parameters
&nbsp;<dl><dt>T</dt><dd>The type of the variable.</dd></dl>

#### Return Value
Type: *T*<br />The value of the variable.

## See Also


#### Reference
<a href="T_ConnectQl_Interfaces_IExecutionContext">IExecutionContext Interface</a><br /><a href="N_ConnectQl_Interfaces">ConnectQl.Interfaces Namespace</a><br />