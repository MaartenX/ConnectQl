# IDataSource.GetRows Method 
 

Retrieves the data from the source as an <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable(T)</a>.

**Namespace:**&nbsp;<a href="N_ConnectQl_Interfaces">ConnectQl.Interfaces</a><br />**Assembly:**&nbsp;ConnectQl (in ConnectQl.dll) Version: 100.0.1-prerelease

## Syntax

**C#**<br />
``` C#
IAsyncEnumerable<Row> GetRows(
	IExecutionContext context,
	IRowBuilder rowBuilder,
	IQuery query
)
```


#### Parameters
&nbsp;<dl><dt>context</dt><dd>Type: <a href="T_ConnectQl_Interfaces_IExecutionContext">ConnectQl.Interfaces.IExecutionContext</a><br />The context.</dd><dt>rowBuilder</dt><dd>Type: <a href="T_ConnectQl_Interfaces_IRowBuilder">ConnectQl.Interfaces.IRowBuilder</a><br />The row builder.</dd><dt>query</dt><dd>Type: <a href="T_ConnectQl_Interfaces_IQuery">ConnectQl.Interfaces.IQuery</a><br />The query expression. Can be `null`.</dd></dl>

#### Return Value
Type: <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">IAsyncEnumerable</a>(<a href="T_ConnectQl_Results_Row">Row</a>)<br />A task returning the data set.

## See Also


#### Reference
<a href="T_ConnectQl_Interfaces_IDataSource">IDataSource Interface</a><br /><a href="N_ConnectQl_Interfaces">ConnectQl.Interfaces Namespace</a><br />