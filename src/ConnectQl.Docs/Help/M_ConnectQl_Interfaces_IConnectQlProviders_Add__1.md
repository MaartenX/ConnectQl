# IConnectQlProviders.Add(*TDataProvider*) Method (String, Expression(Func(*TDataProvider*)))
 

The add.

**Namespace:**&nbsp;<a href="N_ConnectQl_Interfaces">ConnectQl.Interfaces</a><br />**Assembly:**&nbsp;ConnectQl (in ConnectQl.dll) Version: 100.0.1-prerelease

## Syntax

**C#**<br />
``` C#
void Add<TDataProvider>(
	string key,
	Expression<Func<TDataProvider>> function
)
where TDataProvider : IDataAccess

```


#### Parameters
&nbsp;<dl><dt>key</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/s1wwdcbf" target="_blank">System.String</a><br />The key.</dd><dt>function</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/bb335710" target="_blank">System.Linq.Expressions.Expression</a>(<a href="http://msdn2.microsoft.com/en-us/library/bb534960" target="_blank">Func</a>(*TDataProvider*))<br />The function.</dd></dl>

#### Type Parameters
&nbsp;<dl><dt>TDataProvider</dt><dd>The type of the data provider.</dd></dl>

## See Also


#### Reference
<a href="T_ConnectQl_Interfaces_IConnectQlProviders">IConnectQlProviders Interface</a><br /><a href="Overload_ConnectQl_Interfaces_IConnectQlProviders_Add">Add Overload</a><br /><a href="N_ConnectQl_Interfaces">ConnectQl.Interfaces Namespace</a><br />