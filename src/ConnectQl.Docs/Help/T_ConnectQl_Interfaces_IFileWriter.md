# IFileWriter Interface
 

The FileWriter interface.

**Namespace:**&nbsp;<a href="N_ConnectQl_Interfaces">ConnectQl.Interfaces</a><br />**Assembly:**&nbsp;ConnectQl (in ConnectQl.dll) Version: 100.0.1-prerelease

## Syntax

**C#**<br />
``` C#
public interface IFileWriter : IFileAccess
```

The IFileWriter type exposes the following members.


## Properties
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td><a href="P_ConnectQl_Interfaces_IFileWriter_ShouldMaterialize">ShouldMaterialize</a></td><td>
Gets a value indicating whether the collection of rows should be materialized when calling this writer. When all columns are needed in the header (e.g. for CSV or Excel), you should return `true` here. Other formats that use the columns per object (like JSON) can return `false`.</td></tr></table>&nbsp;
<a href="#ifilewriter-interface">Back to Top</a>

## Methods
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_ConnectQl_Interfaces_IFileWriter_CanWriteThisFile">CanWriteThisFile</a></td><td>
Checks if the file writer can write this file.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_ConnectQl_Interfaces_IFileWriter_WriteFooter">WriteFooter</a></td><td>
Writes the footer to the file.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_ConnectQl_Interfaces_IFileWriter_WriteHeader">WriteHeader</a></td><td>
Writes the header to the file.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_ConnectQl_Interfaces_IFileWriter_WriteRows">WriteRows</a></td><td>
Writes rows to the file.</td></tr></table>&nbsp;
<a href="#ifilewriter-interface">Back to Top</a>

## See Also


#### Reference
<a href="N_ConnectQl_Interfaces">ConnectQl.Interfaces Namespace</a><br />