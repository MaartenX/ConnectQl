# Row.IRowImplementation.CombineFrom(*TOther*) Method 
 

Joins the other row to the current row.

**Namespace:**&nbsp;<a href="N_ConnectQl_Results">ConnectQl.Results</a><br />**Assembly:**&nbsp;ConnectQl (in ConnectQl.dll) Version: 100.0.1-prerelease

## Syntax

**C#**<br />
``` C#
Row CombineFrom<TOther>(
	IRowBuilder rowBuilder,
	Row.RowImplementation<TOther> other
)

```


#### Parameters
&nbsp;<dl><dt>rowBuilder</dt><dd>Type: <a href="T_ConnectQl_Interfaces_IRowBuilder">ConnectQl.Interfaces.IRowBuilder</a><br />The row Builder.</dd><dt>other</dt><dd>Type: <a href="T_ConnectQl_Results_Row_RowImplementation_1">ConnectQl.Results.Row.RowImplementation</a>(*TOther*)<br />The other row.</dd></dl>

#### Type Parameters
&nbsp;<dl><dt>TOther</dt><dd>The type of the other row's unique id.</dd></dl>

#### Return Value
Type: <a href="T_ConnectQl_Results_Row">Row</a><br />The joined row, or null when the rows cannot be joined.

## See Also


#### Reference
<a href="T_ConnectQl_Results_Row_IRowImplementation">Row.IRowImplementation Interface</a><br /><a href="N_ConnectQl_Results">ConnectQl.Results Namespace</a><br />