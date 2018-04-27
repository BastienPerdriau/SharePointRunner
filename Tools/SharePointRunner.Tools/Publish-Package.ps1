#
# Publish_Package.ps1
#
Param(
    [string]
    $RepositoryName,
    [string]
    $RepositoryApiKey,
    [string]
    $PackageName
)

Import-Module PowerShellGet
Write-Output "Debug 0"

Publish-Module -Name PackageName -Repository $RepositoryName -NuGetApiKey $RepositoryApiKey
Write-Output "Debug 1"