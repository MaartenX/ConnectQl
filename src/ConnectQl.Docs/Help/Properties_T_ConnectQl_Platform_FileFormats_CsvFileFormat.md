# CsvFileFormat Properties
 

The <a href="T_ConnectQl_Platform_FileFormats_CsvFileFormat">CsvFileFormat</a> type exposes the following members.


## Properties
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td><a href="P_ConnectQl_Platform_FileFormats_CsvFileFormat_ShouldMaterialize">ShouldMaterialize</a></td><td>
Gets a value indicating whether the collection of rows should be materialized when calling this writer. When all columns are needed in the header (e.g. for CSV or Excel), you should return `true` here. Other formats that use the columns per object (like JSON) can return `false`.</td></tr></table>&nbsp;
<a href="#csvfileformat-properties">Back to Top</a>

## See Also


#### Reference
<a href="T_ConnectQl_Platform_FileFormats_CsvFileFormat">CsvFileFormat Class</a><br /><a href="N_ConnectQl_Platform_FileFormats">ConnectQl.Platform.FileFormats Namespace</a><br />