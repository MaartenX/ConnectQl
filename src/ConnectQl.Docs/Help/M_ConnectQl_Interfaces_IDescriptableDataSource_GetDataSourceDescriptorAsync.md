# IDescriptableDataSource.GetDataSourceDescriptorAsync Method 
 

Gets the descriptor for this data source.

**Namespace:**&nbsp;<a href="N_ConnectQl_Interfaces">ConnectQl.Interfaces</a><br />**Assembly:**&nbsp;ConnectQl (in ConnectQl.dll) Version: 100.0.1-prerelease

## Syntax

**C#**<br />
``` C#
Task<IDataSourceDescriptor> GetDataSourceDescriptorAsync(
	string sourceAlias,
	IExecutionContext context
)
```


#### Parameters
&nbsp;<dl><dt>sourceAlias</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/s1wwdcbf" target="_blank">System.String</a><br />The data source source alias.</dd><dt>context</dt><dd>Type: <a href="T_ConnectQl_Interfaces_IExecutionContext">ConnectQl.Interfaces.IExecutionContext</a><br />The execution context.</dd></dl>

#### Return Value
Type: <a href="http://msdn2.microsoft.com/en-us/library/dd321424" target="_blank">Task</a>(<a href="T_ConnectQl_Interfaces_IDataSourceDescriptor">IDataSourceDescriptor</a>)<br />The <a href="http://msdn2.microsoft.com/en-us/library/dd235678" target="_blank">Task</a>.

## See Also


#### Reference
<a href="T_ConnectQl_Interfaces_IDescriptableDataSource">IDescriptableDataSource Interface</a><br /><a href="N_ConnectQl_Interfaces">ConnectQl.Interfaces Namespace</a><br />