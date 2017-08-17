# IStorageProvider.GetFileAsync Method 
 

Gets the file by its id. Returns an opened stream.

**Namespace:**&nbsp;<a href="N_ConnectQl_AsyncEnumerablePolicies">ConnectQl.AsyncEnumerablePolicies</a><br />**Assembly:**&nbsp;ConnectQl (in ConnectQl.dll) Version: 100.0.1-prerelease

## Syntax

**C#**<br />
``` C#
Task<Stream> GetFileAsync(
	int id,
	FileAccessType access
)
```


#### Parameters
&nbsp;<dl><dt>id</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/td2s409d" target="_blank">System.Int32</a><br />The id of the file. When this is a new id, the file will be created.</dd><dt>access</dt><dd>Type: <a href="T_ConnectQl_AsyncEnumerablePolicies_FileAccessType">ConnectQl.AsyncEnumerablePolicies.FileAccessType</a><br />The file access. Can be read or write.</dd></dl>

#### Return Value
Type: <a href="http://msdn2.microsoft.com/en-us/library/dd321424" target="_blank">Task</a>(<a href="http://msdn2.microsoft.com/en-us/library/8f86tw9e" target="_blank">Stream</a>)<br />The stream.

## See Also


#### Reference
<a href="T_ConnectQl_AsyncEnumerablePolicies_IStorageProvider">IStorageProvider Interface</a><br /><a href="N_ConnectQl_AsyncEnumerablePolicies">ConnectQl.AsyncEnumerablePolicies Namespace</a><br />