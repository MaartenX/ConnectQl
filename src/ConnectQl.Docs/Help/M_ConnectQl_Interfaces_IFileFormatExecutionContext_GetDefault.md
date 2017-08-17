# IFileFormatExecutionContext.GetDefault Method 
 

Gets the default setting for a data source. A 'USE DEFAULT' statement can be used to set a default value for a function.

**Namespace:**&nbsp;<a href="N_ConnectQl_Interfaces">ConnectQl.Interfaces</a><br />**Assembly:**&nbsp;ConnectQl (in ConnectQl.dll) Version: 100.0.1-prerelease

## Syntax

**C#**<br />
``` C#
Object GetDefault(
	string setting,
	bool throwOnError
)
```


#### Parameters
&nbsp;<dl><dt>setting</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/s1wwdcbf" target="_blank">System.String</a><br />The default setting get the value for.</dd><dt>throwOnError</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/a28wyd50" target="_blank">System.Boolean</a><br />`true` to throw an exception when an error occurs, false otherwise.</dd></dl>

#### Return Value
Type: <a href="http://msdn2.microsoft.com/en-us/library/e5kfa45b" target="_blank">Object</a><br />The value for the function for the specified source.

## See Also


#### Reference
<a href="T_ConnectQl_Interfaces_IFileFormatExecutionContext">IFileFormatExecutionContext Interface</a><br /><a href="N_ConnectQl_Interfaces">ConnectQl.Interfaces Namespace</a><br />