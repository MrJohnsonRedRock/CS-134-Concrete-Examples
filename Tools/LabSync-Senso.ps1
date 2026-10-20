# Paste THIS into Senso (its script box stops at ~4096 characters).
# It downloads the current Tools/LabSync.ps1 from GitHub and runs it.
[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
$url = 'https://raw.githubusercontent.com/MrJohnsonRedRock/CS-134-Concrete-Examples/main/Tools/LabSync.ps1'
try
{
    $code = (Invoke-WebRequest -Uri "$url`?t=$([DateTime]::UtcNow.Ticks)" -UseBasicParsing).Content
}
catch
{
    Write-Output "RESULT: FAILED  Could not download LabSync.ps1: $($_.Exception.Message)"
    exit 1
}
& ([scriptblock]::Create($code))
