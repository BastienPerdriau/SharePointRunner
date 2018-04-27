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
Write-Output "Debug 0"
Write-Output "RepositoryName $RepositoryName"
Write-Output "RepositorySourceUri $RepositorySourceUri"
Write-Output "RepositoryPublishUri $RepositoryPublishUri"
Write-Output "RepositoryUsername $RepositoryUsername"
Write-Output "RepositoryPwd $RepositoryPwd"
Get-PSRepository
$repo = Get-PSRepository -Name $RepositoryName -ErrorAction SilentlyContinue
Write-Output "Debug 1"

if($repo -eq $null)
{
    Write-Output "Debug 2"
    nuget sources add -name $RepositoryName -source $RepositorySourceUri -username $RepositoryUsername -password $RepositoryPwd -storePasswordInClearText -verbosity detailed
    Write-Output "Debug 3"

    $securePass = ConvertTo-SecureString -String $RepositoryPwd -AsPlainText -Force
    $cred = New-Object System.Management.Automation.PSCredential ($RepositoryUsername, $securePass)
    Write-Output "Debug 4"

    Write-Debug "Adding the Repository $RepositoryName"
    Register-PSRepository -Name $RepositoryName -SourceLocation $RepositorySourceUri -PublishLocation $RepositoryPublishUri -Credential $cred -PackageManagementProvider Nuget -InstallationPolicy Trusted
    Write-Output "Debug 5"
}
else
{
    Write-Output "Debug 6"
    Write-Debug "The repository $RepositoryName is already registered on this node. Skipped registration."
}
Write-Output "Debug 7"