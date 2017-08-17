# FileDataSource.OpenStreamAsync Method 
 

Opens the stream.

**Namespace:**&nbsp;<a href="N_ConnectQl_DataSources">ConnectQl.DataSources</a><br />**Assembly:**&nbsp;ConnectQl (in ConnectQl.dll) Version: 100.0.1-prerelease

## Syntax

**C#**<br />
``` C#
protected virtual Task<Stream> OpenStreamAsync(
	IExecutionContext context,
	UriResolveMode uriResolveMode,
	string fileUri
)
```


#### Parameters
&nbsp;<dl><dt>context</dt><dd>Type: <a href="T_ConnectQl_Interfaces_IExecutionContext">ConnectQl.Interfaces.IExecutionContext</a><br />The execution context.</dd><dt>uriResolveMode</dt><dd>Type: <a href="T_ConnectQl_UriResolveMode">ConnectQl.UriResolveMode</a><br />The file Mode.</dd><dt>fileUri</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/s1wwdcbf" target="_blank">System.String</a><br />The uri.</dd></dl>

#### Return Value
Type: <a href="http://msdn2.microsoft.com/en-us/library/dd321424" target="_blank">Task</a>(<a href="http://msdn2.microsoft.com/en-us/library/8f86tw9e" target="_blank">Stream</a>)<br />The <a href="http://msdn2.microsoft.com/en-us/library/dd235678" target="_blank">Task</a>.

## See Also


#### Reference
<a href="T_ConnectQl_DataSources_FileDataSource">FileDataSource Class</a><br /><a href="N_ConnectQl_DataSources">ConnectQl.DataSources Namespace</a><br />