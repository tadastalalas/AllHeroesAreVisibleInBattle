using System.Collections.Generic;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace AllHeroesAreVisibleInBattle.UI;

internal sealed class HeroNameLabelsVM : ViewModel
{
    private readonly MBBindingList<HeroNameLabelItemVM> _labels = [];
    private bool _isEnabled;

    internal void Sync(List<Agent> agents, List<string> colors)
    {
        for (int i = _labels.Count - 1; i >= 0; i--)
            if (!agents.Contains(_labels[i].Agent)) _labels.RemoveAt(i);

        for (int i = 0; i < agents.Count; i++)
        {
            HeroNameLabelItemVM? item = Find(agents[i]);
            if (item == null) _labels.Add(new HeroNameLabelItemVM(agents[i], colors[i]));
            else item.Color = colors[i];
        }
    }

    internal void UpdatePositions(Camera camera)
    {
        for (int i = 0; i < _labels.Count; i++)
            _labels[i].UpdatePosition(camera);
    }

    internal void Clear() { IsEnabled = false; _labels.Clear(); }

    private HeroNameLabelItemVM? Find(Agent agent)
    {
        for (int i = 0; i < _labels.Count; i++)
            if (_labels[i].Agent == agent) return _labels[i];
        return null;
    }

    [DataSourceProperty] public MBBindingList<HeroNameLabelItemVM> Labels => _labels;
    [DataSourceProperty] public bool IsEnabled { get => _isEnabled; set { if (value != _isEnabled) { _isEnabled = value; OnPropertyChangedWithValue(value, nameof(IsEnabled)); } } }
}
