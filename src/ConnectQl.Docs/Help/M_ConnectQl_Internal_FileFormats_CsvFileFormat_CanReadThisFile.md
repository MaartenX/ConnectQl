# CsvFileFormat.CanReadThisFile Method 
 

Checks if the file reader can read this file.

**Namespace:**&nbsp;<a href="N_ConnectQl_Internal_FileFormats">ConnectQl.Internal.FileFormats</a><br />**Assembly:**&nbsp;ConnectQl (in ConnectQl.dll) Version: 100.0.1-prerelease

## Syntax

**C#**<br />
``` C#
public bool CanReadThisFile(
	IFileFormatExecutionContext context,
	string fileName,
	byte[] firstBytes
)
```


#### Parameters
&nbsp;<dl><dt>context</dt><dd>Type: <a href="T_ConnectQl_Interfaces_IFileFormatExecutionContext">ConnectQl.Interfaces.IFileFormatExecutionContext</a><br />The context.</dd><dt>fileName</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/s1wwdcbf" target="_blank">System.String</a><br />The file name.</dd><dt>firstBytes</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/yyb1w04y" target="_blank">System.Byte</a>[]<br />The first bytes of the file.</dd></dl>

#### Return Value
Type: <a href="http://msdn2.microsoft.com/en-us/library/a28wyd50" target="_blank">Boolean</a><br />`true` if this reader can read the file, `false` otherwise.

#### Implements
<a href="M_ConnectQl_Interfaces_IFileReader_CanReadThisFile">IFileReader.CanReadThisFile(IFileFormatExecutionContext, String, Byte[])</a><br />

## See Also


#### Reference
<a href="T_ConnectQl_Internal_FileFormats_CsvFileFormat">CsvFileFormat Class</a><br /><a href="N_ConnectQl_Internal_FileFormats">ConnectQl.Internal.FileFormats Namespace</a><br />