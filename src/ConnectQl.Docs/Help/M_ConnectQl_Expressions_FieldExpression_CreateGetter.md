# FieldExpression.CreateGetter Method 
 

Creates a method call that gets the value from the specified parameter.

**Namespace:**&nbsp;<a href="N_ConnectQl_Expressions">ConnectQl.Expressions</a><br />**Assembly:**&nbsp;ConnectQl (in ConnectQl.dll) Version: 100.0.1-prerelease

## Syntax

**C#**<br />
``` C#
public MethodCallExpression CreateGetter(
	ParameterExpression row,
	Type type = null
)
```


#### Parameters
&nbsp;<dl><dt>row</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/bb302740" target="_blank">System.Linq.Expressions.ParameterExpression</a><br />The parameter to get the field from.</dd><dt>type (Optional)</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/42892f65" target="_blank">System.Type</a><br />The type to return (when omitted, the node's type will be returned).</dd></dl>

#### Return Value
Type: <a href="http://msdn2.microsoft.com/en-us/library/bb357368" target="_blank">MethodCallExpression</a><br />The <a href="http://msdn2.microsoft.com/en-us/library/bb357368" target="_blank">MethodCallExpression</a>.

## See Also


#### Reference
<a href="T_ConnectQl_Expressions_FieldExpression">FieldExpression Class</a><br /><a href="N_ConnectQl_Expressions">ConnectQl.Expressions Namespace</a><br />