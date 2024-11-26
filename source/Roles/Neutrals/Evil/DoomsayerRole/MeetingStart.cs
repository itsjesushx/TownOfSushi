using TownOfSushi.Roles.Crewmates.Support.ImitatorRole;

namespace TownOfSushi.Roles.Neutral.Evil.DoomsayerRole
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class MeetingStart
    {
        public static void Postfix(MeetingHud __instance)
        {
            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Doomsayer)) return;
            var doomsayerRole = GetRole<Doomsayer>(PlayerControl.LocalPlayer);
            if (doomsayerRole.LastObservedPlayer != null)
            {
                var playerResults = PlayerReportFeedback(doomsayerRole.LastObservedPlayer);
                var roleResults = RoleReportFeedback(doomsayerRole.LastObservedPlayer);

                if (!string.IsNullOrWhiteSpace(playerResults)) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, playerResults);
                if (!string.IsNullOrWhiteSpace(roleResults)) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, roleResults);
            }
        }

        public static string PlayerReportFeedback(PlayerControl player)
        {
            if (player.Is(RoleEnum.Imitator) || StartImitate.ImitatingPlayer == player
                || player.Is(RoleEnum.Morphling) || player.Is(RoleEnum.Mystic)
                  || player.Is(RoleEnum.Glitch))
                return $"You observe that {player.GetDefaultOutfit().PlayerName} has an altered perception of reality";
            else if (player.Is(RoleEnum.Blackmailer) ||player.Is(RoleEnum.Witch)|| player.Is(RoleEnum.Doomsayer)
                 || player.Is(RoleEnum.Oracle) || player.Is(RoleEnum.Snitch) || player.Is(RoleEnum.Trapper) || player.Is(RoleEnum.Agent))
                return $"You observe that {player.GetDefaultOutfit().PlayerName} has an insight for private information";
            else if (player.Is(RoleEnum.Amnesiac) || player.Is(RoleEnum.Janitor)
                 || player.Is(RoleEnum.Medium) || player.Is(RoleEnum.Undertaker) ||player.Is(RoleEnum.Hitman) || player.Is(RoleEnum.Vulture) || player.Is(RoleEnum.Vampire))
                return $"You observe that {player.GetDefaultOutfit().PlayerName} has an unusual obsession with dead bodies";
            else if (player.Is(RoleEnum.Investigator) || player.Is(RoleEnum.Swooper) || player.Is(RoleEnum.Tracker)
                || player.Is(RoleEnum.Venerer) || player.Is(RoleEnum.SerialKiller))
                return $"You observe that {player.GetDefaultOutfit().PlayerName} is well trained in hunting down prey";
            else if (player.Is(RoleEnum.Arsonist) || player.Is(RoleEnum.Miner) || player.Is(RoleEnum.Plaguebearer)
                  || player.Is(RoleEnum.Prosecutor) || player.Is(RoleEnum.Seer) || player.Is(RoleEnum.Transporter))
                return $"You observe that {player.GetDefaultOutfit().PlayerName} spreads fear amonst the group";
            else if (player.Is(RoleEnum.Engineer) || player.Is(RoleEnum.Escapist) || player.Is(RoleEnum.Grenadier)
                || player.Is(RoleEnum.GuardianAngel) || player.Is(RoleEnum.Medic)|| player.Is(RoleEnum.Romantic))
                return $"You observe that {player.GetDefaultOutfit().PlayerName} hides to guard themself or others";
            else if (player.Is(RoleEnum.Executioner) || player.Is(RoleEnum.Jester) || player.Is(RoleEnum.Mayor)
                 ||player.Is(RoleEnum.Veteran))
                return $"You observe that {player.GetDefaultOutfit().PlayerName} has a trick up their sleeve";
            else if (player.Is(RoleEnum.Bomber) || player.Is(RoleEnum.Juggernaut) || player.Is(RoleEnum.Pestilence)
                 || player.Is(RoleEnum.Vigilante) || player.Is(RoleEnum.Vigilante) || player.Is(RoleEnum.Warlock))
                return $"You observe that {player.GetDefaultOutfit().PlayerName} is capable of performing relentless attacks";
            else if (player.Is(RoleEnum.Crewmate) || player.Is(RoleEnum.Impostor))
                return $"You observe that {player.GetDefaultOutfit().PlayerName} appears to be roleless";
            else
                return ColorString(Colors.Impostor,"Error");
        }

        public static string RoleReportFeedback(PlayerControl player)
        {
            if (player.Is(RoleEnum.Imitator) || StartImitate.ImitatingPlayer == player
                || player.Is(RoleEnum.Morphling) || player.Is(RoleEnum.Mystic)
                || player.Is(RoleEnum.Glitch))
                return "(" + ColorString(Colors.Imitator,"Imitator") + ", " + ColorString(Colors.Impostor,"Morphling") +", "+ ColorString(Colors.Mystic,"Mystic") + " or " + ColorString(Colors.Glitch,"Glitch") + ")";
            
            else if (player.Is(RoleEnum.Blackmailer)|| player.Is(RoleEnum.Doomsayer)
                 || player.Is(RoleEnum.Oracle) || player.Is(RoleEnum.Snitch) ||player.Is(RoleEnum.Witch)|| player.Is(RoleEnum.Trapper) || player.Is(RoleEnum.Agent))
                return "(" + ColorString(Colors.Impostor,"Blackmailer") + ", " + ColorString(Colors.Agent,"Agent") + ", " + ColorString(Colors.Doomsayer,"Doomsayer") +", "+ ColorString(Colors.Oracle,"Oracle") + ", "+ ColorString(Colors.Impostor,"Witch ") + ", " + ColorString(Colors.Snitch,"Snitch") + " or " + ColorString(Colors.Trapper, "Trapper") + ")";
            
            else if (player.Is(RoleEnum.Amnesiac) || player.Is(RoleEnum.Janitor)
                 || player.Is(RoleEnum.Medium) || player.Is(RoleEnum.Undertaker) || player.Is(RoleEnum.Hitman) ||player.Is(RoleEnum.Vampire)|| player.Is(RoleEnum.Vulture) )
                return "("+ ColorString(Colors.Amnesiac,"Amnesiac") + ", " + ColorString(Colors.Vulture,"Vulture") + ", " + ColorString(Colors.Impostor,"Janitor") +", "+ ColorString(Colors.Medium,"Medium") + ", "+ ColorString(Colors.Impostor,"Undertaker ")+ ", "+ ColorString(Colors.Hitman,"Hitman ") + " or " + ColorString(Colors.Vampire,"Vampire") + ")";
            
            else if (player.Is(RoleEnum.Investigator) || player.Is(RoleEnum.Swooper) || player.Is(RoleEnum.Tracker)
                 || player.Is(RoleEnum.Venerer) || player.Is(RoleEnum.SerialKiller))
                return "(" + ColorString(Colors.Investigator,"Investigator") + ", " + ColorString(Colors.Impostor,"Swooper") + ", " + ColorString(Colors.Tracker,"Tracker") + ", "+ ColorString(Colors.Impostor,"Venerer ") + " or " + ColorString(Colors.SerialKiller,"SerialKiller") + ")";
            
            else if (player.Is(RoleEnum.Arsonist) || player.Is(RoleEnum.Miner) || player.Is(RoleEnum.Plaguebearer)
                  || player.Is(RoleEnum.Prosecutor) || player.Is(RoleEnum.Seer) || player.Is(RoleEnum.Transporter))
                return "(" + ColorString(Colors.Arsonist,"Arsonist") + ", " + ColorString(Colors.Impostor,"Miner") + ", " + ColorString(Colors.Plaguebearer,"Plaguebearer") +", "+ ColorString(Colors.Prosecutor,"Prosecutor") + ", "+ ColorString(Colors.Seer,"Seer ") + " or " + ColorString(Colors.Transporter,"Transporter") + ")";
            
            else if (player.Is(RoleEnum.Engineer) || player.Is(RoleEnum.Escapist) || player.Is(RoleEnum.Grenadier)
                || player.Is(RoleEnum.GuardianAngel) || player.Is(RoleEnum.Medic) || player.Is(RoleEnum.Romantic))
                return "(" + ColorString(Colors.Engineer,"Engineer") + ", " + ColorString(Colors.Impostor,"Escapist") + ", " + ColorString(Colors.Impostor,"Grenadier") +", " + ColorString(Colors.GuardianAngel,"Guardian Angel") + ", "+ ColorString(Colors.Medic,"Medic ") + " or " + ColorString(Colors.Romantic,"Romantic")+ ")";
            
            else if (player.Is(RoleEnum.Executioner) || player.Is(RoleEnum.Jester) || player.Is(RoleEnum.Mayor)
                 ||player.Is(RoleEnum.Veteran))
                return "(" + ColorString(Colors.Doomsayer,"Doomsayer") + ", " + ColorString(Colors.Jester,"Jester") + ", " + ColorString(Colors.Mayor,"Mayor") + " or " + ColorString(Colors.Veteran,"Veteran")+ ")";
            
            else if (player.Is(RoleEnum.Bomber) || player.Is(RoleEnum.Juggernaut) || player.Is(RoleEnum.Pestilence)
                 || player.Is(RoleEnum.Vigilante) || player.Is(RoleEnum.Vigilante) || player.Is(RoleEnum.Warlock))
                return "(" + ColorString(Colors.Impostor,"Bomber") + ", " + ColorString(Colors.Juggernaut,"Juggernaut") + ", " + ColorString(Colors.Pestilence,"Pestilence") +", "+ ColorString(Colors.Vigilante,"Vigilante") + ", "+ ColorString(Colors.Vigilante,"Vigilante ") + " or " + ColorString(Colors.Impostor,"Warlock")+ ")";
            
            else if (player.Is(RoleEnum.Crewmate) || player.Is(RoleEnum.Impostor))
                return "(" + ColorString(Colors.Impostor,"Impostor") + ", " + ColorString(Colors.Crewmate,"Crewmate") + ")";
            
            else return ColorString(Colors.Impostor,"Error");
        }
    }
}