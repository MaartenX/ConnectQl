# CustomExpression.MakeCompare Method 
 

Creates a compare expression.

**Namespace:**&nbsp;<a href="N_ConnectQl_Expressions">ConnectQl.Expressions</a><br />**Assembly:**&nbsp;ConnectQl (in ConnectQl.dll) Version: 100.0.1-prerelease

## Syntax

**C#**<br />
``` C#
public static CompareExpression MakeCompare(
	ExpressionType compareType,
	Expression left,
	Expression right
)
```


#### Parameters
&nbsp;<dl><dt>compareType</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/bb361179" target="_blank">System.Linq.Expressions.ExpressionType</a><br />The compare type.</dd><dt>left</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/bb356138" target="_blank">System.Linq.Expressions.Expression</a><br />The left expression.</dd><dt>right</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/bb356138" target="_blank">System.Linq.Expressions.Expression</a><br />The right expression.</dd></dl>

#### Return Value
Type: <a href="T_ConnectQl_Expressions_CompareExpression">CompareExpression</a><br />The <a href="T_ConnectQl_Expressions_CompareExpression">CompareExpression</a>.

## Exceptions
&nbsp;<table><tr><th>Exception</th><th>Condition</th></tr><tr><td><a href="http://msdn2.microsoft.com/en-us/library/8xt94y6e" target="_blank">ArgumentOutOfRangeException</a></td><td>Thrown when an invalid <a href="http://msdn2.microsoft.com/en-us/library/bb361179" target="_blank">ExpressionType</a> is passed in.</td></tr></table>

## See Also


#### Reference
<a href="T_ConnectQl_Expressions_CustomExpression">CustomExpression Class</a><br /><a href="N_ConnectQl_Expressions">ConnectQl.Expressions Namespace</a><br />