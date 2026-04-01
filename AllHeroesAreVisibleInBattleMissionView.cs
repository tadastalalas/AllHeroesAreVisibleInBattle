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
        public static AllHeroesAreVisibleInBattleMissionView Instance { get; set; }

        private GauntletLayer _layer;
        private GauntletMovieIdentifier _movie;
        private Hero _hero = null;

        private bool _companionsHighlighted = false;
        private bool _friendlyLordsHighlighted = false;
        private bool _enemyLordsHighlighted = false;

        public AllHeroesAreVisibleInBattleMissionView()
        {
            AllHeroesAreVisibleInBattleMissionView.Instance = this;
        }

        public override void OnMissionScreenTick(float dt)
        {
            base.OnMissionScreenTick(dt);
            
            if (Mission.Mode == MissionMode.Battle)
            {
                if (Input.IsKeyPressed(InputKey.LeftAlt))
                {
                    HighlightCompanions(true);
                    HighlightFriendlyLords(true);
                    HighlightEnemyLords(true);
                }
                if (Input.IsKeyReleased(InputKey.LeftAlt))
                {
                    HighlightCompanions(false);
                    HighlightFriendlyLords(false);
                    HighlightEnemyLords(false);
                }
            }
        }

        private void HighlightCompanions(bool show = true)
        {
            if (show == this._companionsHighlighted)
                return;

            if (AllHeroesAreVisibleInBattleMissionLogic.Instance == null || 
                AllHeroesAreVisibleInBattleMissionLogic.Instance.MissionCompanionAgents == null || 
                AllHeroesAreVisibleInBattleMissionLogic.Instance.MissionCompanionAgents.Count == 0)
                return;

            uint companionContourColor = new Color(0f, 0.85f, 0.1f, 1f).ToUnsignedInteger();

            foreach (Agent agent in AllHeroesAreVisibleInBattleMissionLogic.Instance.MissionCompanionAgents)
            {
                if (agent != null && agent.IsActive())
                {
                    if (show)
                    {
                        if (Agent.Main != null && Agent.Main.IsActive())
                        {
                            if (Agent.Main.Position.Distance(agent.Position) < 30f)
                            {
                                MBAgentVisuals agentVisuals = agent.AgentVisuals;

                                if (agentVisuals != null)
                                    agentVisuals.SetContourColor(new uint?(companionContourColor), true);
                            }
                        }
                        else if (Agent.Main == null || !Agent.Main.IsActive())
                        {
                            MBAgentVisuals agentVisuals = agent.AgentVisuals;

                            if (agentVisuals != null)
                                agentVisuals.SetContourColor(new uint?(companionContourColor), true);
                        }
                    }
                    else if (agent != null)
                    {
                        MBAgentVisuals agentVisuals2 = agent.AgentVisuals;
                        if (agentVisuals2 != null)
                        {
                            agentVisuals2.SetContourColor(null, true);
                        }
                    }
                }
            }
            
            this._companionsHighlighted = show;
        }

        private void HighlightFriendlyLords(bool show = true)
        {
            if (show == this._friendlyLordsHighlighted)
                return;

            if (AllHeroesAreVisibleInBattleMissionLogic.Instance == null ||
                AllHeroesAreVisibleInBattleMissionLogic.Instance.MissionFriendlyLordAgents == null ||
                AllHeroesAreVisibleInBattleMissionLogic.Instance.MissionFriendlyLordAgents.Count == 0)
                return;

            uint focusedContourColor = new Color(1f, 1f, 0f, 1f).ToUnsignedInteger();

            foreach (Agent agent in AllHeroesAreVisibleInBattleMissionLogic.Instance.MissionFriendlyLordAgents)
            {
                if (agent != null && agent.IsActive())
                {
                    if (show)
                    {
                        if (Agent.Main != null && Agent.Main.IsActive())
                        {
                            if (Agent.Main.Position.Distance(agent.Position) < 30f)
                            {
                                MBAgentVisuals agentVisuals = agent.AgentVisuals;

                                if (agentVisuals != null)
                                    agentVisuals.SetContourColor(new uint?(focusedContourColor), true);
                            }
                        }
                        else if (Agent.Main == null || !Agent.Main.IsActive())
                        {
                            MBAgentVisuals agentVisuals = agent.AgentVisuals;

                            if (agentVisuals != null)
                                agentVisuals.SetContourColor(new uint?(focusedContourColor), true);
                        }
                    }
                    else if (agent != null)
                    {
                        MBAgentVisuals agentVisuals2 = agent.AgentVisuals;

                        if (agentVisuals2 != null)
                            agentVisuals2.SetContourColor(null, true);
                    }
                }
            }

            this._friendlyLordsHighlighted = show;
        }

        private void HighlightEnemyLords(bool show = true)
        {
            if (show == this._enemyLordsHighlighted)
                return;

            if (AllHeroesAreVisibleInBattleMissionLogic.Instance == null || 
                AllHeroesAreVisibleInBattleMissionLogic.Instance.MissionEnemyLordAgents == null || 
                AllHeroesAreVisibleInBattleMissionLogic.Instance.MissionEnemyLordAgents.Count == 0)
                return;

            uint focusedContourColor = new Color(1f, 0f, 0f, 1f).ToUnsignedInteger();

            foreach (Agent agent in AllHeroesAreVisibleInBattleMissionLogic.Instance.MissionEnemyLordAgents)
            {
                if (agent != null && agent.IsActive())
                {
                    if (show)
                    {
                        if (Agent.Main != null && Agent.Main.IsActive())
                        {
                            if (Agent.Main.Position.Distance(agent.Position) < 30f)
                            {
                                MBAgentVisuals agentVisuals = agent.AgentVisuals;

                                if (agentVisuals != null)
                                    agentVisuals.SetContourColor(new uint?(focusedContourColor), true);
                            }
                        }
                        else if (Agent.Main == null || !Agent.Main.IsActive())
                        {
                            MBAgentVisuals agentVisuals = agent.AgentVisuals;

                            if (agentVisuals != null)
                                agentVisuals.SetContourColor(new uint?(focusedContourColor), true);
                        }
                    }
                    else if (agent != null)
                    {
                        MBAgentVisuals agentVisuals2 = agent.AgentVisuals;

                        if (agentVisuals2 != null)
                            agentVisuals2.SetContourColor(null, true);
                    }
                }
            }

            this._enemyLordsHighlighted = show;
        }

        public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow blow)
        {
            bool flag = base.Mission != null && base.Mission.Mode == MissionMode.Battle;
            if (flag)
            {
                if (affectedAgent != null)
                {
                    MBAgentVisuals agentVisuals = affectedAgent.AgentVisuals;
                    if (agentVisuals != null)
                    {
                        agentVisuals.SetContourColor(null, true);
                    }
                }
            }
            base.OnAgentRemoved(affectedAgent, affectorAgent, agentState, blow);
        }
    }
}