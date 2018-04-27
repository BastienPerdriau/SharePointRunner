#
# Unregister_Repository.ps1
#
Param(
    [string]
    $RepositoryName
)
Import-Module PowerShellGet
Write-Output "Debug 0"

$repo = Get-PSRepository -Name $RepositoryName -ErrorAction SilentlyContinue
Write-Output "Debug 1"

if($repo -ne $null)
{
    Write-Output "Debug 2"
    nuget sources remove -name $RepositoryName -verbosity detailed
    Write-Output "Debug 3"
    Write-Debug "Removing the Repository $RepositoryName"
    Unregister-PSRepository -Name $RepositoryName
    Write-Output "Debug 4"
}
else
{
    Write-Debug "The repository $RepositoryName does not exist. Skipped removal."
    Write-Output "Debug 5"
}
Write-Output "Debug 6"