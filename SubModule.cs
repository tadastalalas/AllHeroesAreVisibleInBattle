using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.View;
using System;
using System.Runtime.CompilerServices;

namespace AllHeroesAreVisibleInBattle
{
    public class SubModule : MBSubModuleBase
    {
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
        }

        protected override void OnSubModuleUnloaded()
        {
            base.OnSubModuleUnloaded();
        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            base.OnBeforeInitialModuleScreenSetAsRoot();
        }

        public override void OnMissionBehaviorInitialize(Mission mission)
        {
            if (mission != null)
            {
                base.OnMissionBehaviorInitialize(mission);
                mission.AddMissionBehavior(new AllHeroesAreVisibleInBattleMissionView());
                mission.AddMissionBehavior(new AllHeroesAreVisibleInBattleMissionLogic());
            }
        }
    }
}