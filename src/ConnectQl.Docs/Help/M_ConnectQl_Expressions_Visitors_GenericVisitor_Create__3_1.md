# GenericVisitor.Create(*T1*, *T2*, *T3*) Method (Func(*T1*, Expression), Func(*T2*, Expression), Func(*T3*, Expression))
 

Creates a <a href="T_ConnectQl_Expressions_Visitors_GenericVisitor">GenericVisitor</a> with the specified action.

**Namespace:**&nbsp;<a href="N_ConnectQl_Expressions_Visitors">ConnectQl.Expressions.Visitors</a><br />**Assembly:**&nbsp;ConnectQl (in ConnectQl.dll) Version: 100.0.1-prerelease

## Syntax

**C#**<br />
``` C#
public static GenericVisitor Create<T1, T2, T3>(
	Func<T1, Expression> firstAction,
	Func<T2, Expression> secondAction,
	Func<T3, Expression> thirdAction
)
where T1 : Expression
where T2 : Expression
where T3 : Expression

```


#### Parameters
&nbsp;<dl><dt>firstAction</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/bb549151" target="_blank">System.Func</a>(*T1*, <a href="http://msdn2.microsoft.com/en-us/library/bb356138" target="_blank">Expression</a>)<br />The first action to add to the visitor.</dd><dt>secondAction</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/bb549151" target="_blank">System.Func</a>(*T2*, <a href="http://msdn2.microsoft.com/en-us/library/bb356138" target="_blank">Expression</a>)<br />The second action to add to the visitor.</dd><dt>thirdAction</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/bb549151" target="_blank">System.Func</a>(*T3*, <a href="http://msdn2.microsoft.com/en-us/library/bb356138" target="_blank">Expression</a>)<br />The third action to add to the visitor.</dd></dl>

#### Type Parameters
&nbsp;<dl><dt>T1</dt><dd>The type of the first function parameter.</dd><dt>T2</dt><dd>The type of the second function parameter.</dd><dt>T3</dt><dd>The type of the third function parameter.</dd></dl>

#### Return Value
Type: <a href="T_ConnectQl_Expressions_Visitors_GenericVisitor">GenericVisitor</a><br />The <a href="http://msdn2.microsoft.com/en-us/library/bb356138" target="_blank">Expression</a>.

## See Also


#### Reference
<a href="T_ConnectQl_Expressions_Visitors_GenericVisitor">GenericVisitor Class</a><br /><a href="Overload_ConnectQl_Expressions_Visitors_GenericVisitor_Create">Create Overload</a><br /><a href="N_ConnectQl_Expressions_Visitors">ConnectQl.Expressions.Visitors Namespace</a><br />