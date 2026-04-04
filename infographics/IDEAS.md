# Infographic Ideas

This document lists interesting infographic ideas for each category, based on the Elite Dangerous journal event schemas. For each idea, the table(s) to query, the key columns, and the recommended visualization type are noted.

Visualization types:
- **bar-chart** — best when a meaningful categorical column drives GROUP BY
- **summary-tile** — best when the answer is a small set of aggregate scalars
- **table** — future type; not yet supported by the renderer

Considerations:
- **Interesting** - Infographics that tend toward larger numbers as the primary metric (the one compared against the threshold) are more interesting than a simple count of events.  Large totals canand shoould be suplemented with the number of events that resulted in that total in a detail query
- **Details** for summary-tile infographics text like "You earned 7000 Cr by performing x operation 5 times" is not interesting by itself.  Adding some lore-specifc description of the operation to fill out the tile makes what might be a boring statistic an interesting piece of information.  If the title or terms are not inherently obvious, provide an explanation in the detail section, or consider a more descriptive title.  e.g. what does "Trade Turnover" mean?  If that's really "credits earned through sales - creadit spent buying from the market" call it profit.
- **Report Type** - consider whether an infographic is well suited to a by-system type of report.  For instance, "Most visited system", "jump range profile", "total number of jumps" will always be 1 for a single system. Add an attribute to identify infographic definitions like that so we can filter those out when running by-system reports even though they're interesting in a summary type report

---

## Exploration

Source schemas: `FSDJump`, `Scan`, `ScanOrganic`, `SellOrganicData`, `MultiSellExplorationData`, `SAAScanComplete`, `FSSDiscoveryScan`, `FSSBodySignals`, `SAASignalsFound`, `FuelScoop`, `JetConeBoost`, `CodexEntry`, `Touchdown`

| # | Idea | Table(s) | Key Columns | Viz | Why interesting to players |
|---|------|----------|-------------|-----|---------------------------|
| 1 | **Cartographic Earnings** — total credits from exploration data sales split by base value vs. first-discovery bonus | `MultiSellExplorationData` | `SUM(TotalEarnings)`, `SUM(BaseValue)`, `SUM(Bonus)`, `COUNT(*)` | summary-tile | Exploration payouts can be enormous after a deep-space run — a big earnings number here validates the weeks spent far from the bubble |
| 2 | **Light Years Traveled** — total jump distance and jump count | `FSDJump` | `SUM(JumpDist)`, `COUNT(*)` | summary-tile | The explorer's odometer. A single number capturing the sheer scale of a commander's journeys — "I crossed 47,000 ly this period" carries more weight than any other stat |
| 3 | **Planet Classes Mapped** — distribution of planet types from detailed surface scans | `Scan` | `PlanetClass` (WHERE PlanetClass IS NOT NULL, GROUP BY, COUNT) | bar-chart | Earth-like worlds and water worlds are the crown jewels — the chart reveals whether you're hunting rare gems or sweeping rocky ice fields, and spotting that single ELW bar feels like a trophy |
| 4 | **Terraformable Worlds Found** — bodies with terraforming potential vs. total bodies scanned | `Scan` | `COUNT(*) WHERE TerraformState = 'Terraformable'`, total `COUNT(*)` | summary-tile | Terraformable worlds are rare prizes representing humanity's future colonies — each one is a genuine landmark moment in an explorer's log |
| 5 | **Systems Fully Honked** — total systems surveyed with the FSS and total bodies revealed across them | `FSSDiscoveryScan` | `COUNT(*)` (systems), `SUM(BodyCount)`, `SUM(NonBodyCount)` | summary-tile | "Honk count" is the explorer's handshake — it shows thoroughness and how many systems you've truly surveyed rather than just jumped through |
| 6 | **Surface Probe Efficiency** — proportion of detailed surface scans completed at or under the efficiency target | `SAAScanComplete` | `COUNT(*) WHERE ProbesUsed <= EfficiencyTarget` vs. total `COUNT(*)` | summary-tile | Efficient mapping earns a first-mapped bonus payout — your hit rate shows whether your probe placement skills are earning their keep |
| 7 | **Neutron Highway Boosts** — neutron star jet cone boosts taken and cumulative boost factor | `JetConeBoost` | `COUNT(*)`, `SUM(BoostValue)` | summary-tile | The neutron highway is the expressway of the galaxy — tracking boosts tells the story of a long-range expedition and the daring of diving into a neutron cone |
| 8 | **Stellar Remnants Encountered** — exotic stellar objects (neutron stars, black holes, white dwarfs) jumped to | `Scan` | `StarType` (GROUP BY, COUNT WHERE StarType IN list of remnant codes) | bar-chart | Exotic stellar objects are the landmarks of deep space — a black hole visit carries a sense of awe that a standard main-sequence star never will |
| 9 | **Codex Discoveries by Category** — new codex entries recorded, grouped by category | `CodexEntry` | `Category` (WHERE IsNewEntry = true, GROUP BY, COUNT) | bar-chart | Each codex entry is a page added to humanity's catalogue of the galaxy — the breakdown reveals whether you're a biologist, stellar cartographer, or xenophenomenon hunter |
| 10 | **Planet Landings** — bodies actually landed on (touchdowns outside stations), grouped by planet class | `Touchdown`, `Scan` | `COUNT(*) WHERE OnStation = false`; join to `Scan` on BodyName for `PlanetClass` | bar-chart | Landing on a world is an intimate act of exploration — this count shows how much you've gone boots-to-regolith rather than scanning from orbit |

---

## Missions

Source schemas: `MissionAccepted`, `MissionCompleted`, `MissionFailed`, `MissionAbandoned`, `MissionRedirected`

| # | Idea | Table(s) | Key Columns | Viz | Why interesting to players |
|---|------|----------|-------------|-----|---------------------------|
| 1 | **Mission Rewards Earned** — total credits earned from completed missions | `MissionCompleted` | `SUM(Reward)`, `COUNT(*)` | summary-tile | Mission running is the bread-and-butter grind for most commanders — a large reward total here is a satisfying measure of hustle and reputation building across the bubble |
| 2 | **Mission Outcomes** — completed vs. failed vs. abandoned breakdown | `MissionCompleted`, `MissionFailed`, `MissionAbandoned` | `COUNT(*)` per table | summary-tile | Your completion rate is your reputation score — even a small fail or abandon count tells a story about a chaotic session or an over-ambitious contract |
| 3 | **Top Mission Types** — which mission categories you ran most (delivery, assassinate, rescue, massacre, etc.) | `MissionAccepted` | `Name` (strip suffix, GROUP BY, COUNT) | bar-chart | The distribution reveals your playstyle at a glance — a massacre-heavy chart paints a very different commander portrait than a delivery-dominant one |
| 4 | **Most Active Mission Factions** — which minor factions you worked for most | `MissionAccepted` | `Faction` (GROUP BY, COUNT) | table | Faction loyalty is the heartbeat of the BGS — this shows which powers you've been building influence for, and the Inara link lets you look them up instantly |
| 5 | **Mission Destination Systems** — most common target systems for accepted missions | `MissionAccepted` | `DestinationSystem` (GROUP BY, COUNT) | table | Repeated destinations reveal your trade lanes and influence corridors — useful for seeing the geographic footprint of your mission work |
| 6 | **Massacres vs. Assassinations** — targeted kill mission counts and total reward | `MissionAccepted` | `COUNT(*) WHERE Name LIKE 'Mission_Massacre%'` vs. `LIKE 'Mission_Assassin%'`, `SUM` from `MissionCompleted` join | summary-tile | The difference encapsulates a philosophical split: massacre missions are efficient bounty farming while assassinations are high-stakes single-target contracts — both tell a story about how aggressively you played |
| 7 | **Wing Missions Taken** — wing-flagged missions accepted vs. total accepted | `MissionAccepted` | `COUNT(*) WHERE Wing = true` vs. total `COUNT(*)` | summary-tile | Wing missions offer multiplied rewards for coordinated crews — a high wing ratio shows you flew in good company and probably earned far more than solo grinders |
| 8 | **Redirected Missions** — missions redirected mid-flight vs. completed as-is | `MissionRedirected`, `MissionCompleted` | `COUNT(*)` each | summary-tile | Redirects are one of the BGS's hidden gears — they happen when the destination station changes due to faction conflict, quietly revealing that the galaxy's politics shifted around you |

---

## CombatShip

Source schemas: `Died`, `PVPKill`, `Bounty`, `CapShipBond`, `FactionKillBond`, `Interdicted`, `EscapeInterdiction`, `UnderAttack`, `ShieldState`, `HullDamage`

| # | Idea | Table(s) | Key Columns | Viz | Why interesting to players |
|---|------|----------|-------------|-----|---------------------------|
| 1 | **Bounty Vouchers Earned** — total credits collected from bounty hunting kills | `Bounty` | `SUM(TotalReward)`, `COUNT(*)` | summary-tile | A massive bounty total is the combat pilot's trophy case — it represents wanted ships turned to scrap and the accumulated price on a lot of criminal heads |
| 2 | **Most Hunted Ship Types** — the NPC ship types you destroyed most often | `Bounty` | `Target` (GROUP BY, COUNT) | bar-chart | Whether it's hordes of Sidewinders or a string of Anacondas, this chart tells you what you've been hunting — and hints at which combat zones or nav beacons you favour |
| 3 | **Faction Kill Bond Earnings** — combat zone bonds earned, by awarding faction | `FactionKillBond` | `AwardingFaction` (GROUP BY, SUM(Reward)) | bar-chart | CZ bonds represent pledged service to a faction in their hour of need — the breakdown shows which side of the conflict you were fighting for and how deeply committed you were |
| 4 | **Combat Zone Capital Ship Bonds** — earnings from wing CZ capital ship engagements | `CapShipBond` | `SUM(Reward)`, `AwardingFaction` (GROUP BY) | bar-chart | Taking down a capital ship is a rare and lucrative event that only happens in high-intensity conflict zones — each bond is a badge of honour from a massive fleet engagement |
| 5 | **Interdiction Outcomes** — submitted vs. escaped vs. evaded interdictions | `Interdicted`, `EscapeInterdiction` | `COUNT(*) WHERE Submitted = true/false`; `EscapeInterdiction COUNT(*)` | summary-tile | Your interdiction record is a survival report — a clean escape rate means your manoeuvre skills are paying off, while a string of submissions reveals the sessions where things got desperate |
| 6 | **PVP Kills Recorded** — player ships destroyed, with commander names | `PVPKill` | `COUNT(*)`, `Victim` (GROUP BY) | table | PVP kills are the rarest and most contentious entries in any combat log — even a handful here carries serious bragging rights |
| 7 | **Shield Failures** — shield-down events as a proxy for dangerous engagements | `ShieldState` | `COUNT(*) WHERE ShieldsUp = false` | summary-tile | Every shield failure is a moment where you were one bad decision away from rebuy — a high count proves you were in the thick of it rather than farming easy kills |
| 8 | **Hull Damage Under Fire** — how often your hull dropped into critical zones (<50% health) | `HullDamage` | `COUNT(*) WHERE Health < 0.5` | summary-tile | Hull damage events below 50% are near-death experiences — tracking them shows how close to disaster your combat sessions really were |

---

## CombatOnFoot

Source schemas: `Died`, `PVPKill`, `CommitCrime`, `FactionKillBond`, `BookDropship`, `DropshipDeploy`, `Touchdown`, `Embark`, `Disembark`, `SuitLoadout`, `BuyWeapon`, `SellWeapon`

| # | Idea | Table(s) | Key Columns | Viz | Why interesting to players |
|---|------|----------|-------------|-----|---------------------------|
| 1 | **On-Foot Kill Bond Earnings** — combat zone bonds earned in ground operations, by awarding faction | `FactionKillBond` | `AwardingFaction` (GROUP BY, SUM(Reward)) | bar-chart | Ground CZ bonds are the reward for room-by-room urban warfare — the faction breakdown shows which settlements' conflicts you've been dismantling, one guard at a time |
| 2 | **Dropship Deployments** — total dropship insertions and most frequent deployment sites | `DropshipDeploy` | `COUNT(*)`, `NearestDestination_Localised` (GROUP BY, COUNT) | bar-chart | Getting dropped into a hot settlement from orbit captures the whole Odyssey fantasy — this shows how many times you went in loud and how spread across the galaxy your operations were |
| 3 | **Suit Loadouts Used** — which suit types you deployed in most (Dominator, Maverick, Artemis) | `SwitchSuitLoadout` | `SuitName` (GROUP BY, COUNT) | bar-chart | Your suit distribution is your Odyssey identity — a Dominator-dominant chart says soldier, a Maverick says thief, an Artemis says scientist; most commanders are a mix and this reveals that balance |
| 4 | **Weapons Purchased** — on-foot weapons acquired by type and total credits spent | `BuyWeapon` | `Name_Localised` (GROUP BY, COUNT, SUM(Price)) | bar-chart | Gear upgrades are a big part of on-foot progression — seeing which weapons you've invested in over the period shows where your combat priorities and playstyle lie |
| 5 | **On-Foot Deaths** — how often you died on the ground | `Died` | `COUNT(*)` (where KillerName or context indicates on-foot) | summary-tile | Ground deaths hurt more than ship loss — no insurance payout, just respawn shame. Even a single on-foot death in a period is noteworthy, and a high count tells a story of aggressive or unlucky deployments |
| 6 | **Crimes Committed On-Foot** — breakdown by crime type during ground operations | `CommitCrime` | `CrimeType` (GROUP BY, COUNT) | bar-chart | The crime breakdown is a confessional for covert operators — whether it's murder, theft, or trespass, the mix of crimes paints a vivid picture of what kind of operative you are in the settlements |
| 7 | **Bookings vs. Actual Deployments** — dropship bookings made vs. successful deploy events | `BookDropship`, `DropshipDeploy` | `COUNT(*)` each | summary-tile | The gap between bookings and deployments hints at cancelled ops, mission failures, or rethought plans — a tidy ratio shows disciplined mission execution |

---

## Trade

Source schemas: `MarketBuy`, `MarketSell`, `CargoDepot`, `CargoTransfer`, `CarrierTradeOrder`

| # | Idea | Table(s) | Key Columns | Viz | Why interesting to players |
|---|------|----------|-------------|-----|---------------------------|
| 1 | **Trade Profit** — net credits earned from market trading (sales revenue minus cost of goods) | `MarketSell`, `MarketBuy` | `SUM(TotalSale)` vs. `SUM(TotalCost)` | summary-tile | Net profit is the trader's ultimate scorecard — it cuts through raw volume to reveal whether you actually made money after buying all that cargo |
| 2 | **Top Commodities by Units Moved** — which commodity types you traded the most of by unit count | `MarketBuy`, `MarketSell` | `Type_Localised` (UNION, GROUP BY, SUM(Count)) | bar-chart | Your top commodity is the fingerprint of your trade route — Tritium means carrier logistics, LTDs mean void opal mining payoff, Biowaste means something unusual is going on |
| 3 | **Best Selling Stations** — stations where you completed the most market sales by revenue | `MarketSell` | `StationName` (GROUP BY, SUM(TotalSale)) | table | Your favourite selling ports are the anchors of your trade network — Inara links let you revisit them quickly to check whether current prices still justify the route |
| 4 | **Black Market Sales** — illegal commodity sales by type | `MarketSell` | `Type_Localised` (WHERE IllegalGoods = true, GROUP BY, SUM(Count)) | bar-chart | Black market runs are the dark side of trading — a commander with Nerve Agents and Slaves in this chart has a very different story to tell than one running stolen artwork |
| 5 | **Carrier Trade Orders** — commodities listed as carrier buy/sell orders, by volume | `CarrierTradeOrder` | `Commodity` (GROUP BY, SUM(Quantity)) | bar-chart | Running carrier trade orders is operating a space trading post — the volume breakdown shows what markets you've been facilitating and how active your carrier economy has been |
| 6 | **Cargo Depot Completions** — wing cargo depot deliveries and pickups completed | `CargoDepot` | `COUNT(*) GROUP BY UpdateType` | summary-tile | Wing cargo depot missions are collaborative economy in action — completing them requires coordination and contributes directly to community goals and faction influence |

---

## Mining

Source schemas: `AsteroidCracked`, `ProspectedAsteroid`, `MiningRefined`, `LaunchDrone`, `JettisonCargo`, `Cargo`, `MarketSell`

| # | Idea | Table(s) | Key Columns | Viz | Why interesting to players |
|---|------|----------|-------------|-----|---------------------------|
| 1 | **Refined Materials** — total units refined grouped by material type | `MiningRefined` | `Type_Localised` (GROUP BY, SUM(Count)) | bar-chart | Your refined output chart is a mining resume — LTDs and Void Opals dominate lucrative hauls while plain iron suggests a very different kind of ring session |
| 2 | **Core Asteroids Cracked** — total core mining detonations | `AsteroidCracked` | `COUNT(*)` | summary-tile | Core mining is the most skill-intensive variant — each crack is a precision detonation and treasure hunt. A large crack count is a legitimate badge of expertise |
| 3 | **Prospecting Outcomes** — most common materials found in prospected asteroids | `ProspectedAsteroid` | `Content` (GROUP BY, COUNT) | bar-chart | Prospecting is the intelligence-gathering phase before the real work — the distribution tells you what the rings were hiding and whether your ring-type choices were paying off |
| 4 | **Mining Revenue** — total credits earned selling refined materials at markets | `MarketSell` | `SUM(TotalSale)` (WHERE commodity is a mineable type) | summary-tile | This is the payoff number that validates all the time spent in the rings — a large total means you found a good spot, refined efficiently, and got to market before prices shifted |
| 5 | **Drone Usage by Type** — collector vs. prospector drone launches | `LaunchDrone` | `Type` (GROUP BY, COUNT) | bar-chart | Drone burn rate determines how often you're reloading mid-session — the breakdown shows the operational overhead of your mining workflow |
| 6 | **Motherlode Strike Rate** — prospected asteroids flagged as motherlode vs. common | `ProspectedAsteroid` | `MotherlodeIndicator` (GROUP BY true/false, COUNT) | summary-tile | Motherlode asteroids are the jackpots of core mining — a healthy strike rate means your chosen rings and scanning technique are finding the premium rocks |

---

## Exobiology

Source schemas: `ScanOrganic`, `SellOrganicData`, `FSSBodySignals`, `SAASignalsFound`, `Scan`, `CodexEntry`

| # | Idea | Table(s) | Key Columns | Viz | Why interesting to players |
|---|------|----------|-------------|-----|---------------------------|
| 1 | **Exobiology Earnings** — total credits from organic data sales with first-discovery bonus shown separately | `SellOrganicData` | `SUM(Value)`, `SUM(Bonus)`, `COUNT(*)` | summary-tile | Exobiology has quietly become one of the most lucrative activities in the game — a first-discovery bonus on a rare species can run into hundreds of millions of credits |
| 2 | **Species Analysed by Genus** — completed three-sample analyses grouped by biological genus | `ScanOrganic` | `Genus_Localised` (WHERE ScanType = 'Analyse', GROUP BY, COUNT) | bar-chart | Each genus is a showcase of alien biology — the breakdown shows whether you're a specialist or a generalist and hints at which atmospheric world types you've been targeting |
| 3 | **First Discoveries Made** — species where you earned a first-discovery bonus | `SellOrganicData` | `Species_Localised` (WHERE Bonus > 0, GROUP BY, COUNT) | table | First discoveries are the rarest achievement in exploration — being the first human to scientifically record a species is one of the game's most meaningful moments |
| 4 | **Scan Completion Funnel** — Log vs. Sample vs. Analyse counts showing how often you finished the full sequence | `ScanOrganic` | `ScanType` (GROUP BY, COUNT) | bar-chart | The funnel exposes how often you started a species and didn't finish — a gap between Logs and Analyses is the signature of impatience or interrupted sessions |
| 5 | **Biological Signal Bodies** — planets carrying the most biological signal indicators from FSS | `FSSBodySignals` | `BodyName` (WHERE Type_Localised = 'Biological', GROUP BY, SUM(Count)) | bar-chart | Bodies flagged with 6+ biological signals are the holy grail for exobiologists — this chart lets you look back at which worlds offered the richest hunting grounds |
| 6 | **Variant Diversity** — count of unique species variants you've completed analyses for | `ScanOrganic` | `COUNT(DISTINCT Variant_Localised) WHERE ScanType = 'Analyse'` | summary-tile | Variant diversity measures the breadth of your biological catalogue — many biomes produce the same genus in different colours depending on the star type, and tracking this shows how far you've ranged |

---

## Powerplay

Source schemas: `PowerplayCollect`, `PowerplayDeliver`, `PowerplayFastTrack`, `PowerplaySalary`, `PowerplayVote`, `PowerplayDefect`, `PowerplayJoin`, `PowerplayLeave`

| # | Idea | Table(s) | Key Columns | Viz | Why interesting to players |
|---|------|----------|-------------|-----|---------------------------|
| 1 | **Merits Earned** — total merits collected and delivered across all powerplay activities | `PowerplayCollect`, `PowerplayDeliver` | `SUM(Count)` combined | summary-tile | Merits are the currency of political loyalty — a high total shows serious commitment to your Power's expansion agenda during the period |
| 2 | **Powerplay Salary Income** — total weekly salary credits accumulated | `PowerplaySalary` | `SUM(Amount)`, `COUNT(*)` | summary-tile | Powerplay salary is the passive income stream most commanders overlook — the cumulative total over a period can be surprisingly large and validates your rank investment |
| 3 | **Merits by Power** — collection and delivery totals broken down by which Power benefited | `PowerplayCollect`, `PowerplayDeliver` | `Power` (GROUP BY, SUM(Count)) | bar-chart | For commanders who've served multiple powers, this reveals the true distribution of their political work — loyalty is easy to claim but the merit data shows where the effort actually went |
| 4 | **Fast-Track Spending** — credits spent fast-tracking merit deliveries | `PowerplayFastTrack` | `SUM(Cost)`, `Faction` (GROUP BY) | summary-tile | Fast-tracking is an expensive shortcut that shows you were willing to spend big to hit a deadline — the cumulative cost reveals how urgently you were driving your power's agenda |
| 5 | **Allegiance Timeline** — power join, defection, and leave events | `PowerplayJoin`, `PowerplayDefect`, `PowerplayLeave` | `Power`, `timestamp` per event | summary-tile | Switching powers is a high-drama event in powerplay circles — a defection entry in your log is the equivalent of a political scandal, and the timeline shows how stable or mercurial your loyalties have been |
| 6 | **Votes Cast** — how many times you voted and for which Power | `PowerplayVote` | `Faction` (GROUP BY, COUNT) | bar-chart | Voting is the most passive form of powerplay participation — even seeing whether you voted at all, and for whom, is a snapshot of your political engagement week to week |

---

## FleetCarrierOperations

Source schemas: `CarrierStats`, `CarrierJump`, `CarrierJumpRequest`, `CarrierJumpCancelled`, `CarrierBuy`, `CarrierFinance`, `CarrierBankTransfer`, `CarrierDepositFuel`, `CarrierModulePack`, `CarrierShipPack`, `CarrierTradeOrder`, `CarrierDockingPermission`

| # | Idea | Table(s) | Key Columns | Viz | Why interesting to players |
|---|------|----------|-------------|-----|---------------------------|
| 1 | **Carrier Jump Destinations** — systems your carrier jumped to, most frequent first | `CarrierJump` | `StarSystem` (GROUP BY, COUNT) | table | Your carrier's itinerary is a travel diary writ large — the jump destinations tell the story of expeditions, mining operations, and the grand tours you've taken your floating city to |
| 2 | **Tritium Fuel Deposited** — total tritium loaded into the carrier fuel tank | `CarrierDepositFuel` | `SUM(Amount)`, `COUNT(*)` | summary-tile | Tritium is liquid gold for carrier owners — the cumulative amount deposited represents enormous investment of either your own mining time or serious market spending |
| 3 | **Carrier Bank Transfers** — credits deposited into vs. withdrawn from the carrier bank | `CarrierBankTransfer` | `SUM(Amount) GROUP BY Direction` | summary-tile | The carrier bank is where fleet finances live — the ratio of deposits to withdrawals reveals whether your carrier is self-sustaining or quietly bleeding credits every week |
| 4 | **Jumps Requested vs. Completed** — jump requests filed (including cancellations) vs. actual completed jumps | `CarrierJumpRequest`, `CarrierJumpCancelled`, `CarrierJump` | `COUNT(*)` each | summary-tile | Cancelled jumps are the fingerprints of changed plans — too many cancellations vs. completions suggests indecision or persistent obstacles to your fleet mobility |
| 5 | **Packs Purchased for Carrier** — module and ship packs stocked in the carrier outfitting and shipyard | `CarrierModulePack`, `CarrierShipPack` | `PackTheme` (GROUP BY, COUNT) | bar-chart | What packs you stock defines what your carrier offers the galaxy — a module-heavy carrier caters to engineers while a ship-pack carrier is a mobile shipyard |
| 6 | **Carrier Trade Order Volume** — commodities listed in trade orders, ranked by total quantity | `CarrierTradeOrder` | `Commodity` (GROUP BY, SUM(Quantity)) | bar-chart | Running carrier trade orders is operating a trading post in space — the commodity volume shows what markets you've been facilitating and how active your carrier economy has been |

---

## EngineeringAndSynthesis

Source schemas: `EngineerContribution`, `EngineerCraft`, `EngineerProgress`, `Synthesis`, `TechnologyBroker`, `MaterialTrade`

| # | Idea | Table(s) | Key Columns | Viz | Why interesting to players |
|---|------|----------|-------------|-----|---------------------------|
| 1 | **Blueprints Applied** — engineer craft attempts by blueprint name | `EngineerCraft` | `BlueprintName` (GROUP BY, COUNT) | bar-chart | Your most-crafted blueprints are the foundation of your build identity — a commander hammering Long Range FSD is building an exploration ship; someone spamming Charge Enhanced Power Plant is heading into combat |
| 2 | **Engineers Worked With** — which engineers you visited most, by craft count | `EngineerCraft` | `Engineer` (GROUP BY, COUNT) | bar-chart | Each engineer is a unique character with their own unlock story — the visit distribution reveals your specialist focus and how far along the engineering unlock tree you've progressed |
| 3 | **Experimental Effects Applied** — which experimental effects were added to modules | `EngineerCraft` | `ExperimentalEffect_Localised` (WHERE NOT NULL, GROUP BY, COUNT) | bar-chart | Experimentals are the finishing touches that define a high-end build — certain combos like Drag Munitions or Mass Lock Modifier are the signatures of a pilot who knows exactly what they're doing |
| 4 | **Synthesis Recipes Used** — which synthesis recipes were consumed most (ammo, repair, heatsink, fuel) | `Synthesis` | `Name` (GROUP BY, COUNT) | bar-chart | Synthesis reveals your play habits better than almost anything — spamming basic SRV fuel synthesis means surface grinding; burning through heatsink synthesis means deep neutron runs or intense AX combat |
| 5 | **Technology Broker Unlocks** — which tech broker items were unlocked | `TechnologyBroker` | `ItemsUnlocked[].Name` (GROUP BY, COUNT) | bar-chart | Tech broker unlocks are major one-time progression milestones — Guardian modules and meta-alloy reinforcements represent access to the game's most powerful non-engineered equipment |
| 6 | **Material Trades Made** — most common material trade pairs (Paid category → Received category) | `MaterialTrade` | `Paid.Category` + `Received.Category` (GROUP BY, COUNT) | bar-chart | Material trading shows how you solved your resource gaps — the flow from cheaper to rarer grades is a key part of engineering pipeline optimisation |

---

## MaterialGathering

Source schemas: `MaterialCollected`, `MaterialDiscarded`, `Materials`, `MaterialTrade`, `ScientificResearch`

| # | Idea | Table(s) | Key Columns | Viz | Why interesting to players |
|---|------|----------|-------------|-----|---------------------------|
| 1 | **Materials Collected by Category** — total units gathered broken down by Raw, Manufactured, Encoded | `MaterialCollected` | `Category` (GROUP BY, SUM(Count)) | bar-chart | The three material categories feed different engineering paths — a Manufactured-heavy chart says combat engineer; lots of Encoded says data-scanning work; Raw dominance says surface-SRV grinding |
| 2 | **Top Materials Gathered** — most collected individual material types by unit count | `MaterialCollected` | `Name_Localised` (GROUP BY, SUM(Count)) | bar-chart | Your top materials are a map of where you've been spending time — certain materials only come from specific sources like wake scanning, combat, or particular planet types, so the list tells the story of your activities |
| 3 | **Grade 5 Rare Materials Collected** — highest-grade materials accumulated during the period | `MaterialCollected` | `Name_Localised` (WHERE Grade = 5, GROUP BY, SUM(Count)) | bar-chart | G5 materials are the bottleneck of every serious engineer build — seeing how much of each you've accumulated validates the grind and shows whether your stockpile can sustain continued crafting |
| 4 | **Materials Discarded** — which materials were most often jettisoned from inventory | `MaterialDiscarded` | `Name_Localised` (GROUP BY, SUM(Count)) | bar-chart | Discards reveal the hidden friction of material gathering — constantly discarding the same material means your inventory is perpetually full; it's an honest look at which slots you're fighting for space in |
| 5 | **Net Material Flow** — total collected vs. total discarded as a balance indicator | `MaterialCollected`, `MaterialDiscarded` | `SUM(Count)` each | summary-tile | Net flow shows whether your gathering is outpacing your consumption and cap losses — a large discard total relative to collection means you're hitting caps regularly and potentially leaving valuable materials behind |
| 6 | **Scientific Research Donations** — materials contributed to research projects and community goals | `ScientificResearch` | `Name_Localised` (GROUP BY, SUM(Count)) | bar-chart | Donating materials to research is quiet civic participation in the galaxy's science — this tracks your contribution to the Pilots Federation's collective knowledge and community goal outcomes |

---

## PassengerTransport

Source schemas: `MissionAccepted`, `MissionCompleted`, `MissionFailed`, `MissionAbandoned`, `BookTaxi`, `TaxiCancelled`

| # | Idea | Table(s) | Key Columns | Viz | Why interesting to players |
|---|------|----------|-------------|-----|---------------------------|
| 1 | **Passenger Mission Earnings** — total credits from completed passenger transport missions | `MissionCompleted` | `SUM(Reward)`, `COUNT(*)` (WHERE Name LIKE 'Mission_Passenger%') | summary-tile | Passenger running in a well-fitted Beluga or Python is one of the steadiest credit generators in the game — a hefty earnings total here validates the cabin outfitting investment |
| 2 | **Passengers by Cabin Class** — total passengers transported broken down by Economy, Business, First, Luxury | `MissionAccepted` | `PassengerType` (WHERE PassengerMission = true, GROUP BY, SUM(PassengerCount)) | bar-chart | The cabin mix tells you what market you're serving — Luxury VIPs pay the most but are the most demanding; a bulk Economy chart means you're running a people-mover, not a private charter service |
| 3 | **Most Popular Destinations** — systems passengers most frequently wanted to travel to | `MissionAccepted` | `DestinationSystem` (WHERE PassengerMission = true, GROUP BY, COUNT) | table | The destination breakdown reveals the economic and political hot spots attracting the galaxy's travellers — conflict zones, tourist beacons, and permit-locked systems each draw distinct passenger types |
| 4 | **VIP vs. Bulk Passenger Ratio** — luxury and first-class missions vs. economy and business volume | `MissionAccepted` | `PassengerType` (aggregate as VIP vs. Bulk) | summary-tile | High-end VIP service and bulk transport are two very different businesses with different ship builds — this split shows whether you're running a boutique charter or a mass-transit operation |
| 5 | **Hostile Passenger Incidents** — missions involving wanted or hostile passengers | `MissionAccepted` | `COUNT(*) WHERE PassengerWanted = true` | summary-tile | Carrying a wanted passenger means every interdiction is a potential firefight — even one is a story worth noting; a pattern of carrying wanted passengers reveals something about your tolerance for risk (or your credit desperation) |
| 6 | **Taxi Bookings** — Apex taxi trips booked (NPC taxi service) | `BookTaxi` | `COUNT(*)`, `DestinationSystem` (GROUP BY) | summary-tile | Booking the Apex taxi is the game's own admission that sometimes you just need a lift — a non-zero count hints at at least one session where your ship was elsewhere and you needed to get somewhere in a hurry |

---

## CrimeAndSecurity

Source schemas: `CommitCrime`, `Bounty`, `FactionKillBond`, `PayBounty`, `PayFines`, `ClearImpound`, `GetImpounded`, `Resurrect`

| # | Idea | Table(s) | Key Columns | Viz | Why interesting to players |
|---|------|----------|-------------|-----|---------------------------|
| 1 | **Crimes Committed by Type** — your full rap sheet broken down by crime category | `CommitCrime` | `CrimeType` (GROUP BY, COUNT) | bar-chart | The crime type chart is your official rap sheet — murder vs. assault vs. cargo theft vs. trespass paint distinct pictures of whether you're a pirate, a careless pilot, or a full-time criminal |
| 2 | **Bounties Generated by Faction** — total bounty value incurred per faction jurisdiction | `CommitCrime` | `Faction` (GROUP BY, SUM(Fine)) | bar-chart | A large bounty total means you've been very busy, very reckless, or both — the faction breakdown reveals which jurisdictions have the biggest price on your head |
| 3 | **Bounties Paid Off** — credits spent clearing bounties by faction | `PayBounty` | `Faction` (GROUP BY, SUM(Amount)) | bar-chart | Paying off bounties is the cost of doing illegal business — large payouts mean real money spent to make your record clean, and the faction split shows which police forces were most expensive to appease |
| 4 | **Fines Paid** — total minor fines paid across all factions | `PayFines` | `Faction` (GROUP BY, SUM(Amount)) | bar-chart | Fines are the slap-on-the-wrist for minor infractions — a high fines chart suggests aggressive flying, careless docking scans, or persistent scanning of ships that really didn't appreciate the attention |
| 5 | **Impound Incidents** — ships impounded and total credits spent recovering them | `GetImpounded`, `ClearImpound` | `COUNT(*)`, `SUM(Cost)` | summary-tile | Getting impounded is expensive and humiliating — it means a station had enough of your criminal record and seized your ship; even one impound event is a memorable session moment |
| 6 | **Crime Revenue vs. Penalty Cost** — total bounty rewards earned compared to total bounties and fines paid (net crime profitability) | `Bounty`, `PayBounty`, `PayFines` | `SUM(TotalReward)` vs. `SUM(Amount)` combined | summary-tile | Did crime actually pay? This single comparison answers it directly and might reveal that your law-breaking career is costing you more than it earns |

---

## SalvageAndRecovery

Source schemas: `CollectCargo`, `EjectCargo`, `SearchAndRescue`, `MarketSell`

| # | Idea | Table(s) | Key Columns | Viz | Why interesting to players |
|---|------|----------|-------------|-----|---------------------------|
| 1 | **Search and Rescue Earnings** — total credits from S&R deliveries to rescue megaships | `SearchAndRescue` | `SUM(Reward)`, `COUNT(*)` | summary-tile | S&R work is among the most heroic activities in Elite — delivering escape pods, black boxes, and personal effects recovered from disaster sites is a genuine act of service rewarded by the Pilots Federation |
| 2 | **S&R Items Delivered by Type** — commodities handed in at rescue megaships | `SearchAndRescue` | `Name_Localised` (GROUP BY, SUM(Count)) | bar-chart | The mix of recovered items tells the story of the disaster you were responding to — escape pods mean survivors; black boxes mean catastrophe investigation; occupied cryopods are the most haunting of all |
| 3 | **Salvage Collected by Commodity** — which salvage types were scooped from signal sources and debris fields | `CollectCargo` | `Type_Localised` (WHERE MissionID IS NULL, GROUP BY, SUM(Count)) | bar-chart | The salvage haul reveals the nature of the wrecks you've been picking over — cargo containers, technical blueprints, or personal effects each paint a different picture of the disaster sites you've visited |
| 4 | **Cargo Ejected by Type** — what was jettisoned and how much, per commodity | `EjectCargo` | `Type_Localised` (GROUP BY, SUM(Count)) | bar-chart | Jettisoned cargo is either a tactical sacrifice (dumping contraband before a police scan) or operational waste — the types tell you whether you were running hot legally or disposing of something incriminating |
| 5 | **Recovery vs. Jettison Ratio** — total cargo collected vs. ejected as a haul efficiency measure | `CollectCargo`, `EjectCargo` | `SUM(Count)` each | summary-tile | How much you jettison compared to what you scoop is a measure of discipline and cargo management — a high jettison ratio means you're scooping indiscriminately and then paying the price in wasted trips |

---

## SocialMulticrew

Source schemas: `CrewMemberJoins`, `CrewMemberQuits`, `CrewMemberRoleChange`, `KickCrewMember`, `EndCrewSession`, `JoinACrew`, `QuitACrew`, `ChangeCrewRole`, `NpcCrewPaidWage`

| # | Idea | Table(s) | Key Columns | Viz | Why interesting to players |
|---|------|----------|-------------|-----|---------------------------|
| 1 | **Crew Sessions Hosted** — total multicrew sessions captained and total crew-slots served | `EndCrewSession` | `COUNT(*)`, `SUM(OnCrewCount)` | summary-tile | Hosting multicrew is an act of community — every session is a shared adventure where someone trusted you with their time, and the crew count tells you how many player-sessions you provided |
| 2 | **Sessions Joined as Crew** — times you boarded another commander's ship as a crew member | `JoinACrew` | `COUNT(*)`, `Captain` (GROUP BY, COUNT) | bar-chart | Joining as crew is the opposite of captaining — trusting someone else to lead while you fly their weapons or fighters. The captain breakdown shows whose ships you've spent time aboard |
| 3 | **Crew Roles Distribution** — which multicrew roles were filled most (Helm, Gunner, Fighter Con) | `CrewMemberRoleChange`, `ChangeCrewRole` | `Role` (GROUP BY, COUNT) | bar-chart | Role distribution reveals what kind of crew experience you're creating and seeking — a Gunner-heavy chart means combat multicrew; Fighter Con focus means you're running fighter wing operations |
| 4 | **Crew Stability** — voluntary quits vs. kicks during hosted sessions | `CrewMemberQuits`, `KickCrewMember` | `COUNT(*)` each | summary-tile | A captain with no kicks and few quits runs tight, enjoyable sessions — a high kick count might indicate strict discipline or repeated encounters with crew who weren't pulling their weight |
| 5 | **NPC Crew Wages Paid** — total credits paid to hired NPC crew members | `NpcCrewPaidWage` | `SUM(Amount)`, `NpcCrewName` (GROUP BY, SUM) | bar-chart | NPC crew quietly drain credits through wages — this shows exactly how much your hired hands have cost you and which crew members have been most on the payroll during the period |

---

## SettlementActivities

Source schemas: `ApproachSettlement`, `Disembark`, `Embark`, `Touchdown`, `Liftoff`, `BookDropship`, `DropshipDeploy`, `CreateSuitLoadout`, `DeleteSuitLoadout`, `SwitchSuitLoadout`, `BuySuit`, `BuyWeapon`

| # | Idea | Table(s) | Key Columns | Viz | Why interesting to players |
|---|------|----------|-------------|-----|---------------------------|
| 1 | **Settlements Visited** — named settlements approached, most frequent first | `ApproachSettlement` | `SettlementName` (GROUP BY, COUNT) | table | Favoured settlements are the recurring stages of your Odyssey story — repeatedly visiting the same installation means you know its layout, its patrol routes, and exactly where the loot spawns |
| 2 | **Suit Usage Distribution** — which suit you deployed in most based on loadout switches | `SwitchSuitLoadout` | `SuitName` (GROUP BY, COUNT) | bar-chart | Your most-used suit tells your Odyssey story in one bar — Dominator means soldier, Maverick means rogue operative, Artemis means explorer; most commanders are a mix and this reveals the true balance |
| 3 | **Suits Purchased** — which suit types bought and total credits spent on them | `BuySuit` | `Name_Localised` (GROUP BY, COUNT, SUM(Price)) | bar-chart | Suit purchases mark the milestones of on-foot progression — each one represents a new capability unlocked or a major loadout upgrade, and the spend total shows the scale of your Odyssey investment |
| 4 | **Weapons Purchased** — on-foot weapons acquired by type and total credits spent | `BuyWeapon` | `Name_Localised` (GROUP BY, COUNT, SUM(Price)) | bar-chart | Your weapons purchases are the armoury receipts of your Odyssey career — the distribution shows where your combat priorities lie and which weapon types you keep coming back to upgrade |
| 5 | **Equipment Spend** — total credits spent on suits and weapons combined | `BuySuit`, `BuyWeapon` | `SUM(Price)` each | summary-tile | On-foot gear is a significant credit sink that's easy to lose track of — a large combined total here validates the grind that funded those upgrades and shows how seriously you've invested in Odyssey content |
| 6 | **Disembark Locations** — most frequent places you went on foot (surface settlements vs. stations) | `Disembark` | `StationName` / `NearestDestination_Localised` (GROUP BY, COUNT) | bar-chart | Where you choose to disembark is where the action is — heavy surface settlement disembarks mark an operative; lots of station disembarks suggest engineering visits, bar missions, or social stops |

---

## ShipManagementAndOutfitting

Source schemas: `Loadout`, `ModuleBuy`, `ModuleSell`, `ModuleRetrieve`, `ModuleStore`, `ModuleSwap`, `ShipyardBuy`, `ShipyardSell`, `ShipyardTransfer`, `ShipyardSwap`, `FetchRemoteModule`, `Repair`, `RepairAll`

| # | Idea | Table(s) | Key Columns | Viz | Why interesting to players |
|---|------|----------|-------------|-----|---------------------------|
| 1 | **Ships Purchased** — which ship types were bought and total credits spent | `ShipyardBuy` | `ShipType_Localised` (GROUP BY, COUNT, SUM(ShipPrice)) | bar-chart | Every ship purchase is a character decision — an Anaconda says endgame multi-role, a Krait Phantom says explorer, a Vulture says determined bounty hunter; the purchase history reads like a career progression |
| 2 | **Module Spend by Slot Type** — outfitting credits broken down by slot category | `ModuleBuy` | `Slot` (GROUP BY, SUM(BuyPrice)) | bar-chart | Module spend is where the real outfitting investment lives — the slot breakdown reveals your build priorities: FSD upgrades for range, weapons for combat, or shield generators for survival |
| 3 | **Shipyard Net Spend** — total credits spent buying ships vs. recovered selling them | `ShipyardBuy`, `ShipyardSell` | `SUM(ShipPrice)` each | summary-tile | Net ship spend is the true cost of your fleet evolution — the gap between buy and sell totals shows how much money you've permanently sunk into hulls you've moved on from |
| 4 | **Module Sell Revenue** — credits recovered selling modules back, by module type | `ModuleSell` | `Name_Localised` (GROUP BY, SUM(SellPrice)) | bar-chart | Module selling is the financial aftermath of a refit — a large sell revenue and a diverse list of sold modules tells the story of a significant reconfiguration or a purposeful fleet overhaul |
| 5 | **Remote Module Fetches** — modules retrieved from remote storage by type | `FetchRemoteModule` | `Module_Localised` (GROUP BY, COUNT) | bar-chart | Fetching modules remotely saves a cross-bubble trip but adds a surcharge — a high fetch count shows active multi-ship fleet management and suggests you're keeping several builds operational in parallel |
| 6 | **Repair Activity** — hull and module repair events, total credits spent on repairs | `Repair`, `RepairAll` | `COUNT(*)`, `Cost` (SUM if available) | summary-tile | How much you spend on repairs is an honest measure of how rough your sessions were — a large repair bill means you were in the thick of it, taking punishment, and pushing your ship to its limits |

---

## TravelAndNavigation

Source schemas: `FSDJump`, `SupercruiseEntry`, `SupercruiseExit`, `DockingRequested`, `DockingGranted`, `Docked`, `Undocked`, `Liftoff`, `Touchdown`, `NavRoute`, `NavRouteClear`, `FuelScoop`, `JetConeBoost`

| # | Idea | Table(s) | Key Columns | Viz | Why interesting to players |
|---|------|----------|-------------|-----|---------------------------|
| 1 | **Total Distance Traveled** — total light years jumped and overall jump count | `FSDJump` | `SUM(JumpDist)`, `COUNT(*)` | summary-tile | The cosmic odometer — total light years traveled is the single most evocative number a pilot can show; a weekly total spanning hundreds or thousands of ly is a genuine testament to how much of the galaxy you've moved through. *Summary reports only — not meaningful in by-system reports.* |
| 2 | **Most Docked Stations** — stations you landed at most often | `Docked` | `StationName` (GROUP BY, COUNT) | table | Your most-visited stations are your home ports — the Inara link lets you instantly look up the place you keep returning to, whether it's a trading hub, an engineering base, or your personal carrier |
| 3 | **Station Types Visited** — which station architectures you docked at (Coriolis, Orbis, Outpost, Megaship, Carrier) | `Docked` | `StationType` (GROUP BY, COUNT) | bar-chart | The station type mix is a travel demographic — lots of outpost docks suggest frontier work; lots of Coriolis and Orbis suggests core worlds trading; megaship docking flags rescue or Thargoid response activity |
| 4 | **Fuel Scooped from Stars** — total fuel collected from scoopable stars | `FuelScoop` | `SUM(Scooped)`, `COUNT(*)` | summary-tile | Fuel scooping is the quiet pulse of long-range travel — a large total represents many stellar passes, carefully calculated coronaskirting, and the skill of knowing which stars are worth the approach |
| 5 | **Systems by Security Level** — distribution of security levels in systems visited | `FSDJump` | `SystemSecurity_Localised` (GROUP BY, COUNT) | bar-chart | Your security level distribution is an honest map of where you operate — a low-security-heavy chart flags a frontier trader, bounty hunter, or criminal; a high-security-dominant chart suggests core world activity |
| 6 | **Supercruise Exit Targets** — most common supercruise drop destinations | `SupercruiseExit` | `NearestDestination_Localised` (GROUP BY, COUNT) | bar-chart | Drop destinations tell your daily routine at fine granularity — frequent drops at Compromise Beacons suggest bounty hunting, Resource Extraction Sites signal mining, and Tourist Beacons hint at exploration side trips |

---

## ThargoidAX (Anti-Xeno Combat)

Source schemas: `Bounty`, `FactionKillBond`, `Interdicted`, `EscapeInterdiction`, `HullDamage`, `ShieldState`, `Died`

*Note: ED journals do not have dedicated Thargoid-specific event types — AX activity is identified by correlating `Bounty.VictimFaction = 'Thargoid'` and similar victim/target fields.*

| # | Idea | Table(s) | Key Columns | Viz | Why interesting to players |
|---|------|----------|-------------|-----|---------------------------|
| 1 | **Thargoid Ships Destroyed** — total confirmed Thargoid kills and bounty rewards earned | `Bounty` | `COUNT(*)`, `SUM(TotalReward)` (WHERE VictimFaction = 'Thargoid') | summary-tile | Killing Thargoids is the most dangerous thing most commanders will attempt — a confirmed kill count represents real skill, preparation, and the courage to face enemies that can neutralise an entire ship in moments |
| 2 | **AX Kill Types** — breakdown of interceptor variants vs. scouts destroyed | `Bounty` | `Target` (WHERE VictimFaction = 'Thargoid', GROUP BY, COUNT) | bar-chart | The difference between a Cyclops and a Hydra is the difference between a challenging fight and a career-defining one — this chart shows how far up the interceptor ladder you've climbed |
| 3 | **AX Combat Bond Earnings** — kill bonds earned fighting in anti-xeno conflict zones | `FactionKillBond` | `SUM(Reward)`, `AwardingFaction` (GROUP BY) (filtered to AX contexts) | bar-chart | AX CZ bonds are paid specifically for fighting in defence of human settlements — they carry a different weight than ordinary bounties and represent organised resistance against the Thargoid threat |
| 4 | **Hull Damage in AX Engagements** — how often your hull dropped below 50% fighting Thargoids | `HullDamage` | `COUNT(*) WHERE Health < 0.5` | summary-tile | Getting hulled by a Thargoid is terrifying in a way NPC ships rarely achieve — each critical hull event represents caustic damage, lightning strikes, or a direct interceptor hit that nearly put you in the rebuy screen |
| 5 | **Shield Breaches in AX Combat** — shield-down events correlated with Thargoid engagement | `ShieldState` | `COUNT(*) WHERE ShieldsUp = false` | summary-tile | Thargoids punch through human shields faster than almost any NPC — a high breach count alongside low deaths shows genuine AX resilience; alongside many deaths it honestly shows the learning curve |
| 6 | **AX Interdictions Survived** — Thargoid interdictions faced and outcomes | `Interdicted`, `EscapeInterdiction` | `COUNT(*)` each (in Thargoid space context) | summary-tile | Being pulled from hyperspace by a Thargoid is one of the game's defining moments of terror — surviving it by escaping is a genuine test of skill and the kind of story that gets retold in the Pilot's Bar |

---

## Administrivia

Source schemas: `LoadGame`, `Statistics`, `EngineerProgress`, `Rank`, `Progress`

| # | Idea | Table(s) | Key Columns | Viz | Why interesting to players |
|---|------|----------|-------------|-----|---------------------------|
| 1 | **Active Play Days** — unique calendar days on which journal activity was recorded | `LoadGame` | `COUNT(DISTINCT DATE(timestamp))` | summary-tile | Days played is the simplest measure of dedication — even a handful of active days shows real commitment to the galaxy, and a full week of consecutive sessions is worth acknowledging |
| 2 | **Commander Rank Snapshot** — combat, trade, exploration, CQC, and superpower ranks as of the end of the report period | `Rank`, `Progress` | `Combat`, `Trade`, `Explore`, `CQC`, `Federation`, `Empire` fields | summary-tile | Your rank profile is your official Pilots Federation biography — seeing all ranks in one tile shows whether you're a balanced commander or a specialist pushing one path toward its Elite ceiling |
| 3 | **Session Count** — number of distinct game sessions (LoadGame events) during the period | `LoadGame` | `COUNT(*)` | summary-tile | Session count puts scale on everything else in the report — a huge earnings total spread across one session is very different from the same total earned across thirty separate play sessions |

---

## CodexAndDiscoveries

Source schemas: `CodexEntry`, `Scan`, `FSSAllBodiesFound`, `SAAScanComplete`, `ScanOrganic`

| # | Idea | Table(s) | Key Columns | Viz | Why interesting to players |
|---|------|----------|-------------|-----|---------------------------|
| 1 | **Codex Entries by Category** — new codex discoveries grouped by knowledge category | `CodexEntry` | `Category` (WHERE IsNewEntry = true, GROUP BY, COUNT) | bar-chart | The Codex is humanity's collective knowledge of the galaxy — each new entry you add is genuinely unique to you, and the category breakdown shows the breadth of your curiosity across biology, stellar phenomena, and alien artefacts |
| 2 | **Systems Fully Surveyed** — systems where all bodies were found via the FSS | `FSSAllBodiesFound` | `COUNT(*)` | summary-tile | The FSS completion chime when the last body registers is one of the game's most satisfying sounds — this count shows how many systems you've documented completely rather than just passing through |
| 3 | **First Mapped Bodies** — planets where you performed the first detailed surface scan | `SAAScanComplete` | `COUNT(*) WHERE FirstMappedBy matches current commander` | summary-tile | Being first to map a world means your name is in the game's database permanently — first-mapped worlds are the explorer's equivalent of planting a flag, and this count shows how many bear your name |
| 4 | **Codex Discoveries by Region** — new entries broken down by galactic region | `CodexEntry` | `Region` (WHERE IsNewEntry = true, GROUP BY, COUNT) | bar-chart | Different regions of the galaxy hold different secrets — this shows where your discoveries clustered, whether it's the Orion Spur, the Norma Arm, or the far reaches of the Outer Arm |

---

## EconomyAndMarket

Source schemas: `MarketBuy`, `MarketSell`, `CommodityReward`, `MissionCompleted`

| # | Idea | Table(s) | Key Columns | Viz | Why interesting to players |
|---|------|----------|-------------|-----|---------------------------|
| 1 | **Total Market Volume** — total units bought and sold across all commodity transactions | `MarketBuy`, `MarketSell` | `SUM(Count)` each | summary-tile | Raw volume tells the story of a trading operation's scale — a commander moving millions of tons of cargo is playing the economy game at an entirely different level from someone dabbling between combat sessions |
| 2 | **Net Profit by Commodity** — which commodities generated the most profit (sell revenue minus buy cost) | `MarketSell`, `MarketBuy` | `Type_Localised` (GROUP BY, SUM(TotalSale) - SUM(TotalCost)) | bar-chart | Not all commodities are created equal — this shows where the real money came from and whether your instincts about which goods to haul were validated by the actual profit numbers |
| 3 | **Market Buy vs. Sell Balance** — total credits spent at markets vs. total earned from sales | `MarketBuy`, `MarketSell` | `SUM(TotalCost)` vs. `SUM(TotalSale)` | summary-tile | This ratio cuts straight to the bottom line — if you spent more than you earned, either prices moved against you, you were stockpiling for a future run, or something in the supply chain needs rethinking |
| 4 | **Commodity Reward Income** — total value of commodities received as mission or event rewards | `CommodityReward` | `Name_Localised` (GROUP BY, SUM(Count)) | bar-chart | Commodity rewards are the in-kind payment economy of the galaxy — rare goods handed over as mission rewards or Thargoid site loot can be worth far more than the credit payout appears at first glance |
