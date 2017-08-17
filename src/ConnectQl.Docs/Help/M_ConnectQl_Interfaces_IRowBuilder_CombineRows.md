# IRowBuilder.CombineRows Method 
 

Combines two rows.

**Namespace:**&nbsp;<a href="N_ConnectQl_Interfaces">ConnectQl.Interfaces</a><br />**Assembly:**&nbsp;ConnectQl (in ConnectQl.dll) Version: 100.0.1-prerelease

## Syntax

**C#**<br />
``` C#
Row CombineRows(
	Row first,
	Row second
)
```


#### Parameters
&nbsp;<dl><dt>first</dt><dd>Type: <a href="T_ConnectQl_Results_Row">ConnectQl.Results.Row</a><br />The first row.</dd><dt>second</dt><dd>Type: <a href="T_ConnectQl_Results_Row">ConnectQl.Results.Row</a><br />The second row.</dd></dl>

#### Return Value
Type: <a href="T_ConnectQl_Results_Row">Row</a><br />The combined rows or `null` if both rows were `null`.

## See Also


#### Reference
<a href="T_ConnectQl_Interfaces_IRowBuilder">IRowBuilder Interface</a><br /><a href="N_ConnectQl_Interfaces">ConnectQl.Interfaces Namespace</a><br />