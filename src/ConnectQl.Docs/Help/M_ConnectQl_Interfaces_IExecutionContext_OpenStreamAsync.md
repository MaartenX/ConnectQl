# IExecutionContext.OpenStreamAsync Method 
 

Opens a file.

**Namespace:**&nbsp;<a href="N_ConnectQl_Interfaces">ConnectQl.Interfaces</a><br />**Assembly:**&nbsp;ConnectQl (in ConnectQl.dll) Version: 100.0.1-prerelease

## Syntax

**C#**<br />
``` C#
Task<Stream> OpenStreamAsync(
	string uri,
	UriResolveMode uriResolveMode
)
```


#### Parameters
&nbsp;<dl><dt>uri</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/s1wwdcbf" target="_blank">System.String</a><br />The uri of the file.</dd><dt>uriResolveMode</dt><dd>Type: <a href="T_ConnectQl_UriResolveMode">ConnectQl.UriResolveMode</a><br />The file mode.</dd></dl>

#### Return Value
Type: <a href="http://msdn2.microsoft.com/en-us/library/dd321424" target="_blank">Task</a>(<a href="http://msdn2.microsoft.com/en-us/library/8f86tw9e" target="_blank">Stream</a>)<br />The stream containing the data of the file.

## See Also


#### Reference
<a href="T_ConnectQl_Interfaces_IExecutionContext">IExecutionContext Interface</a><br /><a href="N_ConnectQl_Interfaces">ConnectQl.Interfaces Namespace</a><br />