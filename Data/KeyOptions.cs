using System;
using AllHeroesAreVisibleInBattle.Core;
using TaleWorlds.InputSystem;

namespace AllHeroesAreVisibleInBattle.Data;

// MCM persists the dropdown SelectedIndex, not the value.
// These lists are APPEND-ONLY - never reorder or remove entries, or existing binds will shift.
internal static class KeyOptions
{
    // Xml name for "no key" (InputKey.Invalid).
    internal const string NoneName = "None";

    internal static readonly KeyOption[] Keyboard =
    [
        new KeyOption(InputKey.Invalid, Texts.Ids.KeyNone),
        new KeyOption(InputKey.LeftAlt, Texts.Ids.KeyLeftAlt),
        new KeyOption(InputKey.RightAlt, Texts.Ids.KeyRightAlt),
        new KeyOption(InputKey.LeftControl, Texts.Ids.KeyLeftCtrl),
        new KeyOption(InputKey.RightControl, Texts.Ids.KeyRightCtrl),
        new KeyOption(InputKey.LeftShift, Texts.Ids.KeyLeftShift),
        new KeyOption(InputKey.RightShift, Texts.Ids.KeyRightShift),
        new KeyOption(InputKey.Tab, "Tab"),
        new KeyOption(InputKey.CapsLock, Texts.Ids.KeyCapsLock),
        new KeyOption(InputKey.Tilde, Texts.Ids.KeyTilde),
        new KeyOption(InputKey.Space, Texts.Ids.KeySpace),
        new KeyOption(InputKey.Q, "Q"),
        new KeyOption(InputKey.E, "E"),
        new KeyOption(InputKey.R, "R"),
        new KeyOption(InputKey.T, "T"),
        new KeyOption(InputKey.F, "F"),
        new KeyOption(InputKey.G, "G"),
        new KeyOption(InputKey.H, "H"),
        new KeyOption(InputKey.Z, "Z"),
        new KeyOption(InputKey.X, "X"),
        new KeyOption(InputKey.C, "C"),
        new KeyOption(InputKey.V, "V"),
        new KeyOption(InputKey.B, "B"),
        new KeyOption(InputKey.N, "N"),
        new KeyOption(InputKey.M, "M"),
        new KeyOption(InputKey.MiddleMouseButton, Texts.Ids.KeyMiddleMouse),
        new KeyOption(InputKey.X1MouseButton, Texts.Ids.KeyMouse4),
        new KeyOption(InputKey.X2MouseButton, Texts.Ids.KeyMouse5),
        new KeyOption(InputKey.F1, "F1"),
        new KeyOption(InputKey.F2, "F2"),
        new KeyOption(InputKey.F3, "F3"),
        new KeyOption(InputKey.F4, "F4"),
        new KeyOption(InputKey.F5, "F5"),
        new KeyOption(InputKey.F6, "F6"),
        new KeyOption(InputKey.F7, "F7"),
        new KeyOption(InputKey.F8, "F8"),
        new KeyOption(InputKey.F9, "F9"),
        new KeyOption(InputKey.F10, "F10"),
        new KeyOption(InputKey.F11, "F11"),
        new KeyOption(InputKey.F12, "F12")
    ];

    internal static readonly KeyOption[] Controller =
    [
        new KeyOption(InputKey.Invalid, Texts.Ids.KeyNone),
        new KeyOption(InputKey.ControllerLBumper, Texts.Ids.PadLeftBumper),
        new KeyOption(InputKey.ControllerRBumper, Texts.Ids.PadRightBumper),
        new KeyOption(InputKey.ControllerLTrigger, Texts.Ids.PadLeftTrigger),
        new KeyOption(InputKey.ControllerRTrigger, Texts.Ids.PadRightTrigger),
        new KeyOption(InputKey.ControllerLThumb, Texts.Ids.PadLeftStick),
        new KeyOption(InputKey.ControllerRThumb, Texts.Ids.PadRightStick),
        new KeyOption(InputKey.ControllerLOption, Texts.Ids.PadView),
        new KeyOption(InputKey.ControllerROption, Texts.Ids.PadMenu),
        new KeyOption(InputKey.ControllerLUp, Texts.Ids.PadDPadUp),
        new KeyOption(InputKey.ControllerLDown, Texts.Ids.PadDPadDown),
        new KeyOption(InputKey.ControllerLLeft, Texts.Ids.PadDPadLeft),
        new KeyOption(InputKey.ControllerLRight, Texts.Ids.PadDPadRight),
        new KeyOption(InputKey.ControllerRUp, Texts.Ids.PadFaceUp),
        new KeyOption(InputKey.ControllerRDown, Texts.Ids.PadFaceDown),
        new KeyOption(InputKey.ControllerRLeft, Texts.Ids.PadFaceLeft),
        new KeyOption(InputKey.ControllerRRight, Texts.Ids.PadFaceRight)
    ];

    // Xml value: an InputKey name (any case) or None.
    internal static bool TryParse(string? value, out InputKey key)
    {
        string? trimmed = value?.Trim();

        if (string.Equals(trimmed, NoneName, StringComparison.OrdinalIgnoreCase))
        {
            key = InputKey.Invalid;
            return true;
        }

        return Enum.TryParse(trimmed, true, out key) && Enum.IsDefined(typeof(InputKey), key);
    }

    internal static int IndexOf(KeyOption[] options, string name)
    {
        if (!TryParse(name, out InputKey key))
            return 0;

        for (int i = 0; i < options.Length; i++)
        {
            if (options[i].Key == key)
                return i;
        }

        return 0;
    }
}
