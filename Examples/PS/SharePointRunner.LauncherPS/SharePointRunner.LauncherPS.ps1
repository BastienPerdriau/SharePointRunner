#
# Script.ps1
#

# TODO Install-Module from PowerShell Gallery or at least from a private PowerShell repos

Import-Module -Name ..\..\..\Sources\SharePointRunner.PowerShell\bin\Debug\SharePointRunner.PowerShell.psd1 -PassThru

$xmlConfigFilePath = "D:\Dev\Perso\SharePointRunner\Examples\PS\SharePointRunner.LauncherPS\ConfigFiles\ConfigFile.xml";
$jsonConfigFilePath = "D:\Dev\Perso\SharePointRunner\Examples\PS\SharePointRunner.LauncherPS\ConfigFiles\ConfigFile.json";

# TODO Make it work with relative paths
Start-SharePointRunner -ConfigFilePath $xmlConfigFilePath