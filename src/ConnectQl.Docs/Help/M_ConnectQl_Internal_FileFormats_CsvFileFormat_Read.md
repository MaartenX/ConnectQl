# CsvFileFormat.Read Method 
 

Reads a reader as comma separated values.

**Namespace:**&nbsp;<a href="N_ConnectQl_Internal_FileFormats">ConnectQl.Internal.FileFormats</a><br />**Assembly:**&nbsp;ConnectQl (in ConnectQl.dll) Version: 100.0.1-prerelease

## Syntax

**C#**<br />
``` C#
public IEnumerable<Row> Read(
	IFileFormatExecutionContext context,
	IRowBuilder rowBuilder,
	StreamReader reader,
	HashSet<string> fields
)
```


#### Parameters
&nbsp;<dl><dt>context</dt><dd>Type: <a href="T_ConnectQl_Interfaces_IFileFormatExecutionContext">ConnectQl.Interfaces.IFileFormatExecutionContext</a><br />The context.</dd><dt>rowBuilder</dt><dd>Type: <a href="T_ConnectQl_Interfaces_IRowBuilder">ConnectQl.Interfaces.IRowBuilder</a><br />The data Set.</dd><dt>reader</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/6aetdk20" target="_blank">System.IO.StreamReader</a><br />The stream.</dd><dt>fields</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/bb359438" target="_blank">System.Collections.Generic.HashSet</a>(<a href="http://msdn2.microsoft.com/en-us/library/s1wwdcbf" target="_blank">String</a>)<br />The fields, or `null` to retrieve all fields.</dd></dl>

#### Return Value
Type: <a href="http://msdn2.microsoft.com/en-us/library/9eekhta0" target="_blank">IEnumerable</a>(<a href="T_ConnectQl_Results_Row">Row</a>)<br />The rows.

#### Implements
<a href="M_ConnectQl_Interfaces_IFileReader_Read">IFileReader.Read(IFileFormatExecutionContext, IRowBuilder, StreamReader, HashSet(String))</a><br />

## See Also


#### Reference
<a href="T_ConnectQl_Internal_FileFormats_CsvFileFormat">CsvFileFormat Class</a><br /><a href="N_ConnectQl_Internal_FileFormats">ConnectQl.Internal.FileFormats Namespace</a><br />