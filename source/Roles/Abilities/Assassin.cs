﻿namespace TownOfSushi.Roles.Modifiers
{
    public class Assassin : Ability
    {
        private readonly Dictionary<string, Color> ColorMapping = new();
        public Dictionary<string, Color> SortedColorMapping = new();
        public int RemainingKills { get; set; }
        public Assassin(PlayerControl player) : base(player)
        {
            Name = "Assassin";
            TaskText = () => "Guess and shoot players in meetings";
            Color = Colors.Impostor;
            AbilityType = AbilityEnum.Assassin;
            RemainingKills = CustomGameOptions.AssassinKills;
            
            // Adds all the roles that have a > 0 chance of being in the game.
            if (CustomGameOptions.AssassinCrewmateGuess) ColorMapping.Add("Crewmate", Colors.Crewmate);
            if (CustomGameOptions.EngineerOn > 0) ColorMapping.Add("Engineer", Colors.Engineer);
            if (CustomGameOptions.ImitatorOn > 0) ColorMapping.Add("Imitator", Colors.Imitator);
            if (CustomGameOptions.SwapperOn > 0) ColorMapping.Add("Swapper", Colors.Swapper);
            if (CustomGameOptions.HunterOn > 0) ColorMapping.Add("Hunter", Colors.Hunter);
            if (CustomGameOptions.InvestigatorOn > 0) ColorMapping.Add("Investigator", Colors.Investigator);
            if (CustomGameOptions.MedicOn > 0) ColorMapping.Add("Medic", Colors.Medic);
            if (CustomGameOptions.MediumOn > 0) ColorMapping.Add("Medium", Colors.Medium);
            if (CustomGameOptions.MysticOn > 0) ColorMapping.Add("Mystic", Colors.Mystic);
            // this will be gone for now 
                //if (CustomGameOptions.JailorOn > 0) ColorMapping.Add("Jailor", Colors.Jailor);
            if (CustomGameOptions.OracleOn > 0) ColorMapping.Add("Oracle", Colors.Oracle);
            if (CustomGameOptions.SeerOn > 0) ColorMapping.Add("Seer", Colors.Seer);
            if (CustomGameOptions.SnitchOn > 0) ColorMapping.Add("Snitch", Colors.Snitch);
            if (CustomGameOptions.TrackerOn > 0) ColorMapping.Add("Tracker", Colors.Tracker);
            if (CustomGameOptions.TransporterOn > 0) ColorMapping.Add("Transporter", Colors.Transporter);
            if (CustomGameOptions.TrapperOn > 0) ColorMapping.Add("Trapper", Colors.Trapper);
            if (CustomGameOptions.VeteranOn > 0 ) ColorMapping.Add("Veteran", Colors.Veteran);
            if (CustomGameOptions.VigilanteOn > 0 ) ColorMapping.Add("Vigilante", Colors.Vigilante);
            
            if (CustomGameOptions.ArsonistOn > 0 && !PlayerControl.LocalPlayer.Is(RoleEnum.Arsonist)) ColorMapping.Add("Arsonist", Colors.Arsonist);
            if (CustomGameOptions.AgentOn > 0 && !PlayerControl.LocalPlayer.Is(RoleEnum.Hitman)) ColorMapping.Add("Hitman", Colors.Hitman);
            if (CustomGameOptions.AgentOn > 0 && !PlayerControl.LocalPlayer.Is(RoleEnum.Agent)) ColorMapping.Add("Agent", Colors.Agent);
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Juggernaut) && CustomGameOptions.JuggernautOn > 0) ColorMapping.Add("Juggernaut", Colors.Juggernaut);
            if (CustomGameOptions.PlaguebearerOn > 0 && !PlayerControl.LocalPlayer.Is(RoleEnum.Plaguebearer)) ColorMapping.Add("Plaguebearer", Colors.Plaguebearer);
            if (CustomGameOptions.PlaguebearerOn > 0 && !PlayerControl.LocalPlayer.Is(RoleEnum.Pestilence)) ColorMapping.Add("Pestilence", Colors.Pestilence);
            if (CustomGameOptions.WerewolfOn > 0) ColorMapping.Add("Werewolf", Colors.Werewolf);
            if (CustomGameOptions.GlitchOn > 0 && !PlayerControl.LocalPlayer.Is(RoleEnum.Glitch)) ColorMapping.Add("Glitch", Colors.Glitch);
            if (CustomGameOptions.GameMode == GameMode.Classic && CustomGameOptions.VampireOn > 0 && !PlayerControl.LocalPlayer.Is(RoleEnum.Vampire)) ColorMapping.Add("Vampire", Colors.Vampire);
            if (CustomGameOptions.SerialKillerOn > 0 && !PlayerControl.LocalPlayer.Is(RoleEnum.SerialKiller)) ColorMapping.Add("Serial Killer", Colors.SerialKiller);

            if (!PlayerControl.LocalPlayer.Is(Faction.Impostors)) ColorMapping.Add("Impostor", Colors.Impostor);
            if (CustomGameOptions.BomberOn > 0 && !PlayerControl.LocalPlayer.Is(Faction.Impostors)) ColorMapping.Add("Bomber", Colors.Impostor);
            if (CustomGameOptions.BlackmailerOn > 0 && !PlayerControl.LocalPlayer.Is(Faction.Impostors)) ColorMapping.Add("Blackmailer", Colors.Impostor);
            if (CustomGameOptions.EscapistOn > 0 && !PlayerControl.LocalPlayer.Is(Faction.Impostors)) ColorMapping.Add("Escapist", Colors.Impostor);
            if (CustomGameOptions.WitchOn > 0 && !PlayerControl.LocalPlayer.Is(Faction.Impostors)) ColorMapping.Add("Witch", Colors.Impostor);
            if (CustomGameOptions.GrenadierOn > 0 && !PlayerControl.LocalPlayer.Is(Faction.Impostors)) ColorMapping.Add("Grenadier", Colors.Impostor);
            if (CustomGameOptions.JanitorOn > 0 && !PlayerControl.LocalPlayer.Is(Faction.Impostors)) ColorMapping.Add("Janitor", Colors.Impostor);
            if (CustomGameOptions.MorphlingOn > 0 && !PlayerControl.LocalPlayer.Is(Faction.Impostors)) ColorMapping.Add("Morphling", Colors.Impostor);
            if (CustomGameOptions.MinerOn > 0 && !PlayerControl.LocalPlayer.Is(Faction.Impostors)) ColorMapping.Add("Miner", Colors.Impostor);
            if (CustomGameOptions.SwooperOn > 0 && !PlayerControl.LocalPlayer.Is(Faction.Impostors)) ColorMapping.Add("Swooper", Colors.Impostor);
            if (CustomGameOptions.VenererOn > 0 && !PlayerControl.LocalPlayer.Is(Faction.Impostors)) ColorMapping.Add("Venerer", Colors.Impostor);
            if (CustomGameOptions.UndertakerOn > 0 && !PlayerControl.LocalPlayer.Is(Faction.Impostors)) ColorMapping.Add("Undertaker", Colors.Impostor);
            if (CustomGameOptions.WarlockOn > 0 && !PlayerControl.LocalPlayer.Is(Faction.Impostors)) ColorMapping.Add("Warlock", Colors.Impostor);

            // Add Neutral roles if enabled
            if (CustomGameOptions.AssassinGuessNeutralBenign)
            {
                if (CustomGameOptions.GuardianAngelOn > 0) ColorMapping.Add("Guardian Angel", Colors.GuardianAngel);
                if (CustomGameOptions.RomanticOn > 0) ColorMapping.Add("Romantic", Colors.Romantic);
                if (CustomGameOptions.AmnesiacOn > 0 || (CustomGameOptions.ExecutionerOn > 0 && CustomGameOptions.OnTargetDead == OnTargetDead.Amnesiac) || (CustomGameOptions.GuardianAngelOn > 0 && CustomGameOptions.GaOnTargetDeath == BecomeOptions.Amnesiac) || (CustomGameOptions.RomanticOn > 0 && CustomGameOptions.RomanticOnBelovedDeath == RomanticBecomeOptions.Amnesiac)) ColorMapping.Add("Amnesiac", Colors.Amnesiac);
            }
            if (CustomGameOptions.AssassinGuessNeutralEvil)
            {
                if (CustomGameOptions.DoomsayerOn > 0) ColorMapping.Add("Doomsayer", Colors.Doomsayer);
                if (CustomGameOptions.VultureOn > 0) ColorMapping.Add("Vulture", Colors.Vulture);
                if (CustomGameOptions.ExecutionerOn > 0) ColorMapping.Add("Executioner", Colors.Executioner);
                if (CustomGameOptions.JesterOn > 0 || (CustomGameOptions.ExecutionerOn > 0 && CustomGameOptions.OnTargetDead == OnTargetDead.Jester) || (CustomGameOptions.GuardianAngelOn > 0 && CustomGameOptions.GaOnTargetDeath == BecomeOptions.Jester)) ColorMapping.Add("Jester", Colors.Jester);
            }
            //Add modifiers
            if (CustomGameOptions.BaitOn > 0) ColorMapping.Add("Bait", Colors.Bait);
            if (CustomGameOptions.TorchOn > 0) ColorMapping.Add("Torch", Colors.Torch);
            if (CustomGameOptions.MultitaskerOn > 0) ColorMapping.Add("Multitasker", Colors.Multitasker);
            if (CustomGameOptions.DiseasedOn > 0) ColorMapping.Add("Diseased", Colors.Diseased);
            if (CustomGameOptions.AftermathOn > 0) ColorMapping.Add("Aftermath", Colors.Aftermath);
            if (CustomGameOptions.FrostyOn > 0) ColorMapping.Add("Frosty", Colors.Frosty);

            // Sorts the list. 
            SortedColorMapping = ColorMapping.ToDictionary(x => x.Key, x => x.Value);
        }
    }
}
