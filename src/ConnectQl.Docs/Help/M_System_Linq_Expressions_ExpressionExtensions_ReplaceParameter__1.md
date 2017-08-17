# ExpressionExtensions.ReplaceParameter(*TExpression*) Method 
 

Replaces the parameter in the specified expression.

**Namespace:**&nbsp;<a href="N_System_Linq_Expressions">System.Linq.Expressions</a><br />**Assembly:**&nbsp;ConnectQl (in ConnectQl.dll) Version: 100.0.1-prerelease

## Syntax

**C#**<br />
``` C#
public static TExpression ReplaceParameter<TExpression>(
	this TExpression haystack,
	ParameterExpression needle,
	Expression replace
)
where TExpression : Expression

```


#### Parameters
&nbsp;<dl><dt>haystack</dt><dd>Type: *TExpression*<br />The expression in which to look for the *needle*.</dd><dt>needle</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/bb302740" target="_blank">System.Linq.Expressions.ParameterExpression</a><br />The parameter expression to replace.</dd><dt>replace</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/bb356138" target="_blank">System.Linq.Expressions.Expression</a><br />The expression to replace the parameter with.</dd></dl>

#### Type Parameters
&nbsp;<dl><dt>TExpression</dt><dd>The type of the expression.</dd></dl>

#### Return Value
Type: *TExpression*<br />The *TExpression*.

#### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type . When you use instance method syntax to call this method, omit the first parameter. For more information, see <a href="http://msdn.microsoft.com/en-us/library/bb384936.aspx">Extension Methods (Visual Basic)</a> or <a href="http://msdn.microsoft.com/en-us/library/bb383977.aspx">Extension Methods (C# Programming Guide)</a>.

## See Also


#### Reference
<a href="T_System_Linq_Expressions_ExpressionExtensions">ExpressionExtensions Class</a><br /><a href="N_System_Linq_Expressions">System.Linq.Expressions Namespace</a><br />