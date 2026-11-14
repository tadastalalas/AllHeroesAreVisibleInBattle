AllHeroesAreVisibleInBattle: workspace instructions. They override the global file.

- <Prefix>: AHAVIB (Ahavib in type names). <Root> namespace and ModuleId: AllHeroesAreVisibleInBattle.
- Logger: `Diagnostics\AhavibLog.cs` (Debug / Warn / Error).
- No Harmony patches: no Patches\ folder, no PatchAll, no Lib.Harmony package, no Harmony entries in SubModule.xml.
- `Core\ModInfo.cs` holds the ModuleId, file and folder names and the module root path.
- `ModSettings.Reload()` runs in `SubModule.OnGameStart`. `SubModule.OnMissionBehaviorInitialize` adds the mission logic;
  the mission view is found by the game through `[DefaultView]`.
- The mod saves nothing (no campaign behavior, no SyncData).
- Kept exception: the MCM Id stays "AllHeroesAreVisibleInBattle" with FormatType "json2". It is released; changing it resets players' settings.
- Kept exception: `Data\KeyOptions.cs` (Keyboard, Controller) and `HighlightModeOption.All` are append-only. MCM saves the dropdown index.
- Kept exception: Gauntlet names stay as released: prefab `AHAVIB_HeroNameLabels`, brush `AHAVIB.HeroNameLabel.Text`, layer "AHAVIBHeroNameLabels".
- Kept exception: `Missions\Helpers.cs` keeps its name (GetAgentHero, IsCombatMode).
- Kept exception: letters, F1–F12 and Tab in the key lists are plain labels, not translated.
- Kept exception: `Missions\` and `UI\` files keep their older code style (this., LINQ FirstOrDefault, foreach, colour literals).
  Do not restyle them unasked; new or changed lines follow the global rules.
