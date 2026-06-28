$ErrorActionPreference = "Stop"

$root = Split-Path -Parent $PSScriptRoot
$dist = Join-Path $root "dist"
$desktopSource = Join-Path $root "desktop\MoliuWriterDesktop.cs"
$appDir = Join-Path $root "app"
$outputExe = Join-Path $dist "MoliuWriter.exe"
$workspaceOutputs = Split-Path -Parent $root

New-Item -ItemType Directory -Force -Path $dist | Out-Null

$cscCandidates = @(
    "$env:WINDIR\Microsoft.NET\Framework64\v4.0.30319\csc.exe",
    "$env:WINDIR\Microsoft.NET\Framework\v4.0.30319\csc.exe"
)
$csc = $cscCandidates | Where-Object { Test-Path $_ } | Select-Object -First 1
if (-not $csc) {
    throw "csc.exe was not found."
}

$webviewDirs = @(
    "$root\vendor\webview2",
    "$workspaceOutputs\moliu-desktop",
    "$workspaceOutputs\moliu-desktop-dist",
    "C:\Program Files\Microsoft OfficePLUS\3.16.0.46159\addin",
    "C:\Program Files (x86)\Microsoft\EdgeWebView\Application"
)

$webviewDir = $null
foreach ($dir in $webviewDirs) {
    if (
        (Test-Path (Join-Path $dir "Microsoft.Web.WebView2.Core.dll")) -and
        (Test-Path (Join-Path $dir "Microsoft.Web.WebView2.WinForms.dll")) -and
        (Test-Path (Join-Path $dir "WebView2Loader.dll"))
    ) {
        $webviewDir = $dir
        break
    }
}

if (-not $webviewDir) {
    throw "WebView2 dlls were not found. Put the three WebView2 dlls into vendor\webview2 and try again."
}

$coreDll = Join-Path $webviewDir "Microsoft.Web.WebView2.Core.dll"
$winformsDll = Join-Path $webviewDir "Microsoft.Web.WebView2.WinForms.dll"
$loaderDll = Join-Path $webviewDir "WebView2Loader.dll"

& $csc `
    /nologo `
    /target:winexe `
    /platform:x64 `
    /out:$outputExe `
    /reference:System.Windows.Forms.dll `
    /reference:System.Drawing.dll `
    /reference:$coreDll `
    /reference:$winformsDll `
    $desktopSource

Copy-Item -LiteralPath $coreDll -Destination $dist -Force
Copy-Item -LiteralPath $winformsDll -Destination $dist -Force
Copy-Item -LiteralPath $loaderDll -Destination $dist -Force
Copy-Item -LiteralPath (Join-Path $appDir "index.html") -Destination $dist -Force
Copy-Item -LiteralPath (Join-Path $appDir "audio_sources.json") -Destination $dist -Force
$distAudio = Join-Path $dist "audio"
New-Item -ItemType Directory -Force -Path $distAudio | Out-Null
Copy-Item -Path (Join-Path $appDir "audio\*.mp3") -Destination $distAudio -Force

Write-Host "Build complete: $dist\MoliuWriter.exe"
