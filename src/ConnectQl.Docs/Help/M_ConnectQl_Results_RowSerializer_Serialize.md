# RowSerializer.Serialize Method 
 

Transforms the value to the serializable object.

**Namespace:**&nbsp;<a href="N_ConnectQl_Results">ConnectQl.Results</a><br />**Assembly:**&nbsp;ConnectQl (in ConnectQl.dll) Version: 100.0.1-prerelease

## Syntax

**C#**<br />
``` C#
public Object Serialize(
	IDisposable context,
	Row value
)
```


#### Parameters
&nbsp;<dl><dt>context</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/aax125c9" target="_blank">System.IDisposable</a><br />The context in which this value is transformed.</dd><dt>value</dt><dd>Type: <a href="T_ConnectQl_Results_Row">ConnectQl.Results.Row</a><br />The value to transform.</dd></dl>

#### Return Value
Type: <a href="http://msdn2.microsoft.com/en-us/library/e5kfa45b" target="_blank">Object</a><br />A serializable version of the value.

#### Implements
<a href="M_ConnectQl_AsyncEnumerablePolicies_ITransform_1_Serialize">ITransform(T).Serialize(IDisposable, T)</a><br />

## See Also


#### Reference
<a href="T_ConnectQl_Results_RowSerializer">RowSerializer Class</a><br /><a href="N_ConnectQl_Results">ConnectQl.Results Namespace</a><br />