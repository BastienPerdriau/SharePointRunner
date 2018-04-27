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

Publish-Module -Name PackageName -Repository $RepositoryName -NuGetApiKey $RepositoryApiKey