# TimeDataSource.GetColumnsAsync Method 
 

Gets the columns for this data source.

**Namespace:**&nbsp;<a href="N_ConnectQl_DataSources">ConnectQl.DataSources</a><br />**Assembly:**&nbsp;ConnectQl (in ConnectQl.dll) Version: 100.0.1-prerelease

## Syntax

**C#**<br />
``` C#
public Task<IEnumerable<IColumnDescriptor>> GetColumnsAsync(
	IExecutionContext context
)
```


#### Parameters
&nbsp;<dl><dt>context</dt><dd>Type: <a href="T_ConnectQl_Interfaces_IExecutionContext">ConnectQl.Interfaces.IExecutionContext</a><br />The execution context.</dd></dl>

#### Return Value
Type: <a href="http://msdn2.microsoft.com/en-us/library/dd321424" target="_blank">Task</a>(<a href="http://msdn2.microsoft.com/en-us/library/9eekhta0" target="_blank">IEnumerable</a>(<a href="T_ConnectQl_Interfaces_IColumnDescriptor">IColumnDescriptor</a>))<br />The columns in this data source.

## See Also


#### Reference
<a href="T_ConnectQl_DataSources_TimeDataSource">TimeDataSource Class</a><br /><a href="N_ConnectQl_DataSources">ConnectQl.DataSources Namespace</a><br />