#
# Script.ps1
#

# TODO Install-Module from PowerShell Gallery or at least from a private PowerShell repos

Install-Module -Name ..\..\..\Sources\SharePointRunner.PowerShell\SharePointRunner.PowerShell.psd1

$xmlConfigFilePath = "ConfigFiles/ConfigFile.xml";
$jsonConfigFilePath = "ConfigFiles/ConfigFile.json";

Start-SharePointRunner -ConfigFilePath $xmlConfigFilePath