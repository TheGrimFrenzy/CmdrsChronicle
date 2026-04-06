-- CmdrsChronicle static SQLite schema
-- Generated from ED Journal schemas (https://github.com/jixxed/ed-journal-schemas/tree/main/schemas)
-- DO NOT EDIT BY HAND

CREATE TABLE IF NOT EXISTS _Event (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    timestamp TEXT NOT NULL,
    event TEXT NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS AfmuRepairs (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Module TEXT NOT NULL,
    Module_Localised TEXT,
    FullyRepaired INTEGER NOT NULL,
    Health REAL NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS AppliedToSquadron (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    SquadronID INTEGER,
    SquadronName TEXT NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS ApproachBody (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    StarSystem TEXT NOT NULL,
    SystemAddress INTEGER NOT NULL,
    Body TEXT NOT NULL,
    BodyID INTEGER NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS ApproachSettlement (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    MarketID INTEGER,
    SystemAddress INTEGER NOT NULL,
    BodyID INTEGER NOT NULL,
    BodyName TEXT NOT NULL,
    Latitude REAL,
    Longitude REAL,
    Name_Localised TEXT,
    StationGovernment TEXT,
    StationGovernment_Localised TEXT,
    StationAllegiance TEXT,
    StationServices TEXT,
    StationEconomy TEXT,
    StationEconomy_Localised TEXT,
    event_timestamp TEXT NOT NULL
);

    CREATE TABLE IF NOT EXISTS ApproachSettlement_StationFaction (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        ApproachSettlement_event_id INTEGER NOT NULL,
        Name TEXT NOT NULL,
        FactionState TEXT,
        FOREIGN KEY(ApproachSettlement_event_id) REFERENCES ApproachSettlement(event_id)
    );

    CREATE TABLE IF NOT EXISTS ApproachSettlement_StationEconomies (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        ApproachSettlement_event_id INTEGER NOT NULL,
        Name TEXT NOT NULL,
        Name_Localised TEXT,
        Proportion REAL NOT NULL,
        FOREIGN KEY(ApproachSettlement_event_id) REFERENCES ApproachSettlement(event_id)
    );


CREATE TABLE IF NOT EXISTS AsteroidCracked (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Body TEXT NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS Backpack (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Items TEXT,
    Components TEXT,
    Consumables TEXT,
    Data TEXT,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS BackpackChange (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Added TEXT,
    Removed TEXT,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS BookDropship (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Retreat INTEGER,
    Cost INTEGER NOT NULL,
    DestinationSystem TEXT NOT NULL,
    DestinationLocation TEXT NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS BookTaxi (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Cost INTEGER NOT NULL,
    DestinationSystem TEXT NOT NULL,
    DestinationLocation TEXT NOT NULL,
    Retreat INTEGER,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS Bounty (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    PilotName TEXT,
    PilotName_Localised TEXT,
    Target TEXT NOT NULL,
    Target_Localised TEXT,
    TotalReward INTEGER,
    VictimFaction TEXT NOT NULL,
    VictimFaction_Localised TEXT,
    SharedWithOthers INTEGER,
    Reward INTEGER,
    Faction TEXT,
    event_timestamp TEXT NOT NULL
);

    CREATE TABLE IF NOT EXISTS Bounty_Rewards (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        Bounty_event_id INTEGER NOT NULL,
        Faction TEXT NOT NULL,
        Reward INTEGER NOT NULL,
        FOREIGN KEY(Bounty_event_id) REFERENCES Bounty(event_id)
    );


CREATE TABLE IF NOT EXISTS BuyAmmo (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Cost INTEGER NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS BuyDrones (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Type TEXT NOT NULL,
    Count INTEGER NOT NULL,
    BuyPrice INTEGER NOT NULL,
    TotalCost INTEGER NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS BuyExplorationData (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    System TEXT NOT NULL,
    Cost INTEGER NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS BuyMicroResources (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT,
    Name_Localised TEXT,
    Category TEXT,
    Count INTEGER,
    Price INTEGER NOT NULL,
    MarketID INTEGER NOT NULL,
    TotalCount INTEGER,
    event_timestamp TEXT NOT NULL
);

    CREATE TABLE IF NOT EXISTS BuyMicroResources_MicroResources (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        BuyMicroResources_event_id INTEGER NOT NULL,
        Name TEXT NOT NULL,
        Name_Localised TEXT,
        Category TEXT NOT NULL,
        Count INTEGER NOT NULL,
        FOREIGN KEY(BuyMicroResources_event_id) REFERENCES BuyMicroResources(event_id)
    );


CREATE TABLE IF NOT EXISTS BuySuit (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    Name_Localised TEXT,
    Price INTEGER NOT NULL,
    SuitID INTEGER NOT NULL,
    SuitMods TEXT NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS BuyTradeData (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    System TEXT NOT NULL,
    Cost INTEGER NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS BuyWeapon (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    Name_Localised TEXT,
    Class INTEGER NOT NULL,
    Price INTEGER NOT NULL,
    SuitModuleID INTEGER NOT NULL,
    WeaponMods TEXT NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS CancelDropship (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Refund INTEGER NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS CancelledSquadronApplication (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    SquadronID INTEGER NOT NULL,
    SquadronName TEXT NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS CancelTaxi (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Refund INTEGER NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS CapShipBond (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Reward INTEGER NOT NULL,
    AwardingFaction TEXT NOT NULL,
    VictimFaction TEXT NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS Cargo (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Vessel TEXT NOT NULL,
    Count INTEGER NOT NULL,
    event_timestamp TEXT NOT NULL
);

    CREATE TABLE IF NOT EXISTS Cargo_Inventory (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        Cargo_event_id INTEGER NOT NULL,
        Name TEXT NOT NULL,
        Name_Localised TEXT,
        Count INTEGER NOT NULL,
        Stolen INTEGER NOT NULL,
        MissionID INTEGER,
        FOREIGN KEY(Cargo_event_id) REFERENCES Cargo(event_id)
    );


CREATE TABLE IF NOT EXISTS CargoDepot (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    MissionID INTEGER NOT NULL,
    UpdateType TEXT NOT NULL,
    CargoType TEXT,
    Count INTEGER,
    StartMarketID INTEGER NOT NULL,
    EndMarketID INTEGER NOT NULL,
    ItemsCollected INTEGER NOT NULL,
    ItemsDelivered INTEGER NOT NULL,
    TotalItemsToDeliver INTEGER NOT NULL,
    Progress REAL NOT NULL,
    CargoType_Localised TEXT,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS CargoTransfer (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    event_timestamp TEXT NOT NULL
);

    CREATE TABLE IF NOT EXISTS CargoTransfer_Transfers (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        CargoTransfer_event_id INTEGER NOT NULL,
        Type TEXT NOT NULL,
        Type_Localised TEXT,
        Count INTEGER NOT NULL,
        Direction TEXT NOT NULL,
        MissionID INTEGER,
        FOREIGN KEY(CargoTransfer_event_id) REFERENCES CargoTransfer(event_id)
    );


CREATE TABLE IF NOT EXISTS CarrierBankTransfer (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    CarrierID INTEGER NOT NULL,
    CarrierType TEXT,
    Deposit INTEGER,
    Withdraw INTEGER,
    PlayerBalance INTEGER NOT NULL,
    CarrierBalance INTEGER NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS CarrierBuy (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    CarrierID INTEGER NOT NULL,
    CarrierType TEXT,
    BoughtAtMarket INTEGER NOT NULL,
    Location TEXT NOT NULL,
    SystemAddress INTEGER NOT NULL,
    Price INTEGER NOT NULL,
    Variant TEXT NOT NULL,
    Callsign TEXT NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS CarrierCancelDecommission (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    CarrierID INTEGER NOT NULL,
    CarrierType TEXT,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS CarrierCrewServices (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    CarrierID INTEGER NOT NULL,
    CrewRole TEXT NOT NULL,
    CarrierType TEXT,
    Operation TEXT NOT NULL,
    CrewName TEXT NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS CarrierDecommission (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    CarrierID INTEGER NOT NULL,
    ScrapRefund INTEGER NOT NULL,
    ScrapTime INTEGER NOT NULL,
    CarrierType TEXT,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS CarrierDepositFuel (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    CarrierID INTEGER NOT NULL,
    Amount INTEGER NOT NULL,
    Total INTEGER NOT NULL,
    CarrierType TEXT,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS CarrierDockingPermission (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    CarrierID INTEGER NOT NULL,
    DockingAccess TEXT NOT NULL,
    AllowNotorious INTEGER NOT NULL,
    CarrierType TEXT,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS CarrierFinance (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    CarrierID INTEGER NOT NULL,
    CarrierType TEXT,
    CarrierBalance INTEGER NOT NULL,
    ReserveBalance INTEGER NOT NULL,
    AvailableBalance INTEGER NOT NULL,
    ReservePercent INTEGER NOT NULL,
    TaxRate_pioneersupplies INTEGER,
    TaxRate_shipyard INTEGER,
    TaxRate_rearm INTEGER,
    TaxRate_outfitting INTEGER,
    TaxRate_refuel INTEGER,
    TaxRate_repair INTEGER,
    TaxRate INTEGER,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS CarrierJump (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Docked INTEGER NOT NULL,
    OnFoot INTEGER,
    StationName TEXT NOT NULL,
    StationType TEXT NOT NULL,
    MarketID INTEGER,
    StationGovernment TEXT NOT NULL,
    StationGovernment_Localised TEXT,
    StationServices TEXT,
    StationEconomy TEXT NOT NULL,
    StationEconomy_Localised TEXT,
    Taxi INTEGER,
    Multicrew INTEGER,
    Wanted INTEGER,
    StarSystem TEXT NOT NULL,
    SystemAddress INTEGER NOT NULL,
    StarPos TEXT NOT NULL,
    SystemAllegiance TEXT NOT NULL,
    SystemEconomy TEXT NOT NULL,
    SystemEconomy_Localised TEXT,
    SystemSecondEconomy TEXT NOT NULL,
    SystemSecondEconomy_Localised TEXT,
    SystemGovernment TEXT NOT NULL,
    SystemGovernment_Localised TEXT,
    SystemSecurity TEXT NOT NULL,
    SystemSecurity_Localised TEXT,
    Population INTEGER NOT NULL,
    Body TEXT NOT NULL,
    BodyID INTEGER NOT NULL,
    BodyType TEXT NOT NULL,
    Powers TEXT,
    ControllingPower TEXT,
    PowerplayState TEXT,
    PowerplayStateControlProgress REAL,
    PowerplayStateReinforcement INTEGER,
    PowerplayStateUndermining INTEGER,
    event_timestamp TEXT NOT NULL
);

    CREATE TABLE IF NOT EXISTS CarrierJump_StationFaction (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        CarrierJump_event_id INTEGER NOT NULL,
        Name TEXT NOT NULL,
        FOREIGN KEY(CarrierJump_event_id) REFERENCES CarrierJump(event_id)
    );

    CREATE TABLE IF NOT EXISTS CarrierJump_StationEconomies (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        CarrierJump_event_id INTEGER NOT NULL,
        Name TEXT NOT NULL,
        Name_Localised TEXT,
        Proportion REAL NOT NULL,
        FOREIGN KEY(CarrierJump_event_id) REFERENCES CarrierJump(event_id)
    );

    CREATE TABLE IF NOT EXISTS CarrierJump_Factions (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        CarrierJump_event_id INTEGER NOT NULL,
        Name TEXT NOT NULL,
        FactionState TEXT NOT NULL,
        Government TEXT NOT NULL,
        Influence REAL NOT NULL,
        Allegiance TEXT NOT NULL,
        Happiness TEXT NOT NULL,
        Happiness_Localised TEXT,
        HappiestSystem INTEGER,
        MyReputation REAL NOT NULL,
        PendingStates TEXT,
        ActiveStates TEXT,
        SquadronFaction INTEGER,
        HomeSystem INTEGER,
        RecoveringStates TEXT,
        FOREIGN KEY(CarrierJump_event_id) REFERENCES CarrierJump(event_id)
    );

    CREATE TABLE IF NOT EXISTS CarrierJump_SystemFaction (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        CarrierJump_event_id INTEGER NOT NULL,
        Name TEXT NOT NULL,
        FactionState TEXT,
        FOREIGN KEY(CarrierJump_event_id) REFERENCES CarrierJump(event_id)
    );

    CREATE TABLE IF NOT EXISTS CarrierJump_Conflicts (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        CarrierJump_event_id INTEGER NOT NULL,
        WarType TEXT NOT NULL,
        Status TEXT NOT NULL,
        Faction1 TEXT NOT NULL,
        Faction2 TEXT NOT NULL,
        FOREIGN KEY(CarrierJump_event_id) REFERENCES CarrierJump(event_id)
    );

    CREATE TABLE IF NOT EXISTS CarrierJump_ThargoidWar (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        CarrierJump_event_id INTEGER NOT NULL,
        CurrentState TEXT NOT NULL,
        NextStateSuccess TEXT,
        NextStateFailure TEXT,
        SuccessStateReached INTEGER NOT NULL,
        WarProgress REAL,
        RemainingPorts INTEGER,
        EstimatedRemainingTime TEXT,
        FOREIGN KEY(CarrierJump_event_id) REFERENCES CarrierJump(event_id)
    );

    CREATE TABLE IF NOT EXISTS CarrierJump_PowerplayConflictProgress (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        CarrierJump_event_id INTEGER NOT NULL,
        Power TEXT NOT NULL,
        ConflictProgress REAL NOT NULL,
        FOREIGN KEY(CarrierJump_event_id) REFERENCES CarrierJump(event_id)
    );


CREATE TABLE IF NOT EXISTS CarrierJumpCancelled (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    CarrierID INTEGER NOT NULL,
    CarrierType TEXT,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS CarrierJumpRequest (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    CarrierID INTEGER NOT NULL,
    CarrierType TEXT,
    SystemName TEXT NOT NULL,
    Body TEXT,
    SystemAddress INTEGER NOT NULL,
    BodyID INTEGER NOT NULL,
    DepartureTime TEXT,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS CarrierLocation (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    CarrierID INTEGER NOT NULL,
    CarrierType TEXT,
    StarSystem TEXT NOT NULL,
    SystemAddress INTEGER NOT NULL,
    BodyID INTEGER NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS CarrierModulePack (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    CarrierID INTEGER NOT NULL,
    CarrierType TEXT,
    Operation TEXT NOT NULL,
    PackTheme TEXT NOT NULL,
    PackTier INTEGER NOT NULL,
    Refund INTEGER,
    Cost INTEGER,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS CarrierNameChange (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    CarrierID INTEGER NOT NULL,
    CarrierType TEXT,
    Name TEXT NOT NULL,
    Callsign TEXT NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS CarrierShipPack (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    CarrierID INTEGER NOT NULL,
    CarrierType TEXT,
    Operation TEXT NOT NULL,
    PackTheme TEXT NOT NULL,
    PackTier INTEGER NOT NULL,
    Refund INTEGER,
    Cost INTEGER,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS CarrierStats (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    CarrierID INTEGER NOT NULL,
    Callsign TEXT NOT NULL,
    CarrierType TEXT,
    Name TEXT NOT NULL,
    DockingAccess TEXT NOT NULL,
    AllowNotorious INTEGER NOT NULL,
    FuelLevel INTEGER NOT NULL,
    JumpRangeCurr REAL NOT NULL,
    JumpRangeMax REAL NOT NULL,
    PendingDecommission INTEGER NOT NULL,
    event_timestamp TEXT NOT NULL
);

    CREATE TABLE IF NOT EXISTS CarrierStats_SpaceUsage (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        CarrierStats_event_id INTEGER NOT NULL,
        TotalCapacity INTEGER NOT NULL,
        Crew INTEGER NOT NULL,
        Cargo INTEGER NOT NULL,
        CargoSpaceReserved INTEGER NOT NULL,
        ShipPacks INTEGER NOT NULL,
        ModulePacks INTEGER NOT NULL,
        FreeSpace INTEGER NOT NULL,
        FOREIGN KEY(CarrierStats_event_id) REFERENCES CarrierStats(event_id)
    );

    CREATE TABLE IF NOT EXISTS CarrierStats_Finance (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        CarrierStats_event_id INTEGER NOT NULL,
        CarrierBalance INTEGER NOT NULL,
        ReserveBalance INTEGER NOT NULL,
        AvailableBalance INTEGER NOT NULL,
        ReservePercent INTEGER,
        TaxRate_shipyard INTEGER,
        TaxRate_rearm INTEGER,
        TaxRate_outfitting INTEGER,
        TaxRate_refuel INTEGER,
        TaxRate_repair INTEGER,
        TaxRate_pioneersupplies INTEGER,
        TaxRate INTEGER,
        FOREIGN KEY(CarrierStats_event_id) REFERENCES CarrierStats(event_id)
    );

    CREATE TABLE IF NOT EXISTS CarrierStats_Crew (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        CarrierStats_event_id INTEGER NOT NULL,
        CrewRole TEXT NOT NULL,
        Activated INTEGER NOT NULL,
        Enabled INTEGER,
        CrewName TEXT,
        FOREIGN KEY(CarrierStats_event_id) REFERENCES CarrierStats(event_id)
    );

    CREATE TABLE IF NOT EXISTS CarrierStats_ShipPacks (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        CarrierStats_event_id INTEGER NOT NULL,
        PackTheme TEXT NOT NULL,
        PackTier INTEGER NOT NULL,
        FOREIGN KEY(CarrierStats_event_id) REFERENCES CarrierStats(event_id)
    );

    CREATE TABLE IF NOT EXISTS CarrierStats_ModulePacks (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        CarrierStats_event_id INTEGER NOT NULL,
        PackTheme TEXT NOT NULL,
        PackTier INTEGER NOT NULL,
        FOREIGN KEY(CarrierStats_event_id) REFERENCES CarrierStats(event_id)
    );


CREATE TABLE IF NOT EXISTS CarrierTradeOrder (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    CarrierID INTEGER NOT NULL,
    CarrierType TEXT,
    BlackMarket INTEGER NOT NULL,
    Commodity TEXT NOT NULL,
    Commodity_Localised TEXT,
    PurchaseOrder INTEGER,
    SaleOrder INTEGER,
    CancelTrade INTEGER,
    Price INTEGER,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS ChangeCrewRole (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Role TEXT NOT NULL,
    Telepresence INTEGER,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS ClearImpound (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    ShipType TEXT NOT NULL,
    ShipType_Localised TEXT,
    ShipID INTEGER NOT NULL,
    ShipMarketID INTEGER NOT NULL,
    MarketID INTEGER NOT NULL,
    System TEXT,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS ClearSavedGame (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    FID TEXT NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS CockpitBreached (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS CodexEntry (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    EntryID INTEGER NOT NULL,
    Name TEXT NOT NULL,
    Name_Localised TEXT,
    SubCategory TEXT NOT NULL,
    SubCategory_Localised TEXT,
    Category TEXT NOT NULL,
    Category_Localised TEXT,
    Region TEXT NOT NULL,
    Region_Localised TEXT,
    System TEXT NOT NULL,
    SystemAddress INTEGER NOT NULL,
    BodyID INTEGER,
    Latitude REAL,
    Longitude REAL,
    VoucherAmount INTEGER,
    NearestDestination TEXT,
    NearestDestination_Localised TEXT,
    IsNewEntry INTEGER,
    NewTraitsDiscovered INTEGER,
    Traits TEXT,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS CollectCargo (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Type TEXT NOT NULL,
    Type_Localised TEXT,
    Stolen INTEGER NOT NULL,
    MissionID INTEGER,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS CollectItems (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    Name_Localised TEXT,
    Type TEXT NOT NULL,
    OwnerID INTEGER NOT NULL,
    Count INTEGER NOT NULL,
    Stolen INTEGER NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS ColonisationBeaconDeployed (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS ColonisationConstructionDepot (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    MarketID INTEGER NOT NULL,
    ConstructionProgress REAL NOT NULL,
    ConstructionComplete INTEGER NOT NULL,
    ConstructionFailed INTEGER NOT NULL,
    event_timestamp TEXT NOT NULL
);

    CREATE TABLE IF NOT EXISTS ColonisationConstructionDepot_ResourcesRequired (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        ColonisationConstructionDepot_event_id INTEGER NOT NULL,
        Name TEXT NOT NULL,
        Name_Localised TEXT NOT NULL,
        RequiredAmount INTEGER NOT NULL,
        ProvidedAmount INTEGER NOT NULL,
        Payment INTEGER NOT NULL,
        FOREIGN KEY(ColonisationConstructionDepot_event_id) REFERENCES ColonisationConstructionDepot(event_id)
    );


CREATE TABLE IF NOT EXISTS ColonisationContribution (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    MarketID INTEGER NOT NULL,
    event_timestamp TEXT NOT NULL
);

    CREATE TABLE IF NOT EXISTS ColonisationContribution_Contributions (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        ColonisationContribution_event_id INTEGER NOT NULL,
        Name TEXT NOT NULL,
        Name_Localised TEXT NOT NULL,
        Amount INTEGER NOT NULL,
        FOREIGN KEY(ColonisationContribution_event_id) REFERENCES ColonisationContribution(event_id)
    );


CREATE TABLE IF NOT EXISTS ColonisationSystemClaim (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    StarSystem TEXT NOT NULL,
    SystemAddress INTEGER NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS ColonisationSystemClaimRelease (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    StarSystem TEXT NOT NULL,
    SystemAddress INTEGER NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS Commander (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    FID TEXT NOT NULL,
    Name TEXT NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS CommitCrime (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    CrimeType TEXT NOT NULL,
    Faction TEXT NOT NULL,
    Victim TEXT,
    Bounty INTEGER,
    Fine INTEGER,
    Victim_Localised TEXT,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS CommunityGoal (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    event_timestamp TEXT NOT NULL
);

    CREATE TABLE IF NOT EXISTS CommunityGoal_CurrentGoals (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        CommunityGoal_event_id INTEGER NOT NULL,
        CGID INTEGER NOT NULL,
        Title TEXT NOT NULL,
        SystemName TEXT NOT NULL,
        MarketName TEXT NOT NULL,
        Expiry TEXT NOT NULL,
        IsComplete INTEGER NOT NULL,
        CurrentTotal INTEGER NOT NULL,
        PlayerContribution INTEGER NOT NULL,
        NumContributors INTEGER NOT NULL,
        TopTier TEXT NOT NULL,
        TopRankSize INTEGER,
        PlayerInTopRank INTEGER,
        TierReached TEXT,
        PlayerPercentileBand INTEGER NOT NULL,
        Bonus INTEGER,
        FOREIGN KEY(CommunityGoal_event_id) REFERENCES CommunityGoal(event_id)
    );


CREATE TABLE IF NOT EXISTS CommunityGoalDiscard (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    CGID INTEGER NOT NULL,
    Name TEXT NOT NULL,
    System TEXT NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS CommunityGoalJoin (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    CGID INTEGER NOT NULL,
    Name TEXT NOT NULL,
    System TEXT NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS CommunityGoalReward (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    CGID INTEGER NOT NULL,
    Name TEXT NOT NULL,
    System TEXT NOT NULL,
    Reward INTEGER NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS CompleteConstruction (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS Continued (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Part INTEGER NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS CreateSuitLoadout (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    SuitID INTEGER NOT NULL,
    SuitName TEXT NOT NULL,
    SuitName_Localised TEXT,
    SuitMods TEXT NOT NULL,
    LoadoutID INTEGER NOT NULL,
    LoadoutName TEXT NOT NULL,
    event_timestamp TEXT NOT NULL
);

    CREATE TABLE IF NOT EXISTS CreateSuitLoadout_Modules (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        CreateSuitLoadout_event_id INTEGER NOT NULL,
        SlotName TEXT NOT NULL,
        SuitModuleID INTEGER NOT NULL,
        ModuleName TEXT NOT NULL,
        ModuleName_Localised TEXT,
        Class INTEGER NOT NULL,
        WeaponMods TEXT NOT NULL,
        FOREIGN KEY(CreateSuitLoadout_event_id) REFERENCES CreateSuitLoadout(event_id)
    );


CREATE TABLE IF NOT EXISTS CrewAssign (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    CrewID INTEGER NOT NULL,
    Role TEXT NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS CrewFire (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    CrewID INTEGER NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS CrewHire (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    CrewID INTEGER NOT NULL,
    Faction TEXT NOT NULL,
    Cost INTEGER NOT NULL,
    CombatRank INTEGER NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS CrewLaunchFighter (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Telepresence INTEGER NOT NULL,
    Crew TEXT NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS CrewMemberJoins (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Telepresence INTEGER,
    Crew TEXT NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS CrewMemberQuits (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Telepresence INTEGER,
    Crew TEXT NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS CrewMemberRoleChange (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Telepresence INTEGER,
    Crew TEXT NOT NULL,
    Role TEXT NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS CrimeVictim (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Offender TEXT NOT NULL,
    Offender_Localised TEXT,
    CrimeType TEXT NOT NULL,
    Fine INTEGER,
    Bounty INTEGER,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS DatalinkScan (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Message TEXT NOT NULL,
    Message_Localised TEXT,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS DatalinkVoucher (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Reward INTEGER NOT NULL,
    VictimFaction TEXT NOT NULL,
    PayeeFaction TEXT NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS DataScanned (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Type TEXT NOT NULL,
    Type_Localised TEXT,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS DeleteSuitLoadout (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    SuitID INTEGER NOT NULL,
    SuitName TEXT NOT NULL,
    SuitName_Localised TEXT,
    LoadoutID INTEGER NOT NULL,
    LoadoutName TEXT NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS DeliverPowerMicroResources (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    TotalCount INTEGER NOT NULL,
    MarketID INTEGER NOT NULL,
    event_timestamp TEXT NOT NULL
);

    CREATE TABLE IF NOT EXISTS DeliverPowerMicroResources_MicroResources (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        DeliverPowerMicroResources_event_id INTEGER NOT NULL,
        Name TEXT NOT NULL,
        Name_Localised TEXT,
        Category TEXT NOT NULL,
        Count INTEGER NOT NULL,
        FOREIGN KEY(DeliverPowerMicroResources_event_id) REFERENCES DeliverPowerMicroResources(event_id)
    );


CREATE TABLE IF NOT EXISTS Died (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    KillerName TEXT,
    KillerShip TEXT,
    KillerRank TEXT,
    KillerName_Localised TEXT,
    Killers TEXT,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS DisbandedSquadron (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    SquadronID INTEGER,
    SquadronName TEXT NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS DiscoveryScan (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    SystemAddress INTEGER NOT NULL,
    Bodies INTEGER NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS Disembark (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    SRV INTEGER NOT NULL,
    Taxi INTEGER NOT NULL,
    Multicrew INTEGER NOT NULL,
    ID INTEGER,
    StarSystem TEXT NOT NULL,
    SystemAddress INTEGER NOT NULL,
    Body TEXT NOT NULL,
    BodyID INTEGER NOT NULL,
    OnStation INTEGER NOT NULL,
    OnPlanet INTEGER NOT NULL,
    StationName TEXT,
    StationType TEXT,
    MarketID INTEGER,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS Docked (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    StationName TEXT NOT NULL,
    StationName_Localised TEXT,
    StationType TEXT NOT NULL,
    Taxi INTEGER,
    Multicrew INTEGER,
    StarSystem TEXT,
    SystemAddress INTEGER,
    MarketID INTEGER,
    StationGovernment TEXT,
    StationGovernment_Localised TEXT,
    StationServices TEXT,
    StationEconomy TEXT,
    StationEconomy_Localised TEXT,
    DistFromStarLS REAL NOT NULL,
    Wanted INTEGER,
    ActiveFine INTEGER,
    StationAllegiance TEXT,
    CockpitBreach INTEGER,
    StationState TEXT,
    event_timestamp TEXT NOT NULL
);

    CREATE TABLE IF NOT EXISTS Docked_StationFaction (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        Docked_event_id INTEGER NOT NULL,
        Name TEXT NOT NULL,
        FactionState TEXT,
        FOREIGN KEY(Docked_event_id) REFERENCES Docked(event_id)
    );

    CREATE TABLE IF NOT EXISTS Docked_StationEconomies (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        Docked_event_id INTEGER NOT NULL,
        Name TEXT NOT NULL,
        Name_Localised TEXT,
        Proportion REAL NOT NULL,
        FOREIGN KEY(Docked_event_id) REFERENCES Docked(event_id)
    );

    CREATE TABLE IF NOT EXISTS Docked_LandingPads (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        Docked_event_id INTEGER NOT NULL,
        Small INTEGER NOT NULL,
        Medium INTEGER NOT NULL,
        Large INTEGER NOT NULL,
        FOREIGN KEY(Docked_event_id) REFERENCES Docked(event_id)
    );


CREATE TABLE IF NOT EXISTS DockFighter (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    ID INTEGER NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS DockingCancelled (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    MarketID INTEGER,
    StationName TEXT,
    StationName_Localised TEXT,
    StationType TEXT,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS DockingDenied (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Reason TEXT NOT NULL,
    MarketID INTEGER NOT NULL,
    StationName TEXT NOT NULL,
    StationName_Localised TEXT,
    StationType TEXT NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS DockingGranted (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    LandingPad INTEGER NOT NULL,
    MarketID INTEGER NOT NULL,
    StationName TEXT NOT NULL,
    StationName_Localised TEXT,
    StationType TEXT NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS DockingRequested (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    MarketID INTEGER NOT NULL,
    StationName TEXT NOT NULL,
    StationName_Localised TEXT,
    StationType TEXT NOT NULL,
    event_timestamp TEXT NOT NULL
);

    CREATE TABLE IF NOT EXISTS DockingRequested_LandingPads (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        DockingRequested_event_id INTEGER NOT NULL,
        Small INTEGER NOT NULL,
        Medium INTEGER NOT NULL,
        Large INTEGER NOT NULL,
        FOREIGN KEY(DockingRequested_event_id) REFERENCES DockingRequested(event_id)
    );


CREATE TABLE IF NOT EXISTS DockingTimeout (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    MarketID INTEGER,
    StationName TEXT,
    StationName_Localised TEXT,
    StationType TEXT,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS DockSRV (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    SRVType TEXT,
    SRVType_Localised TEXT,
    ID INTEGER NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS DropItems (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    Name_Localised TEXT,
    Type TEXT NOT NULL,
    OwnerID INTEGER NOT NULL,
    Count INTEGER NOT NULL,
    MissionID INTEGER,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS DropshipDeploy (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    StarSystem TEXT NOT NULL,
    SystemAddress INTEGER NOT NULL,
    Body TEXT NOT NULL,
    BodyID INTEGER NOT NULL,
    OnStation INTEGER NOT NULL,
    OnPlanet INTEGER NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS EjectCargo (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Type TEXT NOT NULL,
    Type_Localised TEXT,
    Count INTEGER NOT NULL,
    Abandoned INTEGER NOT NULL,
    MissionID INTEGER,
    PowerplayOrigin TEXT,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS Embark (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    SRV INTEGER NOT NULL,
    Taxi INTEGER NOT NULL,
    Multicrew INTEGER NOT NULL,
    ID INTEGER,
    StarSystem TEXT NOT NULL,
    SystemAddress INTEGER NOT NULL,
    Body TEXT NOT NULL,
    BodyID INTEGER NOT NULL,
    OnStation INTEGER NOT NULL,
    OnPlanet INTEGER NOT NULL,
    StationName TEXT,
    StationType TEXT,
    MarketID INTEGER,
    event_timestamp TEXT NOT NULL
);

    CREATE TABLE IF NOT EXISTS Embark_Crew (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        Embark_event_id INTEGER NOT NULL,
        Name TEXT NOT NULL,
        Role TEXT NOT NULL,
        FOREIGN KEY(Embark_event_id) REFERENCES Embark(event_id)
    );


CREATE TABLE IF NOT EXISTS EndCrewSession (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    OnCrime INTEGER NOT NULL,
    Telepresence INTEGER,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS EngineerContribution (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Engineer TEXT NOT NULL,
    EngineerID INTEGER NOT NULL,
    Type TEXT NOT NULL,
    Commodity TEXT,
    Commodity_Localised TEXT,
    Quantity INTEGER NOT NULL,
    TotalQuantity INTEGER NOT NULL,
    Material TEXT,
    Material_Localised TEXT,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS EngineerCraft (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Slot TEXT NOT NULL,
    Module TEXT NOT NULL,
    Engineer TEXT,
    EngineerID INTEGER NOT NULL,
    BlueprintID INTEGER NOT NULL,
    BlueprintName TEXT NOT NULL,
    Level INTEGER NOT NULL,
    Quality REAL NOT NULL,
    ApplyExperimentalEffect TEXT,
    ExperimentalEffect TEXT,
    ExperimentalEffect_Localised TEXT,
    event_timestamp TEXT NOT NULL
);

    CREATE TABLE IF NOT EXISTS EngineerCraft_Ingredients (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        EngineerCraft_event_id INTEGER NOT NULL,
        Name TEXT NOT NULL,
        Name_Localised TEXT,
        Count INTEGER NOT NULL,
        FOREIGN KEY(EngineerCraft_event_id) REFERENCES EngineerCraft(event_id)
    );

    CREATE TABLE IF NOT EXISTS EngineerCraft_Modifiers (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        EngineerCraft_event_id INTEGER NOT NULL,
        Label TEXT NOT NULL,
        Value REAL,
        ValueStr TEXT,
        ValueStr_Localised TEXT,
        OriginalValue REAL,
        LessIsGood INTEGER,
        FOREIGN KEY(EngineerCraft_event_id) REFERENCES EngineerCraft(event_id)
    );


CREATE TABLE IF NOT EXISTS EngineerLegacyConvert (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Slot TEXT NOT NULL,
    Module TEXT NOT NULL,
    IsPreview INTEGER NOT NULL,
    Engineer TEXT NOT NULL,
    EngineerID INTEGER NOT NULL,
    BlueprintID INTEGER NOT NULL,
    BlueprintName TEXT NOT NULL,
    Level INTEGER NOT NULL,
    Quality REAL NOT NULL,
    event_timestamp TEXT NOT NULL
);

    CREATE TABLE IF NOT EXISTS EngineerLegacyConvert_Modifiers (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        EngineerLegacyConvert_event_id INTEGER NOT NULL,
        Label TEXT NOT NULL,
        Value REAL NOT NULL,
        OriginalValue REAL NOT NULL,
        LessIsGood INTEGER NOT NULL,
        FOREIGN KEY(EngineerLegacyConvert_event_id) REFERENCES EngineerLegacyConvert(event_id)
    );


CREATE TABLE IF NOT EXISTS EngineerProgress (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Engineer TEXT,
    EngineerID INTEGER,
    Progress TEXT,
    Rank INTEGER,
    RankProgress INTEGER,
    event_timestamp TEXT NOT NULL
);

    CREATE TABLE IF NOT EXISTS EngineerProgress_Engineers (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        EngineerProgress_event_id INTEGER NOT NULL,
        Engineer TEXT,
        EngineerID INTEGER,
        Progress TEXT,
        RankProgress INTEGER,
        Rank INTEGER,
        FOREIGN KEY(EngineerProgress_event_id) REFERENCES EngineerProgress(event_id)
    );


CREATE TABLE IF NOT EXISTS EscapeInterdiction (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Interdictor TEXT NOT NULL,
    Interdictor_Localised TEXT,
    IsPlayer INTEGER NOT NULL,
    IsThargoid INTEGER,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS FactionKillBond (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Reward INTEGER NOT NULL,
    AwardingFaction TEXT NOT NULL,
    AwardingFaction_Localised TEXT,
    VictimFaction TEXT NOT NULL,
    VictimFaction_Localised TEXT,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS FCMaterials (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    MarketID INTEGER NOT NULL,
    CarrierName TEXT NOT NULL,
    CarrierID TEXT NOT NULL,
    event_timestamp TEXT NOT NULL
);

    CREATE TABLE IF NOT EXISTS FCMaterials_Items (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        FCMaterials_event_id INTEGER NOT NULL,
        id INTEGER NOT NULL,
        Name TEXT NOT NULL,
        Name_Localised TEXT,
        Price INTEGER NOT NULL,
        Stock INTEGER NOT NULL,
        Demand INTEGER NOT NULL,
        FOREIGN KEY(FCMaterials_event_id) REFERENCES FCMaterials(event_id)
    );


CREATE TABLE IF NOT EXISTS FetchRemoteModule (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    StorageSlot INTEGER NOT NULL,
    StoredItem TEXT NOT NULL,
    StoredItem_Localised TEXT,
    ServerId INTEGER NOT NULL,
    TransferCost INTEGER NOT NULL,
    TransferTime INTEGER NOT NULL,
    Ship TEXT NOT NULL,
    ShipID INTEGER NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS FighterDestroyed (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    ID INTEGER,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS FighterRebuilt (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Loadout TEXT NOT NULL,
    ID INTEGER NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS Fileheader (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    part INTEGER NOT NULL,
    language TEXT NOT NULL,
    Odyssey INTEGER NOT NULL,
    gameversion TEXT NOT NULL,
    build TEXT NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS Friends (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Status TEXT NOT NULL,
    Name TEXT NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS FSDJump (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Taxi INTEGER,
    Multicrew INTEGER,
    Wanted INTEGER,
    StarSystem TEXT NOT NULL,
    SystemAddress INTEGER NOT NULL,
    StarPos TEXT NOT NULL,
    SystemAllegiance TEXT NOT NULL,
    SystemEconomy TEXT NOT NULL,
    SystemEconomy_Localised TEXT,
    SystemSecondEconomy TEXT NOT NULL,
    SystemSecondEconomy_Localised TEXT,
    SystemGovernment TEXT NOT NULL,
    SystemGovernment_Localised TEXT,
    SystemSecurity TEXT NOT NULL,
    SystemSecurity_Localised TEXT,
    Population INTEGER NOT NULL,
    Body TEXT NOT NULL,
    BodyID INTEGER NOT NULL,
    BodyType TEXT NOT NULL,
    JumpDist REAL NOT NULL,
    FuelUsed REAL NOT NULL,
    FuelLevel REAL NOT NULL,
    Powers TEXT,
    ControllingPower TEXT,
    PowerplayState TEXT,
    PowerplayStateControlProgress REAL,
    PowerplayStateReinforcement INTEGER,
    PowerplayStateUndermining INTEGER,
    BoostUsed INTEGER,
    event_timestamp TEXT NOT NULL
);

    CREATE TABLE IF NOT EXISTS FSDJump_Factions (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        FSDJump_event_id INTEGER NOT NULL,
        Name TEXT NOT NULL,
        FactionState TEXT NOT NULL,
        Government TEXT NOT NULL,
        Influence REAL NOT NULL,
        Allegiance TEXT NOT NULL,
        Happiness TEXT NOT NULL,
        Happiness_Localised TEXT,
        MyReputation REAL NOT NULL,
        ActiveStates TEXT,
        RecoveringStates TEXT,
        PendingStates TEXT,
        SquadronFaction INTEGER,
        HappiestSystem INTEGER,
        HomeSystem INTEGER,
        FOREIGN KEY(FSDJump_event_id) REFERENCES FSDJump(event_id)
    );

    CREATE TABLE IF NOT EXISTS FSDJump_SystemFaction (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        FSDJump_event_id INTEGER NOT NULL,
        Name TEXT NOT NULL,
        FactionState TEXT,
        FOREIGN KEY(FSDJump_event_id) REFERENCES FSDJump(event_id)
    );

    CREATE TABLE IF NOT EXISTS FSDJump_PowerplayConflictProgress (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        FSDJump_event_id INTEGER NOT NULL,
        Power TEXT NOT NULL,
        ConflictProgress REAL NOT NULL,
        FOREIGN KEY(FSDJump_event_id) REFERENCES FSDJump(event_id)
    );

    CREATE TABLE IF NOT EXISTS FSDJump_Conflicts (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        FSDJump_event_id INTEGER NOT NULL,
        WarType TEXT NOT NULL,
        Status TEXT NOT NULL,
        Faction1 TEXT NOT NULL,
        Faction2 TEXT NOT NULL,
        FOREIGN KEY(FSDJump_event_id) REFERENCES FSDJump(event_id)
    );

    CREATE TABLE IF NOT EXISTS FSDJump_ThargoidWar (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        FSDJump_event_id INTEGER NOT NULL,
        CurrentState TEXT NOT NULL,
        NextStateSuccess TEXT,
        NextStateFailure TEXT,
        SuccessStateReached INTEGER NOT NULL,
        WarProgress REAL,
        RemainingPorts INTEGER,
        EstimatedRemainingTime TEXT,
        FOREIGN KEY(FSDJump_event_id) REFERENCES FSDJump(event_id)
    );


CREATE TABLE IF NOT EXISTS FSDTarget (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    SystemAddress INTEGER NOT NULL,
    StarClass TEXT NOT NULL,
    RemainingJumpsInRoute INTEGER,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS FSSAllBodiesFound (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    SystemName TEXT NOT NULL,
    SystemAddress INTEGER NOT NULL,
    Count INTEGER NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS FSSBodySignals (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    BodyName TEXT NOT NULL,
    BodyID INTEGER NOT NULL,
    SystemAddress INTEGER NOT NULL,
    event_timestamp TEXT NOT NULL
);

    CREATE TABLE IF NOT EXISTS FSSBodySignals_Signals (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        FSSBodySignals_event_id INTEGER NOT NULL,
        Type TEXT NOT NULL,
        Type_Localised TEXT,
        Count INTEGER NOT NULL,
        FOREIGN KEY(FSSBodySignals_event_id) REFERENCES FSSBodySignals(event_id)
    );


CREATE TABLE IF NOT EXISTS FSSDiscoveryScan (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Progress REAL NOT NULL,
    BodyCount INTEGER NOT NULL,
    NonBodyCount INTEGER NOT NULL,
    SystemName TEXT NOT NULL,
    SystemAddress INTEGER NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS FSSSignalDiscovered (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    SystemAddress INTEGER NOT NULL,
    SignalName TEXT NOT NULL,
    SignalType TEXT,
    SignalName_Localised TEXT,
    IsStation INTEGER,
    USSType TEXT,
    USSType_Localised TEXT,
    SpawningState TEXT,
    SpawningState_Localised TEXT,
    SpawningFaction TEXT,
    SpawningFaction_Localised TEXT,
    SpawningPower TEXT,
    OpposingPower TEXT,
    ThreatLevel INTEGER,
    TimeRemaining REAL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS FuelScoop (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Scooped REAL NOT NULL,
    Total REAL NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS HeatDamage (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    ID INTEGER,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS HeatWarning (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS HoloscreenHacked (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    PowerBefore TEXT,
    PowerAfter TEXT NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS HullDamage (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Health REAL NOT NULL,
    PlayerPilot INTEGER NOT NULL,
    Fighter INTEGER,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS Interdicted (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Submitted INTEGER NOT NULL,
    Interdictor TEXT,
    IsPlayer INTEGER NOT NULL,
    IsThargoid INTEGER,
    CombatRank INTEGER,
    Faction TEXT,
    Power TEXT,
    Interdictor_Localised TEXT,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS Interdiction (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Success INTEGER NOT NULL,
    Submitted INTEGER,
    Interdicted TEXT,
    Interdicted_Localised TEXT,
    IsPlayer INTEGER NOT NULL,
    CombatRank INTEGER,
    Faction TEXT,
    Power TEXT,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS InvitedToSquadron (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    SquadronID INTEGER,
    SquadronName TEXT NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS JetConeBoost (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    BoostValue REAL NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS JetConeDamage (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Module TEXT NOT NULL,
    Module_Localised TEXT,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS JoinACrew (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Captain TEXT NOT NULL,
    Telepresence INTEGER,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS JoinedSquadron (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    SquadronID INTEGER,
    SquadronName TEXT NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS KickCrewMember (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Crew TEXT NOT NULL,
    OnCrime INTEGER NOT NULL,
    Telepresence INTEGER,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS KickedFromSquadron (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    SquadronID INTEGER,
    SquadronName TEXT NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS LaunchDrone (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Type TEXT NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS LaunchFighter (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Loadout TEXT NOT NULL,
    ID INTEGER NOT NULL,
    PlayerControlled INTEGER NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS LaunchSRV (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    SRVType TEXT,
    SRVType_Localised TEXT,
    Loadout TEXT NOT NULL,
    ID INTEGER NOT NULL,
    PlayerControlled INTEGER NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS LeaveBody (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    StarSystem TEXT NOT NULL,
    SystemAddress INTEGER NOT NULL,
    Body TEXT NOT NULL,
    BodyID INTEGER NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS LeftSquadron (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    SquadronID INTEGER,
    SquadronName TEXT NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS Liftoff (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    PlayerControlled INTEGER NOT NULL,
    Taxi INTEGER,
    Multicrew INTEGER,
    StarSystem TEXT,
    SystemAddress INTEGER,
    Body TEXT,
    BodyID INTEGER,
    OnStation INTEGER,
    OnPlanet INTEGER,
    Latitude REAL,
    Longitude REAL,
    NearestDestination TEXT,
    NearestDestination_Localised TEXT,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS LoadGame (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    FID TEXT NOT NULL,
    Commander TEXT NOT NULL,
    Horizons INTEGER NOT NULL,
    Odyssey INTEGER,
    Ship TEXT,
    Ship_Localised TEXT,
    ShipID INTEGER,
    ShipName TEXT,
    ShipIdent TEXT,
    FuelLevel REAL,
    FuelCapacity REAL,
    GameMode TEXT,
    Credits INTEGER NOT NULL,
    Loan INTEGER NOT NULL,
    language TEXT,
    gameversion TEXT,
    build TEXT,
    event_Group TEXT,
    StartLanded INTEGER,
    StartDead INTEGER,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS Loadout (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Ship TEXT NOT NULL,
    ShipID INTEGER NOT NULL,
    ShipName TEXT NOT NULL,
    ShipIdent TEXT NOT NULL,
    HullValue INTEGER,
    ModulesValue INTEGER,
    HullHealth REAL NOT NULL,
    UnladenMass REAL NOT NULL,
    CargoCapacity INTEGER NOT NULL,
    MaxJumpRange REAL NOT NULL,
    Rebuy INTEGER NOT NULL,
    Hot INTEGER,
    event_timestamp TEXT NOT NULL
);

    CREATE TABLE IF NOT EXISTS Loadout_FuelCapacity (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        Loadout_event_id INTEGER NOT NULL,
        Main REAL NOT NULL,
        Reserve REAL NOT NULL,
        FOREIGN KEY(Loadout_event_id) REFERENCES Loadout(event_id)
    );

    CREATE TABLE IF NOT EXISTS Loadout_Modules (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        Loadout_event_id INTEGER NOT NULL,
        Slot TEXT NOT NULL,
        Item TEXT NOT NULL,
        event_On INTEGER NOT NULL,
        Priority INTEGER NOT NULL,
        AmmoInClip INTEGER,
        AmmoInHopper INTEGER,
        Health REAL NOT NULL,
        Value INTEGER,
        Engineering TEXT,
        FOREIGN KEY(Loadout_event_id) REFERENCES Loadout(event_id)
    );


CREATE TABLE IF NOT EXISTS LoadoutEquipModule (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    LoadoutName TEXT NOT NULL,
    SuitID INTEGER NOT NULL,
    SuitName TEXT NOT NULL,
    SuitName_Localised TEXT,
    LoadoutID INTEGER NOT NULL,
    SlotName TEXT NOT NULL,
    ModuleName TEXT NOT NULL,
    ModuleName_Localised TEXT,
    Class INTEGER NOT NULL,
    WeaponMods TEXT NOT NULL,
    SuitModuleID INTEGER NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS LoadoutRemoveModule (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    LoadoutName TEXT NOT NULL,
    SuitID INTEGER NOT NULL,
    SuitName TEXT NOT NULL,
    SuitName_Localised TEXT,
    LoadoutID INTEGER NOT NULL,
    SlotName TEXT NOT NULL,
    ModuleName TEXT NOT NULL,
    ModuleName_Localised TEXT,
    Class INTEGER NOT NULL,
    SuitModuleID INTEGER NOT NULL,
    WeaponMods TEXT NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS Location (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    DistFromStarLS REAL,
    Docked INTEGER NOT NULL,
    Taxi INTEGER,
    Multicrew INTEGER,
    Wanted INTEGER,
    StarSystem TEXT NOT NULL,
    SystemAddress INTEGER NOT NULL,
    StarPos TEXT NOT NULL,
    SystemAllegiance TEXT NOT NULL,
    SystemEconomy TEXT NOT NULL,
    SystemEconomy_Localised TEXT,
    SystemSecondEconomy TEXT NOT NULL,
    SystemSecondEconomy_Localised TEXT,
    SystemGovernment TEXT NOT NULL,
    SystemGovernment_Localised TEXT,
    SystemSecurity TEXT NOT NULL,
    SystemSecurity_Localised TEXT,
    Population INTEGER NOT NULL,
    Body TEXT NOT NULL,
    BodyID INTEGER NOT NULL,
    BodyType TEXT NOT NULL,
    Powers TEXT,
    ControllingPower TEXT,
    PowerplayState TEXT,
    PowerplayStateControlProgress REAL,
    PowerplayStateReinforcement INTEGER,
    PowerplayStateUndermining INTEGER,
    OnFoot INTEGER,
    StationName TEXT,
    StationName_Localised TEXT,
    StationType TEXT,
    MarketID INTEGER,
    StationGovernment TEXT,
    StationGovernment_Localised TEXT,
    StationServices TEXT,
    StationEconomy TEXT,
    StationEconomy_Localised TEXT,
    Latitude REAL,
    Longitude REAL,
    InSRV INTEGER,
    StationAllegiance TEXT,
    event_timestamp TEXT NOT NULL
);

    CREATE TABLE IF NOT EXISTS Location_Factions (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        Location_event_id INTEGER NOT NULL,
        Name TEXT NOT NULL,
        FactionState TEXT NOT NULL,
        Government TEXT NOT NULL,
        Influence REAL NOT NULL,
        Allegiance TEXT NOT NULL,
        Happiness TEXT NOT NULL,
        Happiness_Localised TEXT,
        MyReputation REAL NOT NULL,
        ActiveStates TEXT,
        RecoveringStates TEXT,
        PendingStates TEXT,
        SquadronFaction INTEGER,
        HappiestSystem INTEGER,
        HomeSystem INTEGER,
        FOREIGN KEY(Location_event_id) REFERENCES Location(event_id)
    );

    CREATE TABLE IF NOT EXISTS Location_SystemFaction (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        Location_event_id INTEGER NOT NULL,
        Name TEXT NOT NULL,
        FactionState TEXT,
        FOREIGN KEY(Location_event_id) REFERENCES Location(event_id)
    );

    CREATE TABLE IF NOT EXISTS Location_PowerplayConflictProgress (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        Location_event_id INTEGER NOT NULL,
        Power TEXT NOT NULL,
        ConflictProgress REAL NOT NULL,
        FOREIGN KEY(Location_event_id) REFERENCES Location(event_id)
    );

    CREATE TABLE IF NOT EXISTS Location_Conflicts (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        Location_event_id INTEGER NOT NULL,
        WarType TEXT NOT NULL,
        Status TEXT NOT NULL,
        Faction1 TEXT NOT NULL,
        Faction2 TEXT NOT NULL,
        FOREIGN KEY(Location_event_id) REFERENCES Location(event_id)
    );

    CREATE TABLE IF NOT EXISTS Location_ThargoidWar (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        Location_event_id INTEGER NOT NULL,
        CurrentState TEXT NOT NULL,
        NextStateSuccess TEXT,
        NextStateFailure TEXT,
        SuccessStateReached INTEGER NOT NULL,
        WarProgress REAL,
        RemainingPorts INTEGER,
        EstimatedRemainingTime TEXT,
        FOREIGN KEY(Location_event_id) REFERENCES Location(event_id)
    );

    CREATE TABLE IF NOT EXISTS Location_StationFaction (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        Location_event_id INTEGER NOT NULL,
        Name TEXT NOT NULL,
        FactionState TEXT,
        FOREIGN KEY(Location_event_id) REFERENCES Location(event_id)
    );

    CREATE TABLE IF NOT EXISTS Location_StationEconomies (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        Location_event_id INTEGER NOT NULL,
        Name TEXT NOT NULL,
        Name_Localised TEXT,
        Proportion REAL NOT NULL,
        FOREIGN KEY(Location_event_id) REFERENCES Location(event_id)
    );


CREATE TABLE IF NOT EXISTS Market (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    MarketID INTEGER NOT NULL,
    StationName TEXT NOT NULL,
    StationName_Localised TEXT,
    StationType TEXT NOT NULL,
    CarrierDockingAccess TEXT,
    StarSystem TEXT NOT NULL,
    event_timestamp TEXT NOT NULL
);

    CREATE TABLE IF NOT EXISTS Market_Items (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        Market_event_id INTEGER NOT NULL,
        id INTEGER NOT NULL,
        Name TEXT NOT NULL,
        Name_Localised TEXT,
        Category TEXT NOT NULL,
        Category_Localised TEXT,
        BuyPrice INTEGER NOT NULL,
        SellPrice INTEGER NOT NULL,
        MeanPrice INTEGER NOT NULL,
        StockBracket INTEGER NOT NULL,
        DemandBracket INTEGER NOT NULL,
        Stock INTEGER NOT NULL,
        Demand INTEGER NOT NULL,
        Consumer INTEGER NOT NULL,
        Producer INTEGER NOT NULL,
        Rare INTEGER NOT NULL,
        FOREIGN KEY(Market_event_id) REFERENCES Market(event_id)
    );


CREATE TABLE IF NOT EXISTS MarketBuy (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    MarketID INTEGER NOT NULL,
    Type TEXT NOT NULL,
    Count INTEGER NOT NULL,
    BuyPrice INTEGER NOT NULL,
    TotalCost INTEGER NOT NULL,
    Type_Localised TEXT,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS MarketSell (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    MarketID INTEGER NOT NULL,
    Type TEXT NOT NULL,
    Count INTEGER NOT NULL,
    SellPrice INTEGER NOT NULL,
    TotalSale INTEGER NOT NULL,
    AvgPricePaid INTEGER NOT NULL,
    Type_Localised TEXT,
    StolenGoods INTEGER,
    IllegalGoods INTEGER,
    BlackMarket INTEGER,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS MassModuleStore (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    MarketID INTEGER NOT NULL,
    Ship TEXT NOT NULL,
    ShipID INTEGER NOT NULL,
    event_timestamp TEXT NOT NULL
);

    CREATE TABLE IF NOT EXISTS MassModuleStore_Items (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        MassModuleStore_event_id INTEGER NOT NULL,
        Slot TEXT NOT NULL,
        Name TEXT NOT NULL,
        Name_Localised TEXT,
        Hot INTEGER NOT NULL,
        EngineerModifications TEXT,
        Level INTEGER,
        Quality REAL,
        FOREIGN KEY(MassModuleStore_event_id) REFERENCES MassModuleStore(event_id)
    );


CREATE TABLE IF NOT EXISTS MaterialCollected (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Category TEXT NOT NULL,
    Name TEXT NOT NULL,
    Count INTEGER NOT NULL,
    Name_Localised TEXT,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS MaterialDiscarded (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Category TEXT NOT NULL,
    Name TEXT NOT NULL,
    Count INTEGER NOT NULL,
    Name_Localised TEXT,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS MaterialDiscovered (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Category TEXT NOT NULL,
    Name TEXT NOT NULL,
    Name_Localised TEXT,
    DiscoveryNumber INTEGER NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS Materials (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    event_timestamp TEXT NOT NULL
);

    CREATE TABLE IF NOT EXISTS Materials_Raw (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        Materials_event_id INTEGER NOT NULL,
        Name TEXT NOT NULL,
        Name_Localised TEXT,
        Count INTEGER NOT NULL,
        FOREIGN KEY(Materials_event_id) REFERENCES Materials(event_id)
    );

    CREATE TABLE IF NOT EXISTS Materials_Manufactured (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        Materials_event_id INTEGER NOT NULL,
        Name TEXT NOT NULL,
        Name_Localised TEXT,
        Count INTEGER NOT NULL,
        FOREIGN KEY(Materials_event_id) REFERENCES Materials(event_id)
    );

    CREATE TABLE IF NOT EXISTS Materials_Encoded (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        Materials_event_id INTEGER NOT NULL,
        Name TEXT NOT NULL,
        Name_Localised TEXT,
        Count INTEGER NOT NULL,
        FOREIGN KEY(Materials_event_id) REFERENCES Materials(event_id)
    );


CREATE TABLE IF NOT EXISTS MaterialTrade (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    MarketID INTEGER NOT NULL,
    TraderType TEXT NOT NULL,
    event_timestamp TEXT NOT NULL
);

    CREATE TABLE IF NOT EXISTS MaterialTrade_Paid (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        MaterialTrade_event_id INTEGER NOT NULL,
        Material TEXT NOT NULL,
        Material_Localised TEXT,
        Category TEXT NOT NULL,
        Quantity INTEGER NOT NULL,
        FOREIGN KEY(MaterialTrade_event_id) REFERENCES MaterialTrade(event_id)
    );

    CREATE TABLE IF NOT EXISTS MaterialTrade_Received (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        MaterialTrade_event_id INTEGER NOT NULL,
        Material TEXT NOT NULL,
        Material_Localised TEXT,
        Category TEXT NOT NULL,
        Quantity INTEGER NOT NULL,
        FOREIGN KEY(MaterialTrade_event_id) REFERENCES MaterialTrade(event_id)
    );


CREATE TABLE IF NOT EXISTS MiningRefined (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Type TEXT NOT NULL,
    Type_Localised TEXT,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS MissionAbandoned (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    LocalisedName TEXT,
    MissionID INTEGER NOT NULL,
    Fine INTEGER,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS MissionAccepted (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Faction TEXT NOT NULL,
    Name TEXT NOT NULL,
    LocalisedName TEXT NOT NULL,
    Commodity TEXT,
    Commodity_Localised TEXT,
    Count INTEGER,
    Expiry TEXT,
    Wing INTEGER NOT NULL,
    Influence TEXT NOT NULL,
    Reputation TEXT NOT NULL,
    Reward INTEGER,
    MissionID INTEGER NOT NULL,
    TargetType TEXT,
    TargetType_Localised TEXT,
    TargetFaction TEXT,
    DestinationSystem TEXT,
    DestinationStation TEXT,
    NewDestinationSystem TEXT,
    NewDestinationStation TEXT,
    Target TEXT,
    Donation TEXT,
    Target_Localised TEXT,
    DestinationSettlement TEXT,
    KillCount INTEGER,
    PassengerCount INTEGER,
    PassengerVIPs INTEGER,
    PassengerWanted INTEGER,
    PassengerType TEXT,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS MissionCompleted (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Faction TEXT NOT NULL,
    Name TEXT NOT NULL,
    LocalisedName TEXT,
    MissionID INTEGER NOT NULL,
    Commodity TEXT,
    Commodity_Localised TEXT,
    Count INTEGER,
    Reward INTEGER,
    PermitsAwarded TEXT,
    Donation TEXT,
    Donated INTEGER,
    TargetFaction TEXT,
    DestinationSystem TEXT,
    DestinationStation TEXT,
    Target TEXT,
    Target_Localised TEXT,
    DestinationSettlement TEXT,
    TargetType TEXT,
    TargetType_Localised TEXT,
    KillCount INTEGER,
    NewDestinationSystem TEXT,
    NewDestinationStation TEXT,
    event_timestamp TEXT NOT NULL
);

    CREATE TABLE IF NOT EXISTS MissionCompleted_CommodityReward (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        MissionCompleted_event_id INTEGER NOT NULL,
        Name TEXT NOT NULL,
        Name_Localised TEXT,
        Count INTEGER NOT NULL,
        FOREIGN KEY(MissionCompleted_event_id) REFERENCES MissionCompleted(event_id)
    );

    CREATE TABLE IF NOT EXISTS MissionCompleted_MaterialsReward (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        MissionCompleted_event_id INTEGER NOT NULL,
        Name TEXT NOT NULL,
        Name_Localised TEXT,
        Category TEXT NOT NULL,
        Category_Localised TEXT,
        Count INTEGER NOT NULL,
        FOREIGN KEY(MissionCompleted_event_id) REFERENCES MissionCompleted(event_id)
    );

    CREATE TABLE IF NOT EXISTS MissionCompleted_FactionEffects (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        MissionCompleted_event_id INTEGER NOT NULL,
        Faction TEXT NOT NULL,
        Effects TEXT NOT NULL,
        Influence TEXT NOT NULL,
        ReputationTrend TEXT NOT NULL,
        Reputation TEXT NOT NULL,
        FOREIGN KEY(MissionCompleted_event_id) REFERENCES MissionCompleted(event_id)
    );


CREATE TABLE IF NOT EXISTS MissionFailed (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    LocalisedName TEXT,
    MissionID INTEGER NOT NULL,
    Fine INTEGER,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS MissionRedirected (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    MissionID INTEGER NOT NULL,
    Name TEXT NOT NULL,
    LocalisedName TEXT,
    LocalisedName_Localised TEXT,
    NewDestinationStation TEXT NOT NULL,
    NewDestinationSystem TEXT NOT NULL,
    OldDestinationStation TEXT NOT NULL,
    OldDestinationSystem TEXT NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS Missions (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    event_timestamp TEXT NOT NULL
);

    CREATE TABLE IF NOT EXISTS Missions_Active (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        Missions_event_id INTEGER NOT NULL,
        MissionID INTEGER NOT NULL,
        Name TEXT NOT NULL,
        Name_Localised TEXT,
        PassengerMission INTEGER NOT NULL,
        Expires INTEGER NOT NULL,
        FOREIGN KEY(Missions_event_id) REFERENCES Missions(event_id)
    );

    CREATE TABLE IF NOT EXISTS Missions_Failed (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        Missions_event_id INTEGER NOT NULL,
        MissionID INTEGER NOT NULL,
        Name TEXT NOT NULL,
        Name_Localised TEXT,
        PassengerMission INTEGER NOT NULL,
        Expires INTEGER NOT NULL,
        FOREIGN KEY(Missions_event_id) REFERENCES Missions(event_id)
    );

    CREATE TABLE IF NOT EXISTS Missions_Complete (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        Missions_event_id INTEGER NOT NULL,
        MissionID INTEGER NOT NULL,
        Name TEXT NOT NULL,
        Name_Localised TEXT,
        PassengerMission INTEGER NOT NULL,
        Expires INTEGER NOT NULL,
        FOREIGN KEY(Missions_event_id) REFERENCES Missions(event_id)
    );


CREATE TABLE IF NOT EXISTS ModuleBuy (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Slot TEXT NOT NULL,
    BuyItem TEXT NOT NULL,
    BuyItem_Localised TEXT,
    MarketID INTEGER NOT NULL,
    BuyPrice INTEGER NOT NULL,
    Ship TEXT NOT NULL,
    ShipID INTEGER NOT NULL,
    StoredItem TEXT,
    StoredItem_Localised TEXT,
    SellItem TEXT,
    SellItem_Localised TEXT,
    SellPrice INTEGER,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS ModuleBuyAndStore (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    BuyItem TEXT NOT NULL,
    BuyItem_Localised TEXT,
    MarketID INTEGER NOT NULL,
    BuyPrice INTEGER NOT NULL,
    Ship TEXT NOT NULL,
    ShipID INTEGER NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS ModuleInfo (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    event_timestamp TEXT NOT NULL
);

    CREATE TABLE IF NOT EXISTS ModuleInfo_Modules (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        ModuleInfo_event_id INTEGER NOT NULL,
        Slot TEXT NOT NULL,
        Item TEXT NOT NULL,
        Power REAL NOT NULL,
        Priority INTEGER,
        FOREIGN KEY(ModuleInfo_event_id) REFERENCES ModuleInfo(event_id)
    );


CREATE TABLE IF NOT EXISTS ModuleRetrieve (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    MarketID INTEGER NOT NULL,
    Slot TEXT NOT NULL,
    RetrievedItem TEXT NOT NULL,
    RetrievedItem_Localised TEXT,
    Ship TEXT NOT NULL,
    ShipID INTEGER NOT NULL,
    Hot INTEGER NOT NULL,
    EngineerModifications TEXT,
    Level INTEGER,
    Quality REAL,
    SwapOutItem TEXT,
    SwapOutItem_Localised TEXT,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS ModuleSell (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    MarketID INTEGER NOT NULL,
    Slot TEXT NOT NULL,
    SellItem TEXT NOT NULL,
    SellItem_Localised TEXT,
    SellPrice INTEGER NOT NULL,
    Ship TEXT NOT NULL,
    ShipID INTEGER NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS ModuleSellRemote (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    StorageSlot INTEGER NOT NULL,
    SellItem TEXT NOT NULL,
    SellItem_Localised TEXT,
    ServerId INTEGER NOT NULL,
    SellPrice INTEGER NOT NULL,
    Ship TEXT NOT NULL,
    ShipID INTEGER NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS ModuleStore (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    MarketID INTEGER NOT NULL,
    Slot TEXT NOT NULL,
    StoredItem TEXT NOT NULL,
    ReplacementItem TEXT,
    StoredItem_Localised TEXT,
    Ship TEXT NOT NULL,
    ShipID INTEGER NOT NULL,
    Hot INTEGER,
    EngineerModifications TEXT,
    Level INTEGER,
    Quality REAL,
    Cost INTEGER,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS ModuleSwap (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    MarketID INTEGER NOT NULL,
    FromSlot TEXT NOT NULL,
    ToSlot TEXT NOT NULL,
    FromItem TEXT NOT NULL,
    FromItem_Localised TEXT,
    ToItem TEXT NOT NULL,
    ToItem_Localised TEXT,
    Ship TEXT NOT NULL,
    ShipID INTEGER NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS MultiSellExplorationData (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    BaseValue INTEGER NOT NULL,
    Bonus INTEGER NOT NULL,
    TotalEarnings INTEGER NOT NULL,
    event_timestamp TEXT NOT NULL
);

    CREATE TABLE IF NOT EXISTS MultiSellExplorationData_Discovered (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        MultiSellExplorationData_event_id INTEGER NOT NULL,
        SystemName TEXT NOT NULL,
        SystemName_Localised TEXT,
        NumBodies INTEGER NOT NULL,
        FOREIGN KEY(MultiSellExplorationData_event_id) REFERENCES MultiSellExplorationData(event_id)
    );


CREATE TABLE IF NOT EXISTS Music (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    MusicTrack TEXT NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS NavBeaconScan (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    SystemAddress INTEGER NOT NULL,
    NumBodies INTEGER NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS NavRoute (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    event_timestamp TEXT NOT NULL
);

    CREATE TABLE IF NOT EXISTS NavRoute_Route (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        NavRoute_event_id INTEGER NOT NULL,
        StarSystem TEXT NOT NULL,
        SystemAddress INTEGER NOT NULL,
        StarPos TEXT NOT NULL,
        StarClass TEXT NOT NULL,
        FOREIGN KEY(NavRoute_event_id) REFERENCES NavRoute(event_id)
    );


CREATE TABLE IF NOT EXISTS NavRouteClear (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    event_timestamp TEXT NOT NULL
);

    CREATE TABLE IF NOT EXISTS NavRouteClear_Route (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        NavRouteClear_event_id INTEGER NOT NULL,
        FOREIGN KEY(NavRouteClear_event_id) REFERENCES NavRouteClear(event_id)
    );


CREATE TABLE IF NOT EXISTS NewCommander (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    FID TEXT NOT NULL,
    Package TEXT,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS NpcCrewPaidWage (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    NpcCrewName TEXT NOT NULL,
    NpcCrewId INTEGER NOT NULL,
    Amount INTEGER NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS NpcCrewRank (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    NpcCrewName TEXT NOT NULL,
    NpcCrewId INTEGER NOT NULL,
    RankCombat INTEGER NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS Outfitting (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    MarketID INTEGER NOT NULL,
    StationName TEXT NOT NULL,
    StarSystem TEXT NOT NULL,
    Horizons INTEGER,
    event_timestamp TEXT NOT NULL
);

    CREATE TABLE IF NOT EXISTS Outfitting_Items (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        Outfitting_event_id INTEGER NOT NULL,
        id INTEGER NOT NULL,
        Name TEXT NOT NULL,
        BuyPrice INTEGER NOT NULL,
        FOREIGN KEY(Outfitting_event_id) REFERENCES Outfitting(event_id)
    );


CREATE TABLE IF NOT EXISTS Passengers (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    event_timestamp TEXT NOT NULL
);

    CREATE TABLE IF NOT EXISTS Passengers_Manifest (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        Passengers_event_id INTEGER NOT NULL,
        MissionID INTEGER NOT NULL,
        Type TEXT NOT NULL,
        VIP INTEGER NOT NULL,
        Wanted INTEGER NOT NULL,
        Count INTEGER NOT NULL,
        FOREIGN KEY(Passengers_event_id) REFERENCES Passengers(event_id)
    );


CREATE TABLE IF NOT EXISTS PayBounties (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Amount INTEGER NOT NULL,
    AllFines INTEGER,
    ShipID INTEGER NOT NULL,
    BrokerPercentage REAL,
    Faction TEXT,
    Faction_Localised TEXT,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS PayFines (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Amount INTEGER NOT NULL,
    AllFines INTEGER,
    ShipID INTEGER NOT NULL,
    BrokerPercentage REAL,
    Faction TEXT,
    Faction_Localised TEXT,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS Powerplay (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Power TEXT NOT NULL,
    Rank INTEGER NOT NULL,
    Merits INTEGER NOT NULL,
    Votes INTEGER,
    TimePledged INTEGER NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS PowerplayCollect (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Power TEXT NOT NULL,
    Type TEXT NOT NULL,
    Type_Localised TEXT,
    Count INTEGER NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS PowerplayDefect (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    FromPower TEXT NOT NULL,
    ToPower TEXT NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS PowerplayDeliver (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Power TEXT NOT NULL,
    Type TEXT NOT NULL,
    Type_Localised TEXT,
    Count INTEGER NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS PowerplayFastTrack (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Power TEXT NOT NULL,
    Cost INTEGER NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS PowerplayJoin (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Power TEXT NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS PowerplayLeave (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Power TEXT NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS PowerplayMerits (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Power TEXT NOT NULL,
    MeritsGained INTEGER NOT NULL,
    TotalMerits INTEGER NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS PowerplayRank (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Power TEXT NOT NULL,
    Rank INTEGER NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS PowerplaySalary (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Power TEXT NOT NULL,
    Amount INTEGER NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS PowerplayVote (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Power TEXT NOT NULL,
    Votes INTEGER NOT NULL,
    VoteToConsolidate INTEGER NOT NULL,
    System TEXT,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS PowerplayVoucher (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Power TEXT NOT NULL,
    Systems TEXT NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS Progress (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Combat INTEGER NOT NULL,
    Trade INTEGER NOT NULL,
    Explore INTEGER NOT NULL,
    Soldier INTEGER,
    Exobiologist INTEGER,
    Empire INTEGER NOT NULL,
    Federation INTEGER NOT NULL,
    CQC INTEGER NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS Promotion (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Explore INTEGER,
    Combat INTEGER,
    Soldier INTEGER,
    Federation INTEGER,
    Exobiologist INTEGER,
    Empire INTEGER,
    Trade INTEGER,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS ProspectedAsteroid (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Content TEXT NOT NULL,
    Content_Localised TEXT,
    Remaining REAL NOT NULL,
    MotherlodeMaterial TEXT,
    MotherlodeMaterial_Localised TEXT,
    event_timestamp TEXT NOT NULL
);

    CREATE TABLE IF NOT EXISTS ProspectedAsteroid_Materials (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        ProspectedAsteroid_event_id INTEGER NOT NULL,
        Name TEXT NOT NULL,
        Name_Localised TEXT,
        Proportion REAL NOT NULL,
        FOREIGN KEY(ProspectedAsteroid_event_id) REFERENCES ProspectedAsteroid(event_id)
    );


CREATE TABLE IF NOT EXISTS PVPKill (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Victim TEXT NOT NULL,
    CombatRank INTEGER NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS QuitACrew (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Captain TEXT NOT NULL,
    Telepresence INTEGER,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS Rank (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Combat INTEGER NOT NULL,
    Trade INTEGER NOT NULL,
    Explore INTEGER NOT NULL,
    Soldier INTEGER,
    Exobiologist INTEGER,
    Empire INTEGER NOT NULL,
    Federation INTEGER NOT NULL,
    CQC INTEGER NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS RebootRepair (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Modules TEXT NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS ReceiveText (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    event_From TEXT NOT NULL,
    Message TEXT NOT NULL,
    Message_Localised TEXT,
    Channel TEXT,
    From_Localised TEXT,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS RedeemVoucher (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Type TEXT NOT NULL,
    Faction TEXT,
    Amount INTEGER NOT NULL,
    BrokerPercentage REAL,
    event_timestamp TEXT NOT NULL
);

    CREATE TABLE IF NOT EXISTS RedeemVoucher_Factions (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        RedeemVoucher_event_id INTEGER NOT NULL,
        Faction TEXT NOT NULL,
        Amount INTEGER NOT NULL,
        FOREIGN KEY(RedeemVoucher_event_id) REFERENCES RedeemVoucher(event_id)
    );


CREATE TABLE IF NOT EXISTS RefuelAll (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Cost INTEGER NOT NULL,
    Amount REAL NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS RefuelPartial (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Cost INTEGER NOT NULL,
    Amount REAL NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS RenameSuitLoadout (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    SuitID INTEGER NOT NULL,
    SuitName TEXT NOT NULL,
    SuitName_Localised TEXT,
    LoadoutID INTEGER NOT NULL,
    LoadoutName TEXT NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS Repair (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Cost INTEGER NOT NULL,
    Item TEXT,
    Items TEXT,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS RepairAll (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Cost INTEGER NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS RepairDrone (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    HullRepaired REAL,
    CockpitRepaired REAL,
    CorrosionRepaired REAL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS Reputation (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Empire REAL,
    Federation REAL,
    Alliance REAL,
    Independent REAL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS RequestPowerMicroResources (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    TotalCount INTEGER NOT NULL,
    MarketID INTEGER NOT NULL,
    event_timestamp TEXT NOT NULL
);

    CREATE TABLE IF NOT EXISTS RequestPowerMicroResources_MicroResources (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        RequestPowerMicroResources_event_id INTEGER NOT NULL,
        Name TEXT NOT NULL,
        Name_Localised TEXT,
        Category TEXT NOT NULL,
        Count INTEGER NOT NULL,
        FOREIGN KEY(RequestPowerMicroResources_event_id) REFERENCES RequestPowerMicroResources(event_id)
    );


CREATE TABLE IF NOT EXISTS ReservoirReplenished (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    FuelMain REAL NOT NULL,
    FuelReservoir REAL NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS RestockVehicle (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Type TEXT NOT NULL,
    Type_Localised TEXT,
    Loadout TEXT NOT NULL,
    ID INTEGER,
    Cost INTEGER NOT NULL,
    Count INTEGER NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS Resupply (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS Resurrect (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Option TEXT NOT NULL,
    Cost INTEGER NOT NULL,
    Bankrupt INTEGER NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS SAAScanComplete (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    BodyName TEXT NOT NULL,
    SystemAddress INTEGER NOT NULL,
    BodyID INTEGER NOT NULL,
    ProbesUsed INTEGER NOT NULL,
    EfficiencyTarget INTEGER NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS SAASignalsFound (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    BodyName TEXT NOT NULL,
    SystemAddress INTEGER NOT NULL,
    BodyID INTEGER NOT NULL,
    event_timestamp TEXT NOT NULL
);

    CREATE TABLE IF NOT EXISTS SAASignalsFound_Signals (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        SAASignalsFound_event_id INTEGER NOT NULL,
        Type TEXT NOT NULL,
        Type_Localised TEXT,
        Count INTEGER NOT NULL,
        FOREIGN KEY(SAASignalsFound_event_id) REFERENCES SAASignalsFound(event_id)
    );

    CREATE TABLE IF NOT EXISTS SAASignalsFound_Genuses (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        SAASignalsFound_event_id INTEGER NOT NULL,
        Genus TEXT NOT NULL,
        Genus_Localised TEXT,
        FOREIGN KEY(SAASignalsFound_event_id) REFERENCES SAASignalsFound(event_id)
    );


CREATE TABLE IF NOT EXISTS Scan (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    ScanType TEXT NOT NULL,
    BodyName TEXT NOT NULL,
    BodyID INTEGER NOT NULL,
    StarSystem TEXT NOT NULL,
    SystemAddress INTEGER NOT NULL,
    DistanceFromArrivalLS REAL NOT NULL,
    TidalLock INTEGER,
    TerraformState TEXT,
    PlanetClass TEXT,
    Atmosphere TEXT,
    AtmosphereType TEXT,
    Volcanism TEXT,
    MassEM REAL,
    Radius REAL,
    SurfaceGravity REAL,
    SurfaceTemperature REAL,
    SurfacePressure REAL,
    Landable INTEGER,
    SemiMajorAxis REAL,
    Eccentricity REAL,
    OrbitalInclination REAL,
    Periapsis REAL,
    OrbitalPeriod REAL,
    AscendingNode REAL,
    MeanAnomaly REAL,
    RotationPeriod REAL,
    AxialTilt REAL,
    WasDiscovered INTEGER NOT NULL,
    WasMapped INTEGER NOT NULL,
    WasFootfalled INTEGER,
    StarType TEXT,
    Subclass INTEGER,
    StellarMass REAL,
    AbsoluteMagnitude REAL,
    Age_MY INTEGER,
    Luminosity TEXT,
    ReserveLevel TEXT,
    event_timestamp TEXT NOT NULL
);

    CREATE TABLE IF NOT EXISTS Scan_Parents (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        Scan_event_id INTEGER NOT NULL,
        Star INTEGER,
        event_Null INTEGER,
        Ring INTEGER,
        Planet INTEGER,
        FOREIGN KEY(Scan_event_id) REFERENCES Scan(event_id)
    );

    CREATE TABLE IF NOT EXISTS Scan_AtmosphereComposition (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        Scan_event_id INTEGER NOT NULL,
        Name TEXT NOT NULL,
        Percent REAL NOT NULL,
        FOREIGN KEY(Scan_event_id) REFERENCES Scan(event_id)
    );

    CREATE TABLE IF NOT EXISTS Scan_Composition (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        Scan_event_id INTEGER NOT NULL,
        Ice REAL NOT NULL,
        Rock REAL NOT NULL,
        Metal REAL NOT NULL,
        FOREIGN KEY(Scan_event_id) REFERENCES Scan(event_id)
    );

    CREATE TABLE IF NOT EXISTS Scan_Rings (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        Scan_event_id INTEGER NOT NULL,
        Name TEXT NOT NULL,
        RingClass TEXT NOT NULL,
        MassMT REAL NOT NULL,
        InnerRad REAL NOT NULL,
        OuterRad REAL NOT NULL,
        FOREIGN KEY(Scan_event_id) REFERENCES Scan(event_id)
    );

    CREATE TABLE IF NOT EXISTS Scan_Materials (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        Scan_event_id INTEGER NOT NULL,
        Name TEXT NOT NULL,
        Name_Localised TEXT,
        Percent REAL NOT NULL,
        FOREIGN KEY(Scan_event_id) REFERENCES Scan(event_id)
    );


CREATE TABLE IF NOT EXISTS ScanBaryCentre (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    StarSystem TEXT NOT NULL,
    SystemAddress INTEGER NOT NULL,
    BodyID INTEGER NOT NULL,
    SemiMajorAxis REAL NOT NULL,
    Eccentricity REAL NOT NULL,
    OrbitalInclination REAL NOT NULL,
    Periapsis REAL NOT NULL,
    OrbitalPeriod REAL NOT NULL,
    AscendingNode REAL NOT NULL,
    MeanAnomaly REAL NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS Scanned (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    ScanType TEXT NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS ScanOrganic (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    ScanType TEXT NOT NULL,
    Genus TEXT NOT NULL,
    Genus_Localised TEXT,
    Species TEXT NOT NULL,
    Species_Localised TEXT,
    Variant TEXT,
    Variant_Localised TEXT,
    WasLogged INTEGER,
    SystemAddress INTEGER NOT NULL,
    Body INTEGER NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS ScientificResearch (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    MarketID INTEGER NOT NULL,
    Category TEXT NOT NULL,
    Name TEXT NOT NULL,
    Name_Localised TEXT,
    Count INTEGER NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS Screenshot (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Filename TEXT NOT NULL,
    Width INTEGER NOT NULL,
    Height INTEGER NOT NULL,
    System TEXT,
    Body TEXT,
    Latitude REAL,
    Longitude REAL,
    Altitude REAL,
    Heading INTEGER,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS SearchAndRescue (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    MarketID INTEGER NOT NULL,
    Name TEXT NOT NULL,
    Name_Localised TEXT,
    Count INTEGER NOT NULL,
    Reward INTEGER NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS SelfDestruct (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS SellDrones (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Type TEXT NOT NULL,
    Count INTEGER NOT NULL,
    SellPrice INTEGER NOT NULL,
    TotalSale INTEGER NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS SellExplorationData (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Systems TEXT NOT NULL,
    Discovered TEXT NOT NULL,
    BaseValue INTEGER NOT NULL,
    Bonus INTEGER NOT NULL,
    TotalEarnings INTEGER NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS SellMicroResources (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    TotalCount INTEGER NOT NULL,
    Price INTEGER NOT NULL,
    MarketID INTEGER NOT NULL,
    event_timestamp TEXT NOT NULL
);

    CREATE TABLE IF NOT EXISTS SellMicroResources_MicroResources (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        SellMicroResources_event_id INTEGER NOT NULL,
        Name TEXT NOT NULL,
        Name_Localised TEXT,
        Category TEXT NOT NULL,
        Count INTEGER NOT NULL,
        FOREIGN KEY(SellMicroResources_event_id) REFERENCES SellMicroResources(event_id)
    );


CREATE TABLE IF NOT EXISTS SellOrganicData (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    MarketID INTEGER NOT NULL,
    event_timestamp TEXT NOT NULL
);

    CREATE TABLE IF NOT EXISTS SellOrganicData_BioData (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        SellOrganicData_event_id INTEGER NOT NULL,
        Genus TEXT NOT NULL,
        Genus_Localised TEXT,
        Species TEXT NOT NULL,
        Species_Localised TEXT,
        Variant TEXT,
        Variant_Localised TEXT,
        Value INTEGER NOT NULL,
        Bonus INTEGER NOT NULL,
        FOREIGN KEY(SellOrganicData_event_id) REFERENCES SellOrganicData(event_id)
    );


CREATE TABLE IF NOT EXISTS SellShipOnRebuy (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    ShipType TEXT NOT NULL,
    System TEXT NOT NULL,
    SellShipId INTEGER NOT NULL,
    ShipPrice INTEGER NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS SellSuit (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    SuitID INTEGER NOT NULL,
    SuitMods TEXT NOT NULL,
    Name TEXT NOT NULL,
    Name_Localised TEXT,
    Price INTEGER NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS SellWeapon (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    Name_Localised TEXT,
    Class INTEGER NOT NULL,
    WeaponMods TEXT NOT NULL,
    Price INTEGER NOT NULL,
    SuitModuleID INTEGER NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS SendText (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    event_To TEXT NOT NULL,
    To_Localised TEXT,
    Message TEXT NOT NULL,
    Sent INTEGER NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS SetUserShipName (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Ship TEXT NOT NULL,
    ShipID INTEGER NOT NULL,
    UserShipName TEXT NOT NULL,
    UserShipId TEXT,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS SharedBookmarkToSquadron (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    SquadronID INTEGER,
    SquadronName TEXT NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS ShieldState (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    ShieldsUp INTEGER NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS ShipLocker (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Items TEXT,
    Components TEXT,
    Consumables TEXT,
    Data TEXT,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS ShipLockerBackpack (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS ShipLockerMaterials (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    event_timestamp TEXT NOT NULL
);

    CREATE TABLE IF NOT EXISTS ShipLockerMaterials_Items (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        ShipLockerMaterials_event_id INTEGER NOT NULL,
        Name TEXT NOT NULL,
        Name_Localised TEXT,
        OwnerID INTEGER NOT NULL,
        Count INTEGER NOT NULL,
        MissionID INTEGER,
        FOREIGN KEY(ShipLockerMaterials_event_id) REFERENCES ShipLockerMaterials(event_id)
    );

    CREATE TABLE IF NOT EXISTS ShipLockerMaterials_Components (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        ShipLockerMaterials_event_id INTEGER NOT NULL,
        Name TEXT NOT NULL,
        Name_Localised TEXT,
        OwnerID INTEGER NOT NULL,
        Count INTEGER NOT NULL,
        MissionID INTEGER,
        FOREIGN KEY(ShipLockerMaterials_event_id) REFERENCES ShipLockerMaterials(event_id)
    );

    CREATE TABLE IF NOT EXISTS ShipLockerMaterials_Consumables (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        ShipLockerMaterials_event_id INTEGER NOT NULL,
        Name TEXT NOT NULL,
        Name_Localised TEXT,
        OwnerID INTEGER NOT NULL,
        Count INTEGER NOT NULL,
        FOREIGN KEY(ShipLockerMaterials_event_id) REFERENCES ShipLockerMaterials(event_id)
    );

    CREATE TABLE IF NOT EXISTS ShipLockerMaterials_Data (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        ShipLockerMaterials_event_id INTEGER NOT NULL,
        Name TEXT NOT NULL,
        Name_Localised TEXT,
        OwnerID INTEGER NOT NULL,
        Count INTEGER NOT NULL,
        MissionID INTEGER,
        FOREIGN KEY(ShipLockerMaterials_event_id) REFERENCES ShipLockerMaterials(event_id)
    );


CREATE TABLE IF NOT EXISTS ShipRedeemed (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    ShipType TEXT NOT NULL,
    ShipType_Localised TEXT,
    NewShipID INTEGER NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS ShipTargeted (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    TargetLocked INTEGER NOT NULL,
    Ship TEXT,
    ScanStage INTEGER,
    PilotName TEXT,
    PilotName_Localised TEXT,
    PilotRank TEXT,
    ShieldHealth REAL,
    HullHealth REAL,
    Faction TEXT,
    LegalStatus TEXT,
    Ship_Localised TEXT,
    SquadronID TEXT,
    Power TEXT,
    Bounty INTEGER,
    Subsystem TEXT,
    Subsystem_Localised TEXT,
    SubsystemHealth REAL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS Shipyard (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    MarketID INTEGER NOT NULL,
    StationName TEXT NOT NULL,
    StarSystem TEXT NOT NULL,
    Horizons INTEGER,
    AllowCobraMkIV INTEGER,
    event_timestamp TEXT NOT NULL
);

    CREATE TABLE IF NOT EXISTS Shipyard_PriceList (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        Shipyard_event_id INTEGER NOT NULL,
        id INTEGER NOT NULL,
        ShipType TEXT NOT NULL,
        ShipPrice INTEGER NOT NULL,
        ShipType_Localised TEXT,
        FOREIGN KEY(Shipyard_event_id) REFERENCES Shipyard(event_id)
    );


CREATE TABLE IF NOT EXISTS ShipyardBankDeposit (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    MarketID INTEGER NOT NULL,
    ShipType TEXT NOT NULL,
    ShipType_Localised TEXT,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS ShipyardBuy (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    ShipType TEXT NOT NULL,
    ShipType_Localised TEXT,
    ShipPrice INTEGER NOT NULL,
    StoreOldShip TEXT,
    StoreShipID INTEGER,
    MarketID INTEGER NOT NULL,
    SellOldShip TEXT,
    SellShipID INTEGER,
    SellPrice INTEGER,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS ShipyardNew (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    ShipType TEXT NOT NULL,
    ShipType_Localised TEXT,
    NewShipID INTEGER NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS ShipyardRedeem (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    ShipType TEXT NOT NULL,
    ShipType_Localised TEXT,
    BundleID INTEGER,
    MarketID INTEGER,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS ShipyardSell (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    ShipType TEXT NOT NULL,
    SellShipID INTEGER NOT NULL,
    ShipPrice INTEGER NOT NULL,
    MarketID INTEGER NOT NULL,
    ShipType_Localised TEXT,
    System TEXT,
    ShipMarketID INTEGER,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS ShipyardSwap (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    ShipType TEXT NOT NULL,
    ShipType_Localised TEXT,
    ShipID INTEGER NOT NULL,
    StoreOldShip TEXT NOT NULL,
    StoreShipID INTEGER NOT NULL,
    MarketID INTEGER NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS ShipyardTransfer (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    ShipType TEXT NOT NULL,
    ShipType_Localised TEXT,
    ShipID INTEGER NOT NULL,
    System TEXT NOT NULL,
    ShipMarketID INTEGER NOT NULL,
    Distance REAL NOT NULL,
    TransferPrice INTEGER NOT NULL,
    TransferTime INTEGER NOT NULL,
    MarketID INTEGER NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS Shutdown (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS SquadronApplicationApproved (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    SquadronID INTEGER NOT NULL,
    SquadronName TEXT NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS SquadronApplicationRejected (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    SquadronID INTEGER NOT NULL,
    SquadronName TEXT NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS SquadronCreated (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    SquadronID INTEGER,
    SquadronName TEXT NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS SquadronDemotion (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    SquadronID INTEGER,
    SquadronName TEXT NOT NULL,
    OldRank INTEGER NOT NULL,
    NewRank INTEGER NOT NULL,
    OldRankName TEXT,
    OldRankName_Localised TEXT,
    NewRankName TEXT,
    NewRankName_Localised TEXT,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS SquadronPromotion (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    SquadronID INTEGER,
    SquadronName TEXT NOT NULL,
    OldRank INTEGER NOT NULL,
    NewRank INTEGER NOT NULL,
    OldRankName TEXT,
    OldRankName_Localised TEXT,
    NewRankName TEXT,
    NewRankName_Localised TEXT,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS SquadronStartup (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    SquadronName TEXT NOT NULL,
    CurrentRank INTEGER NOT NULL,
    CurrentRankName TEXT,
    CurrentRankName_Localised TEXT,
    SquadronID INTEGER,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS SRVDestroyed (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    ID INTEGER NOT NULL,
    SRVType TEXT,
    SRVType_Localised TEXT,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS StartJump (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Taxi INTEGER,
    JumpType TEXT NOT NULL,
    StarSystem TEXT,
    SystemAddress INTEGER,
    StarClass TEXT,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS Statistics (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    event_timestamp TEXT NOT NULL
);

    CREATE TABLE IF NOT EXISTS Statistics_Bank_Account (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        Statistics_event_id INTEGER NOT NULL,
        Current_Wealth INTEGER NOT NULL,
        Spent_On_Ships INTEGER NOT NULL,
        Spent_On_Outfitting INTEGER NOT NULL,
        Spent_On_Repairs INTEGER NOT NULL,
        Spent_On_Fuel INTEGER NOT NULL,
        Spent_On_Ammo_Consumables INTEGER NOT NULL,
        Insurance_Claims INTEGER NOT NULL,
        Spent_On_Insurance INTEGER NOT NULL,
        Owned_Ship_Count INTEGER,
        Spent_On_Suits INTEGER,
        Spent_On_Weapons INTEGER,
        Spent_On_Suit_Consumables INTEGER,
        Suits_Owned INTEGER,
        Weapons_Owned INTEGER,
        Spent_On_Premium_Stock INTEGER,
        Premium_Stock_Bought INTEGER,
        FOREIGN KEY(Statistics_event_id) REFERENCES Statistics(event_id)
    );

    CREATE TABLE IF NOT EXISTS Statistics_Combat (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        Statistics_event_id INTEGER NOT NULL,
        Bounties_Claimed INTEGER NOT NULL,
        Bounty_Hunting_Profit REAL NOT NULL,
        Combat_Bonds INTEGER NOT NULL,
        Combat_Bond_Profits INTEGER NOT NULL,
        Assassinations INTEGER NOT NULL,
        Assassination_Profits INTEGER NOT NULL,
        Highest_Single_Reward INTEGER NOT NULL,
        Skimmers_Killed INTEGER,
        OnFoot_Combat_Bonds INTEGER,
        OnFoot_Combat_Bonds_Profits INTEGER,
        OnFoot_Vehicles_Destroyed INTEGER,
        OnFoot_Ships_Destroyed INTEGER,
        Dropships_Taken INTEGER,
        Dropships_Booked INTEGER,
        Dropships_Cancelled INTEGER,
        ConflictZone_High INTEGER,
        ConflictZone_Medium INTEGER,
        ConflictZone_Low INTEGER,
        ConflictZone_Total INTEGER,
        ConflictZone_High_Wins INTEGER,
        ConflictZone_Medium_Wins INTEGER,
        ConflictZone_Low_Wins INTEGER,
        ConflictZone_Total_Wins INTEGER,
        Settlement_Defended INTEGER,
        Settlement_Conquered INTEGER,
        OnFoot_Skimmers_Killed INTEGER,
        OnFoot_Scavs_Killed INTEGER,
        FOREIGN KEY(Statistics_event_id) REFERENCES Statistics(event_id)
    );

    CREATE TABLE IF NOT EXISTS Statistics_Crime (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        Statistics_event_id INTEGER NOT NULL,
        Notoriety INTEGER,
        Fines INTEGER NOT NULL,
        Total_Fines INTEGER NOT NULL,
        Bounties_Received INTEGER NOT NULL,
        Total_Bounties INTEGER NOT NULL,
        Highest_Bounty INTEGER NOT NULL,
        Malware_Uploaded INTEGER,
        Settlements_State_Shutdown INTEGER,
        Production_Sabotage INTEGER,
        Production_Theft INTEGER,
        Total_Murders INTEGER,
        Citizens_Murdered INTEGER,
        Omnipol_Murdered INTEGER,
        Guards_Murdered INTEGER,
        Data_Stolen INTEGER,
        Goods_Stolen INTEGER,
        Sample_Stolen INTEGER,
        Total_Stolen INTEGER,
        Turrets_Destroyed INTEGER,
        Turrets_Overloaded INTEGER,
        Turrets_Total INTEGER,
        Value_Stolen_StateChange INTEGER,
        Profiles_Cloned INTEGER,
        FOREIGN KEY(Statistics_event_id) REFERENCES Statistics(event_id)
    );

    CREATE TABLE IF NOT EXISTS Statistics_Smuggling (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        Statistics_event_id INTEGER NOT NULL,
        Black_Markets_Traded_With INTEGER NOT NULL,
        Black_Markets_Profits INTEGER NOT NULL,
        Resources_Smuggled INTEGER NOT NULL,
        Average_Profit REAL NOT NULL,
        Highest_Single_Transaction INTEGER NOT NULL,
        FOREIGN KEY(Statistics_event_id) REFERENCES Statistics(event_id)
    );

    CREATE TABLE IF NOT EXISTS Statistics_Trading (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        Statistics_event_id INTEGER NOT NULL,
        Markets_Traded_With INTEGER NOT NULL,
        Market_Profits INTEGER NOT NULL,
        Resources_Traded INTEGER NOT NULL,
        Average_Profit REAL NOT NULL,
        Highest_Single_Transaction INTEGER NOT NULL,
        Data_Sold INTEGER,
        Goods_Sold INTEGER,
        Assets_Sold INTEGER,
        FOREIGN KEY(Statistics_event_id) REFERENCES Statistics(event_id)
    );

    CREATE TABLE IF NOT EXISTS Statistics_Mining (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        Statistics_event_id INTEGER NOT NULL,
        Mining_Profits INTEGER NOT NULL,
        Quantity_Mined INTEGER NOT NULL,
        Materials_Collected INTEGER,
        FOREIGN KEY(Statistics_event_id) REFERENCES Statistics(event_id)
    );

    CREATE TABLE IF NOT EXISTS Statistics_Exploration (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        Statistics_event_id INTEGER NOT NULL,
        Systems_Visited INTEGER NOT NULL,
        Exploration_Profits INTEGER NOT NULL,
        Planets_Scanned_To_Level_2 INTEGER NOT NULL,
        Planets_Scanned_To_Level_3 INTEGER NOT NULL,
        Efficient_Scans INTEGER,
        Highest_Payout INTEGER NOT NULL,
        Total_Hyperspace_Distance INTEGER NOT NULL,
        Total_Hyperspace_Jumps INTEGER NOT NULL,
        Greatest_Distance_From_Start REAL NOT NULL,
        Time_Played INTEGER NOT NULL,
        OnFoot_Distance_Travelled INTEGER,
        Shuttle_Journeys INTEGER,
        Shuttle_Distance_Travelled REAL,
        Spent_On_Shuttles INTEGER,
        First_Footfalls INTEGER,
        Planet_Footfalls INTEGER,
        Settlements_Visited INTEGER,
        Fuel_Scooped INTEGER,
        Fuel_Purchased INTEGER,
        FOREIGN KEY(Statistics_event_id) REFERENCES Statistics(event_id)
    );

    CREATE TABLE IF NOT EXISTS Statistics_Passengers (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        Statistics_event_id INTEGER NOT NULL,
        Passengers_Missions_Accepted INTEGER,
        Passengers_Missions_Disgruntled INTEGER,
        Passengers_Missions_Bulk INTEGER NOT NULL,
        Passengers_Missions_VIP INTEGER NOT NULL,
        Passengers_Missions_Delivered INTEGER NOT NULL,
        Passengers_Missions_Ejected INTEGER NOT NULL,
        FOREIGN KEY(Statistics_event_id) REFERENCES Statistics(event_id)
    );

    CREATE TABLE IF NOT EXISTS Statistics_Search_And_Rescue (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        Statistics_event_id INTEGER NOT NULL,
        SearchRescue_Traded INTEGER NOT NULL,
        SearchRescue_Profit INTEGER NOT NULL,
        SearchRescue_Count INTEGER NOT NULL,
        Salvage_Legal_POI INTEGER,
        Salvage_Legal_Settlements INTEGER,
        Salvage_Illegal_POI INTEGER,
        Salvage_Illegal_Settlements INTEGER,
        Maglocks_Opened INTEGER,
        Panels_Opened INTEGER,
        Settlements_State_FireOut INTEGER,
        Settlements_State_Reboot INTEGER,
        FOREIGN KEY(Statistics_event_id) REFERENCES Statistics(event_id)
    );

    CREATE TABLE IF NOT EXISTS Statistics_Crafting (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        Statistics_event_id INTEGER NOT NULL,
        Count_Of_Used_Engineers INTEGER NOT NULL,
        Recipes_Generated INTEGER NOT NULL,
        Recipes_Generated_Rank_1 INTEGER NOT NULL,
        Recipes_Generated_Rank_2 INTEGER NOT NULL,
        Recipes_Generated_Rank_3 INTEGER NOT NULL,
        Recipes_Generated_Rank_4 INTEGER NOT NULL,
        Recipes_Generated_Rank_5 INTEGER NOT NULL,
        Suit_Mods_Applied INTEGER,
        Weapon_Mods_Applied INTEGER,
        Suits_Upgraded INTEGER,
        Weapons_Upgraded INTEGER,
        Suits_Upgraded_Full INTEGER,
        Weapons_Upgraded_Full INTEGER,
        Suit_Mods_Applied_Full INTEGER,
        Weapon_Mods_Applied_Full INTEGER,
        Spent_On_Crafting INTEGER,
        Recipes_Applied INTEGER,
        Recipes_Applied_Rank_1 INTEGER,
        Recipes_Applied_Rank_2 INTEGER,
        Recipes_Applied_Rank_3 INTEGER,
        Recipes_Applied_Rank_4 INTEGER,
        Recipes_Applied_Rank_5 INTEGER,
        Recipes_Applied_On_Previously_Modified_Modules INTEGER,
        FOREIGN KEY(Statistics_event_id) REFERENCES Statistics(event_id)
    );

    CREATE TABLE IF NOT EXISTS Statistics_Crew (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        Statistics_event_id INTEGER NOT NULL,
        NpcCrew_TotalWages INTEGER,
        NpcCrew_Hired INTEGER,
        NpcCrew_Fired INTEGER,
        NpcCrew_Died INTEGER,
        FOREIGN KEY(Statistics_event_id) REFERENCES Statistics(event_id)
    );

    CREATE TABLE IF NOT EXISTS Statistics_Multicrew (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        Statistics_event_id INTEGER NOT NULL,
        Multicrew_Time_Total INTEGER NOT NULL,
        Multicrew_Gunner_Time_Total INTEGER NOT NULL,
        Multicrew_Fighter_Time_Total INTEGER NOT NULL,
        Multicrew_Credits_Total INTEGER NOT NULL,
        Multicrew_Fines_Total INTEGER NOT NULL,
        FOREIGN KEY(Statistics_event_id) REFERENCES Statistics(event_id)
    );

    CREATE TABLE IF NOT EXISTS Statistics_Material_Trader_Stats (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        Statistics_event_id INTEGER NOT NULL,
        Trades_Completed INTEGER NOT NULL,
        Materials_Traded INTEGER NOT NULL,
        Encoded_Materials_Traded INTEGER,
        Raw_Materials_Traded INTEGER,
        Grade_1_Materials_Traded INTEGER,
        Grade_2_Materials_Traded INTEGER,
        Grade_3_Materials_Traded INTEGER,
        Grade_4_Materials_Traded INTEGER,
        Grade_5_Materials_Traded INTEGER,
        Assets_Traded_In INTEGER,
        Assets_Traded_Out INTEGER,
        FOREIGN KEY(Statistics_event_id) REFERENCES Statistics(event_id)
    );

    CREATE TABLE IF NOT EXISTS Statistics_FLEETCARRIER (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        Statistics_event_id INTEGER NOT NULL,
        FLEETCARRIER_EXPORT_TOTAL INTEGER NOT NULL,
        FLEETCARRIER_IMPORT_TOTAL INTEGER NOT NULL,
        FLEETCARRIER_TRADEPROFIT_TOTAL INTEGER NOT NULL,
        FLEETCARRIER_TRADESPEND_TOTAL INTEGER NOT NULL,
        FLEETCARRIER_STOLENPROFIT_TOTAL INTEGER NOT NULL,
        FLEETCARRIER_STOLENSPEND_TOTAL INTEGER NOT NULL,
        FLEETCARRIER_DISTANCE_TRAVELLED TEXT NOT NULL,
        FLEETCARRIER_TOTAL_JUMPS INTEGER NOT NULL,
        FLEETCARRIER_SHIPYARD_SOLD INTEGER NOT NULL,
        FLEETCARRIER_SHIPYARD_PROFIT INTEGER NOT NULL,
        FLEETCARRIER_OUTFITTING_SOLD INTEGER NOT NULL,
        FLEETCARRIER_OUTFITTING_PROFIT INTEGER NOT NULL,
        FLEETCARRIER_REARM_TOTAL INTEGER NOT NULL,
        FLEETCARRIER_REFUEL_TOTAL INTEGER NOT NULL,
        FLEETCARRIER_REFUEL_PROFIT INTEGER NOT NULL,
        FLEETCARRIER_REPAIRS_TOTAL INTEGER NOT NULL,
        FLEETCARRIER_VOUCHERS_REDEEMED INTEGER NOT NULL,
        FLEETCARRIER_VOUCHERS_PROFIT INTEGER NOT NULL,
        FOREIGN KEY(Statistics_event_id) REFERENCES Statistics(event_id)
    );

    CREATE TABLE IF NOT EXISTS Statistics_Exobiology (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        Statistics_event_id INTEGER NOT NULL,
        Organic_Genus_Encountered INTEGER NOT NULL,
        Organic_Species_Encountered INTEGER NOT NULL,
        Organic_Variant_Encountered INTEGER NOT NULL,
        Organic_Data_Profits INTEGER NOT NULL,
        Organic_Data INTEGER NOT NULL,
        First_Logged_Profits INTEGER NOT NULL,
        First_Logged INTEGER NOT NULL,
        Organic_Systems INTEGER NOT NULL,
        Organic_Planets INTEGER NOT NULL,
        Organic_Genus INTEGER NOT NULL,
        Organic_Species INTEGER NOT NULL,
        FOREIGN KEY(Statistics_event_id) REFERENCES Statistics(event_id)
    );

    CREATE TABLE IF NOT EXISTS Statistics_TG_ENCOUNTERS (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        Statistics_event_id INTEGER NOT NULL,
        TG_ENCOUNTER_IMPRINT INTEGER,
        TG_ENCOUNTER_WAKES INTEGER,
        TG_ENCOUNTER_KILLED INTEGER,
        TG_ENCOUNTER_TOTAL INTEGER,
        TG_ENCOUNTER_TOTAL_LAST_SYSTEM TEXT,
        TG_ENCOUNTER_TOTAL_LAST_TIMESTAMP TEXT,
        TG_ENCOUNTER_TOTAL_LAST_SHIP TEXT,
        TG_SCOUT_COUNT INTEGER,
        FOREIGN KEY(Statistics_event_id) REFERENCES Statistics(event_id)
    );

    CREATE TABLE IF NOT EXISTS Statistics_CQC (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        Statistics_event_id INTEGER NOT NULL,
        CQC_Credits_Earned INTEGER,
        CQC_Time_Played INTEGER NOT NULL,
        CQC_KD REAL NOT NULL,
        CQC_Kills INTEGER NOT NULL,
        CQC_WL REAL NOT NULL,
        FOREIGN KEY(Statistics_event_id) REFERENCES Statistics(event_id)
    );

    CREATE TABLE IF NOT EXISTS Statistics_Squadron (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        Statistics_event_id INTEGER NOT NULL,
        Squadron_Bank_Credits_Deposited INTEGER NOT NULL,
        Squadron_Bank_Credits_Withdrawn INTEGER NOT NULL,
        Squadron_Bank_Commodities_Deposited_Num INTEGER NOT NULL,
        Squadron_Bank_Commodities_Deposited_Value INTEGER NOT NULL,
        Squadron_Bank_Commodities_Withdrawn_Num INTEGER NOT NULL,
        Squadron_Bank_Commodities_Withdrawn_Value INTEGER NOT NULL,
        Squadron_Bank_PersonalAssets_Deposited_Num INTEGER NOT NULL,
        Squadron_Bank_PersonalAssets_Deposited_Value INTEGER NOT NULL,
        Squadron_Bank_PersonalAssets_Withdrawn_Num INTEGER NOT NULL,
        Squadron_Bank_PersonalAssets_Withdrawn_Value INTEGER NOT NULL,
        Squadron_Bank_Ships_Deposited_Num INTEGER NOT NULL,
        Squadron_Bank_Ships_Deposited_Value INTEGER NOT NULL,
        Squadron_Leaderboard_aegis_highestcontribution INTEGER NOT NULL,
        Squadron_Leaderboard_bgs_highestcontribution INTEGER NOT NULL,
        Squadron_Leaderboard_bounty_highestcontribution INTEGER NOT NULL,
        Squadron_Leaderboard_colonisation_contribution_highestcontribution INTEGER NOT NULL,
        Squadron_Leaderboard_combat_highestcontribution INTEGER NOT NULL,
        Squadron_Leaderboard_cqc_highestcontribution INTEGER NOT NULL,
        Squadron_Leaderboard_exploration_highestcontribution INTEGER NOT NULL,
        Squadron_Leaderboard_mining_highestcontribution INTEGER NOT NULL,
        Squadron_Leaderboard_powerplay_highestcontribution INTEGER NOT NULL,
        Squadron_Leaderboard_trade_highestcontribution INTEGER NOT NULL,
        Squadron_Leaderboard_trade_illicit_highestcontribution INTEGER NOT NULL,
        Squadron_Leaderboard_podiums INTEGER NOT NULL,
        FOREIGN KEY(Statistics_event_id) REFERENCES Statistics(event_id)
    );


CREATE TABLE IF NOT EXISTS Status (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Flags INTEGER NOT NULL,
    Pips TEXT,
    FireGroup INTEGER,
    GuiFocus INTEGER,
    Latitude REAL,
    Longitude REAL,
    Heading INTEGER,
    Altitude INTEGER,
    Flags2 INTEGER,
    Cargo REAL,
    LegalState TEXT,
    Balance INTEGER,
    Oxygen REAL,
    Health REAL,
    Temperature REAL,
    SelectedWeapon TEXT,
    BodyName TEXT,
    PlanetRadius REAL,
    SelectedWeapon_Localised TEXT,
    Gravity REAL,
    event_timestamp TEXT NOT NULL
);

    CREATE TABLE IF NOT EXISTS Status_Fuel (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        Status_event_id INTEGER NOT NULL,
        FuelMain REAL NOT NULL,
        FuelReservoir REAL NOT NULL,
        FOREIGN KEY(Status_event_id) REFERENCES Status(event_id)
    );

    CREATE TABLE IF NOT EXISTS Status_Destination (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        Status_event_id INTEGER NOT NULL,
        System INTEGER NOT NULL,
        Body INTEGER NOT NULL,
        Name TEXT NOT NULL,
        Name_Localised TEXT,
        FOREIGN KEY(Status_event_id) REFERENCES Status(event_id)
    );


CREATE TABLE IF NOT EXISTS StoredModules (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    MarketID INTEGER NOT NULL,
    StationName TEXT NOT NULL,
    StarSystem TEXT NOT NULL,
    event_timestamp TEXT NOT NULL
);

    CREATE TABLE IF NOT EXISTS StoredModules_Items (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        StoredModules_event_id INTEGER NOT NULL,
        Name TEXT NOT NULL,
        Name_Localised TEXT,
        StorageSlot INTEGER NOT NULL,
        StarSystem TEXT,
        MarketID INTEGER,
        TransferCost INTEGER,
        TransferTime INTEGER,
        BuyPrice INTEGER NOT NULL,
        Hot INTEGER NOT NULL,
        EngineerModifications TEXT,
        Level INTEGER,
        Quality REAL,
        InTransit INTEGER,
        FOREIGN KEY(StoredModules_event_id) REFERENCES StoredModules(event_id)
    );


CREATE TABLE IF NOT EXISTS StoredShips (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    StationName TEXT NOT NULL,
    MarketID INTEGER NOT NULL,
    StarSystem TEXT NOT NULL,
    event_timestamp TEXT NOT NULL
);

    CREATE TABLE IF NOT EXISTS StoredShips_ShipsHere (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        StoredShips_event_id INTEGER NOT NULL,
        ShipID INTEGER NOT NULL,
        ShipType TEXT NOT NULL,
        Value INTEGER NOT NULL,
        Hot INTEGER NOT NULL,
        ShipType_Localised TEXT,
        Name TEXT,
        FOREIGN KEY(StoredShips_event_id) REFERENCES StoredShips(event_id)
    );

    CREATE TABLE IF NOT EXISTS StoredShips_ShipsRemote (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        StoredShips_event_id INTEGER NOT NULL,
        ShipID INTEGER NOT NULL,
        ShipType TEXT NOT NULL,
        Name TEXT,
        StarSystem TEXT,
        ShipMarketID INTEGER,
        TransferPrice INTEGER,
        TransferTime INTEGER,
        Value INTEGER NOT NULL,
        Hot INTEGER NOT NULL,
        ShipType_Localised TEXT,
        InTransit INTEGER,
        FOREIGN KEY(StoredShips_event_id) REFERENCES StoredShips(event_id)
    );


CREATE TABLE IF NOT EXISTS SuitLoadout (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    SuitID INTEGER NOT NULL,
    SuitName TEXT NOT NULL,
    SuitName_Localised TEXT,
    SuitMods TEXT NOT NULL,
    LoadoutID INTEGER NOT NULL,
    LoadoutName TEXT NOT NULL,
    event_timestamp TEXT NOT NULL
);

    CREATE TABLE IF NOT EXISTS SuitLoadout_Modules (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        SuitLoadout_event_id INTEGER NOT NULL,
        SlotName TEXT NOT NULL,
        SuitModuleID INTEGER NOT NULL,
        ModuleName TEXT NOT NULL,
        ModuleName_Localised TEXT,
        Class INTEGER NOT NULL,
        WeaponMods TEXT NOT NULL,
        FOREIGN KEY(SuitLoadout_event_id) REFERENCES SuitLoadout(event_id)
    );


CREATE TABLE IF NOT EXISTS SupercruiseDestinationDrop (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Type TEXT NOT NULL,
    Type_Localised TEXT,
    MarketID INTEGER,
    Threat INTEGER NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS SupercruiseEntry (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Taxi INTEGER,
    Multicrew INTEGER,
    StarSystem TEXT NOT NULL,
    SystemAddress INTEGER NOT NULL,
    Wanted INTEGER,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS SupercruiseExit (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Taxi INTEGER,
    Multicrew INTEGER,
    StarSystem TEXT NOT NULL,
    SystemAddress INTEGER NOT NULL,
    Body TEXT NOT NULL,
    BodyID INTEGER NOT NULL,
    BodyType TEXT NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS SwitchSuitLoadout (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    SuitID INTEGER NOT NULL,
    SuitName TEXT NOT NULL,
    SuitName_Localised TEXT,
    SuitMods TEXT NOT NULL,
    LoadoutID INTEGER NOT NULL,
    LoadoutName TEXT NOT NULL,
    event_timestamp TEXT NOT NULL
);

    CREATE TABLE IF NOT EXISTS SwitchSuitLoadout_Modules (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        SwitchSuitLoadout_event_id INTEGER NOT NULL,
        SlotName TEXT NOT NULL,
        SuitModuleID INTEGER NOT NULL,
        ModuleName TEXT NOT NULL,
        ModuleName_Localised TEXT,
        Class INTEGER NOT NULL,
        WeaponMods TEXT NOT NULL,
        FOREIGN KEY(SwitchSuitLoadout_event_id) REFERENCES SwitchSuitLoadout(event_id)
    );


CREATE TABLE IF NOT EXISTS Synthesis (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    event_timestamp TEXT NOT NULL
);

    CREATE TABLE IF NOT EXISTS Synthesis_Materials (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        Synthesis_event_id INTEGER NOT NULL,
        Name TEXT NOT NULL,
        Count INTEGER NOT NULL,
        Name_Localised TEXT,
        FOREIGN KEY(Synthesis_event_id) REFERENCES Synthesis(event_id)
    );


CREATE TABLE IF NOT EXISTS SystemsShutdown (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS TechnologyBroker (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    BrokerType TEXT NOT NULL,
    MarketID INTEGER NOT NULL,
    event_timestamp TEXT NOT NULL
);

    CREATE TABLE IF NOT EXISTS TechnologyBroker_ItemsUnlocked (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        TechnologyBroker_event_id INTEGER NOT NULL,
        Name TEXT NOT NULL,
        Name_Localised TEXT,
        FOREIGN KEY(TechnologyBroker_event_id) REFERENCES TechnologyBroker(event_id)
    );

    CREATE TABLE IF NOT EXISTS TechnologyBroker_Commodities (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        TechnologyBroker_event_id INTEGER NOT NULL,
        Name TEXT NOT NULL,
        Name_Localised TEXT,
        Count INTEGER NOT NULL,
        FOREIGN KEY(TechnologyBroker_event_id) REFERENCES TechnologyBroker(event_id)
    );

    CREATE TABLE IF NOT EXISTS TechnologyBroker_Materials (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        TechnologyBroker_event_id INTEGER NOT NULL,
        Name TEXT NOT NULL,
        Name_Localised TEXT,
        Count INTEGER NOT NULL,
        Category TEXT NOT NULL,
        FOREIGN KEY(TechnologyBroker_event_id) REFERENCES TechnologyBroker(event_id)
    );


CREATE TABLE IF NOT EXISTS Touchdown (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    PlayerControlled INTEGER NOT NULL,
    Taxi INTEGER,
    Multicrew INTEGER,
    StarSystem TEXT,
    SystemAddress INTEGER,
    Body TEXT,
    BodyID INTEGER,
    OnStation INTEGER,
    OnPlanet INTEGER,
    Latitude REAL,
    Longitude REAL,
    NearestDestination TEXT,
    NearestDestination_Localised TEXT,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS TradeMicroResources (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    TotalCount INTEGER NOT NULL,
    Received TEXT NOT NULL,
    Received_Localised TEXT,
    Count INTEGER NOT NULL,
    Category TEXT NOT NULL,
    MarketID INTEGER NOT NULL,
    event_timestamp TEXT NOT NULL
);

    CREATE TABLE IF NOT EXISTS TradeMicroResources_Offered (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        TradeMicroResources_event_id INTEGER NOT NULL,
        Name TEXT NOT NULL,
        Name_Localised TEXT,
        Category TEXT NOT NULL,
        Count INTEGER NOT NULL,
        FOREIGN KEY(TradeMicroResources_event_id) REFERENCES TradeMicroResources(event_id)
    );


CREATE TABLE IF NOT EXISTS TransferMicroResources (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    event_timestamp TEXT NOT NULL
);

    CREATE TABLE IF NOT EXISTS TransferMicroResources_Transfers (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        TransferMicroResources_event_id INTEGER NOT NULL,
        Name TEXT NOT NULL,
        Name_Localised TEXT,
        Category TEXT NOT NULL,
        LockerOldCount INTEGER NOT NULL,
        LockerNewCount INTEGER NOT NULL,
        Direction TEXT NOT NULL,
        FOREIGN KEY(TransferMicroResources_event_id) REFERENCES TransferMicroResources(event_id)
    );


CREATE TABLE IF NOT EXISTS UnderAttack (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Target TEXT,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS Undocked (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    StationName TEXT NOT NULL,
    StationName_Localised TEXT,
    StationType TEXT NOT NULL,
    MarketID INTEGER NOT NULL,
    Taxi INTEGER,
    Multicrew INTEGER,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS UpgradeSuit (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    Name_Localised TEXT,
    SuitID INTEGER NOT NULL,
    Class INTEGER NOT NULL,
    Cost INTEGER NOT NULL,
    event_timestamp TEXT NOT NULL
);

    CREATE TABLE IF NOT EXISTS UpgradeSuit_Resources (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        UpgradeSuit_event_id INTEGER NOT NULL,
        Name TEXT NOT NULL,
        Name_Localised TEXT,
        Count INTEGER NOT NULL,
        FOREIGN KEY(UpgradeSuit_event_id) REFERENCES UpgradeSuit(event_id)
    );


CREATE TABLE IF NOT EXISTS UpgradeWeapon (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    Name_Localised TEXT,
    Class INTEGER NOT NULL,
    SuitModuleID INTEGER NOT NULL,
    Cost INTEGER NOT NULL,
    event_timestamp TEXT NOT NULL
);

    CREATE TABLE IF NOT EXISTS UpgradeWeapon_Resources (
        child_id INTEGER PRIMARY KEY AUTOINCREMENT,
        UpgradeWeapon_event_id INTEGER NOT NULL,
        Name TEXT NOT NULL,
        Name_Localised TEXT,
        Count INTEGER NOT NULL,
        FOREIGN KEY(UpgradeWeapon_event_id) REFERENCES UpgradeWeapon(event_id)
    );


CREATE TABLE IF NOT EXISTS UseConsumable (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    Name_Localised TEXT,
    Type TEXT NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS USSDrop (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    USSType TEXT NOT NULL,
    USSType_Localised TEXT,
    USSThreat INTEGER NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS VehicleSwitch (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    event_To TEXT NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS WingAdd (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS WingInvite (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS WingJoin (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    Others TEXT NOT NULL,
    event_timestamp TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS WingLeave (
    event_id INTEGER PRIMARY KEY AUTOINCREMENT,
    event_timestamp TEXT NOT NULL
);


