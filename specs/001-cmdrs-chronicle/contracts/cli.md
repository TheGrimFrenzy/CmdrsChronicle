# CmdrsChronicle CLI Contract

## Command
```
CmdrsChronicle.exe [--dir <logdir>] [--start <YYYY-MM-DD>] [--end <YYYY-MM-DD>] [--style elegant|colorful] [--type overall|by-system] [--category <cat1|cat2|...>]
```

## Arguments
- `--dir`: Directory containing Elite Dangerous journal files (default: `%USERPROFILE%\Saved Games\Frontier Developments\Elite Dangerous`)
- `--start`: Start date (inclusive, format: `YYYY-MM-DD`, default: 7 days ago)
- `--end`: End date (inclusive, format: `YYYY-MM-DD`, default: today)
- `--style`: Report style (`elegant` or `colorful`, default: `elegant`)
- `--type`: Grouping type (`overall` or `by-system`, default: `overall`)
- `--category`: Pipe-delimited list of categories to filter and order (e.g., `mining|travel|trade`)

## Output
- Generates a single HTML file: `cmdrs-chronicle-<date-generated>.html`
- Exit code `0` on success (including 'no data' report), non-zero on unrecoverable error

## Error Handling
- Malformed or partial journal files: errors are logged in the HTML report, not fatal unless no valid events
- No journal files found: generates a 'nothing of interest happened' report with Cmdr/system/date if available

## Help
- `--help`: Prints usage and argument descriptions

---
This contract defines the CLI interface for CmdrsChronicle as required by the feature spec.