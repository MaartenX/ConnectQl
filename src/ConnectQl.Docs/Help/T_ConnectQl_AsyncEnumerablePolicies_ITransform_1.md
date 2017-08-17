# ITransform(*T*) Interface
 

Allows transforming non-serializable objects to something that can be serialized.

**Namespace:**&nbsp;<a href="N_ConnectQl_AsyncEnumerablePolicies">ConnectQl.AsyncEnumerablePolicies</a><br />**Assembly:**&nbsp;ConnectQl (in ConnectQl.dll) Version: 100.0.1-prerelease

## Syntax

**C#**<br />
``` C#
public interface ITransform<T>

```


#### Type Parameters
&nbsp;<dl><dt>T</dt><dd>The type of the items to transform.</dd></dl>&nbsp;
The ITransform(T) type exposes the following members.


## Properties
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td><a href="P_ConnectQl_AsyncEnumerablePolicies_ITransform_1_TargetType">TargetType</a></td><td>
Gets the target type values will be transformed to.</td></tr></table>&nbsp;
<a href="#itransform(*t*)-interface">Back to Top</a>

## Methods
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_ConnectQl_AsyncEnumerablePolicies_ITransform_1_CreateContext">CreateContext</a></td><td>
Creates a transformation context. This context will be used in all calls to Serialize and Deserialize.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_ConnectQl_AsyncEnumerablePolicies_ITransform_1_Deserialize">Deserialize</a></td><td>
Transforms a serializable object to a value.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_ConnectQl_AsyncEnumerablePolicies_ITransform_1_Serialize">Serialize</a></td><td>
Transforms the value to the serializable object.</td></tr></table>&nbsp;
<a href="#itransform(*t*)-interface">Back to Top</a>

## See Also


#### Reference
<a href="N_ConnectQl_AsyncEnumerablePolicies">ConnectQl.AsyncEnumerablePolicies Namespace</a><br />