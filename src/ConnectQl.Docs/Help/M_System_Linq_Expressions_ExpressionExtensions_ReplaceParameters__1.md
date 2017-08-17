# ExpressionExtensions.ReplaceParameters(*TExpression*) Method 
 

Replaces the parameters in the specified expression.

**Namespace:**&nbsp;<a href="N_System_Linq_Expressions">System.Linq.Expressions</a><br />**Assembly:**&nbsp;ConnectQl (in ConnectQl.dll) Version: 100.0.1-prerelease

## Syntax

**C#**<br />
``` C#
public static TExpression ReplaceParameters<TExpression>(
	this TExpression haystack,
	IEnumerable<ParameterExpression> needles,
	IEnumerable<Expression> replaces
)
where TExpression : Expression

```


#### Parameters
&nbsp;<dl><dt>haystack</dt><dd>Type: *TExpression*<br />The expression in which to look for the *needles*.</dd><dt>needles</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/9eekhta0" target="_blank">System.Collections.Generic.IEnumerable</a>(<a href="http://msdn2.microsoft.com/en-us/library/bb302740" target="_blank">ParameterExpression</a>)<br />The parameter expressions to replace.</dd><dt>replaces</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/9eekhta0" target="_blank">System.Collections.Generic.IEnumerable</a>(<a href="http://msdn2.microsoft.com/en-us/library/bb356138" target="_blank">Expression</a>)<br />The expressions to replace the parameters with.</dd></dl>

#### Type Parameters
&nbsp;<dl><dt>TExpression</dt><dd>The type of the expression.</dd></dl>

#### Return Value
Type: *TExpression*<br />The *TExpression*.

#### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type . When you use instance method syntax to call this method, omit the first parameter. For more information, see <a href="http://msdn.microsoft.com/en-us/library/bb384936.aspx">Extension Methods (Visual Basic)</a> or <a href="http://msdn.microsoft.com/en-us/library/bb383977.aspx">Extension Methods (C# Programming Guide)</a>.

## See Also


#### Reference
<a href="T_System_Linq_Expressions_ExpressionExtensions">ExpressionExtensions Class</a><br /><a href="N_System_Linq_Expressions">System.Linq.Expressions Namespace</a><br />