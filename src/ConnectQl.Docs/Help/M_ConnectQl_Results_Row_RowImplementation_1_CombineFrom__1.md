# Row.RowImplementation(*T*).CombineFrom(*TOther*) Method (IRowBuilder, Row.RowImplementation(*TOther*))
 

Joins the other row to the current row.

**Namespace:**&nbsp;<a href="N_ConnectQl_Results">ConnectQl.Results</a><br />**Assembly:**&nbsp;ConnectQl (in ConnectQl.dll) Version: 100.0.1-prerelease

## Syntax

**C#**<br />
``` C#
protected override Row CombineFrom<TOther>(
	IRowBuilder builder,
	Row.RowImplementation<TOther> other
)

```


#### Parameters
&nbsp;<dl><dt>builder</dt><dd>Type: <a href="T_ConnectQl_Interfaces_IRowBuilder">ConnectQl.Interfaces.IRowBuilder</a><br />The builder.</dd><dt>other</dt><dd>Type: <a href="T_ConnectQl_Results_Row_RowImplementation_1">ConnectQl.Results.Row.RowImplementation</a>(*TOther*)<br />The other row.</dd></dl>

#### Type Parameters
&nbsp;<dl><dt>TOther</dt><dd>The type of the other row's unique id.</dd></dl>

#### Return Value
Type: <a href="T_ConnectQl_Results_Row">Row</a><br />The joined row, or null when the rows cannot be joined.

## See Also


#### Reference
<a href="T_ConnectQl_Results_Row_RowImplementation_1">Row.RowImplementation(T) Class</a><br /><a href="Overload_ConnectQl_Results_Row_RowImplementation_1_CombineFrom">CombineFrom Overload</a><br /><a href="N_ConnectQl_Results">ConnectQl.Results Namespace</a><br />