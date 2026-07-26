using TaleWorlds.MountAndBlade;

namespace AllHeroesAreVisibleInBattle
{
    public class SubModule : MBSubModuleBase
    {
        public override void OnMissionBehaviorInitialize(Mission mission)
        {
            base.OnMissionBehaviorInitialize(mission);

            if (mission == null)
                return;

            mission.AddMissionBehavior(new AllHeroesAreVisibleInBattleMissionLogic());
        }
    }
}