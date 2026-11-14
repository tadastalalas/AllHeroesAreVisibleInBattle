using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using AllHeroesAreVisibleInBattle.Data;
using AllHeroesAreVisibleInBattle.Diagnostics;
using TaleWorlds.InputSystem;

namespace AllHeroesAreVisibleInBattle.Core;

// The only place mod code reads settings from. MCM loaded -> MCM. MCM not loaded -> ModConfiguration.xml.
internal static class ModSettings
{
    private const string McmAssemblyName = "MCMv5";

    private static ModConfiguration _xml = new();
    private static XmlSerializer? _serializer;
    private static bool _useMcm;

    // The xml stores names; they are parsed once per Reload. In MCM mode they hold the defaults (fallback values).
    private static HighlightModeType _xmlHighlightMode;
    private static InputKey _xmlKeyboardKey;
    private static InputKey _xmlControllerKey;

    internal static HighlightModeType HighlightMode => _useMcm ? McmHighlightMode() : _xmlHighlightMode;

    internal static InputKey KeyboardKey => _useMcm ? McmKeyboardKey() : _xmlKeyboardKey;

    internal static InputKey ControllerKey => _useMcm ? McmControllerKey() : _xmlControllerKey;

    internal static int HighlightDistance => _useMcm ? McmHighlightDistance() : _xml.General.HighlightDistance;

    internal static bool ShowNames => _useMcm ? McmShowNames() : _xml.General.ShowNames;

    internal static bool LoggingEnabled => _useMcm ? McmLoggingEnabled() : _xml.TechnicalSettings.LoggingEnabled;

    private static XmlSerializer Serializer => _serializer ??= new XmlSerializer(typeof(ModConfiguration));

    // Called on every game start.
    internal static void Reload()
    {
        _useMcm = false;
        _xml = new ModConfiguration();

        if (IsMcmLoaded())
        {
            if (McmHasSettings())
                _useMcm = true;
            else
                AhavibLog.Warn("MCM is loaded but returned no settings for this mod. Using " + ModInfo.ConfigurationFileName + " instead.");
        }

        if (!_useMcm)
            _xml = LoadFromFile();

        ParseXmlNames();
        AhavibLog.Debug("Settings read from " + (_useMcm ? "MCM" : ModInfo.ConfigurationFileName) + ".");
    }

    private static bool IsMcmLoaded()
    {
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        for (int i = 0; i < assemblies.Length; i++)
        {
            if (assemblies[i].GetName().Name == McmAssemblyName) return true;
        }

        return false;
    }

    // Every method that touches MCMSettings must be NoInlining and may only be called when MCM is loaded.
    // Otherwise the game tries to load MCM's classes and crashes when MCM is not installed.
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static bool McmHasSettings() => MCMSettings.Instance != null;

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static HighlightModeType McmHighlightMode() => MCMSettings.Instance?.HighlightMode?.SelectedValue?.Mode ?? _xmlHighlightMode;

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static InputKey McmKeyboardKey() => MCMSettings.Instance?.KeyboardKey?.SelectedValue?.Key ?? _xmlKeyboardKey;

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static InputKey McmControllerKey() => MCMSettings.Instance?.ControllerKey?.SelectedValue?.Key ?? _xmlControllerKey;

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int McmHighlightDistance() => MCMSettings.Instance?.HighlightDistance ?? ModConfiguration.HighlightDistanceDefault;

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static bool McmShowNames() => MCMSettings.Instance?.ShowNames ?? ModConfiguration.ShowNamesDefault;

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static bool McmLoggingEnabled() => MCMSettings.Instance?.LoggingEnabled ?? ModConfiguration.LoggingEnabledDefault;

    // Unknown names fall back to the default in memory only; the file keeps what the player typed.
    private static void ParseXmlNames()
    {
        ModConfiguration.GeneralSection general = _xml.General;

        if (!HighlightModeOption.TryParse(general.HighlightMode, out _xmlHighlightMode))
        {
            WarnUnknownValue(nameof(general.HighlightMode), general.HighlightMode, ModConfiguration.HighlightModeDefault);
            HighlightModeOption.TryParse(ModConfiguration.HighlightModeDefault, out _xmlHighlightMode);
        }

        _xmlKeyboardKey = ParseKey(nameof(general.KeyboardKey), general.KeyboardKey, ModConfiguration.KeyboardKeyDefault);
        _xmlControllerKey = ParseKey(nameof(general.ControllerKey), general.ControllerKey, ModConfiguration.ControllerKeyDefault);
    }

    private static InputKey ParseKey(string settingName, string? value, string defaultName)
    {
        if (KeyOptions.TryParse(value, out InputKey key))
            return key;

        WarnUnknownValue(settingName, value, defaultName);
        KeyOptions.TryParse(defaultName, out key);
        return key;
    }

    private static void WarnUnknownValue(string settingName, string? value, string defaultName) =>
        AhavibLog.Warn(ModInfo.ConfigurationFileName + ": unknown " + settingName + " value '" + value + "'. Using the default " + defaultName + ".");

    private static ModConfiguration LoadFromFile()
    {
        string path = Path.Combine(ModInfo.ModuleRoot, ModInfo.ConfigurationFileName);
        ModConfiguration configuration = new();

        if (!File.Exists(path))
        {
            AhavibLog.Warn(ModInfo.ConfigurationFileName + " was missing. Created it with the default settings.");
            TryWrite(path, configuration);
            return configuration;
        }

        try
        {
            configuration = Read(path);
            if (!HasEverySetting(path, configuration))
            {
                AhavibLog.Warn(ModInfo.ConfigurationFileName + " had missing settings. Added them with their default values.");
                TryWrite(path, configuration);
            }
        }
        catch (Exception ex) when (ex is InvalidOperationException or XmlException)
        {
            AhavibLog.Warn(ModInfo.ConfigurationFileName + " was broken and was replaced with the default settings. Reason: " + (ex.InnerException?.Message ?? ex.Message));
            configuration = new ModConfiguration();
            TryWrite(path, configuration);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            AhavibLog.Warn(ModInfo.ConfigurationFileName + " could not be opened. Using the default settings. Reason: " + ex.Message);
            configuration = new ModConfiguration();
        }

        // Out-of-range values are corrected in memory only; the file keeps what the player typed.
        configuration.Clamp();
        return configuration;
    }

    private static ModConfiguration Read(string path)
    {
        using FileStream stream = File.OpenRead(path);
        return Serializer.Deserialize(stream) as ModConfiguration ?? new ModConfiguration();
    }

    private static void TryWrite(string path, ModConfiguration configuration)
    {
        try
        {
            XmlWriterSettings writerSettings = new() { Indent = true, Encoding = Encoding.UTF8 };
            using XmlWriter writer = XmlWriter.Create(path, writerSettings);
            Serializer.Serialize(writer, configuration, NoNamespaces());
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            AhavibLog.Warn(ModInfo.ConfigurationFileName + " could not be written. The settings are kept in memory for this session. Reason: " + ex.Message);
        }
    }

    private static bool HasEverySetting(string path, ModConfiguration configuration)
    {
        XDocument complete = new();
        using (XmlWriter writer = complete.CreateWriter())
        {
            Serializer.Serialize(writer, configuration, NoNamespaces());
        }

        XElement? fileRoot = XDocument.Load(path).Root;
        return fileRoot != null && complete.Root != null && ContainsEveryElement(fileRoot, complete.Root);
    }

    private static bool ContainsEveryElement(XElement actual, XElement expected)
    {
        foreach (XElement expectedChild in expected.Elements())
        {
            XElement? actualChild = actual.Element(expectedChild.Name);
            if (actualChild == null || !ContainsEveryElement(actualChild, expectedChild)) return false;
        }

        return true;
    }

    private static XmlSerializerNamespaces NoNamespaces()
    {
        XmlSerializerNamespaces namespaces = new();
        namespaces.Add(string.Empty, string.Empty);
        return namespaces;
    }
}
