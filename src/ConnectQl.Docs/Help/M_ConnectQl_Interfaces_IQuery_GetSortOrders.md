# IQuery.GetSortOrders Method 
 

Retrieves the sort orders for the query.

**Namespace:**&nbsp;<a href="N_ConnectQl_Interfaces">ConnectQl.Interfaces</a><br />**Assembly:**&nbsp;ConnectQl (in ConnectQl.dll) Version: 100.0.1-prerelease

## Syntax

**C#**<br />
``` C#
IEnumerable<IOrderByExpression> GetSortOrders(
	IExecutionContext context
)
```


#### Parameters
&nbsp;<dl><dt>context</dt><dd>Type: <a href="T_ConnectQl_Interfaces_IExecutionContext">ConnectQl.Interfaces.IExecutionContext</a><br />The execution context.</dd></dl>

#### Return Value
Type: <a href="http://msdn2.microsoft.com/en-us/library/9eekhta0" target="_blank">IEnumerable</a>(<a href="T_ConnectQl_Interfaces_IOrderByExpression">IOrderByExpression</a>)<br />A collection of sort orders.

## See Also


#### Reference
<a href="T_ConnectQl_Interfaces_IQuery">IQuery Interface</a><br /><a href="N_ConnectQl_Interfaces">ConnectQl.Interfaces Namespace</a><br />