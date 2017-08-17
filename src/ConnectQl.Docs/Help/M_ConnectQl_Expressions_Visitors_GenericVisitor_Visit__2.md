# GenericVisitor.Visit(*T1*, *T2*) Method (Action(*T1*), Action(*T2*), Expression)
 

Creates a <a href="T_ConnectQl_Expressions_Visitors_GenericVisitor">GenericVisitor</a>, visits the *expression* and returns the result.

**Namespace:**&nbsp;<a href="N_ConnectQl_Expressions_Visitors">ConnectQl.Expressions.Visitors</a><br />**Assembly:**&nbsp;ConnectQl (in ConnectQl.dll) Version: 100.0.1-prerelease

## Syntax

**C#**<br />
``` C#
public static Expression Visit<T1, T2>(
	Action<T1> firstAction,
	Action<T2> secondAction,
	Expression expression
)
where T1 : Expression
where T2 : Expression

```


#### Parameters
&nbsp;<dl><dt>firstAction</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/018hxwa8" target="_blank">System.Action</a>(*T1*)<br />The first action to add to the visitor.</dd><dt>secondAction</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/018hxwa8" target="_blank">System.Action</a>(*T2*)<br />The second action to add to the visitor.</dd><dt>expression</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/bb356138" target="_blank">System.Linq.Expressions.Expression</a><br />The expression.</dd></dl>

#### Type Parameters
&nbsp;<dl><dt>T1</dt><dd>The type of the first function parameter.</dd><dt>T2</dt><dd>The type of the second function parameter.</dd></dl>

#### Return Value
Type: <a href="http://msdn2.microsoft.com/en-us/library/bb356138" target="_blank">Expression</a><br />The <a href="http://msdn2.microsoft.com/en-us/library/bb356138" target="_blank">Expression</a>.

## See Also


#### Reference
<a href="T_ConnectQl_Expressions_Visitors_GenericVisitor">GenericVisitor Class</a><br /><a href="Overload_ConnectQl_Expressions_Visitors_GenericVisitor_Visit">Visit Overload</a><br /><a href="N_ConnectQl_Expressions_Visitors">ConnectQl.Expressions.Visitors Namespace</a><br />