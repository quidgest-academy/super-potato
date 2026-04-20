#Requires -RunAsAdministrator
Import-Module WebAdministration

$pubdir = Resolve-Path .\publish
$appname = "FOR_MYAPP"
New-WebAppPool -Name $appname
Set-ItemProperty -Path "IIS:\AppPools\$appname" managedRuntimeVersion ""
New-WebApplication -Name $appname -Site "Default Web Site" -PhysicalPath $pubdir -ApplicationPool $appname