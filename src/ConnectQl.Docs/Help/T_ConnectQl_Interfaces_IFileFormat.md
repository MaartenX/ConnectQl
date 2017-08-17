# IFileFormat Interface
 

The FileAccess interface.

**Namespace:**&nbsp;<a href="N_ConnectQl_Interfaces">ConnectQl.Interfaces</a><br />**Assembly:**&nbsp;ConnectQl (in ConnectQl.dll) Version: 100.0.1-prerelease

## Syntax

**C#**<br />
``` C#
public interface IFileFormat : IFileReader, 
	IFileAccess, IFileWriter
```

The IFileFormat type exposes the following members.


## Properties
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td><a href="P_ConnectQl_Interfaces_IFileWriter_ShouldMaterialize">ShouldMaterialize</a></td><td>
Gets a value indicating whether the collection of rows should be materialized when calling this writer. When all columns are needed in the header (e.g. for CSV or Excel), you should return `true` here. Other formats that use the columns per object (like JSON) can return `false`.
 (Inherited from <a href="T_ConnectQl_Interfaces_IFileWriter">IFileWriter</a>.)</td></tr></table>&nbsp;
<a href="#ifileformat-interface">Back to Top</a>

## Methods
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_ConnectQl_Interfaces_IFileReader_CanReadThisFile">CanReadThisFile</a></td><td>
Checks if the file reader can read this file.
 (Inherited from <a href="T_ConnectQl_Interfaces_IFileReader">IFileReader</a>.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_ConnectQl_Interfaces_IFileWriter_CanWriteThisFile">CanWriteThisFile</a></td><td>
Checks if the file writer can write this file.
 (Inherited from <a href="T_ConnectQl_Interfaces_IFileWriter">IFileWriter</a>.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_ConnectQl_Interfaces_IFileReader_Read">Read</a></td><td>
Reads objects from the stream.
 (Inherited from <a href="T_ConnectQl_Interfaces_IFileReader">IFileReader</a>.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_ConnectQl_Interfaces_IFileWriter_WriteFooter">WriteFooter</a></td><td>
Writes the footer to the file.
 (Inherited from <a href="T_ConnectQl_Interfaces_IFileWriter">IFileWriter</a>.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_ConnectQl_Interfaces_IFileWriter_WriteHeader">WriteHeader</a></td><td>
Writes the header to the file.
 (Inherited from <a href="T_ConnectQl_Interfaces_IFileWriter">IFileWriter</a>.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_ConnectQl_Interfaces_IFileWriter_WriteRows">WriteRows</a></td><td>
Writes rows to the file.
 (Inherited from <a href="T_ConnectQl_Interfaces_IFileWriter">IFileWriter</a>.)</td></tr></table>&nbsp;
<a href="#ifileformat-interface">Back to Top</a>

## See Also


#### Reference
<a href="N_ConnectQl_Interfaces">ConnectQl.Interfaces Namespace</a><br />