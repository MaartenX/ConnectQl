# GenericVisitor.Create(*T*) Method (Action(*T*))
 

Creates a <a href="T_ConnectQl_Expressions_Visitors_GenericVisitor">GenericVisitor</a> with the specified action.

**Namespace:**&nbsp;<a href="N_ConnectQl_Expressions_Visitors">ConnectQl.Expressions.Visitors</a><br />**Assembly:**&nbsp;ConnectQl (in ConnectQl.dll) Version: 100.0.1-prerelease

## Syntax

**C#**<br />
``` C#
public static GenericVisitor Create<T>(
	Action<T> action
)
where T : Expression

```


#### Parameters
&nbsp;<dl><dt>action</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/018hxwa8" target="_blank">System.Action</a>(*T*)<br />The action to add to the visitor.</dd></dl>

#### Type Parameters
&nbsp;<dl><dt>T</dt><dd>The type of the function parameter.</dd></dl>

#### Return Value
Type: <a href="T_ConnectQl_Expressions_Visitors_GenericVisitor">GenericVisitor</a><br />The <a href="http://msdn2.microsoft.com/en-us/library/bb356138" target="_blank">Expression</a>.

## See Also


#### Reference
<a href="T_ConnectQl_Expressions_Visitors_GenericVisitor">GenericVisitor Class</a><br /><a href="Overload_ConnectQl_Expressions_Visitors_GenericVisitor_Create">Create Overload</a><br /><a href="N_ConnectQl_Expressions_Visitors">ConnectQl.Expressions.Visitors Namespace</a><br />