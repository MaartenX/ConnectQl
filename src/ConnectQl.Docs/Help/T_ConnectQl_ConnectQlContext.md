# ConnectQlContext Class
 

The ConnectQl context.


## Inheritance Hierarchy
<a href="http://msdn2.microsoft.com/en-us/library/e5kfa45b" target="_blank">System.Object</a><br />&nbsp;&nbsp;ConnectQl.ConnectQlContext<br />
**Namespace:**&nbsp;<a href="N_ConnectQl">ConnectQl</a><br />**Assembly:**&nbsp;ConnectQl (in ConnectQl.dll) Version: 100.0.1-prerelease

## Syntax

**C#**<br />
``` C#
public class ConnectQlContext : IDisposable
```

The ConnectQlContext type exposes the following members.


## Constructors
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_ConnectQl_ConnectQlContext__ctor">ConnectQlContext()</a></td><td>
Initializes a new instance of the ConnectQlContext class.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_ConnectQl_ConnectQlContext__ctor_1">ConnectQlContext(IPluginResolver)</a></td><td>
Initializes a new instance of the ConnectQlContext class.</td></tr></table>&nbsp;
<a href="#connectqlcontext-class">Back to Top</a>

## Properties
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public property](media/pubproperty.gif "Public property")![Static member](media/static.gif "Static member")</td><td><a href="P_ConnectQl_ConnectQlContext_DefaultPluginResolver">DefaultPluginResolver</a></td><td>
Gets or sets the default plugin resolver.</td></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td><a href="P_ConnectQl_ConnectQlContext_JobRunner">JobRunner</a></td><td>
Gets or sets the job runner.</td></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td><a href="P_ConnectQl_ConnectQlContext_Log">Log</a></td><td>
Gets or sets the logger.</td></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td><a href="P_ConnectQl_ConnectQlContext_MaterializationPolicy">MaterializationPolicy</a></td><td>
Gets or sets the materialization policy.</td></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td><a href="P_ConnectQl_ConnectQlContext_PluginResolver">PluginResolver</a></td><td>
Gets or sets the plugin resolver.</td></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td><a href="P_ConnectQl_ConnectQlContext_UriResolver">UriResolver</a></td><td>
Gets or sets a lambda that opens the file at the specified path and returns the stream.</td></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td><a href="P_ConnectQl_ConnectQlContext_WriteProgressInterval">WriteProgressInterval</a></td><td>
Gets or sets the write progress interval. When this value is anything other than 0, progress is reported after this number of records.</td></tr></table>&nbsp;
<a href="#connectqlcontext-class">Back to Top</a>

## Methods
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td><a href="M_ConnectQl_ConnectQlContext_Dispose">Dispose</a></td><td>
Disposes the context.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="http://msdn2.microsoft.com/en-us/library/bsc2ak47" target="_blank">Equals</a></td><td>
Determines whether the specified object is equal to the current object.
 (Inherited from <a href="http://msdn2.microsoft.com/en-us/library/e5kfa45b" target="_blank">Object</a>.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_ConnectQl_ConnectQlContext_ExecuteAsync">ExecuteAsync(Stream)</a></td><td>
Executes the queries in the stream.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_ConnectQl_ConnectQlContext_ExecuteAsync_1">ExecuteAsync(String)</a></td><td>
Executes the query.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_ConnectQl_ConnectQlContext_ExecuteAsync_2">ExecuteAsync(String, Stream)</a></td><td>
Executes the queries in the stream.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_ConnectQl_ConnectQlContext_ExecuteFileAsync">ExecuteFileAsync</a></td><td>
Executes the queries in the stream.</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td><a href="http://msdn2.microsoft.com/en-us/library/4k87zsw7" target="_blank">Finalize</a></td><td>
Allows an object to try to free resources and perform other cleanup operations before it is reclaimed by garbage collection.
 (Inherited from <a href="http://msdn2.microsoft.com/en-us/library/e5kfa45b" target="_blank">Object</a>.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="http://msdn2.microsoft.com/en-us/library/zdee4b3y" target="_blank">GetHashCode</a></td><td>
Serves as the default hash function.
 (Inherited from <a href="http://msdn2.microsoft.com/en-us/library/e5kfa45b" target="_blank">Object</a>.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="http://msdn2.microsoft.com/en-us/library/dfwy45w9" target="_blank">GetType</a></td><td>
Gets the <a href="http://msdn2.microsoft.com/en-us/library/42892f65" target="_blank">Type</a> of the current instance.
 (Inherited from <a href="http://msdn2.microsoft.com/en-us/library/e5kfa45b" target="_blank">Object</a>.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td><a href="http://msdn2.microsoft.com/en-us/library/57ctke0a" target="_blank">MemberwiseClone</a></td><td>
Creates a shallow copy of the current <a href="http://msdn2.microsoft.com/en-us/library/e5kfa45b" target="_blank">Object</a>.
 (Inherited from <a href="http://msdn2.microsoft.com/en-us/library/e5kfa45b" target="_blank">Object</a>.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="http://msdn2.microsoft.com/en-us/library/7bxwbwt2" target="_blank">ToString</a></td><td>
Returns a string that represents the current object.
 (Inherited from <a href="http://msdn2.microsoft.com/en-us/library/e5kfa45b" target="_blank">Object</a>.)</td></tr></table>&nbsp;
<a href="#connectqlcontext-class">Back to Top</a>

## Extension Methods
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_ConnectQl_Intellisense_ConnectQlExtensions_CreateIntellisenseSession">CreateIntellisenseSession</a></td><td>
Creates the intellisense session.
 (Defined by <a href="T_ConnectQl_Intellisense_ConnectQlExtensions">ConnectQlExtensions</a>.)</td></tr></table>&nbsp;
<a href="#connectqlcontext-class">Back to Top</a>

## See Also


#### Reference
<a href="N_ConnectQl">ConnectQl Namespace</a><br />