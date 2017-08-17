# IConnectQlProviders.Add(*TArgument*, *TDataProvider*) Method (String, Expression(Func(*TArgument*, *TDataProvider*)))
 

The add.

**Namespace:**&nbsp;<a href="N_ConnectQl_Interfaces">ConnectQl.Interfaces</a><br />**Assembly:**&nbsp;ConnectQl (in ConnectQl.dll) Version: 100.0.1-prerelease

## Syntax

**C#**<br />
``` C#
void Add<TArgument, TDataProvider>(
	string key,
	Expression<Func<TArgument, TDataProvider>> function
)
where TDataProvider : IDataAccess

```


#### Parameters
&nbsp;<dl><dt>key</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/s1wwdcbf" target="_blank">System.String</a><br />The key.</dd><dt>function</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/bb335710" target="_blank">System.Linq.Expressions.Expression</a>(<a href="http://msdn2.microsoft.com/en-us/library/bb549151" target="_blank">Func</a>(*TArgument*, *TDataProvider*))<br />The function.</dd></dl>

#### Type Parameters
&nbsp;<dl><dt>TArgument</dt><dd>The type of the argument.</dd><dt>TDataProvider</dt><dd>The type of the data provider.</dd></dl>

## See Also


#### Reference
<a href="T_ConnectQl_Interfaces_IConnectQlProviders">IConnectQlProviders Interface</a><br /><a href="Overload_ConnectQl_Interfaces_IConnectQlProviders_Add">Add Overload</a><br /><a href="N_ConnectQl_Interfaces">ConnectQl.Interfaces Namespace</a><br />