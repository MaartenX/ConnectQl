# RowBuilderExtensions.CreateRow(*T*) Method 
 

Creates a row.

**Namespace:**&nbsp;<a href="N_ConnectQl_Interfaces">ConnectQl.Interfaces</a><br />**Assembly:**&nbsp;ConnectQl (in ConnectQl.dll) Version: 100.0.1-prerelease

## Syntax

**C#**<br />
``` C#
public static Row CreateRow<T>(
	this IRowBuilder rowBuilder,
	T uniqueId,
	params KeyValuePair<string, Object>[] fields
)

```


#### Parameters
&nbsp;<dl><dt>rowBuilder</dt><dd>Type: <a href="T_ConnectQl_Interfaces_IRowBuilder">ConnectQl.Interfaces.IRowBuilder</a><br />The row builder.</dd><dt>uniqueId</dt><dd>Type: *T*<br />The unique id of the row.</dd><dt>fields</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/5tbh8a42" target="_blank">System.Collections.Generic.KeyValuePair</a>(<a href="http://msdn2.microsoft.com/en-us/library/s1wwdcbf" target="_blank">String</a>, <a href="http://msdn2.microsoft.com/en-us/library/e5kfa45b" target="_blank">Object</a>)[]<br />The fields in the row.</dd></dl>

#### Type Parameters
&nbsp;<dl><dt>T</dt><dd>The type of the unique id of the row.</dd></dl>

#### Return Value
Type: <a href="T_ConnectQl_Results_Row">Row</a><br />The <a href="T_ConnectQl_Results_Row">Row</a>.

#### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type <a href="T_ConnectQl_Interfaces_IRowBuilder">IRowBuilder</a>. When you use instance method syntax to call this method, omit the first parameter. For more information, see <a href="http://msdn.microsoft.com/en-us/library/bb384936.aspx">Extension Methods (Visual Basic)</a> or <a href="http://msdn.microsoft.com/en-us/library/bb383977.aspx">Extension Methods (C# Programming Guide)</a>.

## See Also


#### Reference
<a href="T_ConnectQl_Interfaces_RowBuilderExtensions">RowBuilderExtensions Class</a><br /><a href="N_ConnectQl_Interfaces">ConnectQl.Interfaces Namespace</a><br />