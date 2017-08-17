# IDataSourceOrderBySupport.SupportsOrderBy Method 
 

Checks if the data source supports the ORDER BY expressions.

**Namespace:**&nbsp;<a href="N_ConnectQl_Interfaces">ConnectQl.Interfaces</a><br />**Assembly:**&nbsp;ConnectQl (in ConnectQl.dll) Version: 100.0.1-prerelease

## Syntax

**C#**<br />
``` C#
bool SupportsOrderBy(
	IEnumerable<IOrderByExpression> orderBy
)
```


#### Parameters
&nbsp;<dl><dt>orderBy</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/9eekhta0" target="_blank">System.Collections.Generic.IEnumerable</a>(<a href="T_ConnectQl_Interfaces_IOrderByExpression">IOrderByExpression</a>)<br />The expression to check.</dd></dl>

#### Return Value
Type: <a href="http://msdn2.microsoft.com/en-us/library/a28wyd50" target="_blank">Boolean</a><br />`true` if the expressions are supported, false otherwise.

## See Also


#### Reference
<a href="T_ConnectQl_Interfaces_IDataSourceOrderBySupport">IDataSourceOrderBySupport Interface</a><br /><a href="N_ConnectQl_Interfaces">ConnectQl.Interfaces Namespace</a><br />