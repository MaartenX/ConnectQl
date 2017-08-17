# IFileWriter.ShouldMaterialize Property 
 

Gets a value indicating whether the collection of rows should be materialized when calling this writer. When all columns are needed in the header (e.g. for CSV or Excel), you should return `true` here. Other formats that use the columns per object (like JSON) can return `false`.

**Namespace:**&nbsp;<a href="N_ConnectQl_Interfaces">ConnectQl.Interfaces</a><br />**Assembly:**&nbsp;ConnectQl (in ConnectQl.dll) Version: 100.0.1-prerelease

## Syntax

**C#**<br />
``` C#
bool ShouldMaterialize { get; }
```


#### Property Value
Type: <a href="http://msdn2.microsoft.com/en-us/library/a28wyd50" target="_blank">Boolean</a>

## See Also


#### Reference
<a href="T_ConnectQl_Interfaces_IFileWriter">IFileWriter Interface</a><br /><a href="N_ConnectQl_Interfaces">ConnectQl.Interfaces Namespace</a><br />