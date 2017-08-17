# IFileWriter.WriteHeader Method 
 

Writes the header to the file.

**Namespace:**&nbsp;<a href="N_ConnectQl_Interfaces">ConnectQl.Interfaces</a><br />**Assembly:**&nbsp;ConnectQl (in ConnectQl.dll) Version: 100.0.1-prerelease

## Syntax

**C#**<br />
``` C#
void WriteHeader(
	IFileFormatExecutionContext context,
	StreamWriter writer,
	IEnumerable<string> fields
)
```


#### Parameters
&nbsp;<dl><dt>context</dt><dd>Type: <a href="T_ConnectQl_Interfaces_IFileFormatExecutionContext">ConnectQl.Interfaces.IFileFormatExecutionContext</a><br />The context.</dd><dt>writer</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/3ssew6tk" target="_blank">System.IO.StreamWriter</a><br />The stream.</dd><dt>fields</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/9eekhta0" target="_blank">System.Collections.Generic.IEnumerable</a>(<a href="http://msdn2.microsoft.com/en-us/library/s1wwdcbf" target="_blank">String</a>)<br />The fields.</dd></dl>

## See Also


#### Reference
<a href="T_ConnectQl_Interfaces_IFileWriter">IFileWriter Interface</a><br /><a href="N_ConnectQl_Interfaces">ConnectQl.Interfaces Namespace</a><br />