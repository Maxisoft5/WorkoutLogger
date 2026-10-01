[CmdletBinding()]
param()
$ErrorActionPreference = 'Stop'
$webRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../WorkoutLogger.Web'))
$targetRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../WorkoutLogger.WebApi/wwwroot'))
if (!(Test-Path -LiteralPath (Join-Path $webRoot 'dist/index.html'))) {
    throw 'Сначала соберите React-приложение: cd WorkoutLogger.Web; pnpm build'
}
New-Item -ItemType Directory -Path $targetRoot -Force | Out-Null
Copy-Item -LiteralPath (Join-Path $webRoot 'dist/index.html') -Destination $targetRoot -Force
Copy-Item -LiteralPath (Join-Path $webRoot 'dist/assets') -Destination $targetRoot -Recurse -Force
Write-Host 'Сборка React скопирована в WebApi/wwwroot. Теперь выполните dotnet publish для WebApi.'
