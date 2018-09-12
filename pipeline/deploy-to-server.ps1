param(
    [Parameter(Mandatory = $true)][string]$sessionId,
    [Parameter(Mandatory = $true)][string]$sourceLocation,
    [Parameter(Mandatory = $true)][string]$targetLocation
)

$session = Get-PSSession -Id $sessionId

$copySuccess = $false
$numberOfCopyAttempts = 0
$numberOfSecondsToWait = 10
$maxNumberOfCopyAttempts = 12
$errorMessage = ""

while ($copySuccess -eq $false -and $numberOfCopyAttempts -lt $maxNumberOfCopyAttempts) {
    try {
        Write-Host "Attempting to deploy files"
        Copy-Item -Path $sourceLocation* -Destination $targetLocation -Recurse -Force -Verbose -Exclude "*.pdb" -ToSession $session -ErrorAction Stop
        $copySuccess = $true
    }
    catch {
        $errorMessage = $_.Exception
        $numberOfCopyAttempts++
        Write-Host "Failed $numberOfCopyAttempts time(s) to deploy files."
        Start-Sleep -Seconds $numberOfSecondsToWait
    }
}

if ($copySuccess -eq $false) {
    throw $errorMessage
}

Write-Host "Making server available to load balancer"
$lbTestPath = "$targetLocation\wwwroot\lbtest.htm"
Invoke-Command -Session $session -ArgumentList $lbTestPath -ScriptBlock { param($lbTestPath) (Get-Content $lbTestPath).replace('<u>down</u>', '<u>up</u>') | Set-Content $lbTestPath }
Remove-PsSession $session -ErrorAction Stop
Write-Host "Finished deploying files"