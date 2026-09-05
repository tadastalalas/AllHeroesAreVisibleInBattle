using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.Global;
using MCM.Common;
using TaleWorlds.InputSystem;

namespace AllHeroesAreVisibleInBattle.Settings
{
    // Public because MCM discovers and instantiates settings types via reflection.
    public sealed class McmSettings : AttributeGlobalSettings<McmSettings>
    {
        public override string Id => "AllHeroesAreVisibleInBattle";
        public override string DisplayName => "All Heroes Are Visible In Battle";
        public override string FolderName => "AllHeroesAreVisibleInBattle";
        public override string FormatType => "json2";

        [SettingPropertyGroup("General")]
        [SettingPropertyDropdown("Highlight Mode", Order = 0, RequireRestart = false, HintText = "Hold: highlights only while the key is held. Toggle: press once to turn on, press again to turn off.")]
        public Dropdown<string> HighlightMode { get; set; } = new Dropdown<string>(["Hold", "Toggle"], 0);

        [SettingPropertyGroup("General")]
        [SettingPropertyDropdown("Keyboard Key", Order = 1, RequireRestart = false, HintText = "Keyboard / mouse key that triggers the highlight. Works at the same time as the controller button.")]
        public Dropdown<KeyOption> KeyboardKey { get; set; } = new Dropdown<KeyOption>(KeyOptions.Keyboard, KeyOptions.IndexOf(KeyOptions.Keyboard, InputKey.LeftAlt));

        [SettingPropertyGroup("General")]
        [SettingPropertyDropdown("Controller Button", Order = 2, RequireRestart = false, HintText = "Controller button that triggers the highlight. Default is None to avoid clashing with vanilla combat bindings.")]
        public Dropdown<KeyOption> ControllerKey { get; set; } = new Dropdown<KeyOption>(KeyOptions.Controller, 0);

        [SettingPropertyGroup("General")]
        [SettingPropertyInteger("Highlight Distance", 10, 200, "0 m", Order = 3, RequireRestart = false, HintText = "Heroes further than this from the camera are not outlined. Default 30.")]
        public int HighlightDistance { get; set; } = 30;
    }
}