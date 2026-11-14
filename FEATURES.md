# AllHeroesAreVisibleInBattle — Features

| Feature | Gate | File |
|---|---|---|
| Finds companions, friendly lords, enemy lords, formation captains and the army general in campaign battles (rescans every 20 s and when a hero spawns) | always | `Missions\AllHeroesAreVisibleInBattleMissionLogic.cs`, `Missions\Helpers.cs`, `SubModule.cs` |
| Formation captains read from Immersive Battlefields (IB_Core) when it is loaded | always | `Missions\AllHeroesAreVisibleInBattleMissionLogic.cs` |
| Outlines heroes in role colours while the highlight key is held or toggled | setting:HighlightMode, KeyboardKey, ControllerKey | `Missions\AllHeroesAreVisibleInBattleMissionView.cs` |
| Heroes beyond a distance from the camera are not outlined | setting:HighlightDistance | `Missions\AllHeroesAreVisibleInBattleMissionView.cs` |
| Hero names above heads in the outline colour | setting:ShowNames | `Missions\AllHeroesAreVisibleInBattleMissionView.cs`, `UI\HeroNameLabelsVM.cs`, `UI\HeroNameLabelItemVM.cs`, `_Module\GUI\Prefabs\AHAVIB_HeroNameLabels.xml`, `_Module\GUI\Brushes\AHAVIB_HeroNameLabel.xml` |
| Key and mode choices for MCM dropdowns and the xml | — | `Data\KeyOption.cs`, `Data\KeyOptions.cs`, `Data\HighlightModeType.cs`, `Data\HighlightModeOption.cs` |
| Settings: MCM when loaded, otherwise ModConfiguration.xml | always | `Core\ModSettings.cs`, `Data\MCMSettings.cs`, `Data\ModConfiguration.cs`, `_Module\ModConfiguration.xml` |
| Log file (problems always, debug lines only when enabled) | setting:LoggingEnabled | `Diagnostics\AhavibLog.cs`, `Core\ModInfo.cs` |
| Localization (English + 12 languages) | — | `Core\Texts.cs`, `_Module\ModuleData\Languages\` |
