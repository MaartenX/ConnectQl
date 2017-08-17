# ExcelFileFormat.GetDataSourceDescriptorAsync Method 
 

Gets the descriptor for this data source.

**Namespace:**&nbsp;<a href="N_ConnectQl_Excel_FileFormats">ConnectQl.Excel.FileFormats</a><br />**Assembly:**&nbsp;ConnectQl.Excel (in ConnectQl.Excel.dll) Version: 100.0.1-prerelease

## Syntax

**C#**<br />
``` C#
public Task<IDataSourceDescriptor> GetDataSourceDescriptorAsync(
	string alias,
	IFileFormatExecutionContext context,
	StreamReader reader
)
```


#### Parameters
&nbsp;<dl><dt>alias</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/s1wwdcbf" target="_blank">System.String</a><br />The alias of the data source.</dd><dt>context</dt><dd>Type: <a href="T_ConnectQl_Interfaces_IFileFormatExecutionContext">ConnectQl.Interfaces.IFileFormatExecutionContext</a><br />The execution context.</dd><dt>reader</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/6aetdk20" target="_blank">System.IO.StreamReader</a><br />The reader.</dd></dl>

#### Return Value
Type: <a href="http://msdn2.microsoft.com/en-us/library/dd321424" target="_blank">Task</a>(<a href="T_ConnectQl_Interfaces_IDataSourceDescriptor">IDataSourceDescriptor</a>)<br />The <a href="http://msdn2.microsoft.com/en-us/library/dd235678" target="_blank">Task</a>.

#### Implements
<a href="M_ConnectQl_Interfaces_IDescriptableFileFormat_GetDataSourceDescriptorAsync">IDescriptableFileFormat.GetDataSourceDescriptorAsync(String, IFileFormatExecutionContext, StreamReader)</a><br />

## See Also


#### Reference
<a href="T_ConnectQl_Excel_FileFormats_ExcelFileFormat">ExcelFileFormat Class</a><br /><a href="N_ConnectQl_Excel_FileFormats">ConnectQl.Excel.FileFormats Namespace</a><br />