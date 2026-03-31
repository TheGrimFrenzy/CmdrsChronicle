-- CmdrsChronicle Sample DDL for FSDJump and Scan Events
-- Generated from canonical schemas (https://github.com/jixxed/ed-journal-schemas)
-- Mapping rules: table-per-schema, child tables for first-level objects/arrays, synthetic PK, no raw JSON

-- =====================
-- Table: FSDJump
-- =====================
CREATE TABLE FSDJump (
    cc_id INTEGER PRIMARY KEY AUTOINCREMENT,
    timestamp TEXT NOT NULL, -- UTC, to be parsed and stored as local time
    Taxi BOOLEAN,
    Multicrew BOOLEAN,
    StarSystem TEXT,
    SystemAddress INTEGER,
    StarPos_X REAL,
    StarPos_Y REAL,
    StarPos_Z REAL,
    SystemAllegiance TEXT,
    SystemEconomy TEXT,
    SystemEconomy_Localised TEXT,
    SystemGovernment TEXT,
    SystemGovernment_Localised TEXT,
    SystemSecurity TEXT,
    SystemSecurity_Localised TEXT,
    Population INTEGER,
    Body TEXT,
    BodyID INTEGER,
    BodyType TEXT,
    JumpDist REAL,
    FuelUsed REAL,
    FuelLevel REAL,
    BoostUsed BOOLEAN,
    Faction TEXT,
    Factions_child_id INTEGER, -- FK to FSDJump_Factions
    SystemFaction_child_id INTEGER, -- FK to FSDJump_SystemFaction
    Powers_child_id INTEGER, -- FK to FSDJump_Powers
    PowerplayState TEXT,
    PowerplayStateControlProgress REAL,
    PowerplayStateReinforcement INTEGER,
    PowerplayStateUndermining INTEGER,
    PowerplayConflictProgress_child_id INTEGER, -- FK to FSDJump_PowerplayConflictProgress
    Conflicts_child_id INTEGER, -- FK to FSDJump_Conflicts
    ThargoidWar_child_id INTEGER -- FK to FSDJump_ThargoidWar
);

-- Child table for Factions (array of objects)
CREATE TABLE FSDJump_Factions (
    cc_id INTEGER PRIMARY KEY AUTOINCREMENT,
    FSDJump_id INTEGER NOT NULL, -- FK to FSDJump
    Name TEXT,
    FactionState TEXT,
    Government TEXT,
    Influence REAL,
    Allegiance TEXT,
    Happiness TEXT,
    MyReputation REAL,
    ActiveStates TEXT, -- JSON (truncated to 255 chars)
    RecoveringStates TEXT, -- JSON (truncated to 255 chars)
    PendingStates TEXT, -- JSON (truncated to 255 chars)
    SquadFaction BOOLEAN,
    HomeSystem BOOLEAN,
    FOREIGN KEY(FSDJump_id) REFERENCES FSDJump(cc_id)
);

-- Child table for SystemFaction (object)
CREATE TABLE FSDJump_SystemFaction (
    cc_id INTEGER PRIMARY KEY AUTOINCREMENT,
    FSDJump_id INTEGER NOT NULL, -- FK to FSDJump
    Name TEXT,
    FactionState TEXT,
    FOREIGN KEY(FSDJump_id) REFERENCES FSDJump(cc_id)
);

-- Child table for Powers (array of strings)
CREATE TABLE FSDJump_Powers (
    cc_id INTEGER PRIMARY KEY AUTOINCREMENT,
    FSDJump_id INTEGER NOT NULL, -- FK to FSDJump
    Power TEXT,
    FOREIGN KEY(FSDJump_id) REFERENCES FSDJump(cc_id)
);

-- Child table for PowerplayConflictProgress (array of objects)
CREATE TABLE FSDJump_PowerplayConflictProgress (
    cc_id INTEGER PRIMARY KEY AUTOINCREMENT,
    FSDJump_id INTEGER NOT NULL, -- FK to FSDJump
    Power TEXT,
    ConflictProgress REAL,
    FOREIGN KEY(FSDJump_id) REFERENCES FSDJump(cc_id)
);

-- Child table for Conflicts (array of objects)
CREATE TABLE FSDJump_Conflicts (
    cc_id INTEGER PRIMARY KEY AUTOINCREMENT,
    FSDJump_id INTEGER NOT NULL, -- FK to FSDJump
    WarType TEXT,
    Status TEXT,
    Faction1 TEXT, -- JSON (truncated to 255 chars)
    Faction2 TEXT, -- JSON (truncated to 255 chars)
    FOREIGN KEY(FSDJump_id) REFERENCES FSDJump(cc_id)
);

-- Child table for ThargoidWar (object)
CREATE TABLE FSDJump_ThargoidWar (
    cc_id INTEGER PRIMARY KEY AUTOINCREMENT,
    FSDJump_id INTEGER NOT NULL, -- FK to FSDJump
    CurrentState TEXT,
    NextStateSuccess TEXT,
    NextStateFailure TEXT,
    SuccessStateReached BOOLEAN,
    WarProgress REAL,
    RemainingPorts INTEGER,
    EstimatedRemainingTime TEXT,
    FOREIGN KEY(FSDJump_id) REFERENCES FSDJump(cc_id)
);

-- =====================
-- Table: Scan
-- =====================
CREATE TABLE Scan (
    cc_id INTEGER PRIMARY KEY AUTOINCREMENT,
    timestamp TEXT NOT NULL, -- UTC, to be parsed and stored as local time
    ScanType TEXT,
    BodyName TEXT,
    BodyID INTEGER,
    StarSystem TEXT,
    SystemAddress INTEGER,
    DistanceFromArrivalLS REAL,
    WasDiscovered BOOLEAN,
    WasMapped BOOLEAN,
    StarType TEXT,
    Subclass INTEGER,
    StellarMass REAL,
    Radius REAL,
    AbsoluteMagnitude REAL,
    Age_MY INTEGER,
    SurfaceTemperature REAL,
    Luminosity TEXT,
    SemiMajorAxis REAL,
    Eccentricity REAL,
    OrbitalInclination REAL,
    Periapsis REAL,
    OrbitalPeriod REAL,
    RotationPeriod REAL,
    AxialTilt REAL,
    BodyType TEXT,
    Parents TEXT, -- JSON (truncated to 255 chars)
    Atmosphere TEXT,
    AtmosphereType TEXT,
    AtmosphereComposition_child_id INTEGER, -- FK to Scan_AtmosphereComposition
    Volcanism TEXT,
    MassEM REAL,
    SurfaceGravity REAL,
    SurfaceTemperature REAL,
    SurfacePressure REAL,
    Landable BOOLEAN,
    Composition TEXT, -- JSON (truncated to 255 chars)
    TerraformState TEXT,
    PlanetClass TEXT,
    Rings_child_id INTEGER, -- FK to Scan_Rings
    ReserveLevel TEXT
);

-- Child table for AtmosphereComposition (array of objects)
CREATE TABLE Scan_AtmosphereComposition (
    cc_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Scan_id INTEGER NOT NULL, -- FK to Scan
    Name TEXT,
    Percent REAL,
    FOREIGN KEY(Scan_id) REFERENCES Scan(cc_id)
);

-- Child table for Rings (array of objects)
CREATE TABLE Scan_Rings (
    cc_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Scan_id INTEGER NOT NULL, -- FK to Scan
    Name TEXT,
    RingClass TEXT,
    MassMT REAL,
    InnerRad REAL,
    OuterRad REAL,
    FOREIGN KEY(Scan_id) REFERENCES Scan(cc_id)
);

-- Note: For all JSON columns, store as TEXT (truncated to 255 chars). For deeper nesting, do not normalize further.
-- All child tables have synthetic PK (cc_id) and FK to parent event table.
