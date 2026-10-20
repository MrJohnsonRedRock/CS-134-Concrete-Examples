# One-off diagnostic for "could not lock config file ... No such file or directory".
# Paste into Senso on the failing PC while the student is logged in. Changes nothing
# permanent: every test folder it makes is deleted at the end.
$ErrorActionPreference = 'Continue'
function Say($label, $value) { Write-Output ("{0,-22} {1}" -f $label, $value) }

Say 'PS version / 64-bit' "$($PSVersionTable.PSVersion) / $([Environment]::Is64BitProcess)"
$git = (Get-Command git.exe -ErrorAction SilentlyContinue).Source
Say 'git' $git
Say 'git version' (& git --version 2>&1)

$account = (Get-CimInstance Win32_ComputerSystem).UserName
$sid = (New-Object Security.Principal.NTAccount($account)).Translate([Security.Principal.SecurityIdentifier]).Value
$profilePath = (Get-CimInstance Win32_UserProfile -Filter "SID='$sid'").LocalPath
$parent = Join-Path $profilePath 'Unity Projects'
Say 'account' $account
Say 'profile' $profilePath
Say 'profile exists' (Test-Path $profilePath)
Say 'parent exists' (Test-Path $parent)
if (Test-Path $parent) { Say 'parent contents' ((Get-ChildItem $parent -Force -Name) -join ', ') }
Say 'profile attributes' ((Get-Item $profilePath -Force).Attributes)

Write-Output '--- git config as SYSTEM (where each setting comes from)'
& git config --show-origin --list 2>&1 | ForEach-Object { "  $_" }

# Can SYSTEM make a folder and a file there at all?
$probe = Join-Path $profilePath 'labsync-probe'
try
{
    New-Item -ItemType Directory -Path "$probe\.git" -Force | Out-Null
    Set-Content -Path "$probe\.git\config.lock" -Value 'x'
    Say 'plain write test' 'OK'
}
catch { Say 'plain write test' "FAILED: $($_.Exception.Message)" }
Remove-Item $probe -Recurse -Force -ErrorAction SilentlyContinue

# git init in the profile, with tracing, vs. in Windows\Temp.
function Test-Init($dir)
{
    $env:GIT_TRACE = '1'
    $out = & git -c safe.directory=* init $dir 2>&1 | ForEach-Object { "    $_" }
    $code = $LASTEXITCODE
    Remove-Item Env:\GIT_TRACE
    Say "init $dir" "exit $code"
    $out | Select-Object -Last 15
    Remove-Item $dir -Recurse -Force -ErrorAction SilentlyContinue
}
Test-Init (Join-Path $profilePath 'labsync-init-test')
Test-Init (Join-Path $env:windir 'Temp\labsync-init-test')
