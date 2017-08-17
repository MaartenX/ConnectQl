# EnumerableDataSource.Create(*T*) Method (IEnumerable(*T*), Func(*T*, IEnumerable(KeyValuePair(String, Object))))
 

Creates the enumerable data source from the enumerable.

**Namespace:**&nbsp;<a href="N_ConnectQl_DataSources">ConnectQl.DataSources</a><br />**Assembly:**&nbsp;ConnectQl (in ConnectQl.dll) Version: 100.0.1-prerelease

## Syntax

**C#**<br />
``` C#
public static IDataSource Create<T>(
	IEnumerable<T> enumerable,
	Func<T, IEnumerable<KeyValuePair<string, Object>>> rowGenerator
)

```


#### Parameters
&nbsp;<dl><dt>enumerable</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/9eekhta0" target="_blank">System.Collections.Generic.IEnumerable</a>(*T*)<br />The <a href="http://msdn2.microsoft.com/en-us/library/9eekhta0" target="_blank">IEnumerable(T)</a> to create the data source from.</dd><dt>rowGenerator</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/bb549151" target="_blank">System.Func</a>(*T*, <a href="http://msdn2.microsoft.com/en-us/library/9eekhta0" target="_blank">IEnumerable</a>(<a href="http://msdn2.microsoft.com/en-us/library/5tbh8a42" target="_blank">KeyValuePair</a>(<a href="http://msdn2.microsoft.com/en-us/library/s1wwdcbf" target="_blank">String</a>, <a href="http://msdn2.microsoft.com/en-us/library/e5kfa45b" target="_blank">Object</a>)))<br />The row generator.</dd></dl>

#### Type Parameters
&nbsp;<dl><dt>T</dt><dd>The type of the elements.</dd></dl>

#### Return Value
Type: <a href="T_ConnectQl_Interfaces_IDataSource">IDataSource</a><br />The <a href="T_ConnectQl_Interfaces_IDataSource">IDataSource</a>.

## See Also


#### Reference
<a href="T_ConnectQl_DataSources_EnumerableDataSource">EnumerableDataSource Class</a><br /><a href="Overload_ConnectQl_DataSources_EnumerableDataSource_Create">Create Overload</a><br /><a href="N_ConnectQl_DataSources">ConnectQl.DataSources Namespace</a><br />