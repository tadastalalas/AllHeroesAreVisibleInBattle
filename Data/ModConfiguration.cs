using System;
using TaleWorlds.InputSystem;

namespace AllHeroesAreVisibleInBattle.Data;

// Shape of ModConfiguration.xml and the only place where defaults and ranges live (MCMSettings uses them too).
public class ModConfiguration
{
    public const string HighlightModeDefault = nameof(HighlightModeType.Hold);
    public const string KeyboardKeyDefault = nameof(InputKey.LeftAlt);
    public const string ControllerKeyDefault = KeyOptions.NoneName;
    public const int HighlightDistanceDefault = 30;
    public const int HighlightDistanceMin = 10;
    public const int HighlightDistanceMax = 200;
    public const bool ShowNamesDefault = true;
    public const bool LoggingEnabledDefault = false;

    public GeneralSection General { get; set; } = new();

    // Always the last section.
    public TechnicalSettingsSection TechnicalSettings { get; set; } = new();

    internal void Clamp() => General.HighlightDistance = Math.Min(Math.Max(General.HighlightDistance, HighlightDistanceMin), HighlightDistanceMax);

    public class GeneralSection
    {
        // Hold or Toggle.
        public string HighlightMode { get; set; } = HighlightModeDefault;

        // InputKey names (see KeyOptions) or None.
        public string KeyboardKey { get; set; } = KeyboardKeyDefault;
        public string ControllerKey { get; set; } = ControllerKeyDefault;

        // Metres from the camera.
        public int HighlightDistance { get; set; } = HighlightDistanceDefault;
        public bool ShowNames { get; set; } = ShowNamesDefault;
    }

    public class TechnicalSettingsSection
    {
        // Always the last entry.
        public bool LoggingEnabled { get; set; } = LoggingEnabledDefault;
    }
}
