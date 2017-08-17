# JsonFileFormat Class
 

The JSON file reader.


## Inheritance Hierarchy
<a href="http://msdn2.microsoft.com/en-us/library/e5kfa45b" target="_blank">System.Object</a><br />&nbsp;&nbsp;ConnectQl.FileFormats.JsonFileFormat<br />
**Namespace:**&nbsp;<a href="N_ConnectQl_FileFormats">ConnectQl.FileFormats</a><br />**Assembly:**&nbsp;ConnectQl.Platform (in ConnectQl.Platform.dll) Version: 100.0.1-prerelease

## Syntax

**C#**<br />
``` C#
public class JsonFileFormat : IFileFormat, 
	IFileReader, IFileAccess, IFileWriter, IDescriptableFileFormat
```

The JsonFileFormat type exposes the following members.


## Constructors
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_ConnectQl_FileFormats_JsonFileFormat__ctor">JsonFileFormat</a></td><td>
Initializes a new instance of the JsonFileFormat class</td></tr></table>&nbsp;
<a href="#jsonfileformat-class">Back to Top</a>

## Properties
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td><a href="P_ConnectQl_FileFormats_JsonFileFormat_ShouldMaterialize">ShouldMaterialize</a></td><td>
Gets a value indicating whether the collection of rows should be materialized when calling this writer. When all columns are needed in the header (e.g. for CSV or Excel), you should return `true` here. Other formats that use the columns per object (like JSON) can return `false`.</td></tr></table>&nbsp;
<a href="#jsonfileformat-class">Back to Top</a>

## Methods
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_ConnectQl_FileFormats_JsonFileFormat_CanReadThisFile">CanReadThisFile</a></td><td>
Checks if the file reader can read this file.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_ConnectQl_FileFormats_JsonFileFormat_CanWriteThisFile">CanWriteThisFile</a></td><td>
Checks if the file writer can write this file.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="http://msdn2.microsoft.com/en-us/library/bsc2ak47" target="_blank">Equals</a></td><td>
Determines whether the specified object is equal to the current object.
 (Inherited from <a href="http://msdn2.microsoft.com/en-us/library/e5kfa45b" target="_blank">Object</a>.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td><a href="http://msdn2.microsoft.com/en-us/library/4k87zsw7" target="_blank">Finalize</a></td><td>
Allows an object to try to free resources and perform other cleanup operations before it is reclaimed by garbage collection.
 (Inherited from <a href="http://msdn2.microsoft.com/en-us/library/e5kfa45b" target="_blank">Object</a>.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_ConnectQl_FileFormats_JsonFileFormat_GetDataSourceDescriptorAsync">GetDataSourceDescriptorAsync</a></td><td /></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="http://msdn2.microsoft.com/en-us/library/zdee4b3y" target="_blank">GetHashCode</a></td><td>
Serves as the default hash function.
 (Inherited from <a href="http://msdn2.microsoft.com/en-us/library/e5kfa45b" target="_blank">Object</a>.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="http://msdn2.microsoft.com/en-us/library/dfwy45w9" target="_blank">GetType</a></td><td>
Gets the <a href="http://msdn2.microsoft.com/en-us/library/42892f65" target="_blank">Type</a> of the current instance.
 (Inherited from <a href="http://msdn2.microsoft.com/en-us/library/e5kfa45b" target="_blank">Object</a>.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td><a href="http://msdn2.microsoft.com/en-us/library/57ctke0a" target="_blank">MemberwiseClone</a></td><td>
Creates a shallow copy of the current <a href="http://msdn2.microsoft.com/en-us/library/e5kfa45b" target="_blank">Object</a>.
 (Inherited from <a href="http://msdn2.microsoft.com/en-us/library/e5kfa45b" target="_blank">Object</a>.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_ConnectQl_FileFormats_JsonFileFormat_Read">Read</a></td><td /></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="http://msdn2.microsoft.com/en-us/library/7bxwbwt2" target="_blank">ToString</a></td><td>
Returns a string that represents the current object.
 (Inherited from <a href="http://msdn2.microsoft.com/en-us/library/e5kfa45b" target="_blank">Object</a>.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_ConnectQl_FileFormats_JsonFileFormat_WriteFooter">WriteFooter</a></td><td>
Writes the footer to the file.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_ConnectQl_FileFormats_JsonFileFormat_WriteHeader">WriteHeader</a></td><td>
Writes the header to the file.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_ConnectQl_FileFormats_JsonFileFormat_WriteRows">WriteRows</a></td><td>
Writes rows to the file.</td></tr></table>&nbsp;
<a href="#jsonfileformat-class">Back to Top</a>

## See Also


#### Reference
<a href="N_ConnectQl_FileFormats">ConnectQl.FileFormats Namespace</a><br />