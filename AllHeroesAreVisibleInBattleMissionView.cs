using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.Engine.GauntletUI;
using AllHeroesAreVisibleInBattle.UI;
using AllHeroesAreVisibleInBattle.Settings;

namespace AllHeroesAreVisibleInBattle
{
    [DefaultView]
    public class AllHeroesAreVisibleInBattleMissionView : MissionView
    {
        private static readonly uint CompanionColor    = new Color(0f, 0.85f, 0.1f).ToUnsignedInteger();
        private static readonly uint FriendlyLordColor = new Color(1f, 1f, 0f).ToUnsignedInteger();
        private static readonly uint EnemyLordColor    = new Color(1f, 0f, 0f).ToUnsignedInteger();
        private static readonly uint CaptainColor      = new Color(0.2f, 0.4f, 1f).ToUnsignedInteger();
        private static readonly uint GeneralColor      = new Color(0f, 1f, 1f).ToUnsignedInteger();

        private const float RefreshInterval = 0.25f;

        private readonly List<Agent> _contouredAgents = [];
        private readonly List<Agent> _desiredAgents = [];
        private readonly List<uint> _desiredColors = [];
        private bool _highlightActive;
        private bool _toggledOn;
        private float _refreshTimer;
        private const string CompanionHex = "#00D91AFF", FriendlyLordHex = "#FFFF00FF", EnemyLordHex = "#FF0000FF", CaptainHex = "#3366FFFF", GeneralHex = "#00FFFFFF";
        private readonly List<string> _desiredColorHex = [];
        private GauntletLayer? _layer;
        private GauntletMovieIdentifier? _movie;
        private HeroNameLabelsVM? _labelsVM;
        
        public override void OnMissionScreenInitialize()
        {
            base.OnMissionScreenInitialize();
            _labelsVM = new HeroNameLabelsVM();
            _layer = new GauntletLayer("AHAVIBHeroNameLabels", 1);
            _movie = _layer.LoadMovie("AHAVIB_HeroNameLabels", _labelsVM);
            MissionScreen.AddLayer(_layer);
        }
        
        public override void OnMissionScreenTick(float dt)
        {
            base.OnMissionScreenTick(dt);

            if (Mission == null || Mission.Mode != MissionMode.Battle)
            {
                _toggledOn = false;

                if (_highlightActive)
                    ClearContours();

                return;
            }

            SettingsValues settings = ModSettings.Current;
            bool shouldHighlight;

            if (settings.Mode == HighlightModeType.Toggle)
            {
                if (IsKeyPressed(settings.KeyboardKey) || IsKeyPressed(settings.ControllerKey))
                    _toggledOn = !_toggledOn;

                shouldHighlight = _toggledOn;
            }
            else
            {
                shouldHighlight = IsKeyDown(settings.KeyboardKey) || IsKeyDown(settings.ControllerKey);
            }

            if (!shouldHighlight)
            {
                if (_highlightActive)
                    ClearContours();

                return;
            }

            _refreshTimer -= dt;

            if (!_highlightActive || _refreshTimer <= 0f)
            {
                _refreshTimer = RefreshInterval;
                RefreshContours(settings.HighlightDistance * settings.HighlightDistance);
                _highlightActive = true;
            }

            if (_labelsVM == null || _layer == null)
                return;

            if (!settings.ShowNames)
            {
                if (_labelsVM.IsEnabled)
                    _labelsVM.Clear();

                return;
            }

            _labelsVM.IsEnabled = true;
            _labelsVM.UpdatePositions(MissionScreen.CombatCamera);
        }

        private static bool IsKeyDown(InputKey key) => key != InputKey.Invalid && TaleWorlds.InputSystem.Input.IsKeyDown(key);

        private static bool IsKeyPressed(InputKey key) => key != InputKey.Invalid && TaleWorlds.InputSystem.Input.IsKeyPressed(key);
        
        private void RefreshContours(float distanceSquared)
        {
            AllHeroesAreVisibleInBattleMissionLogic? logic = AllHeroesAreVisibleInBattleMissionLogic.Instance;

            _desiredAgents.Clear();
            _desiredColors.Clear();
            _desiredColorHex.Clear();

            if (logic != null)
            {
                Vec3? origin = GetHighlightOrigin();
                CollectAgents(logic.MissionCompanionAgents, CompanionColor, origin, distanceSquared);
                CollectAgents(logic.MissionFriendlyLordAgents, FriendlyLordColor, origin, distanceSquared);
                CollectAgents(logic.MissionEnemyLordAgents, EnemyLordColor, origin, distanceSquared);
                CollectAgents(logic.MissionFormationCaptainAgents, CaptainColor, origin, distanceSquared);
                CollectAgent(logic.MissionArmyGeneralAgent, GeneralColor, origin, distanceSquared);
            }

            for (int i = _contouredAgents.Count - 1; i >= 0; i--)
            {
                Agent agent = _contouredAgents[i];

                if (_desiredAgents.Contains(agent))
                    continue;

                SetContour(agent, null);
                _contouredAgents.RemoveAt(i);
            }

            for (int i = 0; i < _desiredAgents.Count; i++)
            {
                Agent agent = _desiredAgents[i];
                SetContour(agent, _desiredColors[i]);

                if (!_contouredAgents.Contains(agent))
                    _contouredAgents.Add(agent);
            }
            
            if (ModSettings.Current.ShowNames)
                _labelsVM?.Sync(_desiredAgents, _desiredColorHex);
        }

        // The render camera position works with any camera mod (RTS Camera, photo mode, spectator).
        private Vec3? GetHighlightOrigin()
        {
            Scene? scene = Mission?.Scene;

            if (scene != null)
                return scene.LastFinalRenderCameraPosition;

            return Agent.Main != null ? Agent.Main.Position : null;
        }

        private void CollectAgents(List<Agent> agents, uint color, Vec3? origin, float distanceSquared)
        {
            for (int i = 0; i < agents.Count; i++)
                CollectAgent(agents[i], color, origin, distanceSquared);
        }

        private void CollectAgent(Agent? agent, uint color, Vec3? origin, float distanceSquared)
        {
            if (agent == null || agent == Agent.Main || !agent.IsActive())
                return;

            if (origin.HasValue && agent.Position.DistanceSquared(origin.Value) > distanceSquared)
                return;

            if (_desiredAgents.Contains(agent))
                return;

            _desiredAgents.Add(agent);
            _desiredColors.Add(color);
            _desiredColorHex.Add(ColorHex(color));
        }
        
        private static string ColorHex(uint c) =>
            c == CompanionColor ? CompanionHex :
            c == FriendlyLordColor ? FriendlyLordHex :
            c == EnemyLordColor ? EnemyLordHex :
            c == CaptainColor ? CaptainHex : GeneralHex;

        private static void SetContour(Agent agent, uint? color)
        {
            MBAgentVisuals visuals = agent.AgentVisuals;

            if (visuals != null && visuals.IsValid())
                visuals.SetContourColor(color);
        }

        private void ClearContours()
        {
            for (int i = 0; i < _contouredAgents.Count; i++)
                SetContour(_contouredAgents[i], null);

            _labelsVM?.Clear();
            _contouredAgents.Clear();
            _highlightActive = false;
            _toggledOn = false;
            _refreshTimer = 0f;
        }

        public override void OnMissionModeChange(MissionMode oldMissionMode, bool atStart)
        {
            base.OnMissionModeChange(oldMissionMode, atStart);

            if (oldMissionMode == MissionMode.Battle)
                ClearContours();
        }

        public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow blow)
        {
            base.OnAgentRemoved(affectedAgent, affectorAgent, agentState, blow);

            if (affectedAgent == null)
                return;

            int index = _contouredAgents.IndexOf(affectedAgent);

            if (index < 0)
                return;

            SetContour(affectedAgent, null);   // no IsActive() gate — State is already Killed/Unconscious here
            _contouredAgents.RemoveAt(index);
        }

        public override void OnMissionScreenFinalize()
        {
            ClearContours();
            if (_layer != null)
            {
                if (_movie != null) _layer.ReleaseMovie(_movie);
                MissionScreen.RemoveLayer(_layer);
            }
            _labelsVM?.OnFinalize();
            _layer = null; _movie = null; _labelsVM = null;
            base.OnMissionScreenFinalize();
        }
    }
}