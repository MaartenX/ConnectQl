# GenericVisitor.VisitListInit Method 
 

Visits the children of the <a href="http://msdn2.microsoft.com/en-us/library/bb348800" target="_blank">ListInitExpression</a>.

**Namespace:**&nbsp;<a href="N_ConnectQl_Expressions_Visitors">ConnectQl.Expressions.Visitors</a><br />**Assembly:**&nbsp;ConnectQl (in ConnectQl.dll) Version: 100.0.1-prerelease

## Syntax

**C#**<br />
``` C#
protected override Expression VisitListInit(
	ListInitExpression node
)
```


#### Parameters
&nbsp;<dl><dt>node</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/bb348800" target="_blank">System.Linq.Expressions.ListInitExpression</a><br />The expression to visit.</dd></dl>

#### Return Value
Type: <a href="http://msdn2.microsoft.com/en-us/library/bb356138" target="_blank">Expression</a><br />The modified expression, if it or any sub-Expression was modified; otherwise, returns the original Expression.

## See Also


#### Reference
<a href="T_ConnectQl_Expressions_Visitors_GenericVisitor">GenericVisitor Class</a><br /><a href="N_ConnectQl_Expressions_Visitors">ConnectQl.Expressions.Visitors Namespace</a><br />