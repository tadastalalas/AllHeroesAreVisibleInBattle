using AllHeroesAreVisibleInBattle.Core;
using AllHeroesAreVisibleInBattle.Missions;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace AllHeroesAreVisibleInBattle;

public class SubModule : MBSubModuleBase
{
    protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
    {
        base.OnGameStart(game, gameStarterObject);
        ModSettings.Reload();
    }

    public override void OnMissionBehaviorInitialize(Mission mission)
    {
        base.OnMissionBehaviorInitialize(mission);

        if (mission == null)
            return;

        mission.AddMissionBehavior(new AllHeroesAreVisibleInBattleMissionLogic());
    }
}
