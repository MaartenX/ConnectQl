# Gets all projects that had changes since the last successful build.
# When appveyor.yml is changed, all project files are returned.

param(
    # The API token to retrieve the last successful build from Appveyor.
    [string] $ApiToken
)

$api = "https://ci.appveyor.com/api/projects/$($Env:APPVEYOR_ACCOUNT_NAME)/$($Env:APPVEYOR_PROJECT_SLUG)/history?recordsNumber=5000"
$headers = @{ "Authorization" = "Bearer $apiToken"; "Content-Type" = "application/json" };
$slash = "$([IO.Path]::DirectorySeparatorChar)";

$json = Invoke-RestMethod -Uri $api -Headers $headers
$lastBuildCommitId = ($json.builds | ? { $_.status -eq 'success' -and $_.branch -eq $env:APPVEYOR_REPO_BRANCH } | select -First 1).commitId
$changedFiles = (git diff-tree -r --name-only --no-commit-id $lastBuildCommitId HEAD | Get-Item)
$changedFolders = $changedFiles.Directory.FullName | % { $_ + $slash }
$projects = ls *.csproj -r

if ($changedFiles.Name -contains "appveyor.yml" -or $changedFiles.Name -contains "src\ConnectQl.Defaults.targets") {
    $projects
}
else {
    $projects | ? { $project = $_; ($changedFolders | ? { $_.StartsWith($project.Directory.FullName + $slash) }).Count -gt 0 }
}
