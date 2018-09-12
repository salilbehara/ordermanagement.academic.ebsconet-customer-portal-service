param(
    [Parameter(Mandatory = $true)][string]$Fqdn,
    [string]$SiteIp,
    [string]$EnabledProtocols = "http",
    [string]$EnvironmentName = "Development",
    [string]$SiteBasePath = "D:\Program Files\EBSCO\Service\",
    [string]$LogBasePath = "E:\Logs\EBSCO\Service\",
    [bool]$AllowWinAuth = $true,
    [bool]$RequireSsl = $false,
    [bool]$StartWebsite = $true
)

Import-Module WebAdministration

function Invoke-ConfigureServer() {   
    Confirm-AspNetCoreEnvironmentVariableIsPresent

    if ($RequireSsl) {
        Confirm-EbsconetSslCertIsPresent
    }
   
    $sitePath = $SiteBasePath + $Fqdn
    
    Remove-ServerFromLoadBalancer $sitePath $Fqdn

    Set-SitePhysicalPath $sitePath $Fqdn

    $appLogPath = Set-LogPath $LogBasePath $Fqdn

    Set-AppPool $Fqdn

    if ($StartWebsite -eq $false) {
        Stop-AppPool $Fqdn
    }

    Set-Acls $sitePath $Fqdn $appLogPath

    Set-Website $sitePath $Fqdn $appLogPath $SiteIp

    Set-EnabledProtocols $Fqdn $EnabledProtocols
    
    if ($RequireSsl -eq $true) {        
        Set-SslCertificateToEbsconetCert $Fqdn $SiteIp
    }

    Set-HostFileEntry $Fqdn
}

function Confirm-AspNetCoreEnvironmentVariableIsPresent() {
    $envVarCheck = [environment]::GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Machine")
    Write-Host "ASPNETCORE_ENVIRONMENT: $envVarCheck"
    if ($envVarCheck -ne $EnvironmentName) {
        # Write-Host "Updating Machine level Variable: ASPNETCORE_ENVIRONMENT - $EnvironmentName"
        # [Environment]::SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", $EnvironmentName, "Machine")

        # #DANGEROUS! We need IIS to completely restart to pick up these changes!
        # Write-Host "Restarting IIS !!!"
        # & {iisreset}

        throw "ERROR: System variable ASPNETCORE_ENVIRONMENT is not set to expected value of $EnvironmentName"
    }
}

function Get-EbsconetSslCert() {
    $pathToLocalCerts = "cert:\LocalMachine\My"
    $ebsconetCert = Get-ChildItem -Path $pathToLocalCerts | Where-Object {$_.FriendlyName -eq "*.ebsconet.com"} -ErrorAction SilentlyContinue

    return $ebsconetCert
}

function Confirm-EbsconetSslCertIsPresent() {
    $ebsconetCert = Get-EbsconetSslCert

    if ($null -eq $ebsconetCert) {
        throw "ERROR: *.ebsconet.com SSL cert is not present on the server"
    }
}

function Remove-ServerFromLoadBalancer() {
    param (
        [parameter(Mandatory = $true)]
        [string]$SitePath,

        [parameter(Mandatory = $true)]
        [string]$Fqdn
    )

    $lbTestPath = "$SitePath\wwwroot\lbtest.htm"
    $numberOfSecondsToWaitForTrafficToStop = 10 #F5 load balancer polls every 5 seconds
    if (Test-Path $lbTestPath) {
        Write-Host "Taking $Fqdn out of the load balancer"
        (Get-Content $lbTestPath).replace('<u>up</u>', '<u>down</u>') | Set-Content $lbTestPath
        Write-Host "Waiting $numberOfSecondsToWaitForTrafficToStop seconds for load balancer to stop routing traffic to the server"
        Start-Sleep -Seconds $numberOfSecondsToWaitForTrafficToStop
    }
}

function Set-SitePhysicalPath() {
    param (
        [parameter(Mandatory = $true)]
        [string]$SitePath,

        [parameter(Mandatory = $true)]
        [string]$Fqdn
    )

    if (-not(Test-Path IIS:\Sites\$fqdn)) {
        Write-Host "Creating path for web site $Fqdn"
        New-Item -Path $SitePath -type directory 
    }    
}

function Set-LogPath() {
    param (
        [parameter(Mandatory = $true)]
        [string]$LogBasePath,

        [parameter(Mandatory = $true)]
        [string]$Fqdn
    )

    $appLogPath = $LogBasePath + $Fqdn
    if (-Not(Test-Path -Path $appLogPath)) {
        New-Item -Path $appLogPath -type directory 
    }

    return $appLogPath
}

function Set-AppPool() {
    param (
        [parameter(Mandatory = $true)]
        [string]$Fqdn
    )

    if (-Not (Test-Path IIS:\AppPools\$Fqdn)) {  
        Write-Host "Creating app pool: $Fqdn"
        $appPool = New-WebAppPool -Name $Fqdn
        $appPool.processModel.identityType = 4
        $appPool.processModel.loadUserProfile = "true"
        $appPool.managedRuntimeVersion = ""
        $appPool.managedPipelineMode = "Classic"		# TODO -run in integrated mode
        $appPool.startMode = "AlwaysRunning"
        $appPool | Set-Item
    }
}

function Stop-AppPool() {
    param (
        [parameter(Mandatory = $true)]
        [string]$Fqdn
    )
    
    $appPoolState = (Get-WebAppPoolState $Fqdn).Value
    
    $timer = [System.Diagnostics.Stopwatch]::StartNew()
    $timeoutSeconds = 120

    while ($appPoolState -ne "Stopped" -and $timer.Elapsed.TotalSeconds -lt $timeoutSeconds) {
        Stop-WebAppPool -Name $Fqdn
        Write-Host "Waiting for $Fqdn app pool and related processes to stop. Current app pool state: " $appPoolState
        Start-Sleep -Seconds 20
        $appPoolState = (Get-WebAppPoolState $Fqdn).Value
    }

    $timer.Stop()

    if ($timer.Elapsed.TotalSeconds -gt $timeoutSeconds) {
        throw "The $Fqdn app pool did not stop after $timeoutSeconds seconds."
    }
}

function Set-Acls () {
    param (
        [parameter(Mandatory = $true)]
        [string]$SitePath,

        [parameter(Mandatory = $true)]
        [string]$Fqdn,

        [parameter(Mandatory = $true)]
        [string]$AppLogPath
    )

    $inherit = [system.security.accesscontrol.InheritanceFlags]"ContainerInherit, ObjectInherit"
    $propagation = [system.security.accesscontrol.PropagationFlags]"None"

    Write-Host "Setting ACL: $AppLogPath"     
    $acl = Get-Acl $AppLogPath
    $permission = "IIS AppPool\$Fqdn", "FullControl", $inherit, $propagation, "Allow"
    $accessRule = New-Object System.Security.AccessControl.FileSystemAccessRule ($permission)
    $acl.AddAccessRule($accessRule)
    $acl | Set-Acl $AppLogPath

    Write-Host "Setting ACL: $SitePath"     
    $acl = Get-Acl $SitePath 
    $permission = "IIS AppPool\$Fqdn", "FullControl", $inherit, $propagation, "Allow"
    $accessRule = New-Object System.Security.AccessControl.FileSystemAccessRule ($permission)
    $acl.AddAccessRule($accessRule)
    $acl | Set-Acl $SitePath 
}

function Set-Website() {
    param (
        [parameter(Mandatory = $true)]
        [string]$SitePath,

        [parameter(Mandatory = $true)]
        [string]$Fqdn,

        [parameter(Mandatory = $true)]
        [string]$AppLogPath,

        [parameter(Mandatory = $true)]
        [string]$SiteIp
    )

    if (-Not (Test-Path IIS:\Sites\$Fqdn)) { 
        Write-Host "Creating site: $Fqdn"   
        if ($SiteIp -ne "") {
            $website = New-Website -Name $Fqdn -PhysicalPath $SitePath -ApplicationPool $Fqdn -IPAddress $SiteIp
        }
        else {
            $website = New-Website -Name $Fqdn -PhysicalPath $SitePath -ApplicationPool $Fqdn
        }
        $website.logFile.directory = $AppLogPath
        $website | Set-Item
    }
}

function Set-SslCertificateToEbsconetCert() {
    param (
        [parameter(Mandatory = $true)]
        [string]$Fqdn,

        [parameter(Mandatory = $true)]
        [string]$SiteIp
    )

    Set-WebConfiguration -Location "$Fqdn" -Filter "system.webserver/security/access" -Value "Ssl"
    
    $ebsconetCert = Get-EbsconetSslCert
    $ebsconetCertThumbprint = $ebsconetCert.Thumbprint
    $siteHasSslCert = Test-Path IIS:\SslBindings\$SiteIp!443        
    $pathToLocalCerts = "cert:\LocalMachine\My"

    if ($siteHasSslCert) {
        $siteHasEbsconetSslCert = (Get-Item IIS:\SslBindings\$SiteIp!443).Thumbprint -eq $ebsconetCertThumbprint

        if (-not($siteHasEbsconetSslCert)) {
            Write-Host "Removing existing SSL certificate from $Fqdn"
            Get-WebBinding -Port 443 -Name $Fqdn | Remove-WebBinding
            Remove-Item IIS:\SslBindings\$SiteIp!443 -Force -Recurse

            Write-Host "Assigning *.ebsconet.com SSL certificate to $Fqdn"
            New-WebBinding -Name $Fqdn -Port 443 -HostHeader $Fqdn -IPAddress $SiteIp -Protocol "https"
            Get-Item -Path $pathToLocalCerts\$ebsconetCertThumbprint | New-Item -Path IIS:\SslBindings\$SiteIp!443
        }
    }
    else {
        Write-Host "Assigning *.ebsconet.com SSL certificate to $Fqdn"
        New-WebBinding -Name $Fqdn -Port 443 -IPAddress $SiteIp -Protocol "https"
        Get-Item -Path $pathToLocalCerts\$ebsconetCertThumbprint | New-Item -Path IIS:\SslBindings\$SiteIp!443
    }
}

function Set-EnabledProtocols() {
    param (
        [parameter(Mandatory = $true)]
        [string]$Fqdn,

        [parameter(Mandatory = $true)]
        [string]$EnabledProtocols
    )

    if ($EnabledProtocols -ne "") {
        if ((Get-ItemProperty IIS:\sites\$Fqdn -name EnabledProtocols) -ne $EnabledProtocols) {
            Set-ItemProperty IIS:\sites\$Fqdn -name EnabledProtocols -Value $EnabledProtocols
        }
    }
}

function Set-WindowsAuthentication() {
    param (
        [parameter(Mandatory = $true)]
        [string]$Fqdn,

        [parameter(Mandatory = $true)]
        [string]$AllowWinAuth
    )

    if ((Get-WebConfigurationProperty -filter /system.WebServer/security/authentication/windowsAuthentication -PSPath IIS:\ -location $Fqdn -name enabled).Value -ne $AllowWinAuth) {
        Set-WebConfigurationProperty -filter /system.WebServer/security/authentication/windowsAuthentication -PSPath IIS:\ -location $Fqdn -name enabled -value $AllowWinAuth
    }
}

function Set-HostFileEntry() {
    param (
        [parameter(Mandatory = $true)]
        [string]$Fqdn
    )

    $HostFilePath = 'C:\Windows\System32\drivers\etc\hosts'
    $HostFileContents = Get-Content $HostFilePath
    $HostToAdd = "`t127.0.0.1`t$Fqdn"
    
    if ($HostFileContents -contains $HostToAdd) {
        Write-Host "Hosts file already contains $Fqdn"
    }
    else {
        Write-Host "Adding to hosts file: $Fqdn"
        Add-Content -path $HostFilePath -value $HostToAdd
    }
}

Invoke-ConfigureServer