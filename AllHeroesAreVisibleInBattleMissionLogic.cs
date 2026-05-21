using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SandBox.Tournaments.MissionLogics;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.AgentOrigins;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.TournamentGames;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace AllHeroesAreVisibleInBattle
{
    internal class AllHeroesAreVisibleInBattleMissionLogic : MissionLogic
    {
        public static AllHeroesAreVisibleInBattleMissionLogic Instance
        {
            get; set;
        }

        public MissionMode MissionMode;
        public List<Agent> MissionCompanionAgents;
        public List<Agent> MissionFriendlyLordAgents;
        public List<Agent> MissionEnemyLordAgents;
        private TournamentBehavior _tournamentBehavior;
        private TournamentMatch _currentMatch;

        public AllHeroesAreVisibleInBattleMissionLogic()
        {
            AllHeroesAreVisibleInBattleMissionLogic.Instance = this;
            MissionMode = MissionMode.Battle;
            MissionFriendlyLordAgents = new List<Agent>();
            MissionCompanionAgents = new List<Agent>();
            MissionEnemyLordAgents = new List<Agent>();
            _tournamentBehavior = null;
            _currentMatch = null;
        }

        public override void OnDeploymentFinished()
        {
            base.OnDeploymentFinished();
            
            this.GetAllNemesisFromBattle();
            this.GetAllMissionCompanions();
            this.GetAllMissionFriendlyLords();
        }

        public override void OnClearScene()
        {
            this.MissionEnemyLordAgents.Clear();
            this.MissionCompanionAgents.Clear();
            this.MissionFriendlyLordAgents.Clear();
        }

        private const float RescanInterval = 20f;
        private float _rescanTimer = 0f;

        public override void OnMissionTick(float dt)
        {
            base.OnMissionTick(dt);

            _rescanTimer += dt;

            if (_rescanTimer < RescanInterval)
                return;

            _rescanTimer = 0f;

            this.GetAllNemesisFromBattle();
            this.GetAllMissionCompanions();
            this.GetAllMissionFriendlyLords();
        }

        public void GetAllNemesisFromBattle()
        {
            if (Mission.Current == null)
                return;

            if (Mission.Current.PlayerEnemyTeam == null)
                return;

            foreach (Agent agent in Mission.Current.PlayerEnemyTeam.ActiveAgents)
            {
                if (agent.IsHuman && agent.Character != null && agent.Character.IsHero)
                {
                    Hero? hero = Helpers.GetAgentHero(agent);
                    
                    if (hero != null && !this.MissionEnemyLordAgents.Contains(agent))
                    {
                        this.MissionEnemyLordAgents.Add(agent);
                    }
                }
            }
        }

        public void GetAllMissionCompanions()
        {
            if (Mission.Current == null)
                return;

            if (Mission.Current.PlayerTeam == null)
                return;

            foreach (Agent agent in Mission.Current.PlayerTeam.ActiveAgents)
            {
                if (agent.IsHuman && agent.Character != null && agent.Character.IsHero && agent != Agent.Main)
                {
                    Hero? hero = Helpers.GetAgentHero(agent);

                    bool isCompanion = hero != null && hero.IsPlayerCompanion && (hero.Clan == null || hero.Clan.Leader != hero);
                    bool isPlayerClanMember = hero != null && Hero.MainHero != null && hero.Clan == Hero.MainHero.Clan;

                    if ((isCompanion || isPlayerClanMember) && !this.MissionCompanionAgents.Contains(agent))
                    {
                        MissionCompanionAgents.Add(agent);
                    }
                }
            }
        }

        public void GetAllMissionFriendlyLords()
        {
            if (Mission.Current == null)
                return;

            if (Mission.Current.PlayerAllyTeam == null && Mission.Current.PlayerTeam == null)
                return;

            if (Mission.Current.PlayerAllyTeam != null)
            {
                foreach (Agent agent in Mission.Current.PlayerAllyTeam.ActiveAgents)
                {
                    if (agent == null || agent == Agent.Main || !agent.IsHuman)
                        continue;

                    if (agent.Character != null && agent.Character.IsHero)
                    {
                        Hero? hero = Helpers.GetAgentHero(agent);

                        if (hero == null)
                            continue;

                        bool isPlayerClanMember = Hero.MainHero != null && hero.Clan == Hero.MainHero.Clan;

                        if (!hero.IsPlayerCompanion && !isPlayerClanMember && !this.MissionFriendlyLordAgents.Contains(agent))
                        {
                            MissionFriendlyLordAgents.Add(agent);
                        }
                    }
                }
            }

            if (Mission.Current.PlayerTeam != null)
            {
                foreach (Agent agent in Mission.Current.PlayerTeam.ActiveAgents)
                {
                    if (agent == null || agent == Agent.Main || !agent.IsHuman)
                        continue;

                    if (agent.Character != null && agent.Character.IsHero)
                    {
                        Hero? hero = Helpers.GetAgentHero(agent);

                        if (hero == null)
                            continue;

                        bool isPlayerClanMember = Hero.MainHero != null && hero.Clan == Hero.MainHero.Clan;

                        if (!hero.IsPlayerCompanion && !isPlayerClanMember && !this.MissionFriendlyLordAgents.Contains(agent))
                        {
                            MissionFriendlyLordAgents.Add(agent);
                        }
                    }
                }
            }
        }

        public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow killingBlow)
        {
            for (int i = 0; i < this.MissionEnemyLordAgents.Count; i++)
            {
                if (this.MissionEnemyLordAgents[i] == affectedAgent)
                {
                    this.MissionEnemyLordAgents.RemoveAt(i);
                    break;
                }
            }

            for (int j = 0; j < this.MissionCompanionAgents.Count; j++)
            {
                if (this.MissionCompanionAgents[j] == affectedAgent)
                {
                    this.MissionCompanionAgents.RemoveAt(j);
                    break;
                }
            }

            for (int k = 0; k < this.MissionFriendlyLordAgents.Count; k++)
            {
                if (this.MissionFriendlyLordAgents[k] == affectedAgent)
                {
                    this.MissionFriendlyLordAgents.RemoveAt(k);
                    break;
                }
            }
        }
    }

    internal class Helpers
    {
        public static Hero? GetAgentHero(Agent agent)
        {
            Hero? hero = (agent.Character is CharacterObject characterObject) ? characterObject.HeroObject : null;
            return hero == null ? null : hero;
        }
    }
}