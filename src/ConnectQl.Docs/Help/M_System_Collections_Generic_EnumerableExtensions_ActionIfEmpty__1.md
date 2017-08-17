# EnumerableExtensions.ActionIfEmpty(*T*) Method 
 

Returns the same enumerable, but executes an action if the enumerable is empty.

**Namespace:**&nbsp;<a href="N_System_Collections_Generic">System.Collections.Generic</a><br />**Assembly:**&nbsp;ConnectQl (in ConnectQl.dll) Version: 100.0.1-prerelease

## Syntax

**C#**<br />
``` C#
public static IEnumerable<T> ActionIfEmpty<T>(
	this IEnumerable<T> source,
	Action isEmpty
)

```


#### Parameters
&nbsp;<dl><dt>source</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/9eekhta0" target="_blank">System.Collections.Generic.IEnumerable</a>(*T*)<br />The source <a href="http://msdn2.microsoft.com/en-us/library/9eekhta0" target="_blank">IEnumerable(T)</a>.</dd><dt>isEmpty</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/bb534741" target="_blank">System.Action</a><br />The action to call when the enumerable is empty.</dd></dl>

#### Type Parameters
&nbsp;<dl><dt>T</dt><dd>The type of the elements in the <a href="http://msdn2.microsoft.com/en-us/library/9eekhta0" target="_blank">IEnumerable(T)</a>.</dd></dl>

#### Return Value
Type: <a href="http://msdn2.microsoft.com/en-us/library/9eekhta0" target="_blank">IEnumerable</a>(*T*)<br />An enumerable with the same elements as *source*.

#### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type <a href="http://msdn2.microsoft.com/en-us/library/9eekhta0" target="_blank">IEnumerable</a>(*T*). When you use instance method syntax to call this method, omit the first parameter. For more information, see <a href="http://msdn.microsoft.com/en-us/library/bb384936.aspx">Extension Methods (Visual Basic)</a> or <a href="http://msdn.microsoft.com/en-us/library/bb383977.aspx">Extension Methods (C# Programming Guide)</a>.

## See Also


#### Reference
<a href="T_System_Collections_Generic_EnumerableExtensions">EnumerableExtensions Class</a><br /><a href="N_System_Collections_Generic">System.Collections.Generic Namespace</a><br />