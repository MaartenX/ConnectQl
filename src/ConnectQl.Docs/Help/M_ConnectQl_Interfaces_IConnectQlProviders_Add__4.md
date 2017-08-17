# IConnectQlProviders.Add(*TArgument1*, *TArgument2*, *TArgument3*, *TDataProvider*) Method (String, Expression(Func(*TArgument1*, *TArgument2*, *TArgument3*, *TDataProvider*)))
 

The add.

**Namespace:**&nbsp;<a href="N_ConnectQl_Interfaces">ConnectQl.Interfaces</a><br />**Assembly:**&nbsp;ConnectQl (in ConnectQl.dll) Version: 100.0.1-prerelease

## Syntax

**C#**<br />
``` C#
void Add<TArgument1, TArgument2, TArgument3, TDataProvider>(
	string key,
	Expression<Func<TArgument1, TArgument2, TArgument3, TDataProvider>> function
)
where TDataProvider : IDataAccess

```


#### Parameters
&nbsp;<dl><dt>key</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/s1wwdcbf" target="_blank">System.String</a><br />The key.</dd><dt>function</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/bb335710" target="_blank">System.Linq.Expressions.Expression</a>(<a href="http://msdn2.microsoft.com/en-us/library/bb549430" target="_blank">Func</a>(*TArgument1*, *TArgument2*, *TArgument3*, *TDataProvider*))<br />The function.</dd></dl>

#### Type Parameters
&nbsp;<dl><dt>TArgument1</dt><dd>The type of the first argument.</dd><dt>TArgument2</dt><dd>The type of the second argument.</dd><dt>TArgument3</dt><dd>The type of the third argument.</dd><dt>TDataProvider</dt><dd>The type of the data provider.</dd></dl>

## See Also


#### Reference
<a href="T_ConnectQl_Interfaces_IConnectQlProviders">IConnectQlProviders Interface</a><br /><a href="Overload_ConnectQl_Interfaces_IConnectQlProviders_Add">Add Overload</a><br /><a href="N_ConnectQl_Interfaces">ConnectQl.Interfaces Namespace</a><br />