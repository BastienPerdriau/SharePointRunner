#
# Register_Repository.ps1
#
Param(
    [string]
    $RepositoryName,
    [string]
    $RepositorySourceUri,
    [string]
    $RepositoryPublishUri,
    [string]
    $RepositoryUsername,
    [string]
    $RepositoryPwd
)

Import-Module PowerShellGet

$repo = Get-PSRepository -Name $RepositoryName -ErrorAction SilentlyContinue

if($repo -eq $null)
{
    nuget  sources add -name $RepositoryName -source $RepositorySourceUri -username $RepositoryUsername `
    -password $RepositoryPwd -storePasswordInClearText  -verbosity detailed

    $securePass = ConvertTo-SecureString -String $RepositoryPwd -AsPlainText -Force
    $cred = New-Object System.Management.Automation.PSCredential ($RepositoryUsername, $securePass)

    Write-Debug "Adding the Repository $RepositoryName"
    Register-PSRepository -Name $RepositoryName -SourceLocation $RepositorySourceUri `
                         -PublishLocation $RepositoryPublishUri -Credential $cred `
                         -PackageManagementProvider Nuget -InstallationPolicy Trusted
}
else
{
    Write-Debug "The repository $RepositoryName is already registered on this node. Skipped registration."
}I