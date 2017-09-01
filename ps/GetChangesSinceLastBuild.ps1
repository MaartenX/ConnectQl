# Gets all projects that had changes since the last successful build.
# When appveyor.yml is changed, all project files are returned.

param(
    # The API token to retrieve the last successful build from Appveyor.
    [string] $ApiToken
)

$projects = ls *.csproj -r

# Files that will trigger a complete rebuild. All other files will check if they are in a project folder.
$filesForRebuildAll = "appveyor.yml", "ConnectQl.Defaults.targets", "GetChangesSinceLastBuild.ps1"

if ("$apiToken" -eq "")
{
	$result = $projects | ? { $_ -eq "asdf" }
}
else 
{
	$api = "https://ci.appveyor.com/api/projects/$($Env:APPVEYOR_ACCOUNT_NAME)/$($Env:APPVEYOR_PROJECT_SLUG)/history?recordsNumber=5000"
	$headers = @{ "Authorization" = "Bearer $apiToken"; "Content-Type" = "application/json" };
	$slash = "$([IO.Path]::DirectorySeparatorChar)";

	$json = Invoke-RestMethod -Uri $api -Headers $headers
	$lastBuildCommitId = ($json.builds | ? { $_.status -eq 'success' -and $_.branch -eq $env:APPVEYOR_REPO_BRANCH } | select -First 1).commitId
	$changedFiles = (git diff-tree -r --name-only --no-commit-id $lastBuildCommitId HEAD | Get-Item -ErrorAction Ignore)
	$changedFolders = $changedFiles.Directory.FullName | % { $_ + $slash }
    $rebuildAll = ($changedFiles.Name | ? { $filesForRebuildAll -contains $_ }).Count -gt 0

	if ($rebuildAll) {
		$result = $projects
	}
	else {
		$result = $projects | ? { $project = $_; ($changedFolders | ? { $_.StartsWith($project.Directory.FullName + $slash) }).Count -gt 0 }
	}
}

if ($result.Count -eq 0) {
    Write-Host "Not packaging any projects."
}
else {
    Write-Host "Packaging projects:"
    $result.Name | % { Write-Host $_ }
}

return $result