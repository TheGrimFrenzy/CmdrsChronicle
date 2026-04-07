using System.CommandLine;
using CmdrsChronicle.Core;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace CmdrsChronicle.Cli
{
		/// <summary>
		/// CLI entry point. Defines and wires up all command-line options, then dispatches
		/// to the report-generation pipeline via <see cref="BuildRootCommand"/>.
		/// </summary>
		public class Program
		{
			// Helper: Try parse environment variable
			static int? TryParseEnvVar(string varName)
			{
				var val = Environment.GetEnvironmentVariable(varName);
				return int.TryParse(val, out var result) ? result : null;
			}

			// Helper: Try parse config file (optional, looks for config.json in current dir)
			static int? TryParseConfig(string key)
			{
				var configPath = Path.Combine(Directory.GetCurrentDirectory(), "config.json");
				if (!File.Exists(configPath)) return null;
				try
				{
					var json = System.Text.Json.JsonDocument.Parse(File.ReadAllText(configPath));
					if (json.RootElement.TryGetProperty(key, out var prop) && prop.ValueKind == System.Text.Json.JsonValueKind.Number)
						return prop.GetInt32();
				}
				catch { }
				return null;
			}

			// Date formatting now handled by Core helpers

			/// <summary>
			/// Constructs the <see cref="RootCommand"/> with all supported options wired up.
			/// Exposed as a static method so it can be exercised in integration tests without
			/// actually invoking the process entry point.
			/// </summary>
			/// <returns>A fully configured <see cref="RootCommand"/> ready to be invoked.</returns>
			public static RootCommand BuildRootCommand()
			{
				var inputOpt = new Option<string>("--input", "Path to the directory containing journal log files.");
				var outputOpt = new Option<string>("--output", "Path to write the generated HTML report.");
				var startOpt = new Option<string>("--start", "Start date (yyyy-MM-dd) for report filtering.");
				var endOpt = new Option<string>("--end", "End date (yyyy-MM-dd) for report filtering.");
				var typeOpt = new Option<string>("--type", "Report type: summary (default) or by-system.");
				var categoryOpt = new Option<string>("--category", "Infographic category filter (comma-separated).");
				var styleOpt = new Option<string>("--style", "Report style: elegant (default), colorful, or galnet.");
				var sortOpt = new Option<string>("--sort", "Comma-separated list of categories to control infographic sort order. If provided, infographics are sorted by category order (descending metric within each). If not provided, all infographics are sorted by metric ascending.");
				var maxParallelismOpt = new Option<int?>("--max-parallelism", "Maximum number of files to parse in parallel (overrides environment/config).");

			var interactiveOpt = new Option<bool>("--interactive", "Launch interactive mode to review and edit all options before generating the report. Any options supplied on the command line are used as defaults in the UI.");
			var silentOpt      = new Option<bool>("--silent",      "Suppress all console output except the final report path. Intended for batch/scripted use.");

			var rootCommand = new RootCommand("CmdrsChronicle CLI - Generate self-contained Elite Dangerous HTML reports.");
			rootCommand.Add(inputOpt);
			rootCommand.Add(outputOpt);
			rootCommand.Add(startOpt);
			rootCommand.Add(endOpt);
			rootCommand.Add(typeOpt);
			rootCommand.Add(categoryOpt);
			rootCommand.Add(styleOpt);
			rootCommand.Add(sortOpt);
			rootCommand.Add(maxParallelismOpt);
			rootCommand.Add(interactiveOpt);
			rootCommand.Add(silentOpt);

			rootCommand.SetHandler((System.CommandLine.Invocation.InvocationContext ctx) =>
			{
				var input          = ctx.ParseResult.GetValueForOption(inputOpt);
				var output         = ctx.ParseResult.GetValueForOption(outputOpt);
				var start          = ctx.ParseResult.GetValueForOption(startOpt);
				var end            = ctx.ParseResult.GetValueForOption(endOpt);
				var type           = ctx.ParseResult.GetValueForOption(typeOpt);
				var category       = ctx.ParseResult.GetValueForOption(categoryOpt);
				var style          = ctx.ParseResult.GetValueForOption(styleOpt);
				var sort           = ctx.ParseResult.GetValueForOption(sortOpt);
				var maxParallelism = ctx.ParseResult.GetValueForOption(maxParallelismOpt);
				var interactive    = ctx.ParseResult.GetValueForOption(interactiveOpt);
				var silent         = ctx.ParseResult.GetValueForOption(silentOpt);

				// If --interactive, show TUI wizard pre-populated with any CLI-supplied values.
				if (interactive)
				{
					var cliOpts  = new ReportOptions(input, output, start, end, type, category, style, sort, maxParallelism);
					var resolved = InteractiveSetup.Run(cliOpts, InfographicsBase());
					input          = resolved.Input;
					output         = resolved.Output;
					start          = resolved.Start;
					end            = resolved.End;
					type           = resolved.Type;
					category       = resolved.Category;
					style          = resolved.Style;
					sort           = resolved.Sort;
					maxParallelism = resolved.MaxParallelism;
				}

				if (interactive) InteractiveSetup.ShowGenerationHeader();
				void Step(string label)        { if (!silent) InteractiveSetup.ShowStep(label); }
				void StepDone(string detail)   { if (!silent) InteractiveSetup.ShowStepDone(detail); }
				void DbProgress(int n, int t)  { if (!silent) InteractiveSetup.ShowDbInsertProgress(n, t); }
				void VisitProg(int n, int t)   { if (!silent) InteractiveSetup.ShowVisitProgress(n, t); }

					if (string.IsNullOrWhiteSpace(input))
					{
						var userProfile = Environment.GetEnvironmentVariable("USERPROFILE") ?? "";
						input = Path.Combine(userProfile, "Saved Games", "Frontier Developments", "Elite Dangerous");
					}


// Parse --start / --end BEFORE file discovery so we can pre-filter by filename date.
					// endDate is bumped to end-of-day so events with timestamps up to 23:59:59 are included.
					string? tagline = null;
					DateTime? startDate = null, endDate = null;
					if (!string.IsNullOrWhiteSpace(start))
					{
						if (DateTime.TryParse(start, out var dt)) startDate = dt.Date;
						else Console.Error.WriteLine($"[WARN] Could not parse --start date: {start}");
					}
					else
					{
						startDate = DateTime.Today.AddDays(-7);
					}
					if (!string.IsNullOrWhiteSpace(end))
					{
						if (DateTime.TryParse(end, out var dt)) endDate = dt.Date.AddDays(1).AddTicks(-1);
						else Console.Error.WriteLine($"[WARN] Could not parse --end date: {end}");
					}

					// Now that startDate/endDate are known, select the report tagline.
					// For by-system reports a dynamic tagline is generated later; for all other styles pick one now.
					if (!string.Equals(type, "by-system", StringComparison.OrdinalIgnoreCase) || !startDate.HasValue || !endDate.HasValue)
					{
						var taglinesPath = Path.Combine(AppContext.BaseDirectory, "templates", "taglines.txt");
						if (!File.Exists(taglinesPath))
							taglinesPath = Path.Combine(Directory.GetCurrentDirectory(), "templates", "taglines.txt");
						tagline = Report.SelectRandomTagline(taglinesPath);
					}

					// ?? is the null-coalescing operator: use the first non-null value in priority order.
					// Priority: explicit flag → environment variable → config file → CPU count.
					int effectiveParallelism = maxParallelism
						?? TryParseEnvVar("CMDRSCHRONICLE_MAX_PARALLELISM")
						?? TryParseConfig("maxParallelism")
						?? Environment.ProcessorCount;

					if (!Directory.Exists(input))
					{
						Console.Error.WriteLine($"ERROR: Input directory does not exist: {input}");
						Environment.Exit(1);
					}

					var journalFileCount = JournalFileDiscovery.DiscoverJournalFiles(input, startDate, endDate).Count;
					Step($"Parsing {journalFileCount:N0} journal file{(journalFileCount == 1 ? "" : "s")}...");
					var (events, errors) = JournalFileDiscovery.ParseJournalFilesParallel(input, effectiveParallelism, startDate, endDate);
				StepDone($"Parsed {events.Count:N0} events" + (errors.Count > 0 ? $" ({errors.Count} parse error{(errors.Count == 1 ? "" : "s")})" : ""));
					var filteredEvents = new List<System.Text.Json.JsonElement>();						foreach (var evt in events)
						{
							if (!evt.TryGetProperty("timestamp", out var timestampElem)) continue;
							if (!DateTime.TryParse(timestampElem.GetString(), out var ts)) continue;
							if (startDate.HasValue && ts < startDate.Value) continue;
							if (endDate.HasValue && ts > endDate.Value) continue;
							filteredEvents.Add(evt);
						}
					// T303: Insert filtered events into in-memory SQLite DB
					// Prefer schema in app base, then in app base Schema subfolder, then repo root fallback
					var schemaPath = Path.Combine(AppContext.BaseDirectory, "cmdrschronicle_schema.sql");
					if (!File.Exists(schemaPath))
					{
						schemaPath = Path.Combine(AppContext.BaseDirectory, "Schema", "cmdrschronicle_schema.sql");
					}
					if (!File.Exists(schemaPath))
					{
						// Try repo root fallback
						schemaPath = Path.Combine(Directory.GetCurrentDirectory(), "cmdrschronicle_schema.sql");
					}
					if (!File.Exists(schemaPath))
					{
						Console.Error.WriteLine($"ERROR: Schema file not found: {schemaPath}");
						Environment.Exit(1);
					}
					// ":N" format removes hyphens from the GUID, producing a plain alphanumeric identifier
					// safe to use as a SQLite database name. Each run gets its own unique name so
					// concurrent invocations don't share data.
					var dbName = $"reportdb_{Guid.NewGuid():N}";
					// SQLite "shared-cache URI" mode: every connection in this process that opens the
					// same named in-memory DB sees the same data. This lets parallel query tasks open
					// their own connections without going through a single locked connection.
					using var conn = SqliteSchemaInitializer.CreateSharedInMemoryDb(schemaPath, dbName);

					// Build PRAGMA cache once per unique event type (avoid repeating per-event)
					var pragmaCache = new Dictionary<string, Dictionary<string, (string Type, bool NotNull)>>(StringComparer.OrdinalIgnoreCase);
					foreach (var evt0 in filteredEvents)
					{
						if (!evt0.TryGetProperty("event", out var evtElem0)) continue;
						var evtName = evtElem0.GetString();
						if (string.IsNullOrWhiteSpace(evtName) || pragmaCache.ContainsKey(evtName)) continue;
						var info = new Dictionary<string, (string Type, bool NotNull)>();
						using (var pragmaCmd = conn.CreateCommand())
						{
							pragmaCmd.CommandText = $"PRAGMA table_info([{evtName}]);";
							using var r = pragmaCmd.ExecuteReader();
							while (r.Read())
							{
								var colName = r.GetString(1);
								var colType = r.IsDBNull(2) ? "" : r.GetString(2);
								var notNull = !r.IsDBNull(3) && r.GetInt32(3) == 1;
								info[colName] = (colType ?? "", notNull);
							}
						}
						pragmaCache[evtName] = info;
					}

						Step($"Inserting {filteredEvents.Count:N0} events into database...");
						int inserted = 0;
						using (var tx = conn.BeginTransaction())
						{
						foreach (var evt in filteredEvents)
						{
							if (!evt.TryGetProperty("event", out var eventTypeElem) || !evt.TryGetProperty("timestamp", out var timestampElem))
								continue;
							var eventType = eventTypeElem.GetString();
							if (string.IsNullOrWhiteSpace(eventType)) continue;
							var table = eventType;
							if (!pragmaCache.TryGetValue(table, out var tableColumnsInfo))
								continue; // table not in schema
							var columns = new List<string>();
							var columnSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
							var values = new List<object>();
							foreach (var prop in evt.EnumerateObject())
							{
								if (prop.Name == "event") continue; // do not store event property
								var col = ReservedWords.PrefixIfReserved(prop.Name);
								if (!tableColumnsInfo.ContainsKey(col)) continue; // only insert valid columns
								if (!columnSet.Add(col)) continue; // skip duplicate column names
								columns.Add(col);
								values.Add(prop.Value.ValueKind == System.Text.Json.JsonValueKind.String ? (object)prop.Value.GetString()! : prop.Value.ToString());
							}

							foreach (var kv in tableColumnsInfo)
							{
								var colName = kv.Key;
								var colType = kv.Value.Type ?? "";
								var notNull = kv.Value.NotNull;
								if (!notNull) continue;
								if (columnSet.Contains(colName)) continue;
								// Skip primary key autoincrement column
								if (string.Equals(colName, "event_id", StringComparison.OrdinalIgnoreCase)) continue;
								// Provide defaults: integers -> 0, real -> 0.0, otherwise empty string
								object defaultVal;
								if (colType.IndexOf("INT", StringComparison.OrdinalIgnoreCase) >= 0)
									defaultVal = 0;
								else if (colType.IndexOf("REAL", StringComparison.OrdinalIgnoreCase) >= 0 || colType.IndexOf("FLOA", StringComparison.OrdinalIgnoreCase) >= 0 || colType.IndexOf("DOUB", StringComparison.OrdinalIgnoreCase) >= 0)
									defaultVal = 0.0;
								else
									defaultVal = "";
								columnSet.Add(colName);
								columns.Add(colName);
								values.Add(defaultVal);
							}
							if (columns.Count == 0) continue; // nothing to insert
							var colList = string.Join(", ", columns);
							var paramList = string.Join(", ", columns.ConvertAll(c => "@" + c));
							var sql = $"INSERT INTO [{table}] ({colList}) VALUES ({paramList});";
							using var cmd = conn.CreateCommand();
							cmd.CommandText = sql;
							for (int i = 0; i < columns.Count; i++)
								cmd.Parameters.AddWithValue("@" + columns[i], values[i] ?? DBNull.Value);
							try
							{
								cmd.ExecuteNonQuery();
								inserted++;
								if (inserted % 500 == 0 || inserted == filteredEvents.Count)
									DbProgress(inserted, filteredEvents.Count);
							}
							catch (Exception ex)
							{
								Console.Error.WriteLine($"[DB ERROR] {ex.Message}");
							}
						}
						tx.Commit();
						} // end transaction
					if (!silent) Console.WriteLine(); // advance past \r progress line
					StepDone($"Inserted {inserted:N0} events into database");
					string TemplateBase() => Directory.Exists(Path.Combine(AppContext.BaseDirectory, "templates"))
						? Path.Combine(AppContext.BaseDirectory, "templates")
						: Path.Combine(Directory.GetCurrentDirectory(), "templates");

					string InfographicsBase() => Directory.Exists(Path.Combine(AppContext.BaseDirectory, "infographics"))
						? Path.Combine(AppContext.BaseDirectory, "infographics")
						: Path.Combine(Directory.GetCurrentDirectory(), "infographics");

					bool isColorful = string.Equals(style, "colorful", StringComparison.OrdinalIgnoreCase);
					bool isGalnet   = string.Equals(style, "galnet",   StringComparison.OrdinalIgnoreCase);

					string BuildDefaultName()
					{
						var s = (startDate ?? DateTime.Today).ToString("yyyy-MM-dd");
						var e = (endDate ?? DateTime.Today).Date.ToString("yyyy-MM-dd");
						return $"CmdrsChronicle_{s}-{e}.html";
					}

					// T305: No-data report logic
					if (filteredEvents.Count == 0)
					{
						Step("Generating nothing-to-report page...");

						// Locate template and CSS files
						var templateFile = Path.Combine(TemplateBase(), isGalnet ? "galnet-nothing-to-report.html" : isColorful ? "colorful-nothing-to-report.html" : "elegant-nothing-to-report.html");
						var cssFile      = Path.Combine(TemplateBase(), isGalnet ? "galnet.css" : isColorful ? "colorful.css" : "elegant.css");

						if (!File.Exists(templateFile) || !File.Exists(cssFile))
						{
							Console.Error.WriteLine($"[ERROR] Cannot render nothing-to-report page: template or CSS not found.");
							Console.Error.WriteLine($"  Template: {templateFile}");
							Console.Error.WriteLine($"  CSS:      {cssFile}");
							Environment.Exit(1);
						}

						// Load and select message by ordinal day
						var messagesFile = Path.Combine(TemplateBase(), "no-data-messages.json");
						var messages = NoDataMessageSelector.LoadMessages(messagesFile);

						// Use --end date if given, otherwise today
						var reportDate = endDate.HasValue ? endDate.Value.Date : DateTime.Today;
						var selected = NoDataMessageSelector.SelectByDate(messages, reportDate)
							?? new NoDataMessage
							{
								Title       = "Nothing to Report",
								Summary     = "No qualifying activity was found for this period.",
								Body        = "All data channels returned below qualifying thresholds for {cmdrName} during this window.",
								ClosingNote = "Last known location: {lastSystem} on {lastDate}."
							};

							// Resolve cmdr name and last-known system from most-recent LoadGame/Commander event before start date
							string cmdrName   = "Unknown Commander";
							string lastSystem = "Unknown System";
							string lastDate   = "Unknown Date";

							using (var cmdrCmd = conn.CreateCommand())
							{
								// Try LoadGame table first (has Commander + StarSystem columns)
								cmdrCmd.CommandText = @"
									SELECT Commander, StarSystem, event_timestamp
									FROM LoadGame
									ORDER BY event_timestamp DESC
									LIMIT 1;";
								try
								{
									using var r = cmdrCmd.ExecuteReader();
									if (r.Read())
									{
										if (!r.IsDBNull(0)) cmdrName   = r.GetString(0);
										if (!r.IsDBNull(1)) lastSystem = r.GetString(1);
										if (!r.IsDBNull(2))
											// ParseLoreDate returns null when the timestamp can't be
											// parsed; ?? keeps the existing lastDate value in that case.
											lastDate = ReportHelpers.ParseLoreDate(r.GetString(2)) ?? lastDate;
									}
								}
								catch { /* LoadGame table may not exist or have these columns */ }
							}

							// If DB yielded no useful info, try reading the most recent journal file before the start date
							if ((cmdrName == "Unknown Commander" || lastSystem == "Unknown System" || lastDate == "Unknown Date") && Directory.Exists(input))
							{
								try
								{
									var (foundSystem, foundDate, foundCmdr) = ReportHelpers.FindMostRecentStarSystem(input, startDate?.Date);
									if (!string.IsNullOrEmpty(foundSystem) && foundSystem != "Unknown System") lastSystem = foundSystem;
									if (!string.IsNullOrEmpty(foundDate)   && foundDate   != "Unknown Date")   lastDate   = foundDate;
									if (!string.IsNullOrEmpty(foundCmdr)   && foundCmdr   != "Unknown Commander") cmdrName = foundCmdr;
								}
								catch { }
							}

							// Interpolate tokens
							selected.Body        = NoDataMessageSelector.Interpolate(selected.Body,        cmdrName, lastSystem, lastDate);
							selected.ClosingNote = NoDataMessageSelector.Interpolate(selected.ClosingNote, cmdrName, lastSystem, lastDate);

						// Format date range for masthead
						var fromStr = startDate.HasValue ? ReportHelpers.FormatLoreDate(startDate.Value) : "—";
						var toStr   = endDate.HasValue   ? ReportHelpers.FormatLoreDate(endDate.Value.Date) : ReportHelpers.FormatLoreDate(DateTime.Today);

						var html = isGalnet
							? GalnetReportRenderer.RenderNothingToReport(
								templateFile, cssFile,
								tagline ?? "Every jump tells a story.",
								cmdrName, fromStr, toStr,
								selected)
							: isColorful
							? ColorfulReportRenderer.RenderNothingToReport(
								templateFile, cssFile,
								tagline ?? "Every jump tells a story.",
								cmdrName, fromStr, toStr,
								selected)
							: ElegantReportRenderer.RenderNothingToReport(
								templateFile, cssFile,
								tagline ?? "Every jump tells a story.",
								cmdrName, fromStr, toStr,
								selected);

						var outputPath = string.IsNullOrWhiteSpace(output)
							? Path.Combine(Directory.GetCurrentDirectory(), BuildDefaultName())
							: output;

						File.WriteAllText(outputPath, html, System.Text.Encoding.UTF8);
						var ntrErrorComment = ReportDiagnostics.FormatParseErrorComment(errors);
						if (ntrErrorComment.Length > 0)
							File.AppendAllText(outputPath, ntrErrorComment, System.Text.Encoding.UTF8);
						if (interactive) InteractiveSetup.ShowComplete(outputPath);
						else Console.WriteLine(outputPath);
						if (!silent)
						{
							try { System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(outputPath) { UseShellExecute = true }); }
							catch { /* Default browser may not be configured; report is still written to disk */ }
						}
						return;
					}

					// T304: Full report generation (qualifying events found)
					var definitions = InfographicLoader.LoadAll(InfographicsBase());

					// Apply --category filter if provided
					if (!string.IsNullOrWhiteSpace(category))
					{
						var cats = category.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
						definitions = definitions
							.Where(d => cats.Any(c => string.Equals(c, d.Category, StringComparison.OrdinalIgnoreCase)))
							.ToList();
					}

					// Apply --sort: pre-order definitions by category so parallel queries run in roughly
					// the right bucket order; final sort by MainValue happens after queries complete.
					List<string>? sortCatsList = null;
					// Local function: maps a category string to its position in the user-supplied --sort list.
					// Returns int.MaxValue for any category not present in the list, which LINQ's OrderBy
					// uses to push those items to the end of the sorted sequence.
					int CategorySortKey(string category)
					{
						if (sortCatsList is null) return 0; // no --sort: all categories are equivalent
						var idx = sortCatsList.FindIndex(c => string.Equals(c, category, StringComparison.OrdinalIgnoreCase));
						return idx >= 0 ? idx : int.MaxValue; // unlisted categories float to the very end
					}
					if (!string.IsNullOrWhiteSpace(sort))
					{
						sortCatsList = sort.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
						definitions = definitions
							.OrderBy(d => CategorySortKey(d.Category))
							.ToList();
					}

					// Date strings for SQL: startDate inclusive, endDate exclusive (< comparison against ISO timestamps).
					// When --end is omitted, use tomorrow so today's events are included.
					var queryStartDate = startDate?.Date.ToString("yyyy-MM-dd");
					var queryEndDate   = endDate.HasValue
						? endDate.Value.Date.ToString("yyyy-MM-dd")
						: DateTime.Today.AddDays(1).ToString("yyyy-MM-dd");

					// Parallel query phase: gather all results before rendering
					Step("Running infographic queries...");
					var rawResults = InfographicQueryRunner.RunAllAsync(
						definitions, dbName, queryStartDate, queryEndDate, effectiveParallelism
					).GetAwaiter().GetResult();

					// Sort results by MainValue (the primary query metric) now that we have real values.
					// If --sort was given: category order first, then MainValue descending within each category.
					// Otherwise: MainValue descending across all tiles.
					IReadOnlyList<InfographicQueryResult> results;
					if (sortCatsList != null)
					{
						results = rawResults
							.OrderBy(r => CategorySortKey(r.Definition.Category))
							.ThenByDescending(r => r.MainValue)
							.ToList();
					}
					else
					{
						results = rawResults.OrderByDescending(r => r.MainValue).ToList();
					}

					// Print slowest tiles for diagnostics
					var totalConsidered = results.Count;
					var summaryOnlyCount = results.Count(r => r.Definition.SummaryOnly);
					var qualifyingCount = results.Count(r => !r.Definition.SummaryOnly && r.MeetsThreshold);
					var metricDisqualifiedCount = results.Count(r => !r.Definition.SummaryOnly && !r.MeetsThreshold);
					   StepDone($"{qualifyingCount} qualifying (out of {totalConsidered}), {metricDisqualifiedCount} disqualified (metric threshold), {summaryOnlyCount} disqualified (summary-only)");

					// Resolve CMDR name from DB (best-effort)
					string reportCmdrName = "Unknown Commander";
					using (var cmdrLookup = conn.CreateCommand())
					{
						cmdrLookup.CommandText = "SELECT Commander FROM LoadGame ORDER BY event_timestamp DESC LIMIT 1;";
						try
						{
							var val = cmdrLookup.ExecuteScalar();
							if (val != null && val != DBNull.Value) reportCmdrName = val.ToString()!;
						}
						catch { }
					}

					var reportFromStr = startDate.HasValue ? ReportHelpers.FormatLoreDate(startDate.Value) : "\u2014";
					var reportToStr   = endDate.HasValue   ? ReportHelpers.FormatLoreDate(endDate.Value.Date) : ReportHelpers.FormatLoreDate(DateTime.Today);

					var reportTemplatePath = Path.Combine(TemplateBase(), isGalnet ? "galnet-report.html" : isColorful ? "colorful-report.html" : "elegant-report.html");
					var reportCssPath      = Path.Combine(TemplateBase(), isGalnet ? "galnet.css" : isColorful ? "colorful.css" : "elegant.css");

					if (!File.Exists(reportTemplatePath) || !File.Exists(reportCssPath))
					{
						Console.Error.WriteLine($"[ERROR] Report template or CSS not found: {reportTemplatePath}");
						Environment.Exit(1);
					}

					// ── Build report sections (by-system or single) ─────────────────────
					IReadOnlyList<SystemVisit> sections;
					if (string.Equals(type, "by-system", StringComparison.OrdinalIgnoreCase) && queryStartDate != null)
					{
						// Exclude infographics that are only meaningful in a summary report.
						definitions = definitions.Where(d => !d.SummaryOnly).ToList();

						// Walk FSD jumps chronologically to build per-visit windows
						var jumps = new List<(string SystemName, string Timestamp)>();
						using (var jumpCmd = conn.CreateCommand())
						{
							jumpCmd.CommandText =
								"SELECT StarSystem, event_timestamp FROM FSDJump " +
								"WHERE event_timestamp >= @s AND event_timestamp < @e " +
								"ORDER BY event_timestamp ASC";
							jumpCmd.Parameters.AddWithValue("@s", queryStartDate);
							jumpCmd.Parameters.AddWithValue("@e", queryEndDate);
							using var jr = jumpCmd.ExecuteReader();
							while (jr.Read())
								jumps.Add((jr.GetString(0), jr.GetString(1)));
						}

						var visits = new List<(string SystemName, string From, string To)>();
						if (jumps.Count > 0)
						{
							// Include any pre-jump activity: find what system CMDR was in before window start
							using (var priorCmd = conn.CreateCommand())
							{
								priorCmd.CommandText =
									"SELECT StarSystem FROM FSDJump WHERE event_timestamp < @s " +
									"ORDER BY event_timestamp DESC LIMIT 1";
								priorCmd.Parameters.AddWithValue("@s", queryStartDate);
								var priorVal = priorCmd.ExecuteScalar();
								if (priorVal != null && priorVal != DBNull.Value)
									visits.Add((priorVal.ToString()!, queryStartDate, jumps[0].Timestamp));
							}

							for (int i = 0; i < jumps.Count; i++)
							{
								var toTime = i + 1 < jumps.Count ? jumps[i + 1].Timestamp : queryEndDate;
								visits.Add((jumps[i].SystemName, jumps[i].Timestamp, toTime));
							}
						}
						else
						{
							// No FSDJumps in window: look for prior system and create a visit for the whole window
							using (var priorCmd = conn.CreateCommand())
							{
								priorCmd.CommandText =
									"SELECT StarSystem FROM FSDJump WHERE event_timestamp < @s " +
									"ORDER BY event_timestamp DESC LIMIT 1";
								priorCmd.Parameters.AddWithValue("@s", queryStartDate);
								var priorVal = priorCmd.ExecuteScalar();
								if (priorVal != null && priorVal != DBNull.Value)
									visits.Add((priorVal.ToString()!, queryStartDate, queryEndDate));
							}
						}

						if (visits.Count > 0)
						{
							Step($"Querying {visits.Count} system visit{(visits.Count == 1 ? "" : "s")} in parallel...");
							// Pre-sized array preserves chronological order even though visits run concurrently.
							var visitSections = new SystemVisit[visits.Count];
							var visitsDone    = new int[1]; // int[] so the lambda can Interlocked.Increment it
							using var visitSem = new System.Threading.SemaphoreSlim(Math.Max(1, effectiveParallelism));
							var visitTasks = visits.Select(async (v, i) =>
							{
								await visitSem.WaitAsync().ConfigureAwait(false);
								try
								{
									// Each visit runs its tile queries sequentially (concurrency=1); visit-level
									// parallelism is controlled by visitSem above, capped at effectiveParallelism.
									var visitRaw = await InfographicQueryRunner.RunAllAsync(
										definitions, dbName, v.From, v.To, 1
									).ConfigureAwait(false);

									IReadOnlyList<InfographicQueryResult> visitResults;
									if (sortCatsList != null)
									{
										visitResults = visitRaw
										.OrderBy(r => CategorySortKey(r.Definition.Category))
											.ThenByDescending(r => r.MainValue)
											.ToList();
									}
									else
									{
										visitResults = visitRaw.OrderByDescending(r => r.MainValue).ToList();
									}

									DateTime? arr = DateTime.TryParse(v.From, null,
										System.Globalization.DateTimeStyles.RoundtripKind, out var parsedArr)
										? parsedArr : (DateTime?)null;
									var arrivalLore   = arr.HasValue ? ReportHelpers.FormatLoreDate(arr.Value) + " " + arr.Value.ToString("HH:mm") : null;
									var arrivalActual = arr.HasValue ? arr.Value.ToString("yyyy-MM-dd HH:mm") : null;
									visitSections[i] = new SystemVisit(v.SystemName, arrivalLore, arrivalActual, visitResults);

								var done = System.Threading.Interlocked.Increment(ref visitsDone[0]);
								VisitProg(done, visits.Count);
								}
								finally { visitSem.Release(); }
							}).ToList();
							System.Threading.Tasks.Task.WhenAll(visitTasks).GetAwaiter().GetResult();

							var visitSectionsList = visitSections.ToList();
							StepDone($"{visitSectionsList.Count} system visit{(visitSectionsList.Count == 1 ? "" : "s")} ready");
							sections = visitSectionsList.Count > 0
								? visitSectionsList
								: new[] { new SystemVisit(null, null, null, results) };

							// Set custom masthead for by-system report
							if (string.Equals(type, "by-system", StringComparison.OrdinalIgnoreCase) && startDate.HasValue && endDate.HasValue)
							{
								var systemCount = visitSectionsList.Count;
								var days = (endDate.Value.Date - startDate.Value.Date).Days + 1;
								tagline = $"{systemCount} System{(systemCount == 1 ? "" : "s")} in {days} Day{(days == 1 ? "" : "s")}";

							}
						}
						else
						{
							sections = new[] { new SystemVisit(null, null, null, results) };
						}
					}
					else
					{
						sections = new[] { new SystemVisit(null, null, null, results) };
					}

					Step("Rendering HTML report...");
					var reportHtml = isGalnet
						? GalnetReportRenderer.Render(
							reportTemplatePath, reportCssPath,
							tagline ?? "Every jump tells a story.",
							reportCmdrName, reportFromStr, reportToStr,
							sections)
						: isColorful
						? ColorfulReportRenderer.Render(
							reportTemplatePath, reportCssPath,
							tagline ?? "Every jump tells a story.",
							reportCmdrName, reportFromStr, reportToStr,
							sections)
						: ElegantReportRenderer.Render(
							reportTemplatePath, reportCssPath,
							tagline ?? "Every jump tells a story.",
							reportCmdrName, reportFromStr, reportToStr,
							sections);
					var reportOutputPath = string.IsNullOrWhiteSpace(output)
						? Path.Combine(Directory.GetCurrentDirectory(), BuildDefaultName())
						: output;

					Step("Writing output file...");
					File.WriteAllText(reportOutputPath, reportHtml, System.Text.Encoding.UTF8);
					var errorComment = ReportDiagnostics.FormatParseErrorComment(errors);
					if (errorComment.Length > 0)
						File.AppendAllText(reportOutputPath, errorComment, System.Text.Encoding.UTF8);
					if (interactive)
						InteractiveSetup.ShowComplete(reportOutputPath);
					else
						Console.WriteLine(reportOutputPath);
					if (!silent)
					{
						try { System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(reportOutputPath) { UseShellExecute = true }); }
						catch { /* Default browser may not be configured; report is still written to disk */ }
					}
				});

			return rootCommand;
			}

			static async System.Threading.Tasks.Task<int> Main(string[] args)
			{
				var rootCommand = BuildRootCommand();
				return await rootCommand.InvokeAsync(args);
			}
		}
	}
