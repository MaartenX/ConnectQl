# EnumerableExtensions.AggregateAsync(*TElement*, *TResult*) Method 
 

Aggregates an enumerable asynchronously.

**Namespace:**&nbsp;<a href="N_System_Collections_Generic">System.Collections.Generic</a><br />**Assembly:**&nbsp;ConnectQl (in ConnectQl.dll) Version: 100.0.1-prerelease

## Syntax

**C#**<br />
``` C#
public static Task<TResult> AggregateAsync<TElement, TResult>(
	this IEnumerable<TElement> source,
	TResult start,
	Func<TResult, TElement, Task<TResult>> aggregate
)

```


#### Parameters
&nbsp;<dl><dt>source</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/9eekhta0" target="_blank">System.Collections.Generic.IEnumerable</a>(*TElement*)<br />The source.</dd><dt>start</dt><dd>Type: *TResult*<br />The start.</dd><dt>aggregate</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/bb534647" target="_blank">System.Func</a>(*TResult*, *TElement*, <a href="http://msdn2.microsoft.com/en-us/library/dd321424" target="_blank">Task</a>(*TResult*))<br />The aggregate.</dd></dl>

#### Type Parameters
&nbsp;<dl><dt>TElement</dt><dd>The type of the elements.</dd><dt>TResult</dt><dd>The type of the result.</dd></dl>

#### Return Value
Type: <a href="http://msdn2.microsoft.com/en-us/library/dd321424" target="_blank">Task</a>(*TResult*)<br />The <a href="http://msdn2.microsoft.com/en-us/library/dd235678" target="_blank">Task</a>.

#### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type <a href="http://msdn2.microsoft.com/en-us/library/9eekhta0" target="_blank">IEnumerable</a>(*TElement*). When you use instance method syntax to call this method, omit the first parameter. For more information, see <a href="http://msdn.microsoft.com/en-us/library/bb384936.aspx">Extension Methods (Visual Basic)</a> or <a href="http://msdn.microsoft.com/en-us/library/bb383977.aspx">Extension Methods (C# Programming Guide)</a>.

## See Also


#### Reference
<a href="T_System_Collections_Generic_EnumerableExtensions">EnumerableExtensions Class</a><br /><a href="N_System_Collections_Generic">System.Collections.Generic Namespace</a><br />