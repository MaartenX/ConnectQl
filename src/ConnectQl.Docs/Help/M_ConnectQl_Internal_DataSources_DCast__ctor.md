# DCast Constructor 
 

Initializes a new instance of the <a href="T_ConnectQl_Internal_DataSources_DCast">DCast</a> class.

**Namespace:**&nbsp;<a href="N_ConnectQl_Internal_DataSources">ConnectQl.Internal.DataSources</a><br />**Assembly:**&nbsp;ConnectQl (in ConnectQl.dll) Version: 100.0.1-prerelease

## Syntax

**C#**<br />
``` C#
public DCast(
	IAsyncEnumerable<Row> source,
	string columnName,
	string columnValue,
	DCastFunction function
)
```


#### Parameters
&nbsp;<dl><dt>source</dt><dd>Type: <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">ConnectQl.AsyncEnumerables.IAsyncEnumerable</a>(<a href="T_ConnectQl_Results_Row">Row</a>)<br />The source.</dd><dt>columnName</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/s1wwdcbf" target="_blank">System.String</a><br />The column name.</dd><dt>columnValue</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/s1wwdcbf" target="_blank">System.String</a><br />The column value.</dd><dt>function</dt><dd>Type: <a href="T_ConnectQl_Internal_DataSources_DCastFunction">ConnectQl.Internal.DataSources.DCastFunction</a><br />The function.</dd></dl>

## See Also


#### Reference
<a href="T_ConnectQl_Internal_DataSources_DCast">DCast Class</a><br /><a href="N_ConnectQl_Internal_DataSources">ConnectQl.Internal.DataSources Namespace</a><br />