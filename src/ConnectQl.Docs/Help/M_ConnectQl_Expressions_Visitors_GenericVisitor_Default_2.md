# GenericVisitor.Default Method (Func(GenericVisitor, Expression, Expression))
 

Registers a function to call when no other handler is registered.

**Namespace:**&nbsp;<a href="N_ConnectQl_Expressions_Visitors">ConnectQl.Expressions.Visitors</a><br />**Assembly:**&nbsp;ConnectQl (in ConnectQl.dll) Version: 100.0.1-prerelease

## Syntax

**C#**<br />
``` C#
public GenericVisitor Default(
	Func<GenericVisitor, Expression, Expression> implementation
)
```


#### Parameters
&nbsp;<dl><dt>implementation</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/bb534647" target="_blank">System.Func</a>(<a href="T_ConnectQl_Expressions_Visitors_GenericVisitor">GenericVisitor</a>, <a href="http://msdn2.microsoft.com/en-us/library/bb356138" target="_blank">Expression</a>, <a href="http://msdn2.microsoft.com/en-us/library/bb356138" target="_blank">Expression</a>)<br />The handler to register.</dd></dl>

#### Return Value
Type: <a href="T_ConnectQl_Expressions_Visitors_GenericVisitor">GenericVisitor</a><br />The <a href="T_ConnectQl_Expressions_Visitors_GenericVisitor">GenericVisitor</a>.

## See Also


#### Reference
<a href="T_ConnectQl_Expressions_Visitors_GenericVisitor">GenericVisitor Class</a><br /><a href="Overload_ConnectQl_Expressions_Visitors_GenericVisitor_Default">Default Overload</a><br /><a href="N_ConnectQl_Expressions_Visitors">ConnectQl.Expressions.Visitors Namespace</a><br />