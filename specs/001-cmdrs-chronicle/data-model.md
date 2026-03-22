# Schema Source Reference


All event table definitions and property mappings are derived from the canonical Elite Dangerous journal event schemas at: https://github.com/jixxed/ed-journal-schemas/tree/main/schemas

**Schema Discovery Note:**
To generate the full set of event tables, recursively traverse all subdirectories under the canonical schema repository's root (`schemas/`). For each event, locate the corresponding JSON schema file (e.g., `schemas/FSDJump/FSDJump.json`, `schemas/Scan/Scan.json`). Use the property definitions in each JSON file to drive table and column generation.


# Data Model: CmdrsChronicle

## Relational Mapping Directives (for Event Storage)

### Purpose
Provide strict, schema-aligned relational mapping rules so every event schema from the canonical repository maps directly to a relational table with columns that correspond exactly to the schema property names. The stored data must be fully relational (no raw JSON stored) so complex SQL queries can be written against event fields. All event storage and queries use SQLite (in-memory mode) for maximum SQL compatibility and concurrency.


### High-level Mapping Rules
- **Table-per-schema**: For every event schema in the canonical set (e.g., `StartJump`, `Scan`, `Docked`), create a relational table named exactly as the schema name (preserve casing).
- **Complex data**: If an event property is a complex object or array, generate a child table for that property, with a foreign key to the parent event table. For further nested objects/arrays, create a column in the child table to store the JSON text (truncated to 255 characters) for that attribute.
- **Nesting depth**: Only the first layer of child tables is normalized; deeper nesting is stored as truncated JSON text in a column.
- **Column names**: For each property in the event schema, create a column with the same name and an appropriate SQL type. Preserve property name casing exactly as in the schema.
	- **Reserved word handling**: If a property name is a reserved SQL word for the target DB (e.g., `timestamp`, `order`, `group`, etc.), prefix it with `event_` (e.g., `event_timestamp`, `event_order`) to avoid conflicts. Maintain a list of reserved words for the chosen DB engine (e.g., SQLite, SQL Server) and apply this rule consistently in code generation and migrations. Example: the `timestamp` property becomes `event_timestamp` in the table.
- **Unique record id**: Add a synthetic primary key column named `event_id` to every table to differentiate DB records from any schema-defined identifier fields.
- **No raw JSON storage**: Do not persist the original event JSON in any table. All persisted information must be mapped into discrete columns or normalized child tables.
- **Canonical timestamp**: If the schema defines a timestamp property (e.g., `timestamp`, `event_time`, `timestamp_utc`), include that column (using the reserved word rule above). Parse the UTC timestamp using a timezone-aware parser, convert to the application's local timezone, and store as a timestamp (no timezone attached).

## Entities

### LogFile
- path: string
- filename: string (matches pattern Journal.YYYY-MM-DDThhmmss.nn.log)
- modifiedAt: DateTime
- size: long
- lineCount: int

### LogEvent
- timestamp: DateTime
- eventType: string
- payload: JsonElement (raw JSON)
- system: string? (nullable)
- body: string? (nullable)
- planet: string? (nullable)
- commander: string? (nullable)

### InfographicDefinition
- id: string
- title: string
- type: string ("chart" | "counter" | "timeline")
- dataBinding: string (expression)
- presentationHints: Dictionary<string, object>

### Report
- title: string
- dateGenerated: DateTime
- style: string ("elegant" | "colorful")
- sections: List<ReportSection>
- embeddedAssets: List<string> (base64 or inline)

### ReportSection
- title: string
- content: string (HTML)
- events: List<LogEvent>
- infographics: List<InfographicDefinition>

## Relationships
- LogFile contains multiple LogEvents
- Report contains multiple ReportSections
- ReportSection may reference LogEvents and InfographicDefinitions

## Validation Rules
- LogFile must exist, be readable, and match the canonical Elite Dangerous journal log filename pattern: Journal.YYYY-MM-DDThhmmss.nn.log (see research-naming.md)
- LogEvent must have a valid timestamp and eventType
- InfographicDefinition must have unique id per report
- Report must have at least one section (even if 'no data')

## State Transitions
- LogFile: discovered → parsed → processed
- LogEvent: raw → validated → included/excluded
- Report: initialized → populated → rendered → written

---
This data model supports all required report generation, filtering, and error handling scenarios for CmdrsChronicle.