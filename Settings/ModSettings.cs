namespace AllHeroesAreVisibleInBattle.Settings
{
    internal static class ModSettings
    {
        private static bool _initialized;
        private static bool _useMcm;
        private static SettingsValues _fallback = SettingsValues.Default;

        internal static void Initialize()
        {
            if (_initialized)
                return;

            _initialized = true;
            _fallback = ConfigFile.Read() ?? SettingsValues.Default;
            _useMcm = McmBridge.IsLoaded;
        }

        internal static SettingsValues Current => _useMcm && McmBridge.TryRead(out SettingsValues values) ? values : _fallback;
    }
}