# IDataTarget.WriteRowsAsync Method 
 

Writes the rowsToWrite to the specified target.

**Namespace:**&nbsp;<a href="N_ConnectQl_Interfaces">ConnectQl.Interfaces</a><br />**Assembly:**&nbsp;ConnectQl (in ConnectQl.dll) Version: 100.0.1-prerelease

## Syntax

**C#**<br />
``` C#
Task<long> WriteRowsAsync(
	IExecutionContext context,
	IAsyncEnumerable<Row> rowsToWrite,
	bool upsert
)
```


#### Parameters
&nbsp;<dl><dt>context</dt><dd>Type: <a href="T_ConnectQl_Interfaces_IExecutionContext">ConnectQl.Interfaces.IExecutionContext</a><br />The context.</dd><dt>rowsToWrite</dt><dd>Type: <a href="T_ConnectQl_AsyncEnumerables_IAsyncEnumerable_1">ConnectQl.AsyncEnumerables.IAsyncEnumerable</a>(<a href="T_ConnectQl_Results_Row">Row</a>)<br />The rowsToWrite.</dd><dt>upsert</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/a28wyd50" target="_blank">System.Boolean</a><br />True to also update records, false to insert.</dd></dl>

#### Return Value
Type: <a href="http://msdn2.microsoft.com/en-us/library/dd321424" target="_blank">Task</a>(<a href="http://msdn2.microsoft.com/en-us/library/6yy583ek" target="_blank">Int64</a>)<br />The <a href="http://msdn2.microsoft.com/en-us/library/dd235678" target="_blank">Task</a>.

## See Also


#### Reference
<a href="T_ConnectQl_Interfaces_IDataTarget">IDataTarget Interface</a><br /><a href="N_ConnectQl_Interfaces">ConnectQl.Interfaces Namespace</a><br />