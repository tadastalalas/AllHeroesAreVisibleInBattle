using TaleWorlds.InputSystem;

namespace AllHeroesAreVisibleInBattle.Settings
{
    public sealed class KeyOption
    {
        private readonly string _displayName;

        internal InputKey Key { get; }

        internal KeyOption(InputKey key, string displayName)
        {
            Key = key;
            _displayName = displayName;
        }

        public override string ToString() => _displayName;
    }

    // MCM persists the dropdown SelectedIndex, not the value.
    // These lists are APPEND-ONLY - never reorder or remove entries, or existing binds will shift.
    internal static class KeyOptions
    {
        internal static readonly KeyOption[] Keyboard =
        [
            new KeyOption(InputKey.Invalid, "None"),
            new KeyOption(InputKey.LeftAlt, "Left Alt"),
            new KeyOption(InputKey.RightAlt, "Right Alt"),
            new KeyOption(InputKey.LeftControl, "Left Ctrl"),
            new KeyOption(InputKey.RightControl, "Right Ctrl"),
            new KeyOption(InputKey.LeftShift, "Left Shift"),
            new KeyOption(InputKey.RightShift, "Right Shift"),
            new KeyOption(InputKey.Tab, "Tab"),
            new KeyOption(InputKey.CapsLock, "Caps Lock"),
            new KeyOption(InputKey.Tilde, "Tilde (~)"),
            new KeyOption(InputKey.Space, "Space"),
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
            new KeyOption(InputKey.MiddleMouseButton, "Middle Mouse Button"),
            new KeyOption(InputKey.X1MouseButton, "Mouse Button 4"),
            new KeyOption(InputKey.X2MouseButton, "Mouse Button 5"),
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
            new KeyOption(InputKey.Invalid, "None"),
            new KeyOption(InputKey.ControllerLBumper, "Left Bumper (LB)"),
            new KeyOption(InputKey.ControllerRBumper, "Right Bumper (RB)"),
            new KeyOption(InputKey.ControllerLTrigger, "Left Trigger (LT)"),
            new KeyOption(InputKey.ControllerRTrigger, "Right Trigger (RT)"),
            new KeyOption(InputKey.ControllerLThumb, "Left Stick Click (L3)"),
            new KeyOption(InputKey.ControllerRThumb, "Right Stick Click (R3)"),
            new KeyOption(InputKey.ControllerLOption, "View / Back"),
            new KeyOption(InputKey.ControllerROption, "Menu / Start"),
            new KeyOption(InputKey.ControllerLUp, "D-Pad Up"),
            new KeyOption(InputKey.ControllerLDown, "D-Pad Down"),
            new KeyOption(InputKey.ControllerLLeft, "D-Pad Left"),
            new KeyOption(InputKey.ControllerLRight, "D-Pad Right"),
            new KeyOption(InputKey.ControllerRUp, "Face Up (Y)"),
            new KeyOption(InputKey.ControllerRDown, "Face Down (A)"),
            new KeyOption(InputKey.ControllerRLeft, "Face Left (X)"),
            new KeyOption(InputKey.ControllerRRight, "Face Right (B)")
        ];

        internal static int IndexOf(KeyOption[] options, InputKey key)
        {
            for (int i = 0; i < options.Length; i++)
            {
                if (options[i].Key == key)
                    return i;
            }

            return 0;
        }
    }
}