# ExpressionExtensions.GetGroupValues Method 
 

Gets the values for the specified field from a group.

**Namespace:**&nbsp;<a href="N_System_Linq_Expressions">System.Linq.Expressions</a><br />**Assembly:**&nbsp;ConnectQl (in ConnectQl.dll) Version: 100.0.1-prerelease

## Syntax

**C#**<br />
``` C#
public static Object[] GetGroupValues(
	IEnumerable<Row> rows,
	string field
)
```


#### Parameters
&nbsp;<dl><dt>rows</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/9eekhta0" target="_blank">System.Collections.Generic.IEnumerable</a>(<a href="T_ConnectQl_Results_Row">Row</a>)<br />The rows to get the values from.</dd><dt>field</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/s1wwdcbf" target="_blank">System.String</a><br />The field to get.</dd></dl>

#### Return Value
Type: <a href="http://msdn2.microsoft.com/en-us/library/e5kfa45b" target="_blank">Object</a>[]<br />The values of the fields in the specified rows.

## See Also


#### Reference
<a href="T_System_Linq_Expressions_ExpressionExtensions">ExpressionExtensions Class</a><br /><a href="N_System_Linq_Expressions">System.Linq.Expressions Namespace</a><br />