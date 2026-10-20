# Diagnostic 2: git clone, traced, into three places. Paste into Senso on the
# failing PC while the student is logged in. Shallow, no checkout, so it's quick;
# every test folder is deleted at the end.
$ErrorActionPreference = 'Continue'
$env:GIT_TERMINAL_PROMPT = '0'
$url = 'https://github.com/MrJohnsonRedRock/CS-134-Concrete-Examples.git'

$account = (Get-CimInstance Win32_ComputerSystem).UserName
$sid = (New-Object Security.Principal.NTAccount($account)).Translate([Security.Principal.SecurityIdentifier]).Value
$profilePath = (Get-CimInstance Win32_UserProfile -Filter "SID='$sid'").LocalPath
Write-Output "cwd: $PWD   profile: $profilePath"

function Test-Clone($dir)
{
    Write-Output ""
    Write-Output "=== clone -> $dir"
    $env:GIT_TRACE = '1'
    $out = & git -c safe.directory=* clone --no-checkout --depth 1 $url $dir 2>&1 | ForEach-Object { "  $_" }
    $code = $LASTEXITCODE
    Remove-Item Env:\GIT_TRACE
    Write-Output "exit $code"
    $out | Where-Object { $_ -notmatch 'trace: (run_command|exec|built-in: git (remote-https|index-pack|rev-list))' } | Select-Object -Last 25
    if (Test-Path $dir)
    {
        Write-Output "  left behind:"
        Get-ChildItem $dir -Force -Recurse -Depth 1 -Name | Select-Object -First 15 | ForEach-Object { "    $_" }
        Remove-Item $dir -Recurse -Force -ErrorAction SilentlyContinue
    }
    else
    {
        Write-Output "  left behind: nothing (folder doesn't exist)"
    }
}

# 1. The real target (spaces, two new folders deep)
Test-Clone (Join-Path $profilePath 'Unity Projects\CS-134 Concrete Examples')
# 2. Profile, one folder, no spaces
Test-Clone (Join-Path $profilePath 'labsync-clone-test')
# 3. Outside the profile
Test-Clone (Join-Path $env:windir 'Temp\labsync-clone-test')

# 4. The init-then-fetch route, in the real target
$t = Join-Path $profilePath 'Unity Projects\CS-134 Concrete Examples'
Write-Output ""
Write-Output "=== init + fetch -> $t"
& git -c safe.directory=* init $t 2>&1 | ForEach-Object { "  $_" }
Write-Output "init exit $LASTEXITCODE"
& git -c safe.directory=* -C $t remote add origin $url 2>&1 | ForEach-Object { "  $_" }
Write-Output "remote add exit $LASTEXITCODE"
& git -c safe.directory=* -C $t fetch --depth 1 origin main 2>&1 | Select-Object -Last 5 | ForEach-Object { "  $_" }
Write-Output "fetch exit $LASTEXITCODE"
Remove-Item $t -Recurse -Force -ErrorAction SilentlyContinue
