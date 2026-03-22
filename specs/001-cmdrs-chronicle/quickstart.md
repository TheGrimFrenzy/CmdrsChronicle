# Quickstart: CmdrsChronicle

## Prerequisites
- Windows 10 or later (x64/x86)
- No need to install .NET: app is published as a self-contained executable

## Build (for developers)
1. Clone the repository
2. Open in Visual Studio 2022+ or VS Code
3. Build solution (dotnet build)
4. Run tests (dotnet test)

## Usage (for end users)
1. Download or build the published release (single .exe file)
2. Place the executable anywhere (no install required)
3. Run from command prompt:
   ```
   CmdrsChronicle.exe [--dir <logdir>] [--start <YYYY-MM-DD>] [--end <YYYY-MM-DD>] [--style elegant|colorful] [--type overall|by-system] [--category <cat1|cat2|...>]
   ```
4. Open the generated HTML file in your browser

## Example
```
CmdrsChronicle.exe --dir "%USERPROFILE%\Saved Games\Frontier Developments\Elite Dangerous" --start 2026-03-01 --end 2026-03-07 --style colorful --type by-system --category mining|travel|trade
```

## Troubleshooting
- If no report is generated, check that journal files exist in the specified directory and date range
- For corrupted files, see the diagnostics section in the generated HTML
- For help, run:
  ```
  CmdrsChronicle.exe --help
  ```

---
This quickstart covers both developer and end-user flows for CmdrsChronicle.