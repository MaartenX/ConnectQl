# GenericVisitor.VisitCatchBlock Method 
 

Visits the children of the <a href="http://msdn2.microsoft.com/en-us/library/dd294112" target="_blank">CatchBlock</a>.

**Namespace:**&nbsp;<a href="N_ConnectQl_Expressions_Visitors">ConnectQl.Expressions.Visitors</a><br />**Assembly:**&nbsp;ConnectQl (in ConnectQl.dll) Version: 100.0.1-prerelease

## Syntax

**C#**<br />
``` C#
protected override CatchBlock VisitCatchBlock(
	CatchBlock node
)
```


#### Parameters
&nbsp;<dl><dt>node</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/dd294112" target="_blank">System.Linq.Expressions.CatchBlock</a><br />The expression to visit.</dd></dl>

#### Return Value
Type: <a href="http://msdn2.microsoft.com/en-us/library/dd294112" target="_blank">CatchBlock</a><br />The modified expression, if it or any sub-Expression was modified; otherwise, returns the original Expression.

## See Also


#### Reference
<a href="T_ConnectQl_Expressions_Visitors_GenericVisitor">GenericVisitor Class</a><br /><a href="N_ConnectQl_Expressions_Visitors">ConnectQl.Expressions.Visitors Namespace</a><br />