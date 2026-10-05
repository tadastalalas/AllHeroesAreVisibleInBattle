using TaleWorlds.InputSystem;

namespace AllHeroesAreVisibleInBattle.Settings
{
    internal enum HighlightModeType
    {
        Hold,
        Toggle
    }

    internal readonly struct SettingsValues
    {
        internal HighlightModeType Mode { get; init; }
        internal InputKey KeyboardKey { get; init; }
        internal InputKey ControllerKey { get; init; }
        internal float HighlightDistance { get; init; }
        internal bool ShowNames { get; init; }

        internal static SettingsValues Default => new SettingsValues
        {
            Mode = HighlightModeType.Hold,
            KeyboardKey = InputKey.LeftAlt,
            ControllerKey = InputKey.Invalid,
            HighlightDistance = 30f,
            ShowNames = true
        };
    }
}