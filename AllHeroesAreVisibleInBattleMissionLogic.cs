using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        public static AllHeroesAreVisibleInBattleMissionLogic? Instance { get; set; }

        public MissionMode MissionMode;
        public List<Agent> MissionCompanionAgents;
        public List<Agent> MissionFriendlyLordAgents;
        public List<Agent> MissionEnemyLordAgents;
        public List<Agent> MissionFormationCaptainAgents;
        public Agent? MissionArmyGeneralAgent;
        private readonly TournamentBehavior? _tournamentBehavior;
        private readonly TournamentMatch? _currentMatch;

        public AllHeroesAreVisibleInBattleMissionLogic()
        {
            AllHeroesAreVisibleInBattleMissionLogic.Instance = this;
            MissionMode = MissionMode.Battle;
            MissionFriendlyLordAgents = new List<Agent>();
            MissionCompanionAgents = new List<Agent>();
            MissionEnemyLordAgents = new List<Agent>();
            MissionFormationCaptainAgents = new List<Agent>();
            MissionArmyGeneralAgent = null;
            _tournamentBehavior = null;
            _currentMatch = null;
        }

        public override void OnDeploymentFinished()
        {
            base.OnDeploymentFinished();
            
            this.GetAllNemesisFromBattle();
            this.GetAllMissionCompanions();
            this.GetAllMissionFriendlyLords();
            this.GetAllFormationCaptainsAndGeneral();
        }

        public override void OnClearScene()
        {
            this.MissionEnemyLordAgents.Clear();
            this.MissionCompanionAgents.Clear();
            this.MissionFriendlyLordAgents.Clear();
            this.MissionFormationCaptainAgents.Clear();
            this.MissionArmyGeneralAgent = null;
        }

        private const float RescanInterval = 20f;
        private float _rescanTimer = 0f;
        private bool firstScanFlag = true;

        public override void OnMissionTick(float dt)
        {
            base.OnMissionTick(dt);

            if (MissionMode != MissionMode.Battle)
                return;

            if (firstScanFlag) {
                _rescanTimer = RescanInterval - 1;
                firstScanFlag = false;
            }

                _rescanTimer += dt;

            if (_rescanTimer < RescanInterval)
                return;

            _rescanTimer = 0f;

            OnClearScene();

            this.GetAllNemesisFromBattle();
            this.GetAllMissionCompanions();
            this.GetAllMissionFriendlyLords();
            this.GetAllFormationCaptainsAndGeneral();
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

        public void GetAllFormationCaptainsAndGeneral()
        {
            if (Mission.Current == null)
                return;

            this.MissionFormationCaptainAgents.Clear();

            // Try to get IB_Core's captain lists via reflection (ImmersiveBattlefields mod)
            // IB_CaptainList[0] = PlayerTeam, [1] = EnemyTeam, [2] = PlayerAllyTeam
            List<Agent>[]? ibCaptainList = null;

            foreach (MissionLogic logic in Mission.Current.MissionLogics)
            {
                if (logic.GetType().Name == "IB_Core")
                {
                    FieldInfo field = logic.GetType().GetField(
                        "IB_CaptainList",
                        BindingFlags.NonPublic | BindingFlags.Instance);

                    if (field != null)
                        ibCaptainList = field.GetValue(logic) as List<Agent>[];

                    break;
                }
            }

            // Indices 0 (PlayerTeam) and 2 (PlayerAllyTeam) are friendly
            int[] friendlyIndices = new int[] { 0, 2 };

            if (ibCaptainList != null)
            {
                foreach (int idx in friendlyIndices)
                {
                    if (idx >= ibCaptainList.Length || ibCaptainList[idx] == null)
                        continue;

                    foreach (Agent captain in ibCaptainList[idx])
                    {
                        if (captain == null || !captain.IsActive())
                            continue;

                        if (captain == Agent.Main)
                            continue;

                        if (this.MissionCompanionAgents.Contains(captain))
                            continue;

                        if (this.MissionFriendlyLordAgents.Contains(captain))
                            continue;

                        if (!this.MissionFormationCaptainAgents.Contains(captain))
                            this.MissionFormationCaptainAgents.Add(captain);
                    }
                }
            }
            else
            {
                // ImmersiveBattlefields not present — fall back to scanning formations for hero agents
                var teamsToScan = new List<Team>();
                if (Mission.Current.PlayerTeam != null)
                    teamsToScan.Add(Mission.Current.PlayerTeam);
                if (Mission.Current.PlayerAllyTeam != null)
                    teamsToScan.Add(Mission.Current.PlayerAllyTeam);

                foreach (Team team in teamsToScan)
                {
                    foreach (Formation formation in team.FormationsIncludingEmpty)
                    {
                        if (formation.CountOfUnits == 0)
                            continue;

                        Agent captain = formation.Captain;

                        if (captain == null || !captain.IsActive())
                            captain = formation.GetUnitsWithoutDetachedOnes()
                                .FirstOrDefault(a => a != null && a.IsHero && a.IsActive());

                        if (captain == null || !captain.IsActive())
                            continue;

                        if (captain == Agent.Main)
                            continue;

                        if (this.MissionCompanionAgents.Contains(captain))
                            continue;

                        if (this.MissionFriendlyLordAgents.Contains(captain))
                            continue;

                        if (!this.MissionFormationCaptainAgents.Contains(captain))
                            this.MissionFormationCaptainAgents.Add(captain);
                    }
                }
            }

            // Army general — only if not the player
            if (Mission.Current.PlayerAllyTeam != null)
            {
                Agent general = Mission.Current.PlayerAllyTeam.GeneralAgent;

                if (general != null && general != Agent.Main && general.IsActive())
                {
                    if (this.MissionFriendlyLordAgents.Contains(general))
                        MissionFriendlyLordAgents.Remove(general);

                    this.MissionArmyGeneralAgent = general;
                }
                else
                    this.MissionArmyGeneralAgent = null;
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

            for (int l = 0; l < this.MissionFormationCaptainAgents.Count; l++)
            {
                if (this.MissionFormationCaptainAgents[l] == affectedAgent)
                {
                    this.MissionFormationCaptainAgents.RemoveAt(l);
                    break;
                }
            }

            if (this.MissionArmyGeneralAgent == affectedAgent)
                this.MissionArmyGeneralAgent = null;
        }
    }

    internal class Helpers
    {
        public static Hero? GetAgentHero(Agent agent)
        {
            Hero? hero = (agent.Character is CharacterObject characterObject) ? characterObject.HeroObject : null;
            return hero ?? null;
        }
    }
}