# Row.RowImplementation(*T*) Class
 

The row implementation.


## Inheritance Hierarchy
<a href="http://msdn2.microsoft.com/en-us/library/e5kfa45b" target="_blank">System.Object</a><br />&nbsp;&nbsp;<a href="T_ConnectQl_Results_Row">ConnectQl.Results.Row</a><br />&nbsp;&nbsp;&nbsp;&nbsp;ConnectQl.Results.Row.RowImplementation(T)<br />
**Namespace:**&nbsp;<a href="N_ConnectQl_Results">ConnectQl.Results</a><br />**Assembly:**&nbsp;ConnectQl (in ConnectQl.dll) Version: 100.0.1-prerelease

## Syntax

**C#**<br />
``` C#
protected class RowImplementation<T> : Row

```


#### Type Parameters
&nbsp;<dl><dt>T</dt><dd>The type of the unique id.</dd></dl>&nbsp;
The Row.RowImplementation(T) type exposes the following members.


## Properties
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td><a href="P_ConnectQl_Results_Row_ColumnNames">ColumnNames</a></td><td>
Gets the column names.
 (Inherited from <a href="T_ConnectQl_Results_Row">Row</a>.)</td></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td><a href="P_ConnectQl_Results_Row_RowImplementation_1_Id">Id</a></td><td>
Gets the unique id.</td></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td><a href="P_ConnectQl_Results_Row_Item">Item</a></td><td>
Gets or sets the values for the specified field name.
 (Inherited from <a href="T_ConnectQl_Results_Row">Row</a>.)</td></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td><a href="P_ConnectQl_Results_Row_RowImplementation_1_UniqueId">UniqueId</a></td><td>
Gets the unique id for the row.
 (Overrides <a href="P_ConnectQl_Results_Row_UniqueId">Row.UniqueId</a>.)</td></tr><tr><td>![Protected property](media/protproperty.gif "Protected property")</td><td><a href="P_ConnectQl_Results_Row_Values">Values</a></td><td>
Gets the values for debugging purposes.
 (Inherited from <a href="T_ConnectQl_Results_Row">Row</a>.)</td></tr></table>&nbsp;
<a href="#row.rowimplementation(*t*)-class">Back to Top</a>

## Methods
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td><a href="M_ConnectQl_Results_Row_CombineFrom__1">CombineFrom(TOther)(IRowBuilder, Row.RowImplementation(TOther))</a></td><td>
When implemented in a derived class, joins the other row to the current row.
 (Inherited from <a href="T_ConnectQl_Results_Row">Row</a>.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td><a href="M_ConnectQl_Results_Row_RowImplementation_1_CombineFrom__1">CombineFrom(TOther)(IRowBuilder, Row.RowImplementation(TOther))</a></td><td>
Joins the other row to the current row.
 (Overrides <a href="M_ConnectQl_Results_Row_CombineFrom__1">Row.CombineFrom(TOther)(IRowBuilder, Row.RowImplementation(TOther))</a>.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="http://msdn2.microsoft.com/en-us/library/bsc2ak47" target="_blank">Equals</a></td><td>
Determines whether the specified object is equal to the current object.
 (Inherited from <a href="http://msdn2.microsoft.com/en-us/library/e5kfa45b" target="_blank">Object</a>.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td><a href="http://msdn2.microsoft.com/en-us/library/4k87zsw7" target="_blank">Finalize</a></td><td>
Allows an object to try to free resources and perform other cleanup operations before it is reclaimed by garbage collection.
 (Inherited from <a href="http://msdn2.microsoft.com/en-us/library/e5kfa45b" target="_blank">Object</a>.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="http://msdn2.microsoft.com/en-us/library/zdee4b3y" target="_blank">GetHashCode</a></td><td>
Serves as the default hash function.
 (Inherited from <a href="http://msdn2.microsoft.com/en-us/library/e5kfa45b" target="_blank">Object</a>.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="http://msdn2.microsoft.com/en-us/library/dfwy45w9" target="_blank">GetType</a></td><td>
Gets the <a href="http://msdn2.microsoft.com/en-us/library/42892f65" target="_blank">Type</a> of the current instance.
 (Inherited from <a href="http://msdn2.microsoft.com/en-us/library/e5kfa45b" target="_blank">Object</a>.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td><a href="http://msdn2.microsoft.com/en-us/library/57ctke0a" target="_blank">MemberwiseClone</a></td><td>
Creates a shallow copy of the current <a href="http://msdn2.microsoft.com/en-us/library/e5kfa45b" target="_blank">Object</a>.
 (Inherited from <a href="http://msdn2.microsoft.com/en-us/library/e5kfa45b" target="_blank">Object</a>.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_ConnectQl_Results_Row_ToDictionary">ToDictionary</a></td><td>
Converts the row to a dictionary.
 (Inherited from <a href="T_ConnectQl_Results_Row">Row</a>.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_ConnectQl_Results_Row_RowImplementation_1_ToString">ToString</a></td><td>
The to string.
 (Overrides <a href="http://msdn2.microsoft.com/en-us/library/7bxwbwt2" target="_blank">Object.ToString()</a>.)</td></tr></table>&nbsp;
<a href="#row.rowimplementation(*t*)-class">Back to Top</a>

## See Also


#### Reference
<a href="N_ConnectQl_Results">ConnectQl.Results Namespace</a><br />