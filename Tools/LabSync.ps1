
$RepoUrl    = 'https://github.com/MrJohnsonRedRock/CS-134-Concrete-Examples.git'
$ParentDir  = 'Unity Projects'             # inside the student's profile folder
$FolderName = 'CS-134 Concrete Examples'

# Windows PowerShell treats git's progress messages (written to stderr) as errors
# under 'Stop', so keep 'Continue' and check exit codes instead.
$ErrorActionPreference = 'Continue'
$env:GIT_TERMINAL_PROMPT = '0'             # never hang waiting for a password

function Write-Result([string]$status, [string]$detail)
{
    Write-Output ("RESULT: {0}  {1}" -f $status, $detail)
}

# ---------- Find git ----------

$git = (Get-Command git.exe -ErrorAction SilentlyContinue).Source
if (-not $git)
{
    foreach ($candidate in @("$env:ProgramFiles\Git\cmd\git.exe", "${env:ProgramFiles(x86)}\Git\cmd\git.exe"))
    {
        if (Test-Path $candidate) { $git = $candidate; break }
    }
}
if (-not $git) { Write-Result 'FAILED' 'git.exe not found on this PC.'; exit 1 }

# safe.directory: SYSTEM is working in a repo the student owns, which git would
#                 otherwise refuse ("dubious ownership").
# user.name/email: git stash makes a commit, and SYSTEM has no identity.
$gitConfig = @(
    '-c', 'safe.directory=*',
    '-c', 'user.name=CS134 Lab Sync',
    '-c', 'user.email=labsync@localhost'
)

function Invoke-Git([string[]]$Arguments)
{
    $output = & $git @gitConfig @Arguments 2>&1 | ForEach-Object { "$_" } | Out-String
    return [pscustomobject]@{ Code = $LASTEXITCODE; Output = $output.Trim() }
}

# ---------- Who is at the keyboard? ----------

$account = (Get-CimInstance Win32_ComputerSystem).UserName      # e.g. DORAL\jsmith
if (-not $account) { Write-Result 'SKIPPED' 'Nobody is logged in at the console.'; exit 0 }

try
{
    $sid = (New-Object System.Security.Principal.NTAccount($account)).Translate(
        [System.Security.Principal.SecurityIdentifier]).Value
}
catch
{
    Write-Result 'FAILED' "Could not look up $account."; exit 1
}

# Ask Windows for the real profile folder; it isn't always C:\Users\<name>
# (domain accounts can get folders like jsmith.DORAL).
$userProfile = Get-CimInstance Win32_UserProfile -Filter "SID='$sid'"
if (-not $userProfile) { Write-Result 'FAILED' "No profile folder for $account."; exit 1 }

$parent = Join-Path $userProfile.LocalPath $ParentDir
$target = Join-Path $parent $FolderName

# Files git writes as SYSTEM are owned by SYSTEM. Hand them to the student, or
# their own git (and Unity's Get Latest Lesson) will refuse to touch the repo.
function Grant-Ownership([string]$path)
{
    & icacls.exe $path /setowner $account /T /C /Q | Out-Null
}

# ---------- One run at a time ----------

# Two copies running at once wreck each other: each one's cleanup deletes the
# folder the other is halfway through cloning ("could not lock config file").
# A machine-wide mutex lets the first copy work and sends the rest home. Windows
# releases it automatically when this PowerShell process ends.
$mutex = New-Object System.Threading.Mutex($false, 'Global\CS134-LabSync')
try { $owned = $mutex.WaitOne(0) }
catch [System.Threading.AbandonedMutexException] { $owned = $true }   # last holder crashed; it's ours now
if (-not $owned) { Write-Result 'SKIPPED' 'Another LabSync run is already working on this PC.'; exit 0 }

# ---------- Clone or update ----------

if (Test-Path (Join-Path $target '.labsync-skip'))
{
    Write-Result 'SKIPPED' "$target has .labsync-skip (instructor copy)."
    exit 0
}

# A clone that never finished has a .git folder but no commit. Nothing of the
# student's can be in it yet, so clear it and clone fresh.
$repaired = $false
if (Test-Path (Join-Path $target '.git'))
{
    $head = Invoke-Git @('-C', $target, 'rev-parse', '--verify', '--quiet', 'HEAD')
    if ($head.Code -ne 0)
    {
        Remove-Item $target -Recurse -Force
        $repaired = $true
    }
}

if (-not (Test-Path $target))
{
    $createdParent = -not (Test-Path $parent)

    # The repo uses Git LFS, which installs a post-checkout hook during clone.
    # Git 2.45.1 wrongly blocks that ("active post-checkout hook found");
    # 2.45.2 fixed it. Turning the check off is the documented workaround, and
    # it only affects this one clone of our own repo.
    $env:GIT_CLONE_PROTECTION_ACTIVE = 'false'
    $clone = Invoke-Git @('clone', '--quiet', $RepoUrl, $target)
    Remove-Item Env:\GIT_CLONE_PROTECTION_ACTIVE

    if ($clone.Code -ne 0)
    {
        # Don't leave a half-cloned project behind; the next run starts clean.
        if (Test-Path $target) { Remove-Item $target -Recurse -Force }
        Write-Result 'FAILED' "Clone into $target failed. $($clone.Output)"
        exit 1
    }

    Grant-Ownership $target
    if ($createdParent) { & icacls.exe $parent /setowner $account /C /Q | Out-Null }

    $detail = "$account -> $target"
    if ($repaired) { $detail += '  (replaced a half-finished clone)' }
    Write-Result 'CLONED' $detail
    exit 0
}

if (-not (Test-Path (Join-Path $target '.git')))
{
    Write-Result 'FAILED' "$target exists but isn't a git clone. Rename or delete it, then run again."
    exit 1
}

$stamp = Get-Date -Format 'yyyy-MM-dd HH:mm'
$stash = Invoke-Git @('-C', $target, 'stash', 'push', '--include-untracked', '-m', "Lab Sync $stamp")
if ($stash.Code -ne 0) { Write-Result 'FAILED' "Stash failed in $target. $($stash.Output)"; exit 1 }
$stashed = -not ($stash.Output -match 'No local changes')

$pull = Invoke-Git @('-C', $target, 'pull', '--ff-only')
Grant-Ownership $target
if ($pull.Code -ne 0) { Write-Result 'FAILED' "Pull failed in $target (changes are safe in the stash). $($pull.Output)"; exit 1 }

$detail = "$account -> $target"
if ($stashed) { $detail += '  (student changes stashed)' }

if ($pull.Output -match 'Already up to date')
{
    Write-Result 'UP TO DATE' $detail
}
else
{
    Write-Result 'UPDATED' $detail
}
exit 0
