using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SandBox.Tournaments.MissionLogics;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.TournamentGames;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace AllHeroesAreVisibleInBattle
{
    internal class AllHeroesAreVisibleInBattleMissionLogic : MissionLogic
    {
        public static AllHeroesAreVisibleInBattleMissionLogic? Instance { get; set; }

        public readonly List<Agent> MissionCompanionAgents;
        public readonly List<Agent> MissionFriendlyLordAgents;
        public readonly List<Agent> MissionEnemyLordAgents;
        public readonly List<Agent> MissionFormationCaptainAgents;
        public Agent? MissionArmyGeneralAgent;
        private readonly TournamentBehavior? _tournamentBehavior;
        private readonly TournamentMatch? _currentMatch;

        public AllHeroesAreVisibleInBattleMissionLogic()
        {
            AllHeroesAreVisibleInBattleMissionLogic.Instance = this;
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
            
            if (Campaign.Current == null || GameNetwork.IsMultiplayer)
                return;
            
            this.GetAllNemesisFromBattle();
            this.GetAllMissionCompanions();
            this.GetAllMissionFriendlyLords();
            this.GetAllFormationCaptainsAndGeneral();
        }

        public override void OnClearScene() => ClearAgents();

        internal void ClearAgents()
        {
            MissionEnemyLordAgents.Clear();
            MissionCompanionAgents.Clear();
            MissionFriendlyLordAgents.Clear();
            MissionFormationCaptainAgents.Clear();
            MissionArmyGeneralAgent = null;
        }

        private const float RescanInterval = 20f;
        private float _rescanTimer;
        private bool _firstScanFlag = true;
        
        public override void OnMissionModeChange(MissionMode oldMissionMode, bool atStart)
        {
            base.OnMissionModeChange(oldMissionMode, atStart);

            if (Mission.Mode == MissionMode.Battle)
                _rescanTimer = RescanInterval;
        }

        public override void OnMissionTick(float dt)
        {
            base.OnMissionTick(dt);
            
            if (Campaign.Current == null || GameNetwork.IsMultiplayer)
                return;

            if (Mission.Mode != MissionMode.Battle)
                return;

            if (_firstScanFlag) {
                _rescanTimer = RescanInterval - 1;
                _firstScanFlag = false;
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
        
        public override void OnAgentBuild(Agent agent, Banner banner)
        {
            if (agent == null || !agent.IsHuman || agent.Character == null || !agent.Character.IsHero)
                return;

            _rescanTimer = RescanInterval;   // rescan on the next tick
        }

        public void GetAllNemesisFromBattle()
        {
            Team playerTeam = Mission.Current.PlayerTeam;

            if (playerTeam == null)
                return;

            for (int t = 0; t < Mission.Current.Teams.Count; t++)
            {
                Team team = Mission.Current.Teams[t];

                if (!team.IsEnemyOf(playerTeam))
                    continue;

                MBReadOnlyList<Agent> agents = team.ActiveAgents;

                for (int i = 0; i < agents.Count; i++)
                {
                    Agent agent = agents[i];

                    if (!agent.IsHuman || agent.Character == null || !agent.Character.IsHero)
                        continue;

                    if (Helpers.GetAgentHero(agent) != null && !MissionEnemyLordAgents.Contains(agent))
                        MissionEnemyLordAgents.Add(agent);
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
                if (logic.GetType().Name != "IB_Core")
                    continue;
                
                FieldInfo? field = logic.GetType().GetField("IB_CaptainList",BindingFlags.NonPublic | BindingFlags.Instance);

                if (field != null)
                    ibCaptainList = field.GetValue(logic) as List<Agent>[];

                break;
            }

            if (ibCaptainList != null)
            {
                // The list index is not a reliable team indicator, team membership is resolved per agent instead
                for (int i = 0; i < ibCaptainList.Length; i++)
                {
                    List<Agent> captains = ibCaptainList[i];

                    if (captains == null)
                        continue;

                    for (int j = 0; j < captains.Count; j++)
                    {
                        Agent captain = captains[j];

                        if (captain != null && captain.IsHero)
                            TryAddFormationCaptain(captain);
                    }
                }
            }
            else
            {
                // ImmersiveBattlefields not present — fall back to scanning formations for hero agents
                List<Team> teamsToScan = new List<Team>();

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

                        Agent? captain = formation.Captain;

                        if (captain == null || !captain.IsActive())
                            captain = formation.GetUnitsWithoutDetachedOnes()
                                .FirstOrDefault(a => a is { IsHero: true } && a.IsActive());

                        TryAddFormationCaptain(captain);
                    }
                }
            }

            // Army general — only if not the player and not hostile to the player
            if (Mission.Current.PlayerAllyTeam != null)
            {
                Agent general = Mission.Current.PlayerAllyTeam.GeneralAgent;

                if (general != null && general != Agent.Main && general.IsActive() && IsFriendlyToPlayer(general))
                {
                    this.MissionFriendlyLordAgents.Remove(general);
                    this.MissionArmyGeneralAgent = general;
                }
                else
                    this.MissionArmyGeneralAgent = null;
            }
        }

        private void TryAddFormationCaptain(Agent? captain)
        {
            if (captain == null || captain == Agent.Main || !captain.IsActive())
                return;

            // Hostiles must never get the captain colour, they belong in the enemy list
            if (!IsFriendlyToPlayer(captain))
            {
                if (IsEnemyOfPlayer(captain) && Helpers.GetAgentHero(captain) != null && !this.MissionEnemyLordAgents.Contains(captain))
                    this.MissionEnemyLordAgents.Add(captain);

                return;
            }

            if (this.MissionCompanionAgents.Contains(captain))
                return;

            if (this.MissionFriendlyLordAgents.Contains(captain))
                return;

            if (!this.MissionFormationCaptainAgents.Contains(captain))
                this.MissionFormationCaptainAgents.Add(captain);
        }

        private static bool IsFriendlyToPlayer(Agent agent)
        {
            Team? playerTeam = Mission.Current?.PlayerTeam;
            Team? agentTeam = agent.Team;

            return playerTeam != null && agentTeam != null && agentTeam.IsFriendOf(playerTeam);
        }

        private static bool IsEnemyOfPlayer(Agent agent)
        {
            Team? playerTeam = Mission.Current?.PlayerTeam;
            Team? agentTeam = agent.Team;

            return playerTeam != null && agentTeam != null && agentTeam.IsEnemyOf(playerTeam);
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