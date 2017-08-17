# ExpressionExtensions.EvaluateAsValue Method 
 

Tries to evaluate the expression if it is a constant. If an error occurs, returns an <a href="T_ConnectQl_Results_Error">Error</a> object.

**Namespace:**&nbsp;<a href="N_System_Linq_Expressions">System.Linq.Expressions</a><br />**Assembly:**&nbsp;ConnectQl (in ConnectQl.dll) Version: 100.0.1-prerelease

## Syntax

**C#**<br />
``` C#
public static Expression EvaluateAsValue(
	this Expression expression
)
```


#### Parameters
&nbsp;<dl><dt>expression</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/bb356138" target="_blank">System.Linq.Expressions.Expression</a><br />The expression to evaluate.</dd></dl>

#### Return Value
Type: <a href="http://msdn2.microsoft.com/en-us/library/bb356138" target="_blank">Expression</a><br />When the expression contains field references or context references, the original expression, otherwise the result of the evaluation or an <a href="T_ConnectQl_Results_Error">Error</a> object.

#### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type <a href="http://msdn2.microsoft.com/en-us/library/bb356138" target="_blank">Expression</a>. When you use instance method syntax to call this method, omit the first parameter. For more information, see <a href="http://msdn.microsoft.com/en-us/library/bb384936.aspx">Extension Methods (Visual Basic)</a> or <a href="http://msdn.microsoft.com/en-us/library/bb383977.aspx">Extension Methods (C# Programming Guide)</a>.

## See Also


#### Reference
<a href="T_System_Linq_Expressions_ExpressionExtensions">ExpressionExtensions Class</a><br /><a href="N_System_Linq_Expressions">System.Linq.Expressions Namespace</a><br />