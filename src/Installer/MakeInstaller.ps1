param(
    [string]$SolutionDir = "$(Resolve-Path "$(Split-Path -Parent $MyInvocation.MyCommand.Definition)")",
    [string]$Configuration = "Release",
    [string]$Runtime = "win-x64",
    [Switch]$ProduceMsi
)

# Publish the CLI
$cliProj = Join-Path $SolutionDir "..\CmdrsChronicle.Cli\CmdrsChronicle.Cli.csproj"
if (-not (Test-Path $cliProj)) {
    Write-Error "Could not find CLI project at $cliProj"
    exit 1
}

$publishDir = Join-Path $SolutionDir "artifacts\publish"
if (Test-Path $publishDir) { Remove-Item -Recurse -Force $publishDir }
New-Item -ItemType Directory -Path $publishDir | Out-Null

Write-Host "Publishing CLI project..."
dotnet publish $cliProj -c $Configuration -r $Runtime -o $publishDir --self-contained false -p:PublishSingleFile=false
if ($LASTEXITCODE -ne 0) { Write-Error "dotnet publish failed"; exit $LASTEXITCODE }

# Copy infographics and templates if present
$repoRoot = Resolve-Path (Join-Path $SolutionDir "..\..")
$infographicsSrc = Join-Path $repoRoot "infographics"
$templatesSrc = Join-Path $repoRoot "templates"
if (Test-Path $infographicsSrc) { Copy-Item -Recurse -Force $infographicsSrc (Join-Path $publishDir "infographics") }
if (Test-Path $templatesSrc) { Copy-Item -Recurse -Force $templatesSrc (Join-Path $publishDir "templates") }

# Create installer artifact (zip)
$artifactsDir = Join-Path $SolutionDir "artifacts\installer"
if (Test-Path $artifactsDir) { Remove-Item -Recurse -Force $artifactsDir }
New-Item -ItemType Directory -Path $artifactsDir | Out-Null

$zipPath = Join-Path $artifactsDir "CmdrsChronicle-CLI-$Configuration-$Runtime.zip"
Write-Host "Creating zip installer: $zipPath"
Add-Type -AssemblyName System.IO.Compression.FileSystem
[IO.Compression.ZipFile]::CreateFromDirectory($publishDir, $zipPath)

Write-Host "Created installer artifact: $zipPath"

if ($ProduceMsi) {
    Write-Host "MSI production requested. Trying to produce MSI via WiX (requires candle.exe and light.exe in PATH)."
    $wixTemplate = @"
<?xml version='1.0' encoding='UTF-8'?>
<Wix xmlns='http://schemas.microsoft.com/wix/2006/wi'>
  <Product Id='*' Name='CmdrsChronicle' Language='1033' Version='1.0.0.0' Manufacturer='CmdrsChronicle' UpgradeCode='PUT-GUID-HERE'>
    <Package InstallerVersion='200' Compressed='yes' InstallScope='perMachine' />
    <MediaTemplate />
    <Directory Id='TARGETDIR' Name='SourceDir'>
      <Directory Id='ProgramFilesFolder'>
        <Directory Id='INSTALLFOLDER' Name='CmdrsChronicle' />
      </Directory>
    </Directory>
    <DirectoryRef Id='INSTALLFOLDER'>
      <Component Id='CLIFiles' Guid='*'>
        <File Source='__PUBLISH_DIR__\CmdrsChronicle.Cli.dll' />
      </Component>
    </DirectoryRef>
    <Feature Id='DefaultFeature' Level='1'>
      <ComponentRef Id='CLIFiles' />
    </Feature>
  </Product>
</Wix>
"@
    $wxs = Join-Path $artifactsDir "installer.wxs"
    $wxsContent = $wixTemplate -replace "__PUBLISH_DIR__", ($publishDir -replace '\\','\\\\')
    Set-Content -Path $wxs -Value $wxsContent -Encoding UTF8

    # Run WiX tools
    $candle = Get-Command candle.exe -ErrorAction SilentlyContinue
    $light = Get-Command light.exe -ErrorAction SilentlyContinue
    if (-not $candle -or -not $light) {
        Write-Warning "WiX tools not found in PATH. Skipping MSI creation. Install WiX Toolset and ensure candle.exe/light.exe are available.";
    } else {
        Push-Location $artifactsDir
        & $candle $wxs
        if ($LASTEXITCODE -ne 0) { Write-Error "candle failed"; Pop-Location; exit $LASTEXITCODE }
        $wixobj = [System.IO.Path]::ChangeExtension($wxs, '.wixobj')
        & $light $wixobj -o "CmdrsChronicle-CLI-$Configuration-$Runtime.msi"
        if ($LASTEXITCODE -ne 0) { Write-Error "light failed"; Pop-Location; exit $LASTEXITCODE }
        Pop-Location
        Write-Host "MSI created in $artifactsDir"
    }
}

Write-Host "Installer packaging complete. Artifacts in: $artifactsDir"
