# GenericVisitor.Add(*T*) Method (Action(*T*))
 

The add.

**Namespace:**&nbsp;<a href="N_ConnectQl_Expressions_Visitors">ConnectQl.Expressions.Visitors</a><br />**Assembly:**&nbsp;ConnectQl (in ConnectQl.dll) Version: 100.0.1-prerelease

## Syntax

**C#**<br />
``` C#
public GenericVisitor Add<T>(
	Action<T> implementation
)
where T : Expression

```


#### Parameters
&nbsp;<dl><dt>implementation</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/018hxwa8" target="_blank">System.Action</a>(*T*)<br />The implementation.</dd></dl>

#### Type Parameters
&nbsp;<dl><dt>T</dt><dd>The type of the expression this implementation is for.</dd></dl>

#### Return Value
Type: <a href="T_ConnectQl_Expressions_Visitors_GenericVisitor">GenericVisitor</a><br />The <a href="T_ConnectQl_Expressions_Visitors_GenericVisitor">GenericVisitor</a>.

## See Also


#### Reference
<a href="T_ConnectQl_Expressions_Visitors_GenericVisitor">GenericVisitor Class</a><br /><a href="Overload_ConnectQl_Expressions_Visitors_GenericVisitor_Add">Add Overload</a><br /><a href="N_ConnectQl_Expressions_Visitors">ConnectQl.Expressions.Visitors Namespace</a><br />