using Spectre.Console;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CmdrsChronicle.Core;

namespace CmdrsChronicle.Cli;

/// <summary>
/// Cockpit-style interactive UI for CmdrsChronicle.
/// All settings are visible at once across themed panels. The user presses a number to
/// edit a specific field, then the cockpit re-renders with the updated value.
/// Invoked when --interactive is passed; CLI-supplied values are used as initial state.
/// </summary>
internal static class InteractiveSetup
{
    // ── Public entry point ────────────────────────────────────────────────────

    public static ReportOptions Run(ReportOptions defaults, string? infographicsBasePath = null)
    {
        // Normalise so radio buttons always have a valid selection on first render
        var opts = defaults with
        {
            Input = defaults.Input ?? DefaultJournalDir(),
            Start = defaults.Start ?? DateTime.Today.AddDays(-7).ToString("yyyy-MM-dd"),
            Style = NormaliseStyle(defaults.Style),
            Type  = NormaliseType(defaults.Type),
        };

        while (true)
        {
            AnsiConsole.Clear();
            RenderCockpit(opts);

            AnsiConsole.Markup(
                "\n[grey] Select [[1-8]] to edit  ·  [[G]] Generate Report  ·  [[Q]] Quit[/]  > ");
            var key = Console.ReadKey(intercept: true);
            AnsiConsole.WriteLine();

            switch (char.ToUpperInvariant(key.KeyChar))
            {
                case 'G':
                    if (AnsiConsole.Confirm("\n[green]Generate report with these settings?[/]", defaultValue: true))
                        return opts;
                    break;

                case 'Q':
                    AnsiConsole.MarkupLine("[red]Aborted.[/]");
                    Environment.Exit(0);
                    break;

                case '1': opts = opts with { Input          = EditPath("Journal directory",                                          opts.Input)          ?? opts.Input }; break;
                case '2': opts = opts with { Output         = EditText("Output file path (blank = auto-named)",                      opts.Output,         allowEmpty: true) };  break;
                case '3': opts = opts with { Start          = EditDate("Start date (blank = 7 days ago)",                              opts.Start) };         break;
                case '4': opts = opts with { End            = EditDate("End date   (blank = no filter)",                             opts.End) };           break;
                case '5': opts = opts with { Type           = Pick("Report type",   ["summary", "by-system"],          opts.Type  ?? "summary") };         break;
                case '6': opts = opts with { Style          = Pick("Report style",  ["elegant", "colorful", "galnet"], opts.Style ?? "elegant") };         break;
                case '7': opts = opts with { Category       = EditCategorySelection(opts.Category, infographicsBasePath) };                                 break;
                case '8': opts = opts with { Sort           = EditSortOrder(opts.Sort, opts.Category, infographicsBasePath) };                              break;
            }
        }
    }

    // ── Cockpit renderer ──────────────────────────────────────────────────────

    private static void RenderCockpit(ReportOptions opts)
    {
        AnsiConsole.Write(
            new Rule("[gold1] ◆  CMDRS CHRONICLE  —  MISSION CONTROL  ◆ [/]")
                .RuleStyle("gold1")
                .Centered());
        AnsiConsole.WriteLine();

        // Row 1: Navigation | Date Range
        AnsiConsole.Write(new Columns(
            Pane(" NAVIGATION ",
                $"  [grey][[1]][/] Journal Dir  {Val(opts.Input,   "(default ED directory)")}\n" +
                $"  [grey][[2]][/] Output File  {Val(opts.Output,  "(auto-named in current folder)")}"),
            Pane(" DATE RANGE ",
                $"  [grey][[3]][/] Start Date   {Val(opts.Start,   "(7 days ago)")}\n" +
                $"  [grey][[4]][/] End Date     {Val(opts.End,     "(none — up to today)")}")
        ));
        AnsiConsole.WriteLine();

        // Row 2: Mission Config (full width — radio buttons need space)
        AnsiConsole.Write(Pane(" MISSION CONFIG ",
            $"  [grey][[5]][/] Report Type  {Radio(["summary", "by-system"],         opts.Type  ?? "summary")}\n" +
            $"  [grey][[6]][/] Style        {Radio(["elegant", "colorful", "galnet"], opts.Style ?? "elegant")}"));
        AnsiConsole.WriteLine();

        // Row 3: Filters
        AnsiConsole.Write(Pane(" FILTERS ",
            $"  [grey][[7]][/] Category     {Val(opts.Category, "(all categories)")}\n" +
            $"  [grey][[8]][/] Sort Order   {Val(opts.Sort,     "(by metric value)")}"));
    }

    private static Panel Pane(string title, string markup) =>
        new Panel(new Markup(markup))
            .Header($"[yellow]{title}[/]")
            .BorderColor(Color.DarkOrange)
            .Padding(0, 0);

    /// <summary>Cyan for a set value, grey for an unset placeholder.</summary>
    private static string Val(string? value, string placeholder) =>
        string.IsNullOrWhiteSpace(value)
            ? $"[grey]{Markup.Escape(placeholder)}[/]"
            : $"[cyan]{Markup.Escape(Truncate(value, 48))}[/]";

    /// <summary>Gold filled circle for selected option, grey hollow for others.</summary>
    private static string Radio(string[] choices, string selected)
    {
        var parts = choices.Select(c =>
            string.Equals(c, selected, StringComparison.OrdinalIgnoreCase)
                ? $"[gold1]◉ {c}[/]"
                : $"[grey]○ {c}[/]").ToArray();
        return string.Join("  ", parts);
    }

    // ── Field editors (shown below the cockpit after a number is pressed) ─────

    private static string? EditPath(string label, string? current)
    {
        AnsiConsole.WriteLine();
        var prompt = new TextPrompt<string>($"  [yellow]{Markup.Escape(label)}[/]")
            .Validate(v => string.IsNullOrWhiteSpace(v)
                ? ValidationResult.Error("[red]Path cannot be blank[/]")
                : ValidationResult.Success());
        if (!string.IsNullOrEmpty(current)) prompt.DefaultValue(current);
        return AnsiConsole.Prompt(prompt);
    }

    private static string? EditText(string label, string? current, bool allowEmpty)
    {
        AnsiConsole.WriteLine();
        var prompt = new TextPrompt<string>($"  [yellow]{Markup.Escape(label)}[/]");
        if (!string.IsNullOrEmpty(current)) prompt.DefaultValue(current);
        if (allowEmpty) prompt.AllowEmpty();
        return Blank(AnsiConsole.Prompt(prompt));
    }

    private static string? EditDate(string label, string? current)
    {
        AnsiConsole.WriteLine();
        var prompt = new TextPrompt<string>($"  [yellow]{Markup.Escape(label)}[/]")
            .AllowEmpty()
            .Validate(v =>
            {
                if (string.IsNullOrWhiteSpace(v)) return ValidationResult.Success();
                return DateTime.TryParseExact(v, "yyyy-MM-dd",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None, out _)
                    ? ValidationResult.Success()
                    : ValidationResult.Error("[red]Expected format: yyyy-MM-dd[/]");
            });
        if (!string.IsNullOrEmpty(current)) prompt.DefaultValue(current);
        return Blank(AnsiConsole.Prompt(prompt));
    }

    private static string Pick(string label, string[] choices, string current)
    {
        AnsiConsole.WriteLine();
        return AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title($"  [yellow]{Markup.Escape(label)}[/]")
                .AddChoices(PutFirst(choices, current)));
    }

    private static string? EditCategorySelection(string? current, string? infographicsBasePath)
    {
        var categories = LoadCategories(infographicsBasePath);
        if (categories.Count == 0)
            return EditText("Categories (comma-separated, blank = all)", current, allowEmpty: true);

        var currentSet = current is null
            ? new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            : new HashSet<string>(
                current.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries),
                StringComparer.OrdinalIgnoreCase);

        AnsiConsole.WriteLine();
        var prompt = new MultiSelectionPrompt<string>()
            .Title("  [yellow]Select categories[/] [grey](Space=toggle · Enter=confirm · none=all)[/]")
            .InstructionsText("[grey](Use arrow keys to navigate, [blue]<space>[/] to toggle, [green]<enter>[/] to confirm)[/]")
            .AddChoices(categories);
        prompt.Required = false; // allow zero selections = all

        foreach (var cat in categories.Where(c => currentSet.Contains(c)))
            prompt.Select(cat);

        var selected = AnsiConsole.Prompt(prompt);
        return selected.Count == 0 ? null : string.Join(",", selected);
    }

    private static string? EditSortOrder(string? current, string? categoryFilter, string? infographicsBasePath)
    {
        var allCategories = LoadCategories(infographicsBasePath);
        if (allCategories.Count == 0)
            return EditText("Sort order (comma-separated categories, blank = default)", current, allowEmpty: true);

        // Limit to the active category filter when one is set
        var filterParts = string.IsNullOrWhiteSpace(categoryFilter)
            ? null
            : categoryFilter.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var available = filterParts is null
            ? allCategories
            : allCategories
                .Where(c => filterParts.Any(f => string.Equals(f, c, StringComparison.OrdinalIgnoreCase)))
                .ToList();

        if (available.Count == 0)
            available = allCategories;

        AnsiConsole.WriteLine();

        // Show a numbered table of available categories
        var table = new Table()
            .BorderColor(Color.Grey)
            .Border(TableBorder.Rounded)
            .AddColumn(new TableColumn("[grey] # [/]").Centered())
            .AddColumn(new TableColumn("[grey]Category[/]"));
        for (int i = 0; i < available.Count; i++)
            table.AddRow($"[grey]{i + 1,2}[/]", available[i]);
        AnsiConsole.Write(table);

        // Translate existing sort names → numbers for the default value
        var currentDefault = string.Empty;
        if (!string.IsNullOrWhiteSpace(current))
        {
            var nums = current
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(c =>
                {
                    var idx = available.FindIndex(a => string.Equals(a, c, StringComparison.OrdinalIgnoreCase));
                    return idx >= 0 ? (idx + 1).ToString() : null;
                })
                .Where(x => x != null);
            currentDefault = string.Join(" ", nums);
        }

        var orderPrompt = new TextPrompt<string>(
                "  [yellow]Type category numbers in display order[/] [grey](e.g. [white]3 1 2[/] · blank = default by metric)[/]")
            .AllowEmpty()
            .Validate(v =>
            {
                if (string.IsNullOrWhiteSpace(v)) return ValidationResult.Success();
                foreach (var part in v.Split(' ', ',', StringSplitOptions.RemoveEmptyEntries))
                    if (!int.TryParse(part, out var n) || n < 1 || n > available.Count)
                        return ValidationResult.Error($"[red]'{Markup.Escape(part)}' is not valid (1–{available.Count})[/]");
                return ValidationResult.Success();
            });
        if (!string.IsNullOrEmpty(currentDefault)) orderPrompt.DefaultValue(currentDefault);

        var result = AnsiConsole.Prompt(orderPrompt);
        if (string.IsNullOrWhiteSpace(result)) return null;

        var ordered = result
            .Split(' ', ',', StringSplitOptions.RemoveEmptyEntries)
            .Select(p => int.TryParse(p, out var n) && n >= 1 && n <= available.Count ? available[n - 1] : null)
            .Where(c => c != null)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        return ordered.Count == 0 ? null : string.Join(",", ordered);
    }

    // ── Category discovery ────────────────────────────────────────────────────

    private static List<string> LoadCategories(string? infographicsBasePath)
    {
        if (string.IsNullOrWhiteSpace(infographicsBasePath))
        {
            return [];
        }
        if (!Directory.Exists(infographicsBasePath))
        {
            return [];
        }
        try
        {
            return InfographicLoader.LoadAll(infographicsBasePath)
                .Select(d => d.Category)
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(c => c, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }
        catch (Exception)
        {
            return [];
        }
    }

    // ── Utilities ─────────────────────────────────────────────────────────────

    private static string DefaultJournalDir()
    {
        var up = Environment.GetEnvironmentVariable("USERPROFILE") ?? string.Empty;
        return Path.Combine(up, "Saved Games", "Frontier Developments", "Elite Dangerous");
    }

    private static string Truncate(string s, int max) =>
        s.Length <= max ? s : "\u2026" + s[^(max - 1)..];

    private static string[] PutFirst(string[] choices, string preferred)
    {
        var list = new System.Collections.Generic.List<string>(choices);
        var idx  = list.FindIndex(c => string.Equals(c, preferred, StringComparison.OrdinalIgnoreCase));
        if (idx > 0) { list.RemoveAt(idx); list.Insert(0, preferred); }
        return list.ToArray();
    }

    private static string? Blank(string? s) => string.IsNullOrWhiteSpace(s) ? null : s;

    private static string NormaliseType(string? t) =>
        string.Equals(t, "by-system", StringComparison.OrdinalIgnoreCase) ? "by-system" : "summary";

    private static string NormaliseStyle(string? s)
    {
        if (string.Equals(s, "colorful", StringComparison.OrdinalIgnoreCase)) return "colorful";
        if (string.Equals(s, "galnet",   StringComparison.OrdinalIgnoreCase)) return "galnet";
        return "elegant";
    }

    // ── Report generation progress display ───────────────────────────────────

    internal static void ShowGenerationHeader()
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(
            new Rule("[gold1] ◆  CMDRS CHRONICLE  —  GENERATING REPORT  ◆ [/]")
                .RuleStyle("gold1")
                .Centered());
        AnsiConsole.WriteLine();
    }

    internal static void ShowStep(string label) =>
        AnsiConsole.MarkupLine($"  [grey]▸[/]  [white]{Markup.Escape(label)}[/]");

    internal static void ShowStepDone(string detail) =>
        AnsiConsole.MarkupLine($"  [green]✓[/]  [grey]{Markup.Escape(detail)}[/]");

    internal static void ShowDbInsertProgress(int done, int total) =>
        ShowProgress(done, total, "");

    internal static void ShowVisitProgress(int done, int total) =>
        ShowProgress(done, total, " systems");

    private static void ShowProgress(int done, int total, string unit)
    {
        const int width = 28;
        var filled = total > 0 ? (int)Math.Round(done * (double)width / total) : width;
        filled = Math.Clamp(filled, 0, width);
        var bar = new string('█', filled) + new string('░', width - filled);
        var pct = total > 0 ? (int)(done * 100.0 / total) : 100;
        Console.Write($"\r  ▐{bar}▌ {pct,3}%  ({done:N0} / {total:N0}{unit})    ");
    }

    internal static void ShowComplete(string outputPath)
    {
        AnsiConsole.WriteLine();
        AnsiConsole.Write(
            new Panel($"[cyan]{Markup.Escape(outputPath)}[/]")
                .Header("[green] ✓  REPORT READY [/]")
                .BorderColor(Color.Green));
        AnsiConsole.MarkupLine("\n[grey]  Press any key to exit...[/]");
        Console.ReadKey(intercept: true);
    }
}
