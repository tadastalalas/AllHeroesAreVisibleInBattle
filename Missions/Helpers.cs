using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace AllHeroesAreVisibleInBattle.Missions;

internal class Helpers
{
    public static Hero? GetAgentHero(Agent agent)
    {
        Hero? hero = (agent.Character is CharacterObject characterObject) ? characterObject.HeroObject : null;
        return hero ?? null;
    }

    internal static bool IsCombatMode(MissionMode mode) => mode == MissionMode.Battle || mode == MissionMode.Stealth;
}
