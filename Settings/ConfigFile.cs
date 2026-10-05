using System;
using System.IO;
using System.Xml.Linq;
using TaleWorlds.InputSystem;


namespace AllHeroesAreVisibleInBattle.Settings
{
    internal static class ConfigFile
    {
        internal static SettingsValues? Read()
        {
            string? path = GetPath();

            if (path == null || !File.Exists(path))
                return null;

            try
            {
                XElement? root = XDocument.Load(path).Root;

                if (root == null)
                    return null;

                SettingsValues defaults = SettingsValues.Default;

                return new SettingsValues
                {
                    Mode = ReadMode(root, defaults.Mode),
                    KeyboardKey = ReadKey(root, "KeyboardKey", defaults.KeyboardKey),
                    ControllerKey = ReadKey(root, "ControllerKey", defaults.ControllerKey),
                    HighlightDistance = ReadDistance(root, defaults.HighlightDistance),
                    ShowNames = ReadBool(root, "ShowNames", defaults.ShowNames)
                };
            }
            catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or System.Xml.XmlException)
            {
                return null;
            }
        }

        private static string? GetPath()
        {
            string? binaryFolder = Path.GetDirectoryName(typeof(ConfigFile).Assembly.Location);

            // <Module>/bin/<Platform>/*.dll -> <Module>/Config.xml
            return string.IsNullOrEmpty(binaryFolder) ? null : Path.GetFullPath(Path.Combine(binaryFolder, "..", "..", "Config.xml"));
        }

        private static HighlightModeType ReadMode(XElement root, HighlightModeType fallback)
        {
            string? value = root.Element("HighlightMode")?.Value.Trim();

            if (string.IsNullOrEmpty(value))
                return fallback;

            return string.Equals(value, "Toggle", StringComparison.OrdinalIgnoreCase) ? HighlightModeType.Toggle : HighlightModeType.Hold;
        }

        private static InputKey ReadKey(XElement root, string elementName, InputKey fallback)
        {
            string? value = root.Element(elementName)?.Value.Trim();

            if (string.IsNullOrEmpty(value))
                return fallback;

            if (string.Equals(value, "None", StringComparison.OrdinalIgnoreCase))
                return InputKey.Invalid;

            return Enum.TryParse(value, true, out InputKey key) && Enum.IsDefined(typeof(InputKey), key) ? key : fallback;
        }

        private static float ReadDistance(XElement root, float fallback)
        {
            string? value = root.Element("HighlightDistance")?.Value.Trim();

            if (string.IsNullOrEmpty(value) || !float.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float distance))
                return fallback;

            return TaleWorlds.Library.MathF.Min(TaleWorlds.Library.MathF.Max(distance, 10f), 200f);
        }
        
        private static bool ReadBool(XElement root, string elementName, bool fallback)
        {
            string? value = root.Element(elementName)?.Value.Trim();
            return string.IsNullOrEmpty(value) || !bool.TryParse(value, out bool result) ? fallback : result;
        }
    }
}