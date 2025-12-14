using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;

namespace TownOfSushi.Roles.Impostor
{
    public static class WizardEvents
    {
        [RegisterEvent]
        public static void RoundStartEventHandler(RoundStartEvent @event)
        {
            foreach (var witch in CustomRoleUtils.GetActiveRolesOfType<WizardRole>())
            {
                if (witch.Player.HasDied()) continue;

                foreach (var spelled in PlayerControl.AllPlayerControls.ToArray().Where(x => x.HasModifier<WizardSpelledModifier>()))
                {
                    if (spelled == null) 
                    {
                        continue;
                    }
                    if (spelled.GetModifier<WizardSpelledModifier>()?.WizardId != witch.Player.PlayerId) 
                    {
                        continue;
                    }
                    
                    WizardRole.RpcMurderSpelled(witch.Player, spelled);
                }
            }
        }
    }
}