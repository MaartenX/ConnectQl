# CsvFileFormat.WriteRows Method 
 

Writes rows to the file.

**Namespace:**&nbsp;<a href="N_ConnectQl_Internal_FileFormats">ConnectQl.Internal.FileFormats</a><br />**Assembly:**&nbsp;ConnectQl (in ConnectQl.dll) Version: 100.0.1-prerelease

## Syntax

**C#**<br />
``` C#
public long WriteRows(
	IFileFormatExecutionContext context,
	StreamWriter writer,
	IEnumerable<Row> rows,
	bool upsert
)
```


#### Parameters
&nbsp;<dl><dt>context</dt><dd>Type: <a href="T_ConnectQl_Interfaces_IFileFormatExecutionContext">ConnectQl.Interfaces.IFileFormatExecutionContext</a><br />The context.</dd><dt>writer</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/3ssew6tk" target="_blank">System.IO.StreamWriter</a><br />The stream.</dd><dt>rows</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/9eekhta0" target="_blank">System.Collections.Generic.IEnumerable</a>(<a href="T_ConnectQl_Results_Row">Row</a>)<br />The rows to write.</dd><dt>upsert</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/a28wyd50" target="_blank">System.Boolean</a><br />True to upsert, false to insert.</dd></dl>

#### Return Value
Type: <a href="http://msdn2.microsoft.com/en-us/library/6yy583ek" target="_blank">Int64</a><br />The number of rows that were written.

#### Implements
<a href="M_ConnectQl_Interfaces_IFileWriter_WriteRows">IFileWriter.WriteRows(IFileFormatExecutionContext, StreamWriter, IEnumerable(Row), Boolean)</a><br />

## See Also


#### Reference
<a href="T_ConnectQl_Internal_FileFormats_CsvFileFormat">CsvFileFormat Class</a><br /><a href="N_ConnectQl_Internal_FileFormats">ConnectQl.Internal.FileFormats Namespace</a><br />