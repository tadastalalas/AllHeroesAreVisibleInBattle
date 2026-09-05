using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using TaleWorlds.InputSystem;

namespace AllHeroesAreVisibleInBattle.Settings
{
    internal static class McmBridge
    {
        private static bool _checked;
        private static bool _isLoaded;

        internal static bool IsLoaded
        {
            get
            {
                if (!_checked)
                {
                    _isLoaded = Detect();
                    _checked = true;
                }

                return _isLoaded;
            }
        }

        private static bool Detect()
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            for (int i = 0; i < assemblies.Length; i++)
            {
                string? name = assemblies[i].GetName().Name;

                if (name == null)
                    continue;

                if (name.StartsWith("MCMv5", StringComparison.OrdinalIgnoreCase) || name.StartsWith("Bannerlord.MCM", StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }

        // NoInlining keeps MCM types out of the caller's JIT scope so the mod still loads when MCM is absent.
        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static bool TryRead(out SettingsValues values)
        {
            McmSettings? settings = McmSettings.Instance;

            if (settings == null)
            {
                values = SettingsValues.Default;
                return false;
            }

            values = new SettingsValues
            {
                Mode = settings.HighlightMode?.SelectedIndex == 1 ? HighlightModeType.Toggle : HighlightModeType.Hold,
                KeyboardKey = settings.KeyboardKey?.SelectedValue?.Key ?? InputKey.Invalid,
                ControllerKey = settings.ControllerKey?.SelectedValue?.Key ?? InputKey.Invalid,
                HighlightDistance = settings.HighlightDistance
            };

            return true;
        }
    }
}