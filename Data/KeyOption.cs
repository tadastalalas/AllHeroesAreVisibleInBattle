using TaleWorlds.InputSystem;
using TaleWorlds.Localization;

namespace AllHeroesAreVisibleInBattle.Data;

// MCM dropdown item. Public because MCM shows it through reflection (ToString).
public sealed class KeyOption
{
    private readonly TextObject _text;

    internal InputKey Key { get; }

    // text: a Texts.Ids const, or a plain label that is the same in every language (Q, F1, Tab).
    internal KeyOption(InputKey key, string text)
    {
        Key = key;
        _text = new TextObject(text);
    }

    public override string ToString() => _text.ToString();
}
