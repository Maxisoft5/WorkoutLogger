param(
    [string]$AdbPath,
    [string]$ConfigPath = (Join-Path $PSScriptRoot '..\WorkoutLogg\Resources\Raw\appsettings.json'),
    [string]$DeviceSerial
)

$ErrorActionPreference = 'Stop'
try {
    $config = Get-Content -LiteralPath $ConfigPath -Raw | ConvertFrom-Json
    if (-not $config.UseLocalhost) { return }
    $localUrl = if ($config.Api.LocalUrl) { $config.Api.LocalUrl } else { 'https://localhost:5001' }
    $apiUri = [Uri]$localUrl
    if (-not $apiUri.IsLoopback) { return }

    if (-not $AdbPath -or -not (Test-Path -LiteralPath $AdbPath)) {
        $candidates = @(
            "$env:ANDROID_HOME\platform-tools\adb.exe",
            "$env:ANDROID_SDK_ROOT\platform-tools\adb.exe",
            "$env:LOCALAPPDATA\Android\Sdk\platform-tools\adb.exe"
        )
        $AdbPath = $candidates | Where-Object { Test-Path -LiteralPath $_ } | Select-Object -First 1
    }
    if (-not $AdbPath) { throw 'Android SDK adb.exe was not found.' }

    $devices = @(& $AdbPath devices -l)
    if ($LASTEXITCODE -ne 0) { throw 'adb devices failed.' }
    $ready = @($devices | ForEach-Object {
        if ($_ -match '^(\S+)\s+device(?:\s|$)' -and $Matches[1] -notlike 'emulator-*') { $Matches[1] }
    })
    if ($DeviceSerial) {
        if ($DeviceSerial -notin $ready) { throw "Device $DeviceSerial is not connected and authorized." }
    } elseif ($ready.Count -eq 1) {
        $DeviceSerial = $ready[0]
    } elseif ($ready.Count -eq 0) {
        Write-Host '[Local API] No authorized physical Android device; USB forwarding skipped.'
        return
    } else {
        throw 'Multiple Android devices: set LocalApiDeviceSerial in MSBuild, or pass -DeviceSerial to this script.'
    }

    # Repeating reverse is safe; ADB keeps unrelated debugger ports intact.
    foreach ($port in (@($apiUri.Port, 5000) | Select-Object -Unique)) {
        & $AdbPath -s $DeviceSerial reverse "tcp:$port" "tcp:$port" | Out-Null
        if ($LASTEXITCODE -ne 0) { throw "adb reverse failed for port $port." }
        Write-Host "[Local API] ${DeviceSerial}: phone localhost:$port -> PC localhost:$port"
    }

    $probe = New-Object System.Net.Sockets.TcpClient
    try {
        $connect = $probe.ConnectAsync('127.0.0.1', $apiUri.Port)
        if (-not $connect.Wait(1000) -or -not $probe.Connected) { throw 'API port is closed.' }
        Write-Host "[Local API] PC port $($apiUri.Port) is listening."
    } catch {
        Write-Warning "[Local API] Forwarding is ready, but PC port $($apiUri.Port) is unavailable. Start WorkoutLogger.WebApi (https) and keep it running."
    } finally {
        $probe.Dispose()
    }
} catch {
    # A missing phone must not prevent normal builds or CI.
    Write-Warning "[Local API] $($_.Exception.Message) Run tools\Connect-LocalApi.ps1 after connecting the phone."
}
