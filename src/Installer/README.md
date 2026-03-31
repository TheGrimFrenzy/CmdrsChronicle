# Installer

Packaging and deployment helpers for CmdrsChronicle.

This folder provides an MVP packaging flow that produces a zip-based installer and (optionally) an MSI via the WiX Toolset.

What it does:

- Publishes the CLI project (`src/CmdrsChronicle.Cli`) for `win-x64` by default.
- Copies the `infographics/` and `templates/` folders into the publish output (if present).
- Produces a zip installer `artifacts/installer/CmdrsChronicle-CLI-<Configuration>-<Runtime>.zip`.
- Optionally attempts to create an MSI using the WiX Toolset if `-ProduceMsi` is passed to the script and WiX is installed.

Usage

From PowerShell (repo root):

```powershell
# Run with defaults
.\src\Installer\MakeInstaller.ps1

# Produce a WiX MSI (requires WiX toolset installed and in PATH)
.\src\Installer\MakeInstaller.ps1 -ProduceMsi
```

CI

There is a GitHub Actions workflow that runs the packaging script and uploads the produced installer artifacts. See `.github/workflows/installer.yml`.

Notes and next steps

- This is intentionally minimal: a zip installer is cross-platform and simple to produce. It satisfies the immediate need for an installer artifact and allows users to inspect the files before installing.
- For a fully-featured MSIX/MSI packaging experience, install the WiX Toolset and run with `-ProduceMsi` (or adapt the workflow to run on a self-hosted runner with WiX installed).
- T602 will add end-user instructions and ensure the `/infographics` directory is packaged and discoverable after install.

See also: `../../specs/001-cmdrs-chronicle/tasks.md` for T601/T602 task information.
