## Table Chart Column Headings

- When creating an infographic with `"chartType": "table"`, always specify the `tableColumns` property in the JSON definition.
- `tableColumns` should be an array of two strings: the first is the label column heading, the second is the value column heading (e.g., `["Faction", "Missions"]`).
- If `tableColumns` is not provided, the default headings "Label" and "Value" will be used.
- Example:
  ```json
  "tableColumns": ["Material", "Quantity"]
  ```

## Localized Column Naming

- When a table contains both a column and a corresponding column with a `_localised` suffix (e.g., `Material` and `Material_localised`), always prefer the `_localised` column for display purposes.
- When both columns exist, use `COALESCE(column_localised, column)` in your SQL queries to select the localized value if available, falling back to the non-localized value if not.
- Example:
  ```sql
  SELECT COALESCE(r.Material_localised, r.Material) AS Material, SUM(r.Quantity) AS TimesReceived ...
  ```

## Main Query Requirements

- The main `query` for each infographic **must return a numeric type as the first column** (e.g., `COUNT(*)`, `SUM(...)`, or similar). This value is used to determine whether the tile qualifies (meets the threshold) and is displayed in the report.
- If the main query returns multiple columns, the first column must be numeric, or the tile may never qualify. Always ensure the first column is the intended metric for qualification.
- Example:
  ```json
  "query": "SELECT COUNT(*) AS totalTrades FROM MaterialTrade WHERE ..."
  ```

See also: Primary metric selection rules above.
# Infographics Spec

## Purpose
Define clear, consistent rules for infographic content, formatting, and data wiring to make tiles informative, scannable, and reusable.

## Scope
Title content, primary metric formatting, detail text and placeholders, query expectations, chart selection, thresholds, locale & accessibility, and JSON authoring guidelines.

## Principles
- **Clarity:** Titles show the single most important thing a reader needs to know at a glance.
- **Context:** Details explain the metric with human-friendly labels, precise temporal context, and readable numbers.
- **Consistency:** Use the same language, number formats, and placeholder patterns across infographics.
- **Compact / Expand:** Title shows a compact, short metric; Details show the full-value, fully-labeled phrasing.
- **Data-first:** SQL queries should return clear, named scalars/rows. JSON is descriptive, not prescriptive — prefer deriving placeholders from `details.text`.

## Title rules
- What to include: a short descriptive noun phrase and one compact numeric metric (e.g., "Faction Kill Bonds — 1.3K").
- What not to include: currency labels like "Cr" or long numbers — leave currency and full numbers to Details.  Don't prefix the title with a category name
- Length & style: keep title under ~6 words where possible; metric occupies a separate tile-metric slot for visual emphasis.
- Temporal context: do NOT repeat the period in the title; use the Details for temporal wording.

## Primary metric selection
- Source: The main Query may return multiple named scalars; the renderer picks the title metric using this fixed priority order:
  1. `totalReward` or `reward`
  2. `total`
  3. `count`
  4. first scalar returned
- Display: Titles use the compact 3-position multiplier format (see Number formatting), no currency suffix.
- Fallback: If no scalars returned, default to `MainValue` (count).

### Naming scalars to control which metric appears in the title
The picker is name-driven, not position-driven. **The most interesting metric must be named to rank highest.**

Common authoring mistakes to avoid:
- **Do not name a bare event count `count` when a large credit or distance total is the interesting headline.** If the tile is "Cartographic Earnings", the title should show `538M` (total credits), not `95` (submission count). Rename the total to `total` or `totalReward` so it wins the priority race; rename the count to something descriptive like `submissions` or `jumps` so it is still available as a `{placeholder}` in `details.text`.
- **Do not name a large aggregate `totalSomething` if you also return a `count`.** The renderer will prefer `count` over any non-standard name. Use `total` (priority 2) or `totalReward` / `reward` (priority 1) for the headline value.
- **Rule of thumb:** ask "which number would a player notice first on a tile?" — that number must be named `total`, `totalReward`, or `reward`. Everything else gets a descriptive alias used only in `details.text`.

Examples:
```json
// ✅ Cartographic earnings — credits win the title slot
"query": "SELECT COALESCE(SUM(TotalEarnings), 0) AS total, COUNT(*) AS count, COALESCE(SUM(Bonus), 0) AS bonus FROM ..."
// title shows total credits; detail text uses {count} and {bonus}

// ❌ Wrong — count wins, shows 95 instead of 538M
"query": "SELECT COUNT(*) AS count, COALESCE(SUM(TotalEarnings), 0) AS totalEarnings FROM ..."

// ✅ Light years traveled — distance wins the title slot
"query": "SELECT CAST(SUM(JumpDist) AS INTEGER) AS total, COUNT(*) AS jumps FROM FSDJump ..."
// title shows 208K ly; detail text uses {jumps}

// ❌ Wrong — count wins, shows 6.2K jumps instead of 208K ly
"query": "SELECT COUNT(*) AS count, SUM(JumpDist) AS totalDistance FROM FSDJump ..."
```

## Detail section rules
- Purpose: Provide human-readable sentence(s) that explain the metric, include full numeric values, currency labels and explicit temporal phrasing.
- Temporal wording: Use "during the report period" to mean the report's time range.
- Structure:
  - One or two sentences maximum.
  - First sentence: what happened and the full numeric(s) with units/currency (e.g., "You earned 1,346,228 Cr from 560 faction kill bonds during the report period.")
  - Optional second sentence: short explanation/definition (kept in `details.help` for editors).
- Help text: Keep `details.help` in JSON for maintainers but do not render it by default.

### Making summary-tile detail text interesting
A bare restatement of the numbers is not enough. `"You earned 7,000 Cr doing this 5 times."` is technically correct but tells the player nothing they didn't already see in the title metric.

**For summary-tile infographics, `details.text` should do at least one of:**
1. **Explain what the activity is** in plain language if the title is not self-evident. If you're calling it "Exobiology Earnings" but a player new to the activity wouldn't know what that involves, the detail text should say what they actually did (e.g., "…from selling biological sample data collected during full three-scan organism analyses").
2. **Give context to the numbers** — include a secondary metric that makes the primary one feel real. Credits earned become meaningful when you also see the number of events that produced them. Distance traveled is more vivid when you know how many jumps it took.
3. **Use plain, non-jargon language** for the activity description where possible. If the title uses internal game terminology (e.g., "FSS Survey"), explain it — "Full Spectrum Scanner survey of all bodies in a system".

**Tone:** The detail text is informative and factual — it is the caption, not the headline. It is distinct in purpose from `tagLines`:
- `details.text` = the factual explanation: what you did and what the numbers mean.
- `tagLines` = the emotional hook: snarky, lore-evocative, or GalNet-formal one-liners that add personality.

Do not write detail text in the tagline register, and do not write taglines as factual descriptions.

**Examples:**
```
// ❌ Thin — just restates the title
"text": "{count} exploration data sales totalling {total} Cr during the report period."

// ✅ Adds context and explains the activity
"text": "{count} cartographic submission(s) to Universal Cartographics totalling {total} Cr, including {bonus} Cr in first-discovery bonuses earned for systems no commander had previously charted, during the report period."

// ❌ Ambiguous title, thin detail — "Neutron Highway Boosts" not self-explanatory
"text": "{count} jet cone boost(s) taken during the report period."

// ✅ Explains what a jet cone boost is for players who may not know
"text": "{count} neutron star jet cone boost(s) taken during the report period — each one supercharging the frame shift drive to multiply jump range for the next hyperspace jump."
```

## Placeholders & derivation
- Pattern: Use `{name}` placeholders inside `details.text` where `name` maps to:
  - named scalar returned by the main Query (renderer looks case-insensitively),
  - special name `value` or `count` to reference the main `MainValue`.
- Derivation: Do not require a `placeholders` array; renderer will substitute by scanning `details.text`.
- Formatting substitution: Placeholder substitution uses full locale-aware formatting (thousands separators and no multiplier). Currency text (e.g., "Cr") should be present in the `details.text` string itself.

### Prevention: Multi-metric summary-tiles must return all placeholders from main query
**❌ BROKEN:** A `summary-tile` with `details.text` like `"{suitSpend} Cr on suits and {weaponSpend} Cr on weapons"` where:
- Main query returns only `suitSpend`: `SELECT SUM(Price) AS suitSpend FROM BuySuit ...`
- Detail query returns only `weaponSpend`: `SELECT SUM(Price) AS weaponSpend FROM BuyWeapon ...`

This fails because placeholder substitution only reads the main query result. The `{weaponSpend}` placeholder has no matching scalar and displays as literal text.

**✅ CORRECT:** Include all placeholders needed by `details.text` in the main query:
```json
"query": "SELECT (SELECT SUM(Price) FROM BuySuit WHERE ...) AS suitSpend, (SELECT SUM(Price) FROM BuyWeapon WHERE ...) AS weaponSpend, ... AS total",
"detailQuery": ""
```

**Key rule:** Placeholder substitution in `details.text` uses **only columns from the main query**. The `detailQuery` is exclusively for populating chart rows. Summary-tiles do not use `detailQuery` for anything.

## Number formatting
- Title (compact):
  - Use the 3-position multiplier rule:
    - <1,000: show integer (e.g., `567`)
    - 1,000–9,999: show one decimal if needed, but limit to 3 visible positions (e.g., `1.3K` for 1,346; `63K` for 63,200; `900K` for 900,100 should display `900K`)
    - >=1,000,000: `M` with one decimal when <10M (e.g., `1.3M`), integer when >=10M (e.g., `12M`), same for `B` at billions.
  - Do not append currency units in title metric.
  - Use the viewer's locale for decimal and grouping separators in compact strings where appropriate (renderer uses `CultureInfo.CurrentCulture`).
- Details (full):
  - Always show the full integer with locale-appropriate grouping separators and include units/currency in the text (e.g., `1,346,228 Cr` or `1 A346 A228 Cr` depending on locale).
  - Use integer formatting (no decimals) for full counts and currency totals.

## Queries & data wiring
- Main Query: May return one or more named scalars. Prefer returning named columns:
  - `totalReward` or `reward` — for credit totals (highest priority in title picker).
  - `total` — for any large aggregate that should be the headline (distance, units, volume, etc.).
  - `count` — **only** when the event count itself is the most interesting number on the tile. If a credit or volume total is also returned, name that `total` / `totalReward` instead and give the count a descriptive alias (e.g. `jumps`, `submissions`, `kills`).
  - `total` for other totals.
- DetailQuery: Must return rows of `(label, value)` for chart/rows when using a bar chart. Values should be integers.
- Single-row summaries: For `summary-tile`, prefer returning both `COUNT(*) AS count` and `COALESCE(SUM(Reward),0) AS totalReward` where useful.
- SQL naming: Use predictable column aliases to make placeholder substitution straightforward.

## Chart types & when to use
- `summary-tile`: Use when there is one primary metric to emphasize and a short textual explanation suffices.
- `bar-chart`: Use when the distribution across categories is the interesting insight — provide a `detailQuery` that groups by a descriptive field. Show up to 5 rows; roll-up remainder into `Other`.
- `table`: Use when the detail rows represent named real-world entities that a player might want to look up by name — stations, star systems, commanders, minor factions, ships, and similar. Tables support Inara.cz deep-links via `inaraSearchBase` so the player can click through directly from the tile.
- Avoid charts for metrics that don't have meaningful breakdowns — prefer `summary-tile`.

## Chart selection heuristic
When authoring or migrating infographic definitions, prefer a `bar-chart` (or `single-count` with a detail bar chart) when the underlying event table contains a clear categorical field that makes an interesting breakdown. Examples of good categorical columns:
- `StarSystem` / `System` — group jumps or navigation events by system.
- `StationType` / `StationName` — group docking/market events by station.
- `Type_Localised` / `Type` — group item/module/material events by localised type.
- `CrimeType` / `InterdictedBy` / `TargetType` — group combat/crime events by type.
- `Name` / `Name_Localised` — group mission targets, codex entries, or named items.

Heuristic to apply when migrating date-bucket tiles:
- If the event table for the infographic (derived from the JSON filename or `query` table) contains one of the obvious categorical columns above, convert the `detailQuery` to group by that column and render a `bar-chart` showing the Top 5 categories (roll up the rest into `Other`).
- If no obvious categorical column exists, keep or convert to `summary-tile` with a concise `details.text` explaining the full metric and temporal phrasing.

Choosing between `bar-chart` and `table` when a categorical breakdown makes sense:
- Prefer **`table`** when the category values are named real-world entities that Inara.cz can look up (stations, star systems, commanders, minor factions, megaships, fleet carriers, etc.). Set `inaraSearchBase` to the appropriate Inara search URL so rows become clickable links.
- Prefer **`bar-chart`** when the categories are type codes, material names, crime types, planet classes, economy labels, or other enumerations that aren't searchable as individual entities on Inara.
- When in doubt, ask: "Would the player benefit from clicking this item to see more details on Inara?" If yes, use `table`.

This approach keeps charts where they reveal actionable distributions, uses tables where the rows are navigable entities, and uses `summary-tile` where only a single aggregate is meaningful.

## Accessibility & locale
- Locale: Use `CultureInfo.CurrentCulture` (or explicit locale override when rendering for a specific audience) for separators and decimal symbols.
- Screen readers: Ensure `tile-title` and `tile-metric-value` are in semantic tags or ARIA-friendly spans when template evolves.
- Color / contrast: Keep contrast high and avoid encoding meaning in color alone.

## Report context

### `summaryOnly`
Set `"summaryOnly": true` on infographics that are only meaningful in a whole-career or date-range summary report and should be **excluded** when generating a by-system report.

An infographic is `summaryOnly` when its primary metric or breakdown would always be trivially small or nonsensical when filtered to a single star system. Examples:
- **Total jumps / total distance** — a by-system query would always return 1 jump and a few light years.
- **Most visited system** — the answer is always just the system itself.
- **Jump distance distribution** — every jump in a single system is the same outbound leg.
- **Neutron boost mileage** — neutron boosts happen between systems, not within them.

Omit the field (or set to `false`) for infographics that are equally valid in both report types (e.g. bounty earnings, trade profit, combat kills — all make sense filtered to a single system).

### `tagLines`
An array of **exactly 3 short flavour strings** attached to the tile. The renderer picks one at random to display as a subtitle beneath the tile title, rotating on each report generation.

Purpose: dress the tile in personality — make a dry statistic feel like GalNet copy, a pilot's log entry, or wry editorial comment. Think: the voice used in in-game loading screen tips, GalNet headlines, or the Pilot's Bar rumour mill.

Style guidance — vary register across the three entries:
- **GalNet bureaucratic:** clinical, passive voice, slightly absurd formal register. *"The Pilots Federation notes a statistically significant deviation in cartographic submission volume."*
- **Snarky / editorial:** dry wit, second-person address, light mockery. *"Someone's been busy. The bubble's mapping agencies are quietly grateful."*
- **Lore-flavoured:** evocative, world-building, treats the number as a moment of in-universe significance. *"Thousands of light years etched into the galaxy's permanent record. Your name, written in stars."*

Keep each tagline under ~15 words. Do not repeat the metric or the title — the tagline complements rather than restates.

Example for a Cartographic Earnings tile:
```json
"tagLines": [
  "The bubble's cartographers have updated their star charts. Again.",
  "Universal Cartographics acknowledges your… enthusiasm for data submission.",
  "Every credit here is a system that humanity now knows exists."
]
```

## Filename convention
The JSON filename (without extension) must be a kebab-case representation of the infographic's `title` field — lowercase, spaces replaced with hyphens, special characters removed. This makes files self-describing and keeps filenames predictable.

Examples:
- `title: "Stations Docked"` → `stations-docked.json`
- `title: "Faction Kill Bonds"` → `faction-kill-bonds.json`
- `title: "FSS Discovery Scans"` → `fss-discovery-scans.json`

Files are placed in the subfolder matching their `category` field (e.g., `infographics/TravelAndNavigation/stations-docked.json`).

## JSON authoring guidelines
- Keep `details` structured:
  - `details.text`: primary sentence with `{placeholders}` and explicit units (currency like `Cr`) included in text.
  - `details.help`: optional maintainer note (kept but not rendered).
- Do not include `placeholders` in JSON; let the renderer derive them from `details.text`.
- For `table` chart type, add:
  - `tableColumns`: two-element array of column header strings, e.g. `["Station", "Visits"]`. Defaults to `["Label", "Value"]` if omitted.
  - `inaraSearchBase`: base URL for Inara.cz entity search, e.g. `"https://inara.cz/elite/station/?search="`. The URL-encoded row label is appended automatically. Omit when rows are not Inara-searchable.
- Common `inaraSearchBase` values:
  - Stations: `"https://inara.cz/elite/station/?search="`
  - Star systems: `"https://inara.cz/elite/starsystem/?search="`
  - Commanders: `"https://inara.cz/elite/cmdr/?search="`
  - Minor factions: `"https://inara.cz/elite/minorfaction/?search="`
- `summaryOnly`: Set to `true` when the infographic is not meaningful in a by-system report (see **Report context** above). Omit or set to `false` otherwise.
- `tagLines`: Array of exactly 3 short flavour strings (see **Report context** above). The renderer picks one at random per report render. Vary register: one GalNet-formal, one snarky/editorial, one lore-evocative. Keep each under ~15 words.

## Examples
- Title: "Faction Kill Bonds" — Metric: `560` → rendered compact `560` (title) and detail substitution "You earned 1,346,228 Cr from 560 faction kill bonds during the report period." 
- Compact edge cases:
  - 1,346 → `1.3K` in title; detail: `1,346 Cr`
  - 900,120 → `900K` in title; detail: `900,120 Cr`
  - 2,134,000,000 → `2.1B` in title; detail: `2,134,000,000 Cr`

## Authoring checklist
- Query: Return well-named scalars. Name the headline value `total`, `totalReward`, or `reward` so it wins the title picker. Only use `count` when the event count itself is the headline. Give secondary counts a descriptive alias (e.g. `jumps`, `submissions`) used in `details.text`.
- Title: concise noun phrase; no units appended.
- Details: Compose `details.text` with `{placeholders}`, include units/currency and "during the report period". For summary-tiles, go beyond restating the numbers — explain what the activity is if the title isn't self-evident, and include a secondary metric for context. Keep the tone factual and informative (not snarky — that's what `tagLines` are for).
- Filename: Use kebab-case form of the `title` value (e.g. `stations-docked.json`), placed in the matching `category` subfolder.
- Chart: Choose `summary-tile` for single metrics; `bar-chart` for categorical distributions; `table` for named navigable entities (use `inaraSearchBase` when Inara links are available).
- Locale: Assume renderer will localize numbers; do not hardcode separators.
- Maintainability: Put clarifying notes in `details.help`, not in rendered tile.
- Report context: Add `"summaryOnly": true` if the metric is meaningless in a by-system report.
- Tag lines: Include `tagLines` array with exactly 3 strings — one GalNet-formal, one snarky/editorial, one lore-evocative. Keep each under ~15 words.

---
Generated by the Copilot assistant as the canonical infographic authoring spec.
