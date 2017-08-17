# Descriptor.ForDataSource Method 
 

The for data source.

**Namespace:**&nbsp;<a href="N_ConnectQl_Intellisense">ConnectQl.Intellisense</a><br />**Assembly:**&nbsp;ConnectQl (in ConnectQl.dll) Version: 100.0.1-prerelease

## Syntax

**C#**<br />
``` C#
public static IDataSourceDescriptor ForDataSource(
	string alias,
	IEnumerable<IColumnDescriptor> columns,
	bool allowsAnyColumnName = false
)
```


#### Parameters
&nbsp;<dl><dt>alias</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/s1wwdcbf" target="_blank">System.String</a><br />The alias.</dd><dt>columns</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/9eekhta0" target="_blank">System.Collections.Generic.IEnumerable</a>(<a href="T_ConnectQl_Interfaces_IColumnDescriptor">IColumnDescriptor</a>)<br />The columns.</dd><dt>allowsAnyColumnName (Optional)</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/a28wyd50" target="_blank">System.Boolean</a><br />The allows any column name.</dd></dl>

#### Return Value
Type: <a href="T_ConnectQl_Interfaces_IDataSourceDescriptor">IDataSourceDescriptor</a><br />The <a href="T_ConnectQl_Interfaces_IDataSourceDescriptor">IDataSourceDescriptor</a>.

## See Also


#### Reference
<a href="T_ConnectQl_Intellisense_Descriptor">Descriptor Class</a><br /><a href="N_ConnectQl_Intellisense">ConnectQl.Intellisense Namespace</a><br />