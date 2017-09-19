# Updates all referenced packages that are not packed in this build.
# Looks up the last version in the feed and replaces it with that.

param(
    # The projects that were packed by msbuild.
    [Object[]] $PackProjects
)

$projects = ls *.csproj -r

$replaceVersions = ($projects | ?{ $PackProjects.BaseName -notcontains $_.BaseName }).basename | % {
		try { 
			@{ Id=$_; Version=((irm "https://www.myget.org/F/connectql/api/v3/flatcontainer/$($_.tolower())/index.json").versions | select -last 1) } 
		} catch {} 
	}

$nugets = ls *.nupkg.zip -r

[Reflection.Assembly]::LoadWithPartialName( "System.IO" ) | Out-Null
[Reflection.Assembly]::LoadWithPartialName( "System.IO.Compression.FileSystem" ) | Out-Null

$ns = @{ p = "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd" }

$nugets | 
	%{ 
		($archive = [System.IO.Compression.ZipFile]::Open($_,"Update")).Entries | 
			?{ $_.FullName.EndsWith(".nuspec") } | 
			%{
				$xml = [Xml](($r = [System.IO.StreamReader]$_.Open()).ReadToEnd()); $r.Dispose() 

				$replaceVersions |
					%{ 
						$ver = $_.Version; 
						Select-Xml -Xml $xml -XPath "//p:package/p:metadata/p:dependencies/p:group/p:dependency[@id='$($_.Id)']" -Namespace $ns | %{ $_.Node.SetAttribute("version", $ver) }
					}

				($s = $_.Open()).SetLength(0); $xml.Save($s); $s.Dispose()
			}
		$archive.Dispose()
	}