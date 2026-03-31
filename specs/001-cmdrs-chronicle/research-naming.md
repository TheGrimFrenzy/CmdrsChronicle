# Elite Dangerous Journal Log File Naming Convention (Research)

## Default Directory
- `C:\Users\<username>\Saved Games\Frontier Developments\Elite Dangerous`

## File Naming Pattern
- Files are named: `Journal.<YYYY-MM-DDThhmmss>.<nn>.log`
  - Example: `Journal.2026-03-21T193456.01.log`
- Where:
  - `YYYY-MM-DDThhmmss` is the UTC timestamp of the log file's creation (ISO 8601 format, T separator)
  - `<nn>` is a two-digit sequence number (usually `01`, increments if multiple logs for the same session)
  - `.log` is the file extension

## Notes
- Only files matching this pattern should be considered for journal event parsing.
- The timestamp in the filename is UTC and can be used to determine the file's time range.
- There may be other files in the directory (e.g., `Status.json`, `Market.json`) that should be ignored for journal event processing.

---
This research note documents the canonical naming convention for Elite Dangerous journal log files for use in data model and file discovery logic.