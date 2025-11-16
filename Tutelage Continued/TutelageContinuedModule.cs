using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace TutelageContinued
{
    public class TutelageContinuedModule : MBSubModuleBase
    {
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            InformationManager.DisplayMessage(
                new InformationMessage("Tutelage Continued Mod loaded successfully."));

        }

        protected override void OnSubModuleUnloaded()
        {
            base.OnSubModuleUnloaded();
        }

        protected override void OnGameStart(Game game, IGameStarter starter)
        {
            base.OnGameStart(game, starter);
            if (starter is CampaignGameStarter campaignStarter)
            {
                // Add our behavior
                campaignStarter.AddBehavior(new TutelageContinuedBehavior());
            }
        }

    }
}
