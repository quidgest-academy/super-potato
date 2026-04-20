# Assumes a clean installation of Windows Server and a Administration mode Powershell
#base: en-us_windows_server_2022_updated_march_2023_x64_dvd_dd2f76bb.iso

#---------------------------------
function QuidgestIISProvisioning {
    [CmdletBinding()]
    param()

    Install-WindowsFeature -name Web-Server -IncludeManagementTools

    Enable-WindowsOptionalFeature -Online -FeatureName IIS-ApplicationDevelopment -NoRestart
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-BasicAuthentication -NoRestart
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-CommonHttpFeatures -NoRestart
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-CustomLogging -NoRestart
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-DefaultDocument -NoRestart
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-DigestAuthentication -NoRestart
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-DirectoryBrowsing -NoRestart
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-HealthAndDiagnostics -NoRestart
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-HttpCompressionDynamic -NoRestart
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-HttpCompressionStatic -NoRestart
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-HttpErrors -NoRestart
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-HttpLogging -NoRestart
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-HttpRedirect -NoRestart
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-HttpTracing -NoRestart
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-IIS6ManagementCompatibility -NoRestart
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-IPSecurity -NoRestart
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-ISAPIExtensions -NoRestart
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-ISAPIFilter -NoRestart
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-LoggingLibraries -NoRestart
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-ManagementConsole -NoRestart
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-ManagementScriptingTools -NoRestart
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-Metabase -NoRestart
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-ODBCLogging -NoRestart
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-Performance -NoRestart
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-RequestFiltering -NoRestart
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-RequestMonitor -NoRestart
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-Security -NoRestart
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-StaticContent -NoRestart
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-URLAuthorization -NoRestart
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-WebServer -NoRestart
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-WebServerManagementTools -NoRestart
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-WebServerRole -NoRestart
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-WindowsAuthentication -NoRestart
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-WMICompatibility -NoRestart
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-ManagementService -NoRestart
}
#---------------------------------

#IIS Provisioning
QuidgestIISProvisioning

# Create the user for special permissions
$password = ConvertTo-SecureString "Zph2labid123!" -AsPlainText -Force
New-LocalUser -Name "DcomUser" -Password $password -FullName "DcomUser" -Description "Dcom user" -PasswordNeverExpires
Add-LocalGroupMember -Group "Administrators" -Member "DcomUser"

# If you don't want to add DcomUser to Administrators you will need to add it manually in the Dcom configuration
# open Dcom using command:
# mmc comexp.msc /32
# Then go to Component Services > Computers > My Computers
# Right click to open "Properties"
# Go to tab "Com Security"
# In BOTH buttons "Edit default..." add the DcomUser and give it Access Launch and Activation permissions

# Powershell alternative to be tested:
# https://github.com/matherm-aboehm/DCOMPermissions

# Create the application pool and webapp
New-WebAppPool -Name "ConvertServiceAppPool"
Set-ItemProperty IIS:\AppPools\ConvertServiceAppPool -name processModel -value @{userName="DcomUser";password="Zph2labid123!";identitytype=3}

New-Item -ItemType Directory -Force -Path "C:\quidgest\ConvertService"
icacls "C:\quidgest\ConvertService" /inheritance:d /grant:r "DcomUser:(OI)(CI)F" /T

New-WebApplication -Name "cs" -Site "Default Web Site" -PhysicalPath "C:\quidgest\ConvertService" -ApplicationPool "ConvertServiceAppPool"

Copy-Item -Force -Recurse ".\publish" -Destination "C:\quidgest\ConvertService"

# Install Office using Office Deployment Tool (ODT)
curl -o officedeploymenttool_16731-20398.exe https://download.microsoft.com/download/2/7/A/27AF1BE6-DD20-4CB4-B154-EBAB8A7D4A7E/officedeploymenttool_16731-20398.exe
.\officedeploymenttool_16731-20398.exe /quiet /extract:".\Odt"
.\Odt\setup.exe /configure Office2019Configuration.xml

# Alternative configuration can be created using Office Customization Tool
# https://config.office.com/deploymentsettings

# Some arcane requirements (https://stackoverflow.com/questions/7106381/microsoft-office-excel-cannot-access-the-file-c-inetpub-wwwroot-timesheet-app)
New-Item C:\Windows\SysWOW64\config\systemprofile\Desktop -ItemType "directory"
New-Item C:\Windows\System32\config\systemprofile\Desktop -ItemType "directory"


# After instalation the DcomUser must manually login to the system, create a new word document and export it to pdf