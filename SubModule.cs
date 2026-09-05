using AllHeroesAreVisibleInBattle.Settings;
using TaleWorlds.MountAndBlade;

namespace AllHeroesAreVisibleInBattle
{
    public class SubModule : MBSubModuleBase
    {
           protected override void OnBeforeInitialModuleScreenSetAsRoot()
           {
                   base.OnBeforeInitialModuleScreenSetAsRoot();
                   ModSettings.Initialize();
           }
        
        public override void OnMissionBehaviorInitialize(Mission mission)
        {
            base.OnMissionBehaviorInitialize(mission);

            if (mission == null)
                return;

            mission.AddMissionBehavior(new AllHeroesAreVisibleInBattleMissionLogic());
        }
    }
}