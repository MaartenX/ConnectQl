# ITriggerContext.GetLastExecutionTime Method 
 

Gets the date and time this job was executed the last time.

**Namespace:**&nbsp;<a href="N_ConnectQl_Interfaces">ConnectQl.Interfaces</a><br />**Assembly:**&nbsp;ConnectQl (in ConnectQl.dll) Version: 100.0.1-prerelease

## Syntax

**C#**<br />
``` C#
Nullable<DateTime> GetLastExecutionTime(
	string jobName
)
```


#### Parameters
&nbsp;<dl><dt>jobName</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/s1wwdcbf" target="_blank">System.String</a><br />The name of the job.</dd></dl>

#### Return Value
Type: <a href="http://msdn2.microsoft.com/en-us/library/b3h38hb0" target="_blank">Nullable</a>(<a href="http://msdn2.microsoft.com/en-us/library/03ybds8y" target="_blank">DateTime</a>)<br />The date and time this job was executed, or `null` if the job was never executed.

## See Also


#### Reference
<a href="T_ConnectQl_Interfaces_ITriggerContext">ITriggerContext Interface</a><br /><a href="N_ConnectQl_Interfaces">ConnectQl.Interfaces Namespace</a><br />