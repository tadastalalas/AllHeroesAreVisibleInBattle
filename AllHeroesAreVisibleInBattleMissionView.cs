using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using SandBox.Tournaments.MissionLogics;
using SandBox.ViewModelCollection.Missions.MainAgentDetection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.TournamentGames;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;

namespace AllHeroesAreVisibleInBattle
{
    [DefaultView]
    public class AllHeroesAreVisibleInBattleMissionView : MissionView
    {
        private static readonly uint CompanionColor    = new Color(0f, 0.85f, 0.1f, 1f).ToUnsignedInteger();
        private static readonly uint FriendlyLordColor = new Color(1f, 1f, 0f, 1f).ToUnsignedInteger();
        private static readonly uint EnemyLordColor    = new Color(1f, 0f, 0f, 1f).ToUnsignedInteger();
        private static readonly uint CaptainColor      = new Color(0.2f, 0.4f, 1f, 1f).ToUnsignedInteger();
        private static readonly uint GeneralColor      = new Color(0f, 1f, 1f, 1f).ToUnsignedInteger();

        private const float HighlightDistanceSquared = 30f * 30f;
        private const float RefreshInterval = 0.25f;

        private readonly List<Agent> _contouredAgents = new List<Agent>();
        private readonly List<Agent> _desiredAgents = new List<Agent>();
        private readonly List<uint> _desiredColors = new List<uint>();
        private bool _highlightActive;
        private float _refreshTimer;
        
        public override void OnMissionScreenTick(float dt)
        {
            base.OnMissionScreenTick(dt);

            bool shouldHighlight = Mission != null
                                   && Mission.Mode == MissionMode.Battle
                                   && TaleWorlds.InputSystem.Input.IsKeyDown(InputKey.LeftAlt);

            if (!shouldHighlight)
            {
                if (_highlightActive)
                    ClearContours();

                return;
            }

            _refreshTimer -= dt;

            if (_highlightActive && _refreshTimer > 0f)
                return;

            _refreshTimer = RefreshInterval;
            RefreshContours();
            _highlightActive = true;
        }
        
        private void RefreshContours()
        {
            AllHeroesAreVisibleInBattleMissionLogic? logic = AllHeroesAreVisibleInBattleMissionLogic.Instance;

            _desiredAgents.Clear();
            _desiredColors.Clear();

            if (logic != null)
            {
                Vec3? origin = GetHighlightOrigin();
                CollectAgents(logic.MissionCompanionAgents, CompanionColor, origin);
                CollectAgents(logic.MissionFriendlyLordAgents, FriendlyLordColor, origin);
                CollectAgents(logic.MissionEnemyLordAgents, EnemyLordColor, origin);
                CollectAgents(logic.MissionFormationCaptainAgents, CaptainColor, origin);
                CollectAgent(logic.MissionArmyGeneralAgent, GeneralColor, origin);
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
        }

        // The render camera position works with any camera mod (RTS Camera, photo mode, spectator).
        private Vec3? GetHighlightOrigin()
        {
            Scene? scene = Mission?.Scene;

            if (scene != null)
                return scene.LastFinalRenderCameraPosition;

            return Agent.Main != null ? Agent.Main.Position : (Vec3?)null;
        }

        private void CollectAgents(List<Agent> agents, uint color, Vec3? origin)
        {
            for (int i = 0; i < agents.Count; i++)
                CollectAgent(agents[i], color, origin);
        }

        private void CollectAgent(Agent? agent, uint color, Vec3? origin)
        {
            if (agent == null || agent == Agent.Main || !agent.IsActive())
                return;

            if (origin.HasValue && agent.Position.DistanceSquared(origin.Value) > HighlightDistanceSquared)
                return;

            if (_desiredAgents.Contains(agent))
                return;

            _desiredAgents.Add(agent);
            _desiredColors.Add(color);
        }

        private static void SetContour(Agent agent, uint? color)
        {
            MBAgentVisuals visuals = agent.AgentVisuals;

            if (visuals != null && visuals.IsValid())
                visuals.SetContourColor(color, true);
        }

        private void ClearContours()
        {
            for (int i = 0; i < _contouredAgents.Count; i++)
                SetContour(_contouredAgents[i], null);

            _contouredAgents.Clear();
            _highlightActive = false;
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
            base.OnMissionScreenFinalize();
        }
    }
}