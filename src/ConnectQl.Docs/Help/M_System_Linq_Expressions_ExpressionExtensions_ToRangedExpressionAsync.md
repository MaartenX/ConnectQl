# ExpressionExtensions.ToRangedExpressionAsync Method 
 

Replaces all fields in the *expressions* with the ranges for the fields found in *rows*. Leaves all fields of *ignoreAliases* intact.

**Namespace:**&nbsp;<a href="N_System_Linq_Expressions">System.Linq.Expressions</a><br />**Assembly:**&nbsp;ConnectQl (in ConnectQl.dll) Version: 100.0.1-prerelease

## Syntax

**C#**<br />
``` C#
public static Task<Expression[]> ToRangedExpressionAsync(
	this IEnumerable<Expression> expressions,
	IAsyncReadOnlyCollection<Row> rows,
	HashSet<string> ignoreAliases
)
```


#### Parameters
&nbsp;<dl><dt>expressions</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/9eekhta0" target="_blank">System.Collections.Generic.IEnumerable</a>(<a href="http://msdn2.microsoft.com/en-us/library/bb356138" target="_blank">Expression</a>)<br />The expressions.</dd><dt>rows</dt><dd>Type: <a href="T_ConnectQl_AsyncEnumerables_IAsyncReadOnlyCollection_1">ConnectQl.AsyncEnumerables.IAsyncReadOnlyCollection</a>(<a href="T_ConnectQl_Results_Row">Row</a>)<br />The rows.</dd><dt>ignoreAliases</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/bb359438" target="_blank">System.Collections.Generic.HashSet</a>(<a href="http://msdn2.microsoft.com/en-us/library/s1wwdcbf" target="_blank">String</a>)<br />Aliases to ignore.</dd></dl>

#### Return Value
Type: <a href="http://msdn2.microsoft.com/en-us/library/dd321424" target="_blank">Task</a>(<a href="http://msdn2.microsoft.com/en-us/library/bb356138" target="_blank">Expression</a>[])<br />An array of <a href="http://msdn2.microsoft.com/en-us/library/bb356138" target="_blank">Expression</a>s.

#### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type <a href="http://msdn2.microsoft.com/en-us/library/9eekhta0" target="_blank">IEnumerable</a>(<a href="http://msdn2.microsoft.com/en-us/library/bb356138" target="_blank">Expression</a>). When you use instance method syntax to call this method, omit the first parameter. For more information, see <a href="http://msdn.microsoft.com/en-us/library/bb384936.aspx">Extension Methods (Visual Basic)</a> or <a href="http://msdn.microsoft.com/en-us/library/bb383977.aspx">Extension Methods (C# Programming Guide)</a>.

## See Also


#### Reference
<a href="T_System_Linq_Expressions_ExpressionExtensions">ExpressionExtensions Class</a><br /><a href="N_System_Linq_Expressions">System.Linq.Expressions Namespace</a><br />