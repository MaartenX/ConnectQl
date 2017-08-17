# GenericVisitor.Visit(*T*) Method (Func(*T*, Expression), Expression)
 

Creates a <a href="T_ConnectQl_Expressions_Visitors_GenericVisitor">GenericVisitor</a>, visits the *expression* and returns the result.

**Namespace:**&nbsp;<a href="N_ConnectQl_Expressions_Visitors">ConnectQl.Expressions.Visitors</a><br />**Assembly:**&nbsp;ConnectQl (in ConnectQl.dll) Version: 100.0.1-prerelease

## Syntax

**C#**<br />
``` C#
public static Expression Visit<T>(
	Func<T, Expression> action,
	Expression expression
)
where T : Expression

```


#### Parameters
&nbsp;<dl><dt>action</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/bb549151" target="_blank">System.Func</a>(*T*, <a href="http://msdn2.microsoft.com/en-us/library/bb356138" target="_blank">Expression</a>)<br />The action to add to the visitor.</dd><dt>expression</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/bb356138" target="_blank">System.Linq.Expressions.Expression</a><br />The expression.</dd></dl>

#### Type Parameters
&nbsp;<dl><dt>T</dt><dd>The type of the function parameter.</dd></dl>

#### Return Value
Type: <a href="http://msdn2.microsoft.com/en-us/library/bb356138" target="_blank">Expression</a><br />The <a href="http://msdn2.microsoft.com/en-us/library/bb356138" target="_blank">Expression</a>.

## See Also


#### Reference
<a href="T_ConnectQl_Expressions_Visitors_GenericVisitor">GenericVisitor Class</a><br /><a href="Overload_ConnectQl_Expressions_Visitors_GenericVisitor_Visit">Visit Overload</a><br /><a href="N_ConnectQl_Expressions_Visitors">ConnectQl.Expressions.Visitors Namespace</a><br />