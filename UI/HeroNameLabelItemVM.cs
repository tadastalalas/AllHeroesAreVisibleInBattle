using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace AllHeroesAreVisibleInBattle.UI
{
    internal sealed class HeroNameLabelItemVM : ViewModel
    {
        // Must match SuggestedWidth/Height in the prefab (unscaled units).
        private const float HalfWidth = 200f, Height = 40f;

        internal readonly Agent Agent;
        private string _name, _color;
        private float _x, _y;
        private bool _isVisible;

        internal HeroNameLabelItemVM(Agent agent, string color)
        {
            Agent = agent;
            _name = agent.Name;
            _color = color;
        }

        internal void UpdatePosition(Camera camera)
        {
            if (!Agent.IsActive()) { IsVisible = false; return; }

            Vec3 pos = Agent.Position;
            pos.z = Agent.GetEyeGlobalPosition().z + (Agent.HasMount ? 0.8f : 0.6f);
            float x = 0f, y = 0f, w = 0f;
            MBWindowManager.WorldToScreen(camera, pos, ref x, ref y, ref w);

            if (w <= 0f) { IsVisible = false; return; }

            ScreenPositionX = x - HalfWidth;
            ScreenPositionY = y - Height;
            IsVisible = true;
        }

        [DataSourceProperty] public string Name { get => _name; set { if (value != _name) { _name = value; OnPropertyChangedWithValue(value, nameof(Name)); } } }
        [DataSourceProperty] public string Color { get => _color; set { if (value != _color) { _color = value; OnPropertyChangedWithValue(value, nameof(Color)); } } }
        [DataSourceProperty] public float ScreenPositionX { get => _x; set { if (value != _x) { _x = value; OnPropertyChangedWithValue(value, nameof(ScreenPositionX)); } } }
        [DataSourceProperty] public float ScreenPositionY { get => _y; set { if (value != _y) { _y = value; OnPropertyChangedWithValue(value, nameof(ScreenPositionY)); } } }
        [DataSourceProperty] public bool IsVisible { get => _isVisible; set { if (value != _isVisible) { _isVisible = value; OnPropertyChangedWithValue(value, nameof(IsVisible)); } } }
    }
}