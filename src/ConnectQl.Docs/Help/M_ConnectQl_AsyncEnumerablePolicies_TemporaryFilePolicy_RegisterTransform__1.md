# TemporaryFilePolicy.RegisterTransform(*T*) Method 
 

Registers a transform that turns items into serializable objects.

**Namespace:**&nbsp;<a href="N_ConnectQl_AsyncEnumerablePolicies">ConnectQl.AsyncEnumerablePolicies</a><br />**Assembly:**&nbsp;ConnectQl (in ConnectQl.dll) Version: 100.0.1-prerelease

## Syntax

**C#**<br />
``` C#
public TemporaryFilePolicy RegisterTransform<T>(
	ITransform<T> transform
)

```


#### Parameters
&nbsp;<dl><dt>transform</dt><dd>Type: <a href="T_ConnectQl_AsyncEnumerablePolicies_ITransform_1">ConnectQl.AsyncEnumerablePolicies.ITransform</a>(*T*)<br />The transform.</dd></dl>

#### Type Parameters
&nbsp;<dl><dt>T</dt><dd>The type of the items.</dd></dl>

#### Return Value
Type: <a href="T_ConnectQl_AsyncEnumerablePolicies_TemporaryFilePolicy">TemporaryFilePolicy</a><br />The <a href="T_ConnectQl_AsyncEnumerablePolicies_TemporaryFilePolicy">TemporaryFilePolicy</a>.

## See Also


#### Reference
<a href="T_ConnectQl_AsyncEnumerablePolicies_TemporaryFilePolicy">TemporaryFilePolicy Class</a><br /><a href="N_ConnectQl_AsyncEnumerablePolicies">ConnectQl.AsyncEnumerablePolicies Namespace</a><br />