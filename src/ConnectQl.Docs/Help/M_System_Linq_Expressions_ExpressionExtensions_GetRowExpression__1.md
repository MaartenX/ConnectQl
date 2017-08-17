# ExpressionExtensions.GetRowExpression(*TParam*) Method 
 

Creates a function that converts a row into a value.

**Namespace:**&nbsp;<a href="N_System_Linq_Expressions">System.Linq.Expressions</a><br />**Assembly:**&nbsp;ConnectQl (in ConnectQl.dll) Version: 100.0.1-prerelease

## Syntax

**C#**<br />
``` C#
public static Func<TParam, Object> GetRowExpression<TParam>(
	this Expression expression
)

```


#### Parameters
&nbsp;<dl><dt>expression</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/bb356138" target="_blank">System.Linq.Expressions.Expression</a><br />The expression.</dd></dl>

#### Type Parameters
&nbsp;<dl><dt>TParam</dt><dd>The type of the argument the lambda will have.</dd></dl>

#### Return Value
Type: <a href="http://msdn2.microsoft.com/en-us/library/bb549151" target="_blank">Func</a>(*TParam*, <a href="http://msdn2.microsoft.com/en-us/library/e5kfa45b" target="_blank">Object</a>)<br />A function that takes a row and returns the value for the expression.

#### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type <a href="http://msdn2.microsoft.com/en-us/library/bb356138" target="_blank">Expression</a>. When you use instance method syntax to call this method, omit the first parameter. For more information, see <a href="http://msdn.microsoft.com/en-us/library/bb384936.aspx">Extension Methods (Visual Basic)</a> or <a href="http://msdn.microsoft.com/en-us/library/bb383977.aspx">Extension Methods (C# Programming Guide)</a>.

## See Also


#### Reference
<a href="T_System_Linq_Expressions_ExpressionExtensions">ExpressionExtensions Class</a><br /><a href="N_System_Linq_Expressions">System.Linq.Expressions Namespace</a><br />