using AllHeroesAreVisibleInBattle.Core;
using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.Global;
using MCM.Common;

namespace AllHeroesAreVisibleInBattle.Data;

// Used only when MCM is loaded. The only file that may use MCM types.
internal class MCMSettings : AttributeGlobalSettings<MCMSettings>
{
    // Released Id: changing it resets every player's saved settings.
    private const string SettingsId = ModInfo.ModuleId;
    private const string SaveFormat = "json2";
    private const string DistanceFormat = "0 m";

    public override string Id => SettingsId;

    public override string DisplayName => Texts.ModName.ToString();

    public override string FolderName => ModInfo.ModuleId;

    public override string FormatType => SaveFormat;

    [SettingPropertyDropdown(Texts.Ids.HighlightMode, Order = 0, RequireRestart = false, HintText = Texts.Ids.HighlightModeHint)]
    [SettingPropertyGroup(Texts.Ids.GroupGeneral, GroupOrder = 0)]
    public Dropdown<HighlightModeOption> HighlightMode { get; set; } = new Dropdown<HighlightModeOption>(HighlightModeOption.All, HighlightModeOption.IndexOf(ModConfiguration.HighlightModeDefault));

    [SettingPropertyDropdown(Texts.Ids.KeyboardKey, Order = 1, RequireRestart = false, HintText = Texts.Ids.KeyboardKeyHint)]
    [SettingPropertyGroup(Texts.Ids.GroupGeneral, GroupOrder = 0)]
    public Dropdown<KeyOption> KeyboardKey { get; set; } = new Dropdown<KeyOption>(KeyOptions.Keyboard, KeyOptions.IndexOf(KeyOptions.Keyboard, ModConfiguration.KeyboardKeyDefault));

    [SettingPropertyDropdown(Texts.Ids.ControllerKey, Order = 2, RequireRestart = false, HintText = Texts.Ids.ControllerKeyHint)]
    [SettingPropertyGroup(Texts.Ids.GroupGeneral, GroupOrder = 0)]
    public Dropdown<KeyOption> ControllerKey { get; set; } = new Dropdown<KeyOption>(KeyOptions.Controller, KeyOptions.IndexOf(KeyOptions.Controller, ModConfiguration.ControllerKeyDefault));

    [SettingPropertyInteger(Texts.Ids.HighlightDistance, ModConfiguration.HighlightDistanceMin, ModConfiguration.HighlightDistanceMax, DistanceFormat, Order = 3, RequireRestart = false, HintText = Texts.Ids.HighlightDistanceHint)]
    [SettingPropertyGroup(Texts.Ids.GroupGeneral, GroupOrder = 0)]
    public int HighlightDistance { get; set; } = ModConfiguration.HighlightDistanceDefault;

    [SettingPropertyBool(Texts.Ids.ShowNames, Order = 4, RequireRestart = false, HintText = Texts.Ids.ShowNamesHint)]
    [SettingPropertyGroup(Texts.Ids.GroupGeneral, GroupOrder = 0)]
    public bool ShowNames { get; set; } = ModConfiguration.ShowNamesDefault;

    // "Technical settings" is always the last group and "Logging for debugging" its last entry.
    [SettingPropertyBool(Texts.Ids.LoggingEnabled, Order = 0, RequireRestart = false, HintText = Texts.Ids.LoggingEnabledHint)]
    [SettingPropertyGroup(Texts.Ids.GroupTechnical, GroupOrder = 1)]
    public bool LoggingEnabled { get; set; } = ModConfiguration.LoggingEnabledDefault;
}
