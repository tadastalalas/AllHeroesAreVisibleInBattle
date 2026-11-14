using System;
using AllHeroesAreVisibleInBattle.Core;
using TaleWorlds.Localization;

namespace AllHeroesAreVisibleInBattle.Data;

// MCM dropdown item. Public because MCM shows it through reflection (ToString).
public sealed class HighlightModeOption
{
    // MCM saves the dropdown index: APPEND-ONLY, never reorder or remove entries.
    internal static readonly HighlightModeOption[] All =
    [
        new HighlightModeOption(HighlightModeType.Hold, Texts.Ids.ModeHold),
        new HighlightModeOption(HighlightModeType.Toggle, Texts.Ids.ModeToggle)
    ];

    private readonly TextObject _text;

    internal HighlightModeType Mode { get; }

    private HighlightModeOption(HighlightModeType mode, string text)
    {
        Mode = mode;
        _text = new TextObject(text);
    }

    public override string ToString() => _text.ToString();

    // Xml value: Hold or Toggle, any case.
    internal static bool TryParse(string? value, out HighlightModeType mode) =>
        Enum.TryParse(value?.Trim(), true, out mode) && Enum.IsDefined(typeof(HighlightModeType), mode);

    internal static int IndexOf(string name)
    {
        if (!TryParse(name, out HighlightModeType mode))
            return 0;

        for (int i = 0; i < All.Length; i++)
        {
            if (All[i].Mode == mode)
                return i;
        }

        return 0;
    }
}
