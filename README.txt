================================================================================
  CmdrsChronicle — End User Guide
================================================================================

CmdrsChronicle reads your Elite Dangerous journal log files and produces a
self-contained HTML report showing what you did across a chosen date range.
Open the output file in any browser — no internet connection required.

--------------------------------------------------------------------------------
  REQUIREMENTS
--------------------------------------------------------------------------------

  • Windows 10 or later
  • .NET 8 Runtime  (https://dotnet.microsoft.com/download/dotnet/8.0)
    (already bundled with some release packages — check the release notes)

--------------------------------------------------------------------------------
  QUICK START
--------------------------------------------------------------------------------

Open a command prompt, navigate to the folder you extracted the ZIP into, then
run:

    CmdrsChronicle.cmd --start 2026-03-29 --end 2026-04-05

The tool will:
  1. Find your Elite Dangerous journal folder automatically.
  2. Parse all journal files that fall within the date range.
  3. Write an HTML report to the current folder.
  4. Print the output file path, then open it in your default browser.

If you prefer to pick everything from a menu, use interactive mode:

    CmdrsChronicle.cmd --interactive

--------------------------------------------------------------------------------
  INTERACTIVE MODE  (--interactive)
--------------------------------------------------------------------------------

Interactive mode displays a cockpit-style dashboard that shows all settings at
once. Press the numbered key next to any field to edit it, then press G to
generate the report.

  Navigation panel
  ----------------
  [1] Journal Dir   — folder containing your .log files (auto-detected by
                      default; change this if your journals are in a
                      non-standard location)
  [2] Output File   — where to write the HTML file (leave blank to auto-name
                      it "CmdrsChronicle_<start>-<end>.html" in the current
                      folder)

  Date Range panel
  ----------------
  [3] Start Date    — earliest date to include events from (YYYY-MM-DD)
  [4] End Date      — latest date to include events from  (YYYY-MM-DD)
                      Leave both blank to include your entire history.

  Mission Config panel
  --------------------
  [5] Report Type   — choose between:
                        summary    — one combined report across all sessions
                        by-system  — report sections grouped by the star
                                     system you were docked or flying in
  [6] Style         — choose between:
                        elegant    — clean two-column layout (default)
                        colorful   — vivid colour-coded tiles
                        galnet     — GalNet-style prose layout

  Filters panel
  -------------
  [7] Category      — show only infographic tiles from selected categories.
                      A multi-select checklist is shown; leave everything
                      unchecked to include all categories.
  [8] Sort Order    — control which category groups appear first. A numbered
                      list of available categories is shown; enter the numbers
                      in the order you want them (e.g. "3,1,7").

  Controls
  --------
  G   Generate the report with the current settings
  Q   Quit without generating

You can combine --interactive with any other option to pre-populate a field:

    CmdrsChronicle.cmd --interactive --start 2026-03-29 --end 2026-04-05

--------------------------------------------------------------------------------
  COMMAND-LINE OPTIONS
--------------------------------------------------------------------------------

  --input <path>
      Path to the directory containing your Elite Dangerous journal files.
      Journal files must be named in the standard format:
          Journal.YYYY-MM-DDTHHMMSS.NN.log
      If omitted, the tool tries the default ED journal folder:
          %USERPROFILE%\Saved Games\Frontier Developments\Elite Dangerous

  --output <path>
      Full path for the generated HTML file.
      Default: CmdrsChronicle_<start>-<end>.html in the current folder.

  --start <YYYY-MM-DD>
      Earliest date to include. Events before this date are ignored.
      If omitted, all events up to --end are included.

  --end <YYYY-MM-DD>
      Latest date to include. Events after this date are ignored.
      If omitted, events up to today are included.

  --type <summary|by-system>
      summary     (default) One report section covering the entire date range.
      by-system   Separate section per star system visited, in chronological
                  order. Requires --start to be set.

  --style <elegant|colorful|galnet>
      elegant     (default) Clean monochrome two-column tile layout.
      colorful    Vivid colour-coded tiles with gradient headers.
      galnet      GalNet broadcast style — prose-like narrative layout.

  --category <name[,name...]>
      Restrict the report to one or more infographic categories. Separate
      multiple names with a comma. Names are case-insensitive.

      Available categories:
          TravelAndNavigation     Exploration         Exobiology
          CombatShip              CombatOnFoot        ThargoidAX
          Trade                   Mining              Missions
          EngineeringAndSynthesis FleetCarrierOperations Powerplay
          SocialMulticrew         CodexAndDiscoveries SettlementActivities
          CrimeAndSecurity        EconomyAndMarket    ShipManagementAndOutfitting
          MaterialGathering       SalvageAndRecovery  PassengerTransport
          Administrivia

      Example — show only mining and trade tiles:
          --category Mining,Trade

  --sort <category[,category...]>
      Comma-separated list of category names controlling the left-to-right
      (or top-to-bottom) order of tile groups. Categories not listed will
      appear after the listed ones, sorted by metric value.

      Example — put Combat first, then Exploration, then everything else:
          --sort CombatShip,Exploration

  --max-parallelism <n>
      Maximum number of journal files parsed at the same time. Defaults to
      the number of logical CPU cores. Reduce this to limit CPU/memory use
      on slower machines; increase it on fast machines with many journal
      files.

      Alternatively, set the environment variable:
          CMDRSCHRONICLE_MAX_PARALLELISM=2

  --silent
      Suppress all progress output. Only the final report file path is
      printed to stdout. Useful for batch scripts or scheduled tasks.

  --interactive
      Launch the cockpit-style interactive UI. Any options already supplied
      on the command line are used as initial values in the UI.

  --help
      Print a summary of all options and exit.

  --version
      Print the application version and exit.

--------------------------------------------------------------------------------
  EXAMPLES
--------------------------------------------------------------------------------

  Generate a summary report for last week:
      CmdrsChronicle.cmd --start 2026-03-29 --end 2026-04-05

  Generate a report and save it to a specific file:
      CmdrsChronicle.cmd --start 2026-03-29 --end 2026-04-05
          --output "C:\Reports\last_week.html"

  Use the colorful style with a custom journal folder:
      CmdrsChronicle.cmd --start 2026-03-29 --end 2026-04-05
          --style colorful
          --input "D:\Games\Elite Dangerous\Journals"

  Group by star system (useful for expedition logs):
      CmdrsChronicle.cmd --start 2026-03-29 --end 2026-04-05
          --type by-system

  Show only exploration and exobiology tiles:
      CmdrsChronicle.cmd --start 2026-03-29 --end 2026-04-05
          --category Exploration,Exobiology

  Run interactively with a date range pre-filled:
      CmdrsChronicle.cmd --interactive --start 2026-03-29 --end 2026-04-05

  Silent mode for use in scripts:
      CmdrsChronicle.cmd --start 2026-03-29 --end 2026-04-05 --silent

--------------------------------------------------------------------------------
  OUTPUT FILE
--------------------------------------------------------------------------------

The report is a fully self-contained HTML file — all styles and any scripts are
inlined. You can:
  • Move or copy it anywhere.
  • Email it or share it without needing the infographics/ or templates/ folders.
  • Open it offline in any modern browser (Chrome, Edge, Firefox, etc.).

The filename defaults to:
    CmdrsChronicle_<start>-<end>.html
For example:  CmdrsChronicle_2026-03-29-2026-04-05.html

--------------------------------------------------------------------------------
  NOTHING-TO-REPORT PAGE
--------------------------------------------------------------------------------

If no qualifying events are found in the selected date range, the tool writes a
"nothing to report" page instead of an empty report. This page shows your last
known location and commander name (pulled from your journal history).

Check:
  • The --start and --end dates cover the days you actually played.
  • Your journal directory contains files for those dates.

--------------------------------------------------------------------------------
  PARSE ERRORS
--------------------------------------------------------------------------------

If the tool encounters a journal line it cannot read (corrupt file, non-standard
format, etc.), it silently skips that line and continues. All errors are recorded
as an HTML comment at the very top of the output file.

To review them, open the HTML file in a text editor and search for:
    <!-- CMDRS CHRONICLE — PARSE DIAGNOSTICS

--------------------------------------------------------------------------------
  TROUBLESHOOTING
--------------------------------------------------------------------------------

  "ERROR: Input directory does not exist"
      The path given to --input does not exist. Check the spelling and that
      the drive/folder is accessible.

  "ERROR: Schema file not found"
      The file cmdrschronicle_schema.sql is missing from the application
      folder. Re-extract the ZIP release.

  "ERROR: Report template or CSS not found"
      The templates/ folder is missing or incomplete. Re-extract the ZIP
      release alongside the executable.

  Browser does not open automatically
      The HTML file is still written to disk. Open it manually from the
      path printed to the console.

  Report is very slow to generate
      Use --max-parallelism 2 (or a higher number) to control how many files
      are parsed simultaneously. Also ensure your journal folder does not
      contain thousands of old files — use --start/--end to limit the range.

--------------------------------------------------------------------------------
  ENVIRONMENT VARIABLES
--------------------------------------------------------------------------------

  CMDRSCHRONICLE_MAX_PARALLELISM
      Sets the default parallelism level without needing --max-parallelism
      on every run. The command-line flag takes precedence if both are set.

--------------------------------------------------------------------------------
  VERSION / SUPPORT
--------------------------------------------------------------------------------

  Run  CmdrsChronicle.cmd --version  to see the installed version.

================================================================================
