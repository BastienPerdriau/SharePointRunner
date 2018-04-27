#
# Unregister_Repository.ps1
#
Param(
    [string]
    $RepositoryName
)
Import-Module PowerShellGet

$repo = Get-PSRepository -Name $RepositoryName -ErrorAction SilentlyContinue

if($repo -ne $null)
{
    nuget sources remove -name $RepositoryName -verbosity detailed
    Write-Debug "Removing the Repository $RepositoryName"
    Unregister-PSRepository -Name $RepositoryName
}
else
{
    Write-Debug "The repository $RepositoryName does not exist. Skipped removal."
}