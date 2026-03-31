param(
  [string]$SolutionDir = "$(Resolve-Path "$(Split-Path -Parent $MyInvocation.MyCommand.Definition)")",
  [string]$Configuration = "Release",
  [string]$Runtime = "win-x64",
  [Switch]$ProduceMsi,
  [Switch]$SingleFile,
  [Switch]$SelfContained,
  [string]$AssemblyName = "CmdrsChronicle"
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

# Build publish arguments
$publishArgs = @("-c", $Configuration, "-r", $Runtime, "-o", $publishDir)
if ($SingleFile) { $publishArgs += "-p:PublishSingleFile=true" } else { $publishArgs += "-p:PublishSingleFile=false" }
if ($SelfContained) { $publishArgs += "-p:SelfContained=true" } else { $publishArgs += "-p:SelfContained=false" }

dotnet publish $cliProj @publishArgs
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

# If single-file publish produced an exe with a different name, ensure expected name

if ($SingleFile) {
  # Attempt to rename the produced exe to the requested AssemblyName for a cleaner UX
  $exe = Get-ChildItem -Path $publishDir -Filter *.exe | Select-Object -First 1
  if ($exe) {
    $targetExe = Join-Path $publishDir ("$AssemblyName.exe")
    if ($exe.FullName -ne $targetExe) {
      Copy-Item -Path $exe.FullName -Destination $targetExe -Force
      Write-Host "Renamed/created single-file exe: $targetExe"
    }
  } else {
    Write-Warning "No executable found in publish directory. Listing contents:"
    Get-ChildItem -Path $publishDir
  }
}

[IO.Compression.ZipFile]::CreateFromDirectory($publishDir, $zipPath)
    
# Clean up unnecessary files to keep installer minimal
Write-Host "Cleaning up leftover host exe and PDBs from publish folder..."
$removePatterns = @('CmdrsChronicle.Cli.exe','*.pdb')
foreach ($pat in $removePatterns) {
  Get-ChildItem -Path $publishDir -Filter $pat -Recurse -File -ErrorAction SilentlyContinue | ForEach-Object {
    if ($_.FullName -ne (Join-Path $publishDir "$AssemblyName.exe")) {
      Remove-Item -Path $_.FullName -Force -ErrorAction SilentlyContinue
      Write-Host "Removed: $($_.FullName)"
    }
  }
}


Write-Host "Created installer artifact: $zipPath"

    if ($ProduceMsi) {
        Write-Host "MSI production requested. Trying to produce MSI via WiX (requires heat.exe, candle.exe and light.exe in PATH)."

        # Paths for generated WiX artifacts
        $wxsMain = Join-Path $artifactsDir "InstallerMain.wxs"
        $wxsHarvest = Join-Path $artifactsDir "Harvest.wxs"
        $wixobjMain = [System.IO.Path]::ChangeExtension($wxsMain, '.wixobj')
        $wixobjHarvest = [System.IO.Path]::ChangeExtension($wxsHarvest, '.wixobj')

        # Main WXS content: references harvested component group and uses WixUI_InstallDir
        $wxsMainTemplate = @"
<?xml version='1.0' encoding='UTF-8'?>
<Wix xmlns='http://schemas.microsoft.com/wix/2006/wi'>
  <Product Id='*' Name='CmdrsChronicle' Language='1033' Version='1.0.0.0' Manufacturer='CmdrsChronicle' UpgradeCode='PUT-GUID-HERE'>
    <Package InstallerVersion='500' Compressed='yes' InstallScope='perMachine' />
    <MediaTemplate />
    <UIRef Id='WixUI_InstallDir' />
    <Property Id='WIXUI_INSTALLDIR' Value='INSTALLFOLDER' />
    <Directory Id='TARGETDIR' Name='SourceDir'>
      <Directory Id='ProgramFilesFolder'>
        <Directory Id='INSTALLFOLDER' Name='CmdrsChronicle' />
      </Directory>
    </Directory>

    <!-- Component group will be harvested into Harvest.wxs -->
    <Feature Id='ProductFeature' Title='CmdrsChronicle' Level='1'>
      <ComponentGroupRef Id='ProductComponents' />
    </Feature>
  </Product>
  <WixVariable Id='WixUILicenseRtf' Value=''>
  </WixVariable>
</Wix>
"@

        Set-Content -Path $wxsMain -Value $wxsMainTemplate -Encoding UTF8

        $heat = Get-Command heat.exe -ErrorAction SilentlyContinue
        $candle = Get-Command candle.exe -ErrorAction SilentlyContinue
        $light = Get-Command light.exe -ErrorAction SilentlyContinue

        if (-not $heat -or -not $candle -or -not $light) {
            Write-Warning "WiX heat/candle/light not all found in PATH. Skipping MSI creation. Install WiX Toolset (heat.exe, candle.exe, light.exe) and rerun with -ProduceMsi.";
        } else {
            Write-Host "Harvesting publish directory into WiX fragment..."
            # heat dir produces a wxs fragment with ComponentGroup Id=ProductComponents
            & $heat dir $publishDir -cg ProductComponents -dr INSTALLFOLDER -var var.PublishDir -out $wxsHarvest -sreg -srd -template fragment
            if ($LASTEXITCODE -ne 0) { Write-Error "heat failed"; exit $LASTEXITCODE }

            Push-Location $artifactsDir
            & $candle $wxsHarvest -dPublishDir=$publishDir
            if ($LASTEXITCODE -ne 0) { Write-Error "candle (harvest) failed"; Pop-Location; exit $LASTEXITCODE }
            & $candle $wxsMain
            if ($LASTEXITCODE -ne 0) { Write-Error "candle (main) failed"; Pop-Location; exit $LASTEXITCODE }

            & $light (Split-Path $wxsHarvest -LeafBase + '.wixobj') (Split-Path $wxsMain -LeafBase + '.wixobj') -o "CmdrsChronicle-CLI-$Configuration-$Runtime.msi"
            if ($LASTEXITCODE -ne 0) { Write-Error "light failed"; Pop-Location; exit $LASTEXITCODE }
            Pop-Location
            Write-Host "MSI created in $artifactsDir"
        }
    }

Write-Host "Installer packaging complete. Artifacts in: $artifactsDir"
