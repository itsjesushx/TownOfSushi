namespace TownOfSushi.Modules
{
    [HarmonyPatch]
    public static class ChatCommands
    {
        public static bool JailorMessage = false;

        [HarmonyPatch(typeof(ChatController), nameof(ChatController.AddChat))]
        public static class PrivateJaileeChat
        {
            public static bool Prefix(ChatController __instance, [HarmonyArgument(0)] ref PlayerControl sourcePlayer, ref string chatText)
            {
                if (sourcePlayer == PlayerControl.LocalPlayer)
                {
                    if (chatText.ToLower().StartsWith("/crew") || chatText.ToLower().StartsWith("/ crew") 
                    || chatText.ToLower().StartsWith("/ crewmate") || chatText.ToLower().StartsWith("/ crewmate"))
                    {
                        AddRoleMessage(RoleEnum.Crewmate);
                        return false;
                    }
                    else if (chatText.ToLower().StartsWith("/imp") || chatText.ToLower().StartsWith("/ imp") 
                    || chatText.ToLower().StartsWith("/ impostor") || chatText.ToLower().StartsWith("/ impostor"))
                    {
                        AddRoleMessage(RoleEnum.Impostor);
                        return false;
                    }
                    else if (chatText.ToLower().StartsWith("/engi") || chatText.ToLower().StartsWith("/ engi"))
                    {
                        AddRoleMessage(RoleEnum.Engineer);
                        return false;
                    }
                    else if (chatText.ToLower().StartsWith("/rom") || chatText.ToLower().StartsWith("/ rom") 
                    || chatText.ToLower().StartsWith("/ romantic") || chatText.ToLower().StartsWith("/ romantic"))
                    {
                        AddRoleMessage(RoleEnum.Romantic);
                        return false;
                    }
                    else if (chatText.ToLower().StartsWith("/invest") || chatText.ToLower().StartsWith("/investigator") ||
                     chatText.ToLower().StartsWith("/ invest") || chatText.ToLower().StartsWith("/ investigator") )
                    { 
                        AddRoleMessage(RoleEnum.Investigator);
                        return false;
                    }
                    else if (chatText.ToLower().StartsWith("/medic") || chatText.ToLower().StartsWith("/ medic"))
                    {
                        AddRoleMessage(RoleEnum.Medic);
                        return false;
                    }
                    else if (chatText.ToLower().StartsWith("/swap") || chatText.ToLower().StartsWith("/ swap") || 
                    chatText.ToLower().StartsWith("/swapper") ||chatText.ToLower().StartsWith("/ swapper"))
                    {
                        AddRoleMessage(RoleEnum.Swapper);
                        return false;
                    }
                    else if (chatText.ToLower().StartsWith("/seer") || chatText.ToLower().StartsWith("/ seer"))
                    {
                        AddRoleMessage(RoleEnum.Seer);
                        return false;
                    }
                    else if (chatText.ToLower().StartsWith("/vig") || chatText.ToLower().StartsWith("/ vig") 
                    || chatText.ToLower().StartsWith("/ vigilante") || chatText.ToLower().StartsWith("/vigilante"))
                    {
                        AddRoleMessage(RoleEnum.Vigilante);
                        return false;
                    }
                    else if (chatText.ToLower().StartsWith("/hunt") || chatText.ToLower().StartsWith("/ hunt") 
                    || chatText.ToLower().StartsWith("/hunter") || chatText.ToLower().StartsWith("/ hunter"))
                    {
                        AddRoleMessage(RoleEnum.Hunter);
                        return false;
                    }
                    else if (chatText.ToLower().StartsWith("/arso") || chatText.ToLower().StartsWith("/ arso"))
                    {
                        AddRoleMessage(RoleEnum.Arsonist);
                        return false;
                    }
                    else if (chatText.ToLower().StartsWith("/exe") || chatText.ToLower().StartsWith("/ exe"))
                    {
                        AddRoleMessage(RoleEnum.Executioner);
                        return false;
                    }
                    else if (chatText.ToLower().StartsWith("/glitch") || chatText.ToLower().StartsWith("/ glitch"))
                    {
                        AddRoleMessage(RoleEnum.Glitch);
                        return false;
                    }
                    else if (chatText.ToLower().StartsWith("/jester") || chatText.ToLower().StartsWith("/ jest"))
                    {
                        AddRoleMessage(RoleEnum.Jester);
                        return false;
                    }
                    else if (chatText.ToLower().StartsWith("/grenadier") || chatText.ToLower().StartsWith("/gren"))
                    {
                        AddRoleMessage(RoleEnum.Grenadier);
                        return false;
                    }
                    else if (chatText.ToLower().StartsWith("/janitor") || chatText.ToLower().StartsWith("/jan"))
                    {
                        AddRoleMessage(RoleEnum.Janitor);
                        return false;
                    }
                    else if (chatText.ToLower().StartsWith("/mini") || chatText.ToLower().StartsWith("/ mini"))
                    {
                        AddModifierMessage(ModifierEnum.Mini);
                        return false;
                    }
                    else if (chatText.ToLower().StartsWith("/miner") || chatText.ToLower().StartsWith("/ miner"))
                    {
                        AddRoleMessage(RoleEnum.Miner);
                        return false;
                    }
                    else if (chatText.ToLower().StartsWith("/morph") || chatText.ToLower().StartsWith("/ morph"))
                    {
                        AddRoleMessage(RoleEnum.Morphling);
                        return false;
                    }
                    else if (chatText.ToLower().StartsWith("/swoop") || chatText.ToLower().StartsWith("/ swoop"))
                    {
                        AddRoleMessage(RoleEnum.Swooper);
                        return false;
                    }
                    else if (chatText.ToLower().StartsWith("/utaker") || chatText.ToLower().StartsWith("/ utaker") || 
                        chatText.ToLower().StartsWith("/undertaker") || chatText.ToLower().StartsWith("/ undertaker"))
                    {
                        AddRoleMessage(RoleEnum.Undertaker);
                        return false;
                    }
                    else if (chatText.ToLower().StartsWith("/vet") || chatText.ToLower().StartsWith("/ vet"))
                    {
                        AddRoleMessage(RoleEnum.Veteran);
                        return false;
                    }
                    else if (chatText.ToLower().StartsWith("/amne") || chatText.ToLower().StartsWith("/ amne"))
                    {
                        AddRoleMessage(RoleEnum.Amnesiac);
                        return false;
                    }
                    else if (chatText.ToLower().StartsWith("/jugg") || chatText.ToLower().StartsWith("/ jugg"))
                    {
                        AddRoleMessage(RoleEnum.Juggernaut);
                        return false;
                    }
                    else if (chatText.ToLower().StartsWith("/track") || chatText.ToLower().StartsWith("/ track"))
                    {
                        AddRoleMessage(RoleEnum.Tracker);
                        return false;
                    }
                    else if (chatText.ToLower().StartsWith("/med") || chatText.ToLower().StartsWith("/ med"))
                    {
                        AddRoleMessage(RoleEnum.Medium);
                        return false;
                    }
                    else if (chatText.ToLower().StartsWith("/trap") || chatText.ToLower().StartsWith("/ trap"))
                    {
                        AddRoleMessage(RoleEnum.Trapper);
                        return false;
                    }
                    else if (chatText.ToLower().StartsWith("/ga") || chatText.ToLower().StartsWith("/ ga") ||
                        chatText.ToLower().StartsWith("/guardian") || chatText.ToLower().StartsWith("/ guardian"))
                    {
                        AddRoleMessage(RoleEnum.GuardianAngel);
                        return false;
                    }
                    else if (chatText.ToLower().StartsWith("/myst") || chatText.ToLower().StartsWith("/ myst"))
                    {
                        AddRoleMessage(RoleEnum.Mystic);
                        return false;
                    }
                    else if (chatText.ToLower().StartsWith("/bmer") || chatText.ToLower().StartsWith("/ bmer") ||
                        chatText.ToLower().StartsWith("/black") || chatText.ToLower().StartsWith("/ black"))
                    {
                        AddRoleMessage(RoleEnum.Blackmailer);
                        return false;
                    }
                    else if (chatText.ToLower().StartsWith("/pb") || chatText.ToLower().StartsWith("/ pb") ||
                        chatText.ToLower().StartsWith("/plague") || chatText.ToLower().StartsWith("/ plague"))
                    {
                        AddRoleMessage(RoleEnum.Plaguebearer);
                        return false;
                    }
                    else if (chatText.ToLower().StartsWith("/pest") || chatText.ToLower().StartsWith("/ pest"))
                    {
                        AddRoleMessage(RoleEnum.Pestilence);
                        return false;
                    }
                    else if (chatText.ToLower().StartsWith("/sk") || chatText.ToLower().StartsWith("/ sk") ||
                        chatText.ToLower().StartsWith("/serial killer") || chatText.ToLower().StartsWith("/serialkiller"))
                    {
                        AddRoleMessage(RoleEnum.SerialKiller);
                        return false;
                    }
                    else if (chatText.ToLower().StartsWith("/detec") || chatText.ToLower().StartsWith("/ detec"))
                    {
                        AddRoleMessage(RoleEnum.Detective);
                        return false;
                    }
                    else if (chatText.ToLower().StartsWith("/escap") || chatText.ToLower().StartsWith("/ escap"))
                    {
                        AddRoleMessage(RoleEnum.Escapist);
                        return false;
                    }
                    else if (chatText.ToLower().StartsWith("/imitat") || chatText.ToLower().StartsWith("/ imitat"))
                    {
                        AddRoleMessage(RoleEnum.Imitator);
                        return false;
                    }
                    else if (chatText.ToLower().StartsWith("/bomb") || chatText.ToLower().StartsWith("/ bomb"))
                    {
                        AddRoleMessage(RoleEnum.Bomber);
                        return false;
                    }
                    else if (chatText.ToLower().StartsWith("/doom") || chatText.ToLower().StartsWith("/ doom"))
                    {
                        AddRoleMessage(RoleEnum.Doomsayer);
                        return false;
                    }
                    else if (chatText.ToLower().StartsWith("/vamp") || chatText.ToLower().StartsWith("/ vamp"))
                    {
                        AddRoleMessage(RoleEnum.Vampire);
                        return false;
                    }
                    else if (chatText.ToLower().StartsWith("/agent") || chatText.ToLower().StartsWith("/ agent"))
                    {
                        AddRoleMessage(RoleEnum.Agent);
                        return false;
                    }
                    else if (chatText.ToLower().StartsWith("/warlock") || chatText.ToLower().StartsWith("/war") 
                    || chatText.ToLower().StartsWith("/ warlock") ||  chatText.ToLower().StartsWith("/ war") )
                    {
                        AddRoleMessage(RoleEnum.Warlock);
                        return false;
                    }
                    else if (chatText.ToLower().StartsWith("/poisoner") || chatText.ToLower().StartsWith("/poison") 
                    || chatText.ToLower().StartsWith("/ poisoner") ||  chatText.ToLower().StartsWith("/ poison") )
                    {
                        AddRoleMessage(RoleEnum.Warlock);
                        return false;
                    }
                    else if (chatText.ToLower().StartsWith("/vult") || chatText.ToLower().StartsWith("/vulture"))
                    {
                        AddRoleMessage(RoleEnum.Vulture);
                        return false;
                    }
                    else if (chatText.ToLower().StartsWith("/hitman") || chatText.ToLower().StartsWith("/hit") 
                    || chatText.ToLower().StartsWith("/ hitman") ||  chatText.ToLower().StartsWith("/ hit") )
                    {
                        AddRoleMessage(RoleEnum.Hitman);
                        return false;
                    }
                    else if (chatText.ToLower().StartsWith("/saboteur") || chatText.ToLower().StartsWith("/sab"))
                    {
                        AddModifierMessage(ModifierEnum.Saboteur);
                        return false;
                    }
                    else if (chatText.ToLower().StartsWith("/witch") || chatText.ToLower().StartsWith("/ witch"))
                    {
                        AddRoleMessage(RoleEnum.Witch);
                        return false;
                    }
                    else if (chatText.ToLower().StartsWith("/werewolf") ||chatText.ToLower().StartsWith("/werewolf") ||
                     chatText.ToLower().StartsWith("/ww") || chatText.ToLower().StartsWith("/ ww"))
                    {
                        AddRoleMessage(RoleEnum.Warlock);
                        return false;
                    }
                    else if (chatText.ToLower().StartsWith("/ora") || chatText.ToLower().StartsWith("/ ora"))
                    {
                        AddRoleMessage(RoleEnum.Oracle);
                        return false;
                    }
                    else if (chatText.ToLower().StartsWith("/ven") || chatText.ToLower().StartsWith("/ ven"))
                    {
                        AddRoleMessage(RoleEnum.Venerer);
                        return false;
                    }
                    else if (chatText.ToLower().StartsWith("/ward") || chatText.ToLower().StartsWith("/ ward"))
                    {
                        AddRoleMessage(RoleEnum.Crusader);
                        return false;
                    }
                    else if (chatText.ToLower().StartsWith("/jailor") || chatText.ToLower().StartsWith("/ jailor"))
                    {
                        AddRoleMessage(RoleEnum.Jailor);
                        return false;
                    }
                    else if (chatText.ToLower().StartsWith("/dep") || chatText.ToLower().StartsWith("/ dep"))
                    {
                        AddRoleMessage(RoleEnum.Deputy);
                        return false;
                    }
                    else if (chatText.ToLower().StartsWith("/giant") || chatText.ToLower().StartsWith("/ giant"))
                    {
                        AddModifierMessage(ModifierEnum.Giant);
                        return false;
                    }
                    else if (chatText.ToLower().StartsWith("/after") || chatText.ToLower().StartsWith("/ after"))
                    {
                        AddModifierMessage(ModifierEnum.Aftermath);
                        return false;
                    }
                    else if (chatText.ToLower().StartsWith("/bait") || chatText.ToLower().StartsWith("/ bait"))
                    {
                        AddModifierMessage(ModifierEnum.Bait);
                        return false;
                    }
                    else if (chatText.ToLower().StartsWith("/dis") || chatText.ToLower().StartsWith("/ dis"))
                    {
                        AddModifierMessage(ModifierEnum.Diseased);
                        return false;
                    }
                    else if (chatText.ToLower().StartsWith("/dis") || chatText.ToLower().StartsWith("/ dis"))
                    {
                        AddModifierMessage(ModifierEnum.Disperser);
                        return false;
                    }
                    else if (chatText.ToLower().StartsWith("/double") || chatText.ToLower().StartsWith("/ double"))
                    {
                        AddModifierMessage(ModifierEnum.DoubleShot);
                        return false;
                    }
                    else if (chatText.ToLower().StartsWith("/udog") || chatText.ToLower().StartsWith("/ udog") ||
                        chatText.ToLower().StartsWith("/underdog") || chatText.ToLower().StartsWith("/ underdog"))
                    {
                        AddModifierMessage(ModifierEnum.Underdog);
                        return false;
                    }
                    else if (chatText.ToLower().StartsWith("/frost") || chatText.ToLower().StartsWith("/ frost"))
                    {
                        AddModifierMessage(ModifierEnum.Frosty);
                        return false;
                    }
                    else if (chatText.ToLower().StartsWith("/torch") || chatText.ToLower().StartsWith("/ torch"))
                    {
                        AddAbilityMessage(AbilityEnum.Torch);
                        return false;
                    }
                    else if (chatText.ToLower().StartsWith("/lookout") || chatText.ToLower().StartsWith("/lo"))
                    {
                        AddRoleMessage(RoleEnum.Lookout);
                        return false;
                    }
                    else if (chatText.ToLower().StartsWith("/bb") || chatText.ToLower().StartsWith("/ bb"))
                    {
                        AddAbilityMessage(AbilityEnum.ButtonBarry);
                        return false;
                    }
                    else if (chatText.ToLower().StartsWith("/flash") || chatText.ToLower().StartsWith("/ flash"))
                    {
                        AddAbilityMessage(AbilityEnum.Flash);
                        return false;
                    }
                    else if (chatText.ToLower().StartsWith("/multitasker") || chatText.ToLower().StartsWith("/ multitasker"))
                    {
                        AddAbilityMessage(AbilityEnum.Multitasker);
                        return false;
                    }
                    else if (chatText.ToLower().StartsWith("/paranoiac") || chatText.ToLower().StartsWith("/ paranoiac"))
                    {
                        AddAbilityMessage(AbilityEnum.Paranoiac);
                        return false;
                    }
                    else if (chatText.ToLower().StartsWith("/tiebreaker") || chatText.ToLower().StartsWith("/ tiebreaker"))
                    {
                        AddAbilityMessage(AbilityEnum.Tiebreaker);
                        return false;
                    }
                    else if (chatText.ToLower().StartsWith("/drunk") || chatText.ToLower().StartsWith("/ drunk"))
                    {
                        AddAbilityMessage(AbilityEnum.Drunk);
                        return false;
                    }
                    else if (chatText.ToLower().StartsWith("/assassin") || chatText.ToLower().StartsWith("/ assassin"))
                    {
                        AddAbilityMessage(AbilityEnum.Assassin);
                        return false;
                    }
                    else if (chatText.ToLower().StartsWith("/spy") || chatText.ToLower().StartsWith("/ spy"))
                    {
                        AddAbilityMessage(AbilityEnum.Spy);
                        return false;
                    }
                    else if (chatText.ToLower().StartsWith("/sleuth") || chatText.ToLower().StartsWith("/ sleuth"))
                    {
                        AddAbilityMessage(AbilityEnum.Sleuth);
                        return false;
                    }
                    else if (chatText.ToLower().StartsWith("/r") || chatText.ToLower().StartsWith("/ r") || chatText.ToLower().StartsWith("/role") || chatText.ToLower().StartsWith("/ role"))
                    {
                        var role = GetPlayerRole(PlayerControl.LocalPlayer);
                        if (role != null) AddRoleMessage(role.RoleType);
                        else if (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, "You do not have a role.");
                        else DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, "Invalid Command.");
                        return false;
                    }
                    else if (chatText.ToLower().StartsWith("/m") || chatText.ToLower().StartsWith("/ m") || chatText.ToLower().StartsWith("/modifier") || chatText.ToLower().StartsWith("/ modifier"))
                    {
                        var modifier = GetModifier(PlayerControl.LocalPlayer);
                        if (modifier != null) AddModifierMessage(modifier.ModifierType);
                        else if (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, "You do not have a modifier.");
                        else DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, "Invalid Command.");
                        return false;
                    }
                    else if (chatText.ToLower().StartsWith("/a") || chatText.ToLower().StartsWith("/ a") || chatText.ToLower().StartsWith("/modifier") || chatText.ToLower().StartsWith("/ ability"))
                    {
                        var ability = GetAbility(PlayerControl.LocalPlayer);
                        if (ability != null) AddAbilityMessage(ability.AbilityType);
                        else if (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, "You do not have an ability.");
                        else DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, "Invalid Command.");
                        return false;
                    }
                }
                if ((chatText.ToLower().StartsWith("/jail") || chatText.ToLower().StartsWith("/ jail")) && sourcePlayer.Is(RoleEnum.Jailor) && MeetingHud.Instance)
                {
                    if (PlayerControl.LocalPlayer.Is(RoleEnum.Jailor) || PlayerControl.LocalPlayer.IsJailed())
                    {
                        if (chatText.ToLower().StartsWith("/jail")) chatText = chatText[5..];
                        else if (chatText.ToLower().StartsWith("/jail ")) chatText = chatText[6..];
                        else if (chatText.ToLower().StartsWith("/ jail")) chatText = chatText[6..];
                        else if (chatText.ToLower().StartsWith("/ jail ")) chatText = chatText[7..];
                        JailorMessage = true;
                        if (sourcePlayer != PlayerControl.LocalPlayer && PlayerControl.LocalPlayer.IsJailed() && !sourcePlayer.Data.IsDead) sourcePlayer = PlayerControl.LocalPlayer;
                        return true;
                    }
                    else return false;
                }
                if (chatText.ToLower().StartsWith("/"))
                {
                    if (sourcePlayer == PlayerControl.LocalPlayer) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, "Invalid Command.");
                    return false;
                }
                if (sourcePlayer.IsJailed() && MeetingHud.Instance)
                {
                    if (PlayerControl.LocalPlayer == sourcePlayer || PlayerControl.LocalPlayer.Is(RoleEnum.Jailor)) return true;
                    else return false;
                }
                if (PlayerControl.LocalPlayer.IsJailed() && MeetingHud.Instance) return false;
                return true;
            }

            public static void AddRoleMessage(RoleEnum role)
            {
                var ChooseOrNew = CustomGameOptions.RomanticOnBelovedDeath == RomanticBecomeOptions.Repick ? "have to choose a new partner" : $"become {CustomGameOptions.RomanticOnBelovedDeath.ToString()} on your partner's death";
                if (role == RoleEnum.Crewmate) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, "The Crewmate is a vanilla Crewmate.");
                if (role == RoleEnum.Impostor) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, "The Impostor a vanilla Impostor.");
                if (role == RoleEnum.Engineer) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(
                    PlayerControl.LocalPlayer, "The Engineer is a crewmate with the ability to vent and fix sabotages.");
                if (role == RoleEnum.Investigator) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(
                    PlayerControl.LocalPlayer, "The Investigator is a crewmate with the ability to see other player's footsteps for a limited time.");
                    if (role == RoleEnum.Romantic) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(
                    PlayerControl.LocalPlayer, "As the Romantic, you must pick a player to love, working together to ensure both of your survival. You will " + ChooseOrNew + ".");
                if (role == RoleEnum.Medic) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(
                    PlayerControl.LocalPlayer, "The Medic is a crewmate who can place a shield on another player.");
                if (role == RoleEnum.Swapper) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(
                    PlayerControl.LocalPlayer, "The Swapper is a crewmate who can swap the votes of 2 players during meetings.");
                if (role == RoleEnum.Seer) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(
                    PlayerControl.LocalPlayer, "The Seer is a crewmate who can reveal the alliance of other players.");
                if (role == RoleEnum.Vigilante) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer,
                    "The Vigilante is a crewmate who can guess other people's roles during meetings. If they guess correctly they kill the other player, if not, they die instead.");
                if (role == RoleEnum.Hunter) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer,
                    "The Hunter is a crewmate who can stalk other players. If the stalked player uses an ability, the Hunter will then be permitted to kill them. The Hunter has no punishment for killing incorrectly.");
                if (role == RoleEnum.Arsonist) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer,
                    "The Arsonist is a neutral killer with the goal to kill everyone. To do so they must douse players and once enough people are doused they can ignite, killing all doused players immediately.");
                if (role == RoleEnum.Executioner) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(
                    PlayerControl.LocalPlayer, "The Executioner is a neutral evil role with the goal to vote out a specific player.");
                if (role == RoleEnum.Glitch) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer,
                    "The Glitch is a neutral killer with the goal to kill everyone. In addition to killing, they can also hack players, disabling abilities and mimic players, changing their appearance to look like others.");
                if (role == RoleEnum.Jester) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(
                    PlayerControl.LocalPlayer, "The Jester is a neutral evil role with the goal to be voted out.");
                if (role == RoleEnum.Poisoner) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(
                    PlayerControl.LocalPlayer, $"The Poisoner can poison a player every {CustomGameOptions.PoisonCd} seconds, after {CustomGameOptions.PoisonDelay} seconds the player die. Players with protection can't be killed by the poisoner. If you kill an Aftermath, you will suicide. If the poisoner is alive in the last 4, they will directly kill instead of poisoning.");
                if (role == RoleEnum.Witch) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(
                    PlayerControl.LocalPlayer, "The witch can cast a spell on players, doing so adds a cross next to the player name, visible to everyone announcing they've been cursed. The spelled player can not be saved, and will be die after meeting.");
                if (role == RoleEnum.Vulture) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(
                    PlayerControl.LocalPlayer, $"The Vulture is a Neutral role with its own win condition. Their goal is to eat an amount of {CustomGameOptions.VultureBodyCount} dead bodies to win. The body dissapears when the Vulture eats it, similar to how Janitor cleans bodies. If they eat the amount of bodies the setting is set to, they win.");
                if (role == RoleEnum.Grenadier) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(
                    PlayerControl.LocalPlayer, "The Grenadier is an impostor who can use smoke grenades to blind other players.");
                if (role == RoleEnum.Janitor) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(
                    PlayerControl.LocalPlayer, "The Janitor is an impostor who can remove bodies from the map.");
                if (role == RoleEnum.Miner) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(
                    PlayerControl.LocalPlayer, "The Miner is an impostor who can place new vents to create a new vent network.");
                if (role == RoleEnum.Morphling) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(
                    PlayerControl.LocalPlayer, "The Morphling is an impostor who can morph into other players.");
                if (role == RoleEnum.Swooper) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(
                    PlayerControl.LocalPlayer, "The Swooper is an impostor who can turn invisible.");
                if (role == RoleEnum.Undertaker) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(
                    PlayerControl.LocalPlayer, "The Undertaker is an impostor who can drag bodies to different locations.");
                if (role == RoleEnum.Werewolf) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(
                    PlayerControl.LocalPlayer, "The Werewolf can kill all players within a certain radius.");
                if (role == RoleEnum.Lookout) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(
                    PlayerControl.LocalPlayer, "The Lookout is a crewmate who can watch other players. They will see all players who interact with each player they watch.");
                if (role == RoleEnum.Veteran) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(
                    PlayerControl.LocalPlayer, "The Veteran is a crewmate who can alert to kill anyone who interacts with them.");
                if (role == RoleEnum.Amnesiac) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(
                    PlayerControl.LocalPlayer, "The Amnesiac is a neutral benign role that needs to find a body in order to remember a new role.");
                if (role == RoleEnum.Juggernaut) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer,
                    "The Juggernaut is a neutral killer with the goal to kill everyone. Every kill they make reduces their kill cooldown.");
                if (role == RoleEnum.Tracker) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(
                    PlayerControl.LocalPlayer, "The Tracker is a crewmate who can track multiple other players.");
                if (role == RoleEnum.Medium) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(
                    PlayerControl.LocalPlayer, "The Medium is a crewmate who can see dead players the round that they die.");
                if (role == RoleEnum.Trapper) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer,
                    "The Trapper is a crewmate who can place traps around the map. All players who stand in these traps will reveal their role to the Trapper as long as enough players trigger the trap.");
                if (role == RoleEnum.GuardianAngel) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(
                    PlayerControl.LocalPlayer, "The Guardian Angel is a neutral benign role that needs their target to win to win themselves.");
                if (role == RoleEnum.Mystic) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(
                    PlayerControl.LocalPlayer, "The Mystic is a crewmate who gets an alert when a player dies.");
                if (role == RoleEnum.Blackmailer) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(
                    PlayerControl.LocalPlayer, "The Blackmailer is an impostor who can silence other players.");
                if (role == RoleEnum.Plaguebearer) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer,
                    "The Plaguebearer is a neutral killer with the goal to kill everyone. To do this they must infect everyone to turn into Pestilence.");
                if (role == RoleEnum.Pestilence) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer,
                    "The Pestilence is a neutral killer with the goal to kill everyone. In addition to being able to kill, they are invincible and anyone who interacts with them will die.");
                if (role == RoleEnum.SerialKiller) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer,
                    "The Serial Killer is a Neutral role with its own win condition. Although the Serial Killer has a kill button, they can't use it unless they are stabbing. Once the Serial Killer rampages they gain Impostor vision and the ability to kill. However, unlike most killers their kill cooldown is really short. The Serial Killer needs to be the last killer alive to win the game.");
                if (role == RoleEnum.Detective) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer,
                    "The Detective is a crewmate that can inspect crime scenes. Any player who has walked over this crime scene is suspicious. They can then examine players to see who has been at the crime scene.");
                if (role == RoleEnum.Escapist) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(
                    PlayerControl.LocalPlayer, "The Escapist is an impostor who can mark a location and recall to it later.");
                if (role == RoleEnum.Imitator) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer,
                    "The Imitator is a crewmate who can select dead crew roles to use during meetings. The following round they become this new role.");
                if (role == RoleEnum.Bomber) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(
                    PlayerControl.LocalPlayer, "The Bomber is an impostor who can place bombs, these kill anyone in the area a short duration later.");
                if (role == RoleEnum.Hitman) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(
                    PlayerControl.LocalPlayer, "The Hitman is a Neutral role that depending on settings, may spawn naturally or is the become option for the Agent. The Hitman can Morph into players to disguise itself from others. They can additionally drag bodies like the Undertaker. This role cannot spawn on Fungle.");
                if (role == RoleEnum.Agent) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(
                    PlayerControl.LocalPlayer, "The Agent is a Neutral role with its own win condition. The Agent has tasks like the Crewmates but their tasks do not count for a task win. After the Agent finishes its tasks, They automatically become a Hitman. This role cannot spawn on Fungle.");
                if (role == RoleEnum.Doomsayer) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(
                    PlayerControl.LocalPlayer, "The Doomsayer is a neutral evil role with the goal to guess 3 other player's roles to win.");
                if (role == RoleEnum.Vampire) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer,
                    "The Vampire is a neutral killer with the goal to kill everyone. The first crewmate the original Vampire bites will turn into a Vampire, the rest will die.");
                if (role == RoleEnum.Warlock) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(
                    PlayerControl.LocalPlayer, "The Warlock is an impostor who can charge their kill button to kill multiple people at once.");
                if (role == RoleEnum.Oracle) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer,
                    "The Oracle is a crewmate who can make a player confess. This makes it so each meeting the Oracle learns that at least 1 of 3 players is evil, this other player is protected from being voted out and if the Oracle were to die that their potential faction would be revealed.");
                if (role == RoleEnum.Venerer) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer,
                    "The Venerer is an impostor who improves their ability with each kill. First kill is camouflage, second is speed and third is global slow.");
                if (role == RoleEnum.Crusader) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(
                    PlayerControl.LocalPlayer, "As the Crusader, you can Fortify a crewmate with a spell to prevent them from being killed or interacted with by anyone. The spell lasts until next meeting. When the player has a kill attempt, the fortified player will murder the killer.");
                if (role == RoleEnum.Jailor) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer,
                    "The Jailor is a crewmate who can jail other players. Jailed players cannot have meeting abilities used on them and cannot use meeting abilities themself. The Jailor and jailee may also privately talk to each other and the Jailor may also execute their jailee. If they execute a crewmate they lose the ability to jail players.");
                if (role == RoleEnum.Deputy) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer,
                    "The Deputy is a crewmate who can camp other players. If the player is killed they will receive an alert notifying them of their death. During the following meeting they may then shoot anyone. If they shoot the killer, they die unless fortified or invincible, if they are wrong nothing happens.");
            }

            public static void AddModifierMessage(ModifierEnum modifier)
            {
                if (modifier == ModifierEnum.Giant) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(
                    PlayerControl.LocalPlayer, "The Giant is a modifier that increases the size of a player.");
                if (modifier == ModifierEnum.Mini) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(
                    PlayerControl.LocalPlayer, "The Mini is a modifier that decreases the size of a player.");
                if (modifier == ModifierEnum.Aftermath) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(
                    PlayerControl.LocalPlayer, "The Aftermath is a modifier that forces their killer to instantly use their ability.");
                if (modifier == ModifierEnum.Bait) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(
                    PlayerControl.LocalPlayer, "The Bait is a modifier that forces their killer to report their body.");
                if (modifier == ModifierEnum.Diseased) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(
                    PlayerControl.LocalPlayer, "The Diseased is a modifier that increases their killer's kill cooldown.");
                if (modifier == ModifierEnum.Disperser) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer,
                    "The Disperser is an impostor modifier that gives an extra ability to Impostors. This being once per game sending every player to a random vent on the map.");
                if (modifier == ModifierEnum.DoubleShot) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(
                    PlayerControl.LocalPlayer, "The Double Shot is an impostor modifier that gives Assassins an extra life when assassinating.");
                if (modifier == ModifierEnum.Underdog) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(
                    PlayerControl.LocalPlayer, "The Underdog is an impostor modifier that grants Impostors a reduced kill cooldown when alone.");
                if (modifier == ModifierEnum.Saboteur) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(
                    PlayerControl.LocalPlayer, "The Saboteur is an impostor modifier that passively reduces non-door sabotage cooldowns.");
                if (modifier == ModifierEnum.Frosty) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(
                    PlayerControl.LocalPlayer, "The Frosty is a crewmate modifier that reduces the speed of their killer temporarily.");
                    
            }
            public static void AddAbilityMessage(AbilityEnum ability)
            {
                if (ability == AbilityEnum.Assassin) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(
                    PlayerControl.LocalPlayer, "The Assassin is an ability which is given to killers to guess other player's roles during mettings. If they guess correctly they kill the other player, if not, they die instead.");
                if (ability == AbilityEnum.Flash) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(
                    PlayerControl.LocalPlayer, "The Flash is a global modifier that increases the speed of a player.");
                if (ability == AbilityEnum.ButtonBarry) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(
                    PlayerControl.LocalPlayer, "The Button Barry allows the player to button from anywhere on the map.");
                if (ability == AbilityEnum.Tiebreaker) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(
                    PlayerControl.LocalPlayer, "The Tiebreaker is an ability that allows a player's vote to break ties in meetings.");
                if (ability == AbilityEnum.Multitasker) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(
                    PlayerControl.LocalPlayer, "The Multitasker makes their tasks slightly transparent.");
                if (ability == AbilityEnum.Drunk) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer,
                    "The Drunk is an ability that makes the players movements reversed, if they hit to go up, they will go down etc.");
                if (ability == AbilityEnum.Paranoiac) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(
                    PlayerControl.LocalPlayer, "The Paranoiac shows an arrow pointing to the closest player.");
                if (ability == AbilityEnum.Torch) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(
                    PlayerControl.LocalPlayer, "The Torch is a crewmate modifier that allows them to see when lights are off. Killers or players with impostor vision cannot gain this ability.");
                if (ability == AbilityEnum.Spy) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(
                    PlayerControl.LocalPlayer, "The Spy can see the colours of players on the admin table and the death timing on vitals panel.");
            }
        }

        [HarmonyPatch(typeof(ChatBubble), nameof(ChatBubble.SetName))]
        public static class SetName
        {
            public static void Postfix(ChatBubble __instance, [HarmonyArgument(0)] string playerName)
            {
                if (PlayerControl.LocalPlayer.Is(RoleEnum.Jailor) && MeetingHud.Instance)
                {
                    var jailor = Role.GetRole<Jailor>(PlayerControl.LocalPlayer);
                    if (jailor.Jailed != null && jailor.Jailed.Data.PlayerName == playerName)
                    {
                        __instance.NameText.color = jailor.Color;
                        __instance.NameText.text = playerName + " (Jailed)";
                    }
                    else if (JailorMessage)
                    {
                        __instance.NameText.color = jailor.Color;
                        __instance.NameText.text = "Jailor";
                        JailorMessage = false;
                    }
                }
                if (PlayerControl.LocalPlayer.IsJailed() && MeetingHud.Instance)
                {
                    if (JailorMessage)
                    {
                        __instance.NameText.color = Colors.Jailor;
                        __instance.NameText.text = "Jailor";
                        JailorMessage = false;
                    }
                }
            }
        }
    }
}