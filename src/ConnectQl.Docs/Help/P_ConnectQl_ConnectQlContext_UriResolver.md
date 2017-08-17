# ConnectQlContext.UriResolver Property 
 

Gets or sets a lambda that opens the file at the specified path and returns the stream.

**Namespace:**&nbsp;<a href="N_ConnectQl">ConnectQl</a><br />**Assembly:**&nbsp;ConnectQl (in ConnectQl.dll) Version: 100.0.1-prerelease

## Syntax

**C#**<br />
``` C#
public Func<string, UriResolveMode, Task<Stream>> UriResolver { get; set; }
```


#### Property Value
Type: <a href="http://msdn2.microsoft.com/en-us/library/bb534647" target="_blank">Func</a>(<a href="http://msdn2.microsoft.com/en-us/library/s1wwdcbf" target="_blank">String</a>, <a href="T_ConnectQl_UriResolveMode">UriResolveMode</a>, <a href="http://msdn2.microsoft.com/en-us/library/dd321424" target="_blank">Task</a>(<a href="http://msdn2.microsoft.com/en-us/library/8f86tw9e" target="_blank">Stream</a>))

## See Also


#### Reference
<a href="T_ConnectQl_ConnectQlContext">ConnectQlContext Class</a><br /><a href="N_ConnectQl">ConnectQl Namespace</a><br />