namespace TownOfSushi.Roles
{
    public class Werewolf : Role
    {
        private KillButton _maulButton;
        public PlayerControl ClosestPlayer;
        public DateTime LastMauled;
        public Werewolf(PlayerControl player) : base(player)
        {
            Name = "Werewolf";
            StartText = () => "Murder and eliminate everyone";
            TaskText = () => "Kill everyone";
            RoleInfo = "The Werewolf can kill all players within a certain radius.";
            LoreText = "As a Werewolf, you are cursed with a savage hunger that can only be sated through bloodshed. Under the full moon, your instincts take over, and you become a fearsome predator, stalking and murdering those who stand in your way. No one is safe from your ferocity. As you hunt and kill, your power only grows stronger, until nothing but the remnants of your victims remain. Your goal is simple: eliminate everyone, and let the world fear your name.";
            Color = ColorManager.Werewolf;
            LastMauled = DateTime.UtcNow;
            RoleType = RoleEnum.Werewolf;
            AddToRoleHistory(RoleType);
            Faction = Faction.Neutral;
            RoleAlignment = RoleAlignment.NeutralKilling;
        }
        public KillButton MaulButton
        {
            get => _maulButton;
            set
            {
                _maulButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }
        public float MaulTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastMauled;
            var num = CustomGameOptions.MaulCooldown * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public void Maul()
        {
            foreach (var player in GetClosestPlayers(Player.GetTruePosition(), CustomGameOptions.MaulRadius))
            {
                if (player.IsProtected() || Player == player || player.IsFortified() || ClosestPlayer == player || player.IsShielded() || player == ShowRoundOneShield.FirstRoundShielded)
                    continue;

                if (!player.Is(RoleEnum.Pestilence))
                    RpcMurderPlayerNoJump(Player, player);

                if (player.IsOnAlert() || player.Is(RoleEnum.Pestilence))
                    RpcMurderPlayer(player, Player);

                if (player.IsInfected() || Player.IsInfected())
                {
                    foreach (var pb in GetRoles(RoleEnum.Plaguebearer))
                        ((Plaguebearer)pb).RpcSpreadInfection(player, Player);
                }
            }
        }
    }

    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformMaul
    {
        public static bool Prefix(KillButton __instance)
        {
            if (NoButton(LocalPlayer(), RoleEnum.Werewolf))
                return false;

            var role = GetRole<Werewolf>(LocalPlayer());

            if (__instance == role.MaulButton)
            {
                if (!ButtonUsable(__instance))
                    return false;

                if (role.MaulTimer() != 0f)
                    return false;

                if (IsTooFar(role.Player, role.ClosestPlayer))
                    return false;
                
                var interact = Interact(LocalPlayer(), role.ClosestPlayer, true);

                if (interact[3] == true)
                {
                    role.Maul();
                    StartRPC(CustomRPC.Maul, LocalPlayer().PlayerId);
                }
                
                if (interact[0] == true)
                    role.LastMauled = DateTime.UtcNow;
                else if (interact[1] == true)
                    role.LastMauled.AddSeconds(CustomGameOptions.ProtectKCReset);
                else if (interact[2] == true)
                return false;
            }

            return false;
        }
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDMaul
    {
        public static Sprite Maul => TownOfSushi.MaulSprite;

        public static void Postfix(HudManager __instance)
        {
            if (NoButton(LocalPlayer(), RoleEnum.Werewolf))
                return;

            var role = GetRole<Werewolf>(LocalPlayer());

            if (role.MaulButton == null)
            {
                role.MaulButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.MaulButton.graphic.enabled = true;
                role.MaulButton.graphic.sprite = Maul;
                role.MaulButton.gameObject.SetActive(false);
            }

            role.MaulButton.gameObject.SetActive(SetActive(role.Player, __instance));
            role.MaulButton.SetCoolDown(role.MaulTimer(), CustomGameOptions.MaulCooldown);
            SetTarget(ref role.ClosestPlayer, role.MaulButton);
            var renderer = role.MaulButton.graphic;
            
            if (role.ClosestPlayer != null && !role.MaulButton.isCoolingDown)
            {
                renderer.color = Palette.EnabledColor;
                renderer.material.SetFloat("_Desat", 0f);
            }
            else
            {
                renderer.color = Palette.DisabledClear;
                renderer.material.SetFloat("_Desat", 1f);
            }
        }
    }
}