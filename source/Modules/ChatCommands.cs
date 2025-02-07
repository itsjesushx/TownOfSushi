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
                if (sourcePlayer == LocalPlayer)
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
                    else if (chatText.ToLower().StartsWith("/ora") || chatText.ToLower().StartsWith("/oracle"))
                    {
                        AddRoleMessage(RoleEnum.Oracle);
                        return false;
                    }
                    else if (chatText.ToLower().StartsWith("/bh") || chatText.ToLower().StartsWith("/bounty hunter"))
                    {
                        AddRoleMessage(RoleEnum.BountyHunter);
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
                    else if (chatText.ToLower().StartsWith("/r") || chatText.ToLower().StartsWith("/ m") || chatText.ToLower().StartsWith("/m") || chatText.ToLower().StartsWith("/ myrole"))
                    {
                        var role = GetPlayerRole(LocalPlayer);
                        if (role != null) AddRoleMessage(role.RoleType);
                        else if (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started) Chat().AddChat(LocalPlayer, "You do not have a role.");
                        else Chat().AddChat(LocalPlayer, "Invalid Command.");
                        return false;
                    }
                    else if (chatText.ToLower().StartsWith("/md") || chatText.ToLower().StartsWith("/ md") || chatText.ToLower().StartsWith("/mymodifier") || chatText.ToLower().StartsWith("/ mymodifier"))
                    {
                        var modifier = GetModifier(LocalPlayer);
                        if (modifier != null) AddModifierMessage(modifier.ModifierType);
                        else if (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started) Chat().AddChat(LocalPlayer, "You do not have a modifier.");
                        else Chat().AddChat(LocalPlayer, "Invalid Command.");
                        return false;
                    }
                    else if (chatText.ToLower().StartsWith("/ma") || chatText.ToLower().StartsWith("/ ma") || chatText.ToLower().StartsWith("/myability") || chatText.ToLower().StartsWith("/ myability"))
                    {
                        var ability = GetAbility(LocalPlayer);
                        if (ability != null) AddAbilityMessage(ability.AbilityType);
                        else if (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started) Chat().AddChat(LocalPlayer, "You do not have an ability.");
                        else Chat().AddChat(LocalPlayer, "Invalid Command.");
                        return false;
                    }
                }
                if ((chatText.ToLower().StartsWith("/jail") || chatText.ToLower().StartsWith("/ jail")) && sourcePlayer.Is(RoleEnum.Jailor) && Meeting())
                {
                    if (LocalPlayer.Is(RoleEnum.Jailor) || LocalPlayer.IsJailed())
                    {
                        if (chatText.ToLower().StartsWith("/jail")) chatText = chatText[5..];
                        else if (chatText.ToLower().StartsWith("/jail ")) chatText = chatText[6..];
                        else if (chatText.ToLower().StartsWith("/ jail")) chatText = chatText[6..];
                        else if (chatText.ToLower().StartsWith("/ jail ")) chatText = chatText[7..];
                        JailorMessage = true;
                        if (sourcePlayer != LocalPlayer&& LocalPlayer.IsJailed() && !sourcePlayer.Data.IsDead) sourcePlayer = LocalPlayer;
                        return true;
                    }
                    else return false;
                }
                if (chatText.ToLower().StartsWith("/"))
                {
                    if (sourcePlayer == LocalPlayer) Chat().AddChat(LocalPlayer, "Invalid Command.");
                    return false;
                }
                if (sourcePlayer.IsJailed() && Meeting())
                {
                    if (LocalPlayer== sourcePlayer || LocalPlayer.Is(RoleEnum.Jailor)) return true;
                    else return false;
                }
                if (LocalPlayer.IsJailed() && Meeting()) return false;
                return true;
            }

            public static void AddRoleMessage(RoleEnum role)
            {
                var ChooseOrNew = CustomGameOptions.RomanticOnBelovedDeath == RomanticBecomeOptions.Repick ? "have to choose a new partner" : $"become {CustomGameOptions.RomanticOnBelovedDeath.ToString()} on your partner's death";
                if (role == RoleEnum.Crewmate) Chat().AddChat(LocalPlayer, "The Crewmate is a vanilla Crewmate.");
                if (role == RoleEnum.Impostor) Chat().AddChat(LocalPlayer, "The Impostor a vanilla Impostor.");
                if (role == RoleEnum.Engineer) Chat().AddChat(
                    LocalPlayer, "The Engineer is a crewmate with the ability to vent and fix sabotages.");
                if (role == RoleEnum.Investigator) Chat().AddChat(
                    LocalPlayer, "The Investigator is a crewmate with the ability to see other player's footsteps for a limited time.");
                    if (role == RoleEnum.Romantic) Chat().AddChat(
                    LocalPlayer, "As the Romantic, you must pick a player to love, working together to ensure both of your survival. You will " + ChooseOrNew + ".");
                if (role == RoleEnum.Medic) Chat().AddChat(
                    LocalPlayer, "The Medic is a crewmate who can place a shield on another player.");
                if (role == RoleEnum.Swapper) Chat().AddChat(
                    LocalPlayer, "The Swapper is a crewmate who can swap the votes of 2 players during meetings.");
                if (role == RoleEnum.Seer) Chat().AddChat(
                    LocalPlayer, "The Seer is a crewmate who can reveal the alliance of other players.");
                if (role == RoleEnum.Vigilante) Chat().AddChat(LocalPlayer,
                    "The Vigilante is a crewmate who can guess other people's roles during meetings. If they guess correctly they kill the other player, if not, they die instead.");
                if (role == RoleEnum.Hunter) Chat().AddChat(LocalPlayer,
                    "The Hunter is a crewmate who can stalk other players. If the stalked player uses an ability, the Hunter will then be permitted to kill them. The Hunter has no punishment for killing incorrectly.");
                if (role == RoleEnum.Arsonist) Chat().AddChat(LocalPlayer,
                    "The Arsonist is a neutral killer with the goal to kill everyone. To do so they must douse players and once enough people are doused they can ignite, killing all doused players immediately.");
                if (role == RoleEnum.Executioner) Chat().AddChat(
                    LocalPlayer, "The Executioner is a neutral evil role with the goal to vote out a specific player.");
                if (role == RoleEnum.Glitch) Chat().AddChat(LocalPlayer,
                    "The Glitch is a neutral killer with the goal to kill everyone. In addition to killing, they can also hack players, disabling abilities and mimic players, changing their appearance to look like others.");
                if (role == RoleEnum.Jester) Chat().AddChat(
                    LocalPlayer, "The Jester is a neutral evil role with the goal to be voted out.");
                if (role == RoleEnum.Poisoner) Chat().AddChat(
                    LocalPlayer, $"The Poisoner can poison a player every {CustomGameOptions.PoisonCd} seconds, after {CustomGameOptions.PoisonDelay} seconds the player die. Players with protection can't be killed by the poisoner. If you kill an Aftermath, you will suicide. If the poisoner is alive in the last 4, they will directly kill instead of poisoning.");
                if (role == RoleEnum.Witch) Chat().AddChat(
                    LocalPlayer, "The witch can cast a spell on players, doing so adds a cross next to the player name, visible to everyone announcing they've been cursed. The spelled player can not be saved, and will be die after meeting.");
                if (role == RoleEnum.Vulture) Chat().AddChat(
                    LocalPlayer, $"The Vulture is a Neutral role with its own win condition. Their goal is to eat an amount of {CustomGameOptions.VultureBodyCount} dead bodies to win. The body dissapears when the Vulture eats it, similar to how Janitor cleans bodies. If they eat the amount of bodies the setting is set to, they win.");
                if (role == RoleEnum.Grenadier) Chat().AddChat(
                    LocalPlayer, "The Grenadier is an impostor who can use smoke grenades to blind other players.");
                if (role == RoleEnum.Janitor) Chat().AddChat(
                    LocalPlayer, "The Janitor is an impostor who can remove bodies from the map.");
                if (role == RoleEnum.Miner) Chat().AddChat(
                    LocalPlayer, "The Miner is an impostor who can place new vents to create a new vent network.");
                if (role == RoleEnum.Morphling) Chat().AddChat(
                    LocalPlayer, "The Morphling is an impostor who can morph into other players.");
                if (role == RoleEnum.Swooper) Chat().AddChat(
                    LocalPlayer, "The Swooper is an impostor who can turn invisible.");
                if (role == RoleEnum.Undertaker) Chat().AddChat(
                    LocalPlayer, "The Undertaker is an impostor who can drag bodies to different locations.");
                if (role == RoleEnum.Werewolf) Chat().AddChat(
                    LocalPlayer, "The Werewolf can kill all players within a certain radius.");
                if (role == RoleEnum.Lookout) Chat().AddChat(
                    LocalPlayer, "The Lookout is a crewmate who can watch other players. They will see all players who interact with each player they watch.");
                if (role == RoleEnum.Veteran) Chat().AddChat(
                    LocalPlayer, "The Veteran is a crewmate who can alert to kill anyone who interacts with them.");
                if (role == RoleEnum.Amnesiac) Chat().AddChat(
                    LocalPlayer, "The Amnesiac is a neutral benign role that needs to find a body in order to remember a new role.");
                if (role == RoleEnum.Juggernaut) Chat().AddChat(LocalPlayer,
                    "The Juggernaut is a neutral killer with the goal to kill everyone. Every kill they make reduces their kill cooldown.");
                if (role == RoleEnum.Tracker) Chat().AddChat(
                    LocalPlayer, "The Tracker is a crewmate who can track multiple other players.");
                if (role == RoleEnum.Medium) Chat().AddChat(
                    LocalPlayer, "The Medium is a crewmate who can see dead players the round that they die.");
                if (role == RoleEnum.Trapper) Chat().AddChat(LocalPlayer,
                    "The Trapper is a crewmate who can place traps around the map. All players who stand in these traps will reveal their role to the Trapper as long as enough players trigger the trap.");
                if (role == RoleEnum.GuardianAngel) Chat().AddChat(
                    LocalPlayer, "The Guardian Angel is a neutral benign role that needs their target to win to win themselves.");
                if (role == RoleEnum.Mystic) Chat().AddChat(
                    LocalPlayer, "The Mystic is a crewmate who gets an alert when a player dies.");
                if (role == RoleEnum.Blackmailer) Chat().AddChat(
                    LocalPlayer, "The Blackmailer is an impostor who can silence other players.");
                if (role == RoleEnum.Plaguebearer) Chat().AddChat(LocalPlayer,
                    "The Plaguebearer is a neutral killer with the goal to kill everyone. To do this they must infect everyone to turn into Pestilence.");
                if (role == RoleEnum.Pestilence) Chat().AddChat(LocalPlayer,
                    "The Pestilence is a neutral killer with the goal to kill everyone. In addition to being able to kill, they are invincible and anyone who interacts with them will die.");
                if (role == RoleEnum.SerialKiller) Chat().AddChat(LocalPlayer,
                    "The Serial Killer is a Neutral role with its own win condition. Although the Serial Killer has a kill button, they can't use it unless they are stabbing. Once the Serial Killer rampages they gain Impostor vision and the ability to kill. However, unlike most killers their kill cooldown is really short. The Serial Killer needs to be the last killer alive to win the game.");
                if (role == RoleEnum.Detective) Chat().AddChat(LocalPlayer,
                    "The Detective is a crewmate that can inspect crime scenes. Any player who has walked over this crime scene is suspicious. They can then examine players to see who has been at the crime scene.");
                if (role == RoleEnum.Escapist) Chat().AddChat(
                    LocalPlayer, "The Escapist is an impostor who can mark a location and recall to it later.");
                if (role == RoleEnum.Imitator) Chat().AddChat(LocalPlayer,
                    "The Imitator is a crewmate who can select dead crew roles to use during meetings. The following round they become this new role.");
                if (role == RoleEnum.Bomber) Chat().AddChat(
                    LocalPlayer, "The Bomber is an impostor who can place bombs, these kill anyone in the area a short duration later.");
                if (role == RoleEnum.Hitman) Chat().AddChat(
                    LocalPlayer, "The Hitman is a Neutral role that depending on settings, may spawn naturally or is the become option for the Agent. The Hitman can Morph into players to disguise itself from others. They can additionally drag bodies like the Undertaker. This role cannot spawn on Fungle.");
                if (role == RoleEnum.Agent) Chat().AddChat(
                    LocalPlayer, "The Agent is a Neutral role with its own win condition. The Agent has tasks like the Crewmates but their tasks do not count for a task win. After the Agent finishes its tasks, They automatically become a Hitman. This role cannot spawn on Fungle.");
                if (role == RoleEnum.Doomsayer) Chat().AddChat(
                    LocalPlayer, "The Doomsayer is a neutral evil role with the goal to guess 3 other player's roles to win.");
                if (role == RoleEnum.Vampire) Chat().AddChat(LocalPlayer,
                    "The Vampire is a neutral killer with the goal to kill everyone. The first crewmate the original Vampire bites will turn into a Vampire, the rest will die.");
                if (role == RoleEnum.Warlock) Chat().AddChat(
                    LocalPlayer, "The Warlock is an impostor who can charge their kill button to kill multiple people at once.");
                if (role == RoleEnum.Oracle) Chat().AddChat(LocalPlayer,
                    "The Oracle is a crewmate who can make a player confess. This makes it so each meeting the Oracle learns that at least 1 of 3 players is evil, this other player is protected from being voted out and if the Oracle were to die that their potential faction would be revealed.");
                if (role == RoleEnum.Venerer) Chat().AddChat(LocalPlayer,
                    "The Venerer is an impostor who improves their ability with each kill. First kill is camouflage, second is speed and third is global slow.");
                if (role == RoleEnum.Crusader) Chat().AddChat(
                    LocalPlayer, "As the Crusader, you can Fortify a crewmate with a spell to prevent them from being killed or interacted with by anyone. The spell lasts until next meeting. When the player has a kill attempt, the fortified player will murder the killer.");
                if (role == RoleEnum.BountyHunter) Chat().AddChat(
                    LocalPlayer, "As the Bounty Hunter you are given a random target (bounty) to kill, if you kill them you gian a new target and a short cooldown, else you will get a long cooldown penalty.");
                if (role == RoleEnum.Jailor) Chat().AddChat(LocalPlayer,
                    "The Jailor is a crewmate who can jail other players. Jailed players cannot have meeting abilities used on them and cannot use meeting abilities themself. The Jailor and jailee may also privately talk to each other and the Jailor may also execute their jailee. If they execute a crewmate they lose the ability to jail players.");
                if (role == RoleEnum.Deputy) Chat().AddChat(LocalPlayer,
                    "The Deputy is a crewmate who can camp other players. If the player is killed they will receive an alert notifying them of their death. During the following meeting they may then shoot anyone. If they shoot the killer, they die unless fortified or invincible, if they are wrong nothing happens.");
            }

            public static void AddModifierMessage(ModifierEnum modifier)
            {
                if (modifier == ModifierEnum.Giant) Chat().AddChat(
                    LocalPlayer, "The Giant is a modifier that increases the size of a player.");
                if (modifier == ModifierEnum.Mini) Chat().AddChat(
                    LocalPlayer, "The Mini is a modifier that decreases the size of a player.");
                if (modifier == ModifierEnum.Aftermath) Chat().AddChat(
                    LocalPlayer, "The Aftermath is a modifier that forces their killer to instantly use their ability.");
                if (modifier == ModifierEnum.Bait) Chat().AddChat(
                    LocalPlayer, "The Bait is a modifier that forces their killer to report their body.");
                if (modifier == ModifierEnum.Diseased) Chat().AddChat(
                    LocalPlayer, "The Diseased is a modifier that increases their killer's kill cooldown.");
                if (modifier == ModifierEnum.Disperser) Chat().AddChat(LocalPlayer,
                    "The Disperser is an impostor modifier that gives an extra ability to Impostors. This being once per game sending every player to a random vent on the map.");
                if (modifier == ModifierEnum.DoubleShot) Chat().AddChat(
                    LocalPlayer, "The Double Shot is an impostor modifier that gives Assassins an extra life when assassinating.");
                if (modifier == ModifierEnum.Underdog) Chat().AddChat(
                    LocalPlayer, "The Underdog is an impostor modifier that grants Impostors a reduced kill cooldown when alone.");
                if (modifier == ModifierEnum.Saboteur) Chat().AddChat(
                    LocalPlayer, "The Saboteur is an impostor modifier that passively reduces non-door sabotage cooldowns.");
                if (modifier == ModifierEnum.Frosty) Chat().AddChat(
                    LocalPlayer, "The Frosty is a crewmate modifier that reduces the speed of their killer temporarily.");
                    
            }
            public static void AddAbilityMessage(AbilityEnum ability)
            {
                if (ability == AbilityEnum.Assassin) Chat().AddChat(
                    LocalPlayer, "The Assassin is an ability which is given to killers to guess other player's roles during mettings. If they guess correctly they kill the other player, if not, they die instead.");
                if (ability == AbilityEnum.Flash) Chat().AddChat(
                    LocalPlayer, "The Flash is a global modifier that increases the speed of a player.");
                if (ability == AbilityEnum.ButtonBarry) Chat().AddChat(
                    LocalPlayer, "The Button Barry allows the player to button from anywhere on the map.");
                if (ability == AbilityEnum.Tiebreaker) Chat().AddChat(
                    LocalPlayer, "The Tiebreaker is an ability that allows a player's vote to break ties in meetings.");
                if (ability == AbilityEnum.Multitasker) Chat().AddChat(
                    LocalPlayer, "The Multitasker makes their tasks slightly transparent.");
                if (ability == AbilityEnum.Drunk) Chat().AddChat(LocalPlayer,
                    "The Drunk is an ability that makes the players movements reversed, if they hit to go up, they will go down etc.");
                if (ability == AbilityEnum.Paranoiac) Chat().AddChat(
                    LocalPlayer, "The Paranoiac shows an arrow pointing to the closest player.");
                if (ability == AbilityEnum.Torch) Chat().AddChat(
                    LocalPlayer, "The Torch is a crewmate modifier that allows them to see when lights are off. Killers or players with impostor vision cannot gain this ability.");
                if (ability == AbilityEnum.Spy) Chat().AddChat(
                    LocalPlayer, "The Spy can see the colours of players on the admin table and the death timing on vitals panel.");
            }
        }

        [HarmonyPatch(typeof(ChatBubble), nameof(ChatBubble.SetName))]
        public static class SetName
        {
            public static void Postfix(ChatBubble __instance, [HarmonyArgument(0)] string playerName)
            {
                if (LocalPlayer.Is(RoleEnum.Jailor) && Meeting())
                {
                    var jailor = GetRole<Jailor>(LocalPlayer);
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
                if (LocalPlayer.IsJailed() && Meeting())
                {
                    if (JailorMessage)
                    {
                        __instance.NameText.color = ColorManager.Jailor;
                        __instance.NameText.text = "Jailor";
                        JailorMessage = false;
                    }
                }
            }
        }
    }
}