# IRowBuilder.CreateRow(*T*) Method 
 

Creates a row.

**Namespace:**&nbsp;<a href="N_ConnectQl_Interfaces">ConnectQl.Interfaces</a><br />**Assembly:**&nbsp;ConnectQl (in ConnectQl.dll) Version: 100.0.1-prerelease

## Syntax

**C#**<br />
``` C#
Row CreateRow<T>(
	T uniqueId,
	IEnumerable<KeyValuePair<string, Object>> fields
)

```


#### Parameters
&nbsp;<dl><dt>uniqueId</dt><dd>Type: *T*<br />The unique id of the row.</dd><dt>fields</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/9eekhta0" target="_blank">System.Collections.Generic.IEnumerable</a>(<a href="http://msdn2.microsoft.com/en-us/library/5tbh8a42" target="_blank">KeyValuePair</a>(<a href="http://msdn2.microsoft.com/en-us/library/s1wwdcbf" target="_blank">String</a>, <a href="http://msdn2.microsoft.com/en-us/library/e5kfa45b" target="_blank">Object</a>))<br />The fields in the row.</dd></dl>

#### Type Parameters
&nbsp;<dl><dt>T</dt><dd>The type of the unique id of the row.</dd></dl>

#### Return Value
Type: <a href="T_ConnectQl_Results_Row">Row</a><br />The <a href="T_ConnectQl_Results_Row">Row</a>.

## See Also


#### Reference
<a href="T_ConnectQl_Interfaces_IRowBuilder">IRowBuilder Interface</a><br /><a href="N_ConnectQl_Interfaces">ConnectQl.Interfaces Namespace</a><br />