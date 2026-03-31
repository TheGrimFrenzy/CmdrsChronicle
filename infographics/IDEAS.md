# Infographic Ideas

This document lists interesting infographic ideas for each category, based on the Elite Dangerous journal event schemas. For each idea, the table(s) to query, the key columns, and the recommended visualization type are noted.

Visualization types:
- **bar-chart** — best when a meaningful categorical column drives GROUP BY
- **summary-tile** — best when the answer is a small set of aggregate scalars
- **table** — future type; not yet supported by the renderer

---

## Exploration

Source schemas: `FSDJump`, `Scan`, `ScanOrganic`, `SellOrganicData`, `MultiSellExplorationData`, `SAAScanComplete`, `FSSDiscoveryScan`, `FSSBodySignals`, `SAASignalsFound`, `FuelScoop`, `JetConeBoost`

| # | Idea | Table(s) | Key Columns | Viz |
|---|------|----------|-------------|-----|
| 1 | **Most Visited Systems** — which star systems appeared most often in your jumps (useful for loop routes or frequent waypoints) | `FSDJump` | `StarSystem` (GROUP BY, COUNT) | bar-chart |
| 2 | **Jump Distance Distribution** — how many jumps fell into ranges: <10 ly, 10–29 ly, 30–49 ly, 50+ ly | `FSDJump` | `JumpDist` (CASE bucketing) | bar-chart |
| 3 | **Systems by Economy Type** — bar showing which economy types you jumped into most | `FSDJump` | `SystemEconomy_Localised` (GROUP BY, COUNT) | bar-chart |
| 4 | **Exploration Data Earnings** — total credits earned from exploration data sales, split base value vs. first-discovery bonus | `MultiSellExplorationData` | `SUM(TotalEarnings)`, `SUM(BaseValue)`, `SUM(Bonus)` | summary-tile |
| 5 | **Total Bodies Honked** — aggregate body and non-body counts across all FSS discovery scans (shows how thoroughly you honked systems) | `FSSDiscoveryScan` | `SUM(BodyCount)`, `SUM(NonBodyCount)`, `COUNT(*)` (systems honked) | summary-tile |
| 6 | **Planet Classes Scanned** — distribution of planet types found via detailed surface scanner and probes | `Scan` | `PlanetClass` (WHERE PlanetClass IS NOT NULL, GROUP BY, COUNT) | bar-chart |
| 7 | **Terraformable Bodies Found** — how many bodies with terraforming potential you discovered, and bodies scanned total | `Scan` | `COUNT(*) WHERE TerraformState = 'Terraformable'`, total `COUNT(*)` | summary-tile |
| 8 | **Organics by Genus** — which biological genus types you logged the most completed samples of during exobiology scans | `ScanOrganic` | `Genus_Localised` (WHERE ScanType = 'Analyse', GROUP BY, COUNT) | bar-chart |
| 9 | **Neutron Star / White Dwarf Jet Boosts** — how many neutron cone boosts you collected and their cumulative boost factor | `JetConeBoost` | `COUNT(*)`, `SUM(BoostValue)` | summary-tile |
| 10 | **SAA Efficiency Rate** — proportion of surface-mapping runs completed at or under the efficiency probe target, plus total surfaces mapped | `SAAScanComplete` | `COUNT(*) WHERE ProbesUsed <= EfficiencyTarget` vs. `COUNT(*)` total | summary-tile |

---

## Missions

Source schemas: `MissionAccepted`, `MissionCompleted`, `MissionFailed`, `MissionAbandoned`

| # | Idea | Table(s) | Key Columns | Viz |
|---|------|----------|-------------|-----|
| 1 | **Mission Outcomes** — breakdown of completed vs. failed vs. abandoned missions | `MissionCompleted`, `MissionFailed`, `MissionAbandoned` | `COUNT(*)` per table | summary-tile |
| 2 | **Top Mission Types** — which mission `Name` prefixes (delivery, assassinate, rescue, etc.) appeared most | `MissionAccepted` | `Name` (strip suffix, GROUP BY, COUNT) | bar-chart |
| 3 | **Mission Rewards Earned** — total credits earned from mission completions | `MissionCompleted` | `SUM(Reward)` | summary-tile |
| 4 | **Missions by Faction** — which factions gave you the most missions | `MissionAccepted` | `Faction` (GROUP BY, COUNT) | bar-chart |
| 5 | **Missions by Destination System** — which systems were most common destinations | `MissionAccepted` | `DestinationSystem` (GROUP BY, COUNT) | bar-chart |
| 6 | **Commodity Delivery Volume** — total cargo units delivered across delivery missions | `MissionCompleted` | `SUM(CommodityReward[].Count)` or `Commodity`, `Count` | summary-tile |
| 7 | **Passenger Mission Capacity** — total passengers transported, grouped by cabin class (Economy, Business, First, Luxury) | `MissionAccepted` | `PassengerType`, `PassengerCount` (WHERE PassengerMission = true) | bar-chart |
| 8 | **Mission Wing Participation** — how many missions were wing missions vs. solo | `MissionAccepted` | `COUNT(*) WHERE Wing = true` vs. `COUNT(*)` total | summary-tile |
| 9 | **Bounty Voucher Missions** — total bounty voucher reward from massacre/assassinate completions | `MissionCompleted` | `FactionEffects[].Reward[].Reward` WHERE type Bounty | summary-tile |
| 10 | **Fastest Mission Turnaround** — missions accepted and completed within the same session (join on MissionID) | `MissionAccepted`, `MissionCompleted` | `MissionID` join, `timestamp` delta | summary-tile |

---

## CombatShip

Source schemas: `Died`, `PVPKill`, `Bounty`, `CapShipBond`, `FactionKillBond`, `Interdicted`, `EscapeInterdiction`, `UnderAttack`, `ShieldState`, `HullDamage`

| # | Idea | Table(s) | Key Columns | Viz |
|---|------|----------|-------------|-----|
| 1 | **Ships Destroyed by Type** — most frequently killed NPC ship types | `Bounty` | `VictimFaction` or `Target` (GROUP BY, COUNT) | bar-chart |
| 2 | **Bounty Vouchers Earned** — total bounty value collected across all kills | `Bounty` | `SUM(TotalReward)` | summary-tile |
| 3 | **Kill/Death Ratio** — PVP kills vs. player deaths | `PVPKill`, `Died` | `COUNT(*)` each | summary-tile |
| 4 | **Combat Bond Earnings** — CZ combat bond totals, split by allegiance | `FactionKillBond` | `AwardingFaction` (GROUP BY, SUM(Reward)) | bar-chart |
| 5 | **Interdiction Outcomes** — pulled vs. escaped vs. submitted to interdiction | `Interdicted` | `COUNT(*) WHERE Submitted = true/false`, `EscapeInterdiction COUNT(*)` | summary-tile |
| 6 | **Most Targeted Factions** — which enemy factions you killed for most often | `Bounty` | `VictimFaction` (GROUP BY, COUNT) | bar-chart |
| 7 | **Capital Ship Bonds** — earnings from wing CZ capital ship engagements | `CapShipBond` | `SUM(Reward)`, `AwardingFaction` | summary-tile |
| 8 | **Hull Damage Events** — how often your hull was damaged, grouped by health threshold buckets | `HullDamage` | `Health` (CASE to bucket: >75%, 50–75%, 25–50%, <25%) | bar-chart |
| 9 | **Shield Outages** — count of shield-down events (indicators of dangerous engagements) | `ShieldState` | `COUNT(*) WHERE ShieldsUp = false` | summary-tile |
| 10 | **Under Attack by Type** — what was attacking you (ship vs. fighter vs. turret) | `UnderAttack` | `Target` (GROUP BY, COUNT) | bar-chart |

---

## CombatOnFoot

Source schemas: `Died`, `PVPKill`, `CommitCrime`, `FactionKillBond`, `BookDropship`, `DropshipDeploy`, `Touchdown`, `Embark`, `Disembark`

| # | Idea | Table(s) | Key Columns | Viz |
|---|------|----------|-------------|-----|
| 1 | **On-Foot Deaths** — how often you died while on foot | `Died` | `COUNT(*) WHERE event='Died'` (filter by context clues) | summary-tile |
| 2 | **Dropship Deployments by Location** — which settlements you dropped into most | `DropshipDeploy` | `StarSystem` or `NearestDestination_Localised` (GROUP BY, COUNT) | bar-chart |
| 3 | **Combat Bond Earnings On-Foot** — kill bond credits earned in on-foot CZ missions | `FactionKillBond` | `SUM(Reward)`, `AwardingFaction` GROUP BY | bar-chart |
| 4 | **Suit Load-out Swaps** — how many times you changed suit before a deployment | `SuitLoadout` | `COUNT(*)` per `SuitName` | bar-chart |
| 5 | **Dropship Bookings vs. Arrivals** — how many dropships booked made it to arrival | `BookDropship`, `DropshipDeploy` | `COUNT(*)` each | summary-tile |
| 6 | **Crimes Committed On-Foot** — breakdown by crime type committed during on-foot operations | `CommitCrime` | `CrimeType` (GROUP BY, COUNT) | bar-chart |
| 7 | **Settlement Embark Sources** — which settlements you embarked from most | `Embark` | `StationName` or `NearestDestination_Localised` (WHERE OnStation = false, GROUP BY) | bar-chart |
| 8 | **On-Foot Session Time** — total time spent on the ground (touchdowns to lift-offs) | `Touchdown`, `Liftoff` | `timestamp` delta (aggregate by session) | summary-tile |
| 9 | **Tactical Team Bookings** — how many times you booked squad dropship slots | `BookDropship` | `COUNT(*)`, `SeatNumber` distribution | summary-tile |
| 10 | **Most Active Settlement Combat Zones** — settlement names appearing in most kill-bond events | `FactionKillBond` | `(join to mission context)` via StarSystem grouping | bar-chart |

---

## Trade

Source schemas: `MarketBuy`, `MarketSell`, `CargoDepot`, `CargoTransfer`, `CarrierTradeOrder`

| # | Idea | Table(s) | Key Columns | Viz |
|---|------|----------|-------------|-----|
| 1 | **Top Traded Commodities (Buy)** — which commodities you purchased most by unit count | `MarketBuy` | `Type_Localised` (GROUP BY, SUM(Count)) | bar-chart |
| 2 | **Top Traded Commodities (Sell)** — which commodities you sold most by unit count | `MarketSell` | `Type_Localised` (GROUP BY, SUM(Count)) | bar-chart |
| 3 | **Trade Profit Summary** — total credits from market sales minus market buys | `MarketSell`, `MarketBuy` | `SUM(TotalSale)` vs. `SUM(TotalCost)` | summary-tile |
| 4 | **Best Selling Stations** — stations where you sold the most cargo | `MarketSell` | `StarSystem` or `StationName` (GROUP BY, SUM(Count)) | bar-chart |
| 5 | **Black Market Activity** — illegal sales by commodity | `MarketSell` | `WHERE IllegalGoods = true`, `Type_Localised` GROUP BY | bar-chart |
| 6 | **Carrier Trade Volume** — total cargo moved via fleet carrier trade orders | `CarrierTradeOrder` | `SUM(Quantity)`, `Commodity` GROUP BY | bar-chart |
| 7 | **Cargo Transfer Events** — how often cargo was transferred to/from fleet carrier vs. ship | `CargoTransfer` | `COUNT(*)`, `Direction` GROUP BY | summary-tile |
| 8 | **Cargo Depot Missions** — how many cargo depot deliveries vs. pick-ups were completed | `CargoDepot` | `COUNT(*) WHERE UpdateType` GROUP BY | summary-tile |
| 9 | **Average Profit per Commodity** — average profit margin per unit across sell events | `MarketSell` | `AvgPricePaid`, `SellPrice`, `Type_Localised` | bar-chart |
| 10 | **Commodities Bought from Black Market** — illegal purchases by type | `MarketBuy` | `WHERE IllegalGoods = true`, `Type_Localised` GROUP BY | bar-chart |

---

## Mining

Source schemas: `AsteroidCracked`, `ProspectedAsteroid`, `MiningRefined`, `LaunchDrone`, `JettisonCargo`, `Cargo`

| # | Idea | Table(s) | Key Columns | Viz |
|---|------|----------|-------------|-----|
| 1 | **Refined Materials** — total units refined, grouped by material type | `MiningRefined` | `Type_Localised` (GROUP BY, SUM(Count)) | bar-chart |
| 2 | **Asteroids Cracked** — total core asteroids cracked over time | `AsteroidCracked` | `COUNT(*)` | summary-tile |
| 3 | **Prospected Asteroid Content** — most common material found in prospected asteroids | `ProspectedAsteroid` | `Content` (GROUP BY, COUNT) | bar-chart |
| 4 | **Drone Usage by Type** — breakdown of drone launches by drone type (collector, prospector, etc.) | `LaunchDrone` | `Type` (GROUP BY, COUNT) | bar-chart |
| 5 | **Jettisoned Cargo by Commodity** — what cargo got jettisoned most (waste minerals, etc.) | `JettisonCargo` | `Type_Localised` (GROUP BY, SUM(Count)) | bar-chart |
| 6 | **Mining Efficiency** — refined : prospected asteroid ratio (units out per asteroid scanned) | `MiningRefined`, `ProspectedAsteroid` | `SUM(Count)` vs. `COUNT(*)` | summary-tile |
| 7 | **Motherlode vs. Common Cores** — how many prospected asteroids were motherlode vs. common | `ProspectedAsteroid` | `MotherlodeIndicator` GROUP BY (true/false) | summary-tile |
| 8 | **Most Mined Systems** — which star systems you mined in most (jump to ring proximity) | `ProspectedAsteroid` (via FSDJump) | `StarSystem` (GROUP BY, COUNT) | bar-chart |
| 9 | **Collector Drone Efficiency** — drone launches vs. refined count (workflow overhead indicator) | `LaunchDrone`, `MiningRefined` | `COUNT(LaunchDrone WHERE Type=Collect)` vs. `SUM(MiningRefined.Count)` | summary-tile |
| 10 | **Cargo Hold Composition After Mining** — snapshot of most common mined materials in hold | `Cargo` | `Name_Localised` (WHERE MissionID IS NULL, GROUP BY, SUM(Count)) | bar-chart |

---

## Exobiology

Source schemas: `ScanOrganic`, `SellOrganicData`, `FSSBodySignals`, `SAASignalsFound`, `Disembark`, `Embark`

| # | Idea | Table(s) | Key Columns | Viz |
|---|------|----------|-------------|-----|
| 1 | **Species Analysed by Genus** — completed samples (Analyse scan) grouped by genus | `ScanOrganic` | `Genus_Localised` (WHERE ScanType = 'Analyse', GROUP BY, COUNT) | bar-chart |
| 2 | **Top Earning Species** — which species generated the most exobiology credits | `SellOrganicData` | `Species_Localised` (GROUP BY, SUM(Value + Bonus)) | bar-chart |
| 3 | **Exobiology Earnings Total** — total credits earned from organic data sales | `SellOrganicData` | `SUM(Value)`, `SUM(Bonus)`, `SUM(Value + Bonus)` | summary-tile |
| 4 | **First-Discovery Bonus Rate** — what proportion of samples earned a first-discovery bonus | `SellOrganicData` | `COUNT(*) WHERE Bonus > 0` vs. `COUNT(*)` | summary-tile |
| 5 | **Biological Signal Bodies** — planets with the most biological signal indicators | `FSSBodySignals` | `BodyName`, `Signals WHERE Type_Localised = 'Biological'`, `Count` (GROUP BY body, SUM signal count) | bar-chart |
| 6 | **Variant Diversity** — unique variants analysed across all species | `ScanOrganic` | `COUNT(DISTINCT Variant_Localised) WHERE ScanType = 'Analyse'` | summary-tile |
| 7 | **Scan Completion Funnel** — how many Log → Sample → Analyse completions (drop-off at each stage) | `ScanOrganic` | `COUNT(*) GROUP BY ScanType` (Log, Sample, Analyse) | bar-chart |
| 8 | **Species Richest Bodies** — surface-scan bodies with the most genus types indicated | `SAASignalsFound` | `BodyName` + `Genuses` array count (GROUP BY BodyName, COUNT(Genuses)) | bar-chart |
| 9 | **Exobiology Planetary Atmospheres** — atmosphere types on bodies where you completed analyses | `ScanOrganic` + `Scan` | JOIN on BodyName, `AtmosphereType` GROUP BY | bar-chart |
| 10 | **Top Earning Variants** — individual variant-level earnings breakdown | `SellOrganicData` | `Species_Localised` (finer than genus; GROUP BY, SUM(Value))  | bar-chart |

---

## Powerplay

Source schemas: `PowerplayCollect`, `PowerplayDeliver`, `PowerplayFastTrack`, `PowerplaySalary`, `PowerplayVote`, `PowerplayDefect`, `PowerplayJoin`, `PowerplayLeave`

| # | Idea | Table(s) | Key Columns | Viz |
|---|------|----------|-------------|-----|
| 1 | **Merits Collected by Faction** — total merits collected grouped by Power faction | `PowerplayCollect` | `Power` (GROUP BY, SUM(Count)) | bar-chart |
| 2 | **Merits Delivered by Faction** — total merits delivered for each Power | `PowerplayDeliver` | `Power` (GROUP BY, SUM(Count)) | bar-chart |
| 3 | **Total Merits Earned** — combined merits across all powerplay activities | `PowerplayCollect`, `PowerplayDeliver` | `SUM(Count)` combined | summary-tile |
| 4 | **Powerplay Salary Earned** — total weekly salary credits accumulated | `PowerplaySalary` | `SUM(Amount)` | summary-tile |
| 5 | **Fast-Track Spending** — credits spent on fast-tracking power deliveries | `PowerplayFastTrack` | `SUM(Cost)`, `Faction` GROUP BY | bar-chart |
| 6 | **Votes Cast** — how many times you voted and for which Power | `PowerplayVote` | `Faction` (GROUP BY, COUNT) | bar-chart |
| 7 | **Power Allegiance History** — periods of joining/defecting/leaving powers (loyalty timeline) | `PowerplayJoin`, `PowerplayDefect`, `PowerplayLeave` | `Power`, `timestamp` per event | summary-tile |
| 8 | **Collection vs. Delivery Efficiency** — how closely delivery count matches collection count (indicates discipline) | `PowerplayCollect`, `PowerplayDeliver` | `SUM(Count)` each | summary-tile |
| 9 | **Most Active Powerplay Systems** — systems where you collected or delivered most often | `PowerplayCollect`, `PowerplayDeliver` | `StarSystem` (UNION, GROUP BY, COUNT) | bar-chart |
| 10 | **Commodity Types Delivered for Power** — which commodities were moved for powerplay deliveries | `PowerplayDeliver` | `Type_Localised` (GROUP BY, SUM(Count)) | bar-chart |

---

## FleetCarrierOperations

Source schemas: `CarrierStats`, `CarrierJump`, `CarrierJumpRequest`, `CarrierBuy`, `CarrierFinance`, `CarrierBankTransfer`, `CarrierDepositFuel`, `CarrierDecommission`, `CarrierModulePack`, `CarrierShipPack`, `CarrierTradeOrder`, `CarrierNameChange`, `CarrierDockingPermission`, `CarrierCrewServices`

| # | Idea | Table(s) | Key Columns | Viz |
|---|------|----------|-------------|-----|
| 1 | **Carrier Jump Destinations** — which systems your carrier jumped to most | `CarrierJump` | `StarSystem` (GROUP BY, COUNT) | bar-chart |
| 2 | **Bank Transfer Direction** — credits moved to carrier vs. withdrawn from carrier | `CarrierBankTransfer` | `SUM(Amount) GROUP BY Direction (deposit/withdraw)` | summary-tile |
| 3 | **Carrier Finance Summary** — total carrier upkeep paid, balance history | `CarrierFinance` | `CarrierBalance`, `ReserveBalance`, `Finance.MaintenanceTotal` | summary-tile |
| 4 | **Fuel Deposits Made** — total tritium deposited to carrier fuel over time | `CarrierDepositFuel` | `SUM(Amount)` | summary-tile |
| 5 | **Jump Requests Filed** — how many jumps were requested (including cancellations) | `CarrierJumpRequest`, `CarrierJumpCancelled` | `COUNT(*)` each | summary-tile |
| 6 | **Module Pack Purchases** — which module packs were purchased by type | `CarrierModulePack` | `PackTheme` (GROUP BY, COUNT) | bar-chart |
| 7 | **Ship Pack Purchases** — which ship packs were bought for carrier shipyard | `CarrierShipPack` | `PackTheme` (GROUP BY, COUNT) | bar-chart |
| 8 | **Carrier Trade Orders by Commodity** — what commodities were set up as trade orders via the carrier | `CarrierTradeOrder` | `Commodity` (GROUP BY, COUNT, SUM(Quantity)) | bar-chart |
| 9 | **Crew Services Installed** — breakdown of crew service types on carrier | `CarrierCrewServices` | `CrewRole` (GROUP BY, COUNT) | bar-chart |
| 10 | **Docking Permission Changes** — how access permissions changed over time (open/squadron/friends) | `CarrierDockingPermission` | `DockingAccess` (GROUP BY, COUNT) | bar-chart |

---

## EngineeringAndSynthesis

Source schemas: `EngineerContribution`, `EngineerCraft`, `EngineerProgress`, `Synthesis`, `TechnologyBroker`, `MaterialTrade`

| # | Idea | Table(s) | Key Columns | Viz |
|---|------|----------|-------------|-----|
| 1 | **Engineer Contributions by Engineer** — total commodities/data/material contributed to unlock engineers | `EngineerContribution` | `Engineer` (GROUP BY, SUM(Quantity)) | bar-chart |
| 2 | **Blueprints Applied by Module** — which modules were engineered most often | `EngineerCraft` | `Module` (GROUP BY, COUNT) | bar-chart |
| 3 | **Blueprints by Effect Type** — which blueprint effect was applied most (e.g., Increased Range, Thermal Resistance) | `EngineerCraft` | `BlueprintName` (GROUP BY, COUNT) | bar-chart |
| 4 | **Experimental Effects Applied** — count of experimental effect applications by effect name | `EngineerCraft` | `ExperimentalEffect_Localised` (GROUP BY, COUNT) | bar-chart |
| 5 | **Engineer Progress Reached** — grade level progress across all engineers | `EngineerProgress` | `Engineer`, `Rank`, `Progress` | bar-chart |
| 6 | **Synthesis Usage** — which synthesis recipes were used most (ammo, repair, fuel) | `Synthesis` | `Name` (GROUP BY, COUNT) | bar-chart |
| 7 | **Technology Broker Unlocks** — which tech broker items were unlocked | `TechnologyBroker` | `ItemsUnlocked[].Name` (GROUP BY, COUNT) | bar-chart |
| 8 | **Material Trades Made** — most common material trade categories (Raw → Raw, etc.) | `MaterialTrade` | `TradeCategory` or `Paid.Category`, `Received.Category` (GROUP BY) | bar-chart |
| 9 | **Total Materials Consumed in Crafting** — total material counts used in engineer crafts | `EngineerCraft` | `Ingredients[].Count` SUM per material Name_Localised | bar-chart |
| 10 | **Top Engineers Visited** — which engineer workshops you visited most (by craft events) | `EngineerCraft` | `Engineer` (GROUP BY, COUNT) | bar-chart |

---

## MaterialGathering

Source schemas: `MaterialCollected`, `MaterialDiscarded`, `Materials`, `MaterialTrade`, `ScientificResearch`

| # | Idea | Table(s) | Key Columns | Viz |
|---|------|----------|-------------|-----|
| 1 | **Materials Collected by Name** — most gathered materials across all sources | `MaterialCollected` | `Name_Localised` (GROUP BY, SUM(Count)) | bar-chart |
| 2 | **Materials by Category** — breakdown of gathered materials by grade/category (Raw, Manufactured, Encoded) | `MaterialCollected` | `Category` (GROUP BY, SUM(Count)) | bar-chart |
| 3 | **Materials Discarded by Type** — which materials were most often discarded (inventory management) | `MaterialDiscarded` | `Name_Localised` (GROUP BY, SUM(Count)) | bar-chart |
| 4 | **Material Discovery Count** — how many unique material types you discovered | `MaterialCollected` | `COUNT(DISTINCT Name_Localised)` | summary-tile |
| 5 | **Material Trade Ratios** — what you traded away vs. received (net flow analysis) | `MaterialTrade` | `Paid.Material`, `Paid.Quantity`, `Received.Material`, `Received.Quantity` | bar-chart |
| 6 | **Current Material Inventory Snapshot** — what's currently in your materials hold at end of session | `Materials` | `Raw[]/Manufactured[]/Encoded[]` Name + Count | bar-chart |
| 7 | **Scientific Research Donations** — materials donated to research projects | `ScientificResearch` | `Name_Localised` (GROUP BY, SUM(Count)) | bar-chart |
| 8 | **Grade 5 Material Accumulation** — how much of each G5 (rare) material you've gathered | `MaterialCollected` | `Name_Localised WHERE Grade = 5` (GROUP BY, SUM(Count)) | bar-chart |
| 9 | **Data Material Sources** — encoded data by source type (ship scan, wake scan, etc.) | `MaterialCollected` | `Name_Localised WHERE Category = 'Encoded'` GROUP BY | bar-chart |
| 10 | **Net Material Balance** — total collected vs. total discarded to measure efficiency | `MaterialCollected`, `MaterialDiscarded` | `SUM(Count)` each table | summary-tile |

---

## Missions (Passenger Transport)

Source schemas: `MissionAccepted`, `MissionCompleted`, `MissionFailed`, `MissionAbandoned`

| # | Idea | Table(s) | Key Columns | Viz |
|---|------|----------|-------------|-----|
| 1 | **Passenger Trips Completed** — total completed passenger transport missions | `MissionCompleted` | `COUNT(*) WHERE Name LIKE 'Mission_Passenger%'` | summary-tile |
| 2 | **Passengers Transported by Class** — volume by cabin class (Economy, Business, First, Luxury) | `MissionAccepted` | `PassengerType` (WHERE PassengerMission = true, GROUP BY, SUM(PassengerCount)) | bar-chart |
| 3 | **Passenger Mission Rewards** — total credits earned from passenger transport | `MissionCompleted` | `SUM(Reward) WHERE Name LIKE 'Mission_Passenger%'` | summary-tile |
| 4 | **Most Popular Destinations** — which systems passengers wanted to go to most | `MissionAccepted` | `DestinationSystem` (WHERE PassengerMission = true, GROUP BY, COUNT) | bar-chart |
| 5 | **VIP vs. Bulk Passenger Ratio** — first/luxury cabin missions vs. economy/business | `MissionAccepted` | `PassengerType GROUP BY` (aggregate luxury vs. bulk counts) | summary-tile |
| 6 | **Failed Passenger Trips** — how often passenger missions failed or were abandoned | `MissionFailed`, `MissionAbandoned` | `COUNT(*) WHERE Name LIKE 'Mission_Passenger%'` each | summary-tile |
| 7 | **Hostile Passenger Incidents** — how many passenger missions involved wanted passengers | `MissionAccepted` | `COUNT(*) WHERE PassengerWanted = true` | summary-tile |
| 8 | **Mission Departure Stations** — which stations you picked up passengers from most | `MissionAccepted` | (via `StationName` or `Faction` context) | bar-chart |
| 9 | **Passenger Count per Mission** — distribution of how many passengers per accepted mission | `MissionAccepted` | `PassengerCount` (CASE bucket: 1, 2–5, 6–12, 13+) | bar-chart |
| 10 | **Taxi Bookings vs. Custom Missions** — NPC taxi bookings vs. player-accepted passenger missions | `BookTaxi`, `MissionAccepted` | `COUNT(*)` each | summary-tile |

---

## CrimeAndSecurity

Source schemas: `CommitCrime`, `Bounty`, `FactionKillBond`, `PayBounty`, `PayFines`, `ClearImpound`, `GetImpounded`

| # | Idea | Table(s) | Key Columns | Viz |
|---|------|----------|-------------|-----|
| 1 | **Crimes by Type** — breakdown of all crimes committed by CrimeType | `CommitCrime` | `CrimeType` (GROUP BY, COUNT) | bar-chart |
| 2 | **Bounties Accumulated** — total bounty value generated by your crimes per faction | `CommitCrime` | `Faction` (GROUP BY, SUM(Fine)) (or Fine/Bounty field) | bar-chart |
| 3 | **Bounties Paid Off** — total credits spent paying bounties by faction | `PayBounty` | `Faction` (GROUP BY, SUM(Amount)) | bar-chart |
| 4 | **Fines Paid** — total fines paid by faction | `PayFines` | `Faction` (GROUP BY, SUM(Amount)) | bar-chart |
| 5 | **Criminal Activity Rate** — crimes per session day (trend indicator) | `CommitCrime` | `COUNT(*) GROUP BY DATE(timestamp)` | bar-chart |
| 6 | **Impound Events** — how many times your ship was impounded | `GetImpounded` | `COUNT(*)`, `StarSystem` GROUP BY | summary-tile |
| 7 | **Clear Impound Cost** — credits spent releasing impounded ships | `ClearImpound` | `SUM(Cost)` | summary-tile |
| 8 | **Crimes Against Players vs. NPCs** — PVP crimes vs. NPC crimes | `CommitCrime` | (detect via `Victim` field) GROUP BY victim type | summary-tile |
| 9 | **Most Wanted Systems** — systems where you committed most crimes (hot zones) | `CommitCrime` | `StarSystem` (GROUP BY, COUNT) | bar-chart |
| 10 | **Crime Revenue vs. Penalty Cost** — total bounty payouts earned vs. fines/bounties paid (net crime profitability) | `Bounty`, `PayBounty`, `PayFines` | `SUM(TotalReward)` vs. `SUM(Amount)` | summary-tile |

---

## SalvageAndRecovery

Source schemas: `CollectCargo` (MissionID-less), `EjectCargo`, `SearchAndRescue`, `SellSalvagedgoods` (via MarketSell illegal), `CommitCrime` (theft)

| # | Idea | Table(s) | Key Columns | Viz |
|---|------|----------|-------------|-----|
| 1 | **Salvage Collected by Type** — which salvage commodities were picked up most | `CollectCargo` | `Type_Localised` (WHERE MissionLinked = false, GROUP BY, SUM(Count)) | bar-chart |
| 2 | **Search and Rescue Cases** — total S&R deliveries and credits earned | `SearchAndRescue` | `COUNT(*)`, `SUM(Reward)` | summary-tile |
| 3 | **S&R Commodities Delivered** — types of items delivered to rescue megaship | `SearchAndRescue` | `Name_Localised` (GROUP BY, SUM(Count)) | bar-chart |
| 4 | **Cargo Ejected by Commodity** — what was jettisoned most (salvage waste or running hot) | `EjectCargo` | `Type_Localised` (GROUP BY, SUM(Count)) | bar-chart |
| 5 | **Salvage Source Systems** — which systems you salvaged in most | `CollectCargo` | `StarSystem` (WHERE from nav-beacon/USS context, GROUP BY, COUNT) | bar-chart |
| 6 | **Recovery vs. Jettison Ratio** — cargo collected vs. ejected (efficiency measure) | `CollectCargo`, `EjectCargo` | `SUM(Count)` each | summary-tile |
| 7 | **Rescue Vouchers Earned** — total rescue voucher value from S&R completions | `SearchAndRescue` | `SUM(Reward)` | summary-tile |
| 8 | **Blackbox vs. Personal Effects** — proportion of high vs. low value salvage items by type | `CollectCargo` | `Type_Localised` GROUP BY (categorize manually) | bar-chart |
| 9 | **Theft Incidents** — how many cargo-theft crimes correlated with salvage activity | `CommitCrime` | `COUNT(*) WHERE CrimeType = 'CargoTheft'` | summary-tile |
| 10 | **Salvage Selling Revenue** — total credits from selling salvaged goods at markets | `MarketSell` | `SUM(TotalSale) WHERE Type_Localised IN (salvage items)` | summary-tile |

---

## SocialMulticrew

Source schemas: `CrewMemberJoins`, `CrewMemberQuits`, `CrewMemberRoleChange`, `KickCrewMember`, `EndCrewSession`, `JoinACrew`, `QuitACrew`, `ChangeCrewRole`

| # | Idea | Table(s) | Key Columns | Viz |
|---|------|----------|-------------|-----|
| 1 | **Crew Sessions Hosted** — total multicrew sessions you hosted as captain | `EndCrewSession` | `COUNT(*)`, `SUM(OnCrewCount)` | summary-tile |
| 2 | **Average Crew Size** — how many crew members per session on average | `EndCrewSession` | `AVG(OnCrewCount)` | summary-tile |
| 3 | **Crew Member Roles Assigned** — breakdown of which crew roles active members took | `CrewMemberRoleChange` | `Role` (GROUP BY, COUNT) | bar-chart |
| 4 | **Crew Turnover Rate** — joins vs. quits per session (stability indicator) | `CrewMemberJoins`, `CrewMemberQuits` | `COUNT(*)` each | summary-tile |
| 5 | **Kicked vs. Voluntary Quits** — how often crew were kicked vs. leaving voluntarily | `KickCrewMember`, `CrewMemberQuits` | `COUNT(*)` each | summary-tile |
| 6 | **Sessions Joined as Crew** — how many times you were the crew member (not captain) | `JoinACrew` | `COUNT(*)`, `Captain` GROUP BY | bar-chart |
| 7 | **Role Changes During Sessions** — frequency of mid-session role switches | `CrewMemberRoleChange` | `COUNT(*)`, `Role` distribution | bar-chart |
| 8 | **NPC Crew Activity** — NPC crew assignment and dismissal patterns | `NpcCrewPaidWage`, `NpcCrewRank` | `NpcCrewName` GROUP BY, `COUNT(*)` | bar-chart |
| 9 | **Crew Session Duration Distribution** — short vs. long hosted sessions (bucketed by OnCrewCount * time proxy) | `EndCrewSession` | `OnCrewCount` (CASE bucket) | bar-chart |
| 10 | **Crew Combat Contribution** — crew member kills while in fighter/turret role | `FighterDestroyed`, `CrewMemberRoleChange` | JOIN on crew context, COUNT | summary-tile |

---

## SettlementActivities

Source schemas: `ApproachSettlement`, `Disembark`, `Embark`, `Touchdown`, `Liftoff`, `BookDropship`, `DropshipDeploy`, `CreateSuitLoadout`, `DeleteSuitLoadout`, `SwitchSuitLoadout`, `BuySuit`, `BuyWeapon`

| # | Idea | Table(s) | Key Columns | Viz |
|---|------|----------|-------------|-----|
| 1 | **Settlements Visited** — which named settlements you approached most | `ApproachSettlement` | `SettlementName` (GROUP BY, COUNT) | bar-chart |
| 2 | **Disembark Locations** — most common disembark locations (on-foot deployment spots) | `Disembark` | `StationName` or `NearestDestination_Localised` (WHERE OnStation = false, GROUP BY, COUNT) | bar-chart |
| 3 | **Suits Purchased** — which suit types you bought over the session | `BuySuit` | `Name_Localised` (GROUP BY, COUNT, SUM(Price)) | bar-chart |
| 4 | **Weapons Purchased** — weapon types acquired for on-foot loadouts | `BuyWeapon` | `Name_Localised` (GROUP BY, COUNT, SUM(Price)) | bar-chart |
| 5 | **Suit Loadouts Created vs. Deleted** — how many loadout configurations set up vs. removed | `CreateSuitLoadout`, `DeleteSuitLoadout` | `COUNT(*)` each | summary-tile |
| 6 | **Loadout Switches** — how often you swapped loadouts pre-deployment | `SwitchSuitLoadout` | `COUNT(*)`, `SuitName` distribution | bar-chart |
| 7 | **Dropship Deployments by Settlement** — dropship landing zones grouped by settlement | `DropshipDeploy` | `NearestDestination_Localised` (GROUP BY, COUNT) | bar-chart |
| 8 | **Suit Usage by Name** — which suit you deployed in most (guardian vs. dominator vs. maverick) | `SwitchSuitLoadout` | `SuitName` (GROUP BY, COUNT) | bar-chart |
| 9 | **Settlement Security Levels Visited** — LOW/MED/HIGH security settlements (derived from settlement type flags) | `ApproachSettlement` | (filter on settlement name suffix/type if available) | bar-chart |
| 10 | **Equipment Spend Summary** — total credits spent on suits and weapons | `BuySuit`, `BuyWeapon` | `SUM(Price)` each | summary-tile |

---

## ShipManagementAndOutfitting

Source schemas: `Loadout`, `ModuleBuy`, `ModuleSell`, `ModuleRetrieve`, `ModuleStore`, `ModuleSwap`, `SellShipOnRebuy`, `ShipyardBuy`, `ShipyardSell`, `ShipyardTransfer`, `ShipyardSwap`, `FetchRemoteModule`, `StoredModules`, `StoredShips`

| # | Idea | Table(s) | Key Columns | Viz |
|---|------|----------|-------------|-----|
| 1 | **Modules Purchased by Slot Type** — which outfitting slot categories you bought modules for most | `ModuleBuy` | `Slot` (GROUP BY, COUNT) | bar-chart |
| 2 | **Module Spend by Station** — total credits spent on modules at each station | `ModuleBuy` | `StationName` or `StarSystem` (GROUP BY, SUM(BuyPrice)) | bar-chart |
| 3 | **Ships Bought** — which ships were purchased over the session | `ShipyardBuy` | `ShipType_Localised` (GROUP BY, COUNT, SUM(ShipPrice)) | bar-chart |
| 4 | **Ships Sold** — which ships were sold and for how much | `ShipyardSell` | `ShipType_Localised` (GROUP BY, COUNT, SUM(ShipPrice)) | bar-chart |
| 5 | **Module Sell Revenue** — credits recovered by selling modules | `ModuleSell` | `SUM(SellPrice)`, `Name_Localised` GROUP BY | bar-chart |
| 6 | **Fleet Composition** — how many of each ship type you've owned | `ShipyardBuy` | `ShipType_Localised` (GROUP BY, COUNT cumulative) | bar-chart |
| 7 | **Remote Module Fetch Events** — how often modules were fetched from storage remotely | `FetchRemoteModule` | `COUNT(*)`, `Module_Localised` GROUP BY | bar-chart |
| 8 | **Module Swap Frequency** — how often you swapped modules mid-session (loadout tinkering) | `ModuleSwap` | `COUNT(*)`, `SlotSwapped` GROUP BY | bar-chart |
| 9 | **Best Value Module Sales** — revenue per module type sold (average SellPrice) | `ModuleSell` | `Name_Localised` (GROUP BY, AVG(SellPrice)) | bar-chart |
| 10 | **Shipyard Net Spend** — total credits spent buying minus credits received from selling ships | `ShipyardBuy`, `ShipyardSell` | `SUM(ShipPrice)` each | summary-tile |

---

## TravelAndNavigation

Source schemas: `FSDJump`, `SupercruiseEntry`, `SupercruiseExit`, `DockingRequested`, `DockingGranted`, `Docked`, `Undocked`, `Liftoff`, `Touchdown`, `NavRoute`, `NavRouteClear`, `FuelScoop`, `JetConeBoost`

| # | Idea | Table(s) | Key Columns | Viz |
|---|------|----------|-------------|-----|
| 1 | **Total Jumps Made** — overall jump count and total distance covered | `FSDJump` | `COUNT(*)`, `SUM(JumpDist)` | summary-tile |
| 2 | **Most Docked Stations** — which stations you docked at most | `Docked` | `StationName` (GROUP BY, COUNT) | bar-chart |
| 3 | **Supercruise Exit Destinations** — most common supercruise exit targets (POIs, stations, outposts) | `SupercruiseExit` | `NearestDestination_Localised` (GROUP BY, COUNT) | bar-chart |
| 4 | **Fuel Scooping Summary** — total fuel scooped and estimated scooping sessions | `FuelScoop` | `SUM(Scooped)`, `COUNT(*)` | summary-tile |
| 5 | **Docking Request Acceptance Rate** — granted vs. total requested docking events | `DockingRequested`, `DockingGranted` | `COUNT(*)` each | summary-tile |
| 6 | **Neutron Boost Mileage** — how many light years of jump range benefited from neutron boosts | `JetConeBoost` | `SUM(BoostValue)`, `COUNT(*)` | summary-tile |
| 7 | **Station Types Docked** — which types of stations (Coriolis, Orbis, Outpost, Mega) you docked at most | `Docked` | `StationType` (GROUP BY, COUNT) | bar-chart |
| 8 | **Planet Landings by Star System** — touchdown events per star system | `Touchdown` | `StarSystem` (GROUP BY, COUNT) | bar-chart |
| 9 | **Route Plans Filed** — how many nav route plans were set vs. cleared (proxy for long trips) | `NavRoute`, `NavRouteClear` | `COUNT(*)` each | summary-tile |
| 10 | **Systems by Security Level** — distribution of security levels in systems you visited | `FSDJump` | `SystemSecurity_Localised` (GROUP BY, COUNT) | bar-chart |

---

## ThargoidAX (Anti-Xeno Combat)

Source schemas: `UnderAttack`, `Interdicted`, `HullDamage`, `Died`, `Bounty`, `FactionKillBond`, `ShieldState`

*Note: ED journals do not have dedicated Thargoid-specific event types — AX activity is identified by correlating `Bounty.VictimFaction = 'Thargoid'` and similar victim/target fields.*

| # | Idea | Table(s) | Key Columns | Viz |
|---|------|----------|-------------|-----|
| 1 | **Thargoid Ships Killed** — count of Thargoid faction bounties claimed | `Bounty` | `COUNT(*) WHERE VictimFaction = 'Thargoid'` | summary-tile |
| 2 | **AX Bounty Earnings** — total credits from Thargoid bounties | `Bounty` | `SUM(TotalReward) WHERE VictimFaction = 'Thargoid'` | summary-tile |
| 3 | **AX Hull Damage Events** — how often hull was seriously damaged in AX encounters | `HullDamage` | `COUNT(*) WHERE Health < 0.5` (proxy for hard combat) | summary-tile |
| 4 | **AX Interdictions Faced** — how many times you were interdicted in AX zones | `Interdicted` | `COUNT(*) WHERE Interdictor` matches Thargoid context | summary-tile |
| 5 | **Shield Drops in AX Combat** — shield failures during high intensity fights | `ShieldState` | `COUNT(*) WHERE ShieldsUp = false` | summary-tile |
| 6 | **AX Combat Bond Earnings** — kill bonds earned in anti-xeno conflict zones | `FactionKillBond` | `SUM(Reward) WHERE AwardingFaction` matches AX context | summary-tile |
| 7 | **AX Interdiction Escape Rate** — escaped vs. submitted in AX interdictions | `EscapeInterdiction`, `Interdicted` | `COUNT(*)` each | summary-tile |
| 8 | **Heat Damage Incidents** — how often your ship reached critical heat in AX engagements | `HullDamage` | `COUNT(*) WHERE AttackerShip` contains heat context | summary-tile |
| 9 | **Deaths in AX Combat** — how many times you died in Thargoid space | `Died` | `COUNT(*)` (filter by known AX systems using FSDJump) | summary-tile |
| 10 | **AX Kill Types Distribution** — breakdown of interceptor vs. scout kills (if distinguishable via bounty target) | `Bounty` | `Target` (WHERE VictimFaction = 'Thargoid', GROUP BY Target) | bar-chart |
