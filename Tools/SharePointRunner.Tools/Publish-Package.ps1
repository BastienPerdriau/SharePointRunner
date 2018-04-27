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
Write-Output "RepositoryName $RepositoryName"
Write-Output "RepositoryApiKey $RepositoryApiKey"
Write-Output "PackageName $PackageName"
Get-ChildItem -Recurse
# Publish-Module -Name $PackageName -Repository $RepositoryName -NuGetApiKey $RepositoryApiKey
Publish-Module -Path $PackageName -Repository $RepositoryName -NuGetApiKey $RepositoryApiKey
Write-Output "Debug 1"