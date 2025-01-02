using AmongUs.Data;

namespace TownOfSushi.Roles.Abilities
{
    public class Chameleon : Ability
    {
        public bool Moving = false;
        public DateTime LastMoved { get; set; }
        public float Opacity = 1;
        public Chameleon(PlayerControl player) : base(player)
        {
            Name = "Chameleon";
            TaskText = () => "You're hard to see when you're not moving";
            Color = Colors.Chameleon;
            AbilityType = AbilityEnum.Chameleon;
            LastMoved = DateTime.UtcNow;
        }
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class ChameleonUpdate
    {
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;

            foreach (var modifier in AllAbilities.Where(x => x.AbilityType == AbilityEnum.Chameleon))
            {
                var chameleon = (Chameleon)modifier;
                var player = chameleon.Player;
                if (!player.Data.IsDead && !player.Data.Disconnected && player.GetCustomOutfitType() == CustomPlayerOutfitType.Default && player.MyPhysics.body.velocity.magnitude == 0 && chameleon.Moving)
                {
                    chameleon.Moving = false;
                    chameleon.LastMoved = DateTime.UtcNow;
                }
                if (chameleon.Moving) continue;
                if (player.GetCustomOutfitType() == CustomPlayerOutfitType.Swooper)
                {
                    chameleon.Opacity = 0f;
                    if (PlayerControl.LocalPlayer.Data.IsImpostor() && player.Is(RoleEnum.Swooper) ||
                        PlayerControl.LocalPlayer == player && player.Is(RoleEnum.Swooper)) chameleon.Opacity = 0.1f;
                    SetVisiblity(player, chameleon.Opacity, true);
                    chameleon.Moving = true;
                    continue;
                }
                else if (player.GetCustomOutfitType() == CustomPlayerOutfitType.Camouflage)
                {
                    chameleon.Opacity = 1f;
                    SetVisiblity(player, chameleon.Opacity, true);
                    chameleon.Moving = true;
                    continue;
                }
                else if (player.Data.IsDead || player.GetCustomOutfitType() == CustomPlayerOutfitType.Morph || player.MyPhysics.body.velocity.magnitude > 0)
                {
                    chameleon.Opacity = 1f;
                    SetVisiblity(player, chameleon.Opacity);
                    chameleon.Moving = true;
                    continue;
                }
                var timeSpan = DateTime.UtcNow - chameleon.LastMoved;
                if (timeSpan.TotalMilliseconds / 1000f < CustomGameOptions.InvisDelay) continue;
                else if (timeSpan.TotalMilliseconds / 1000f < CustomGameOptions.TransformInvisDuration + CustomGameOptions.InvisDelay)
                {
                    timeSpan = DateTime.UtcNow - chameleon.LastMoved.AddSeconds(CustomGameOptions.InvisDelay);
                    chameleon.Opacity = 1f - ((float)timeSpan.TotalMilliseconds / 1000f / CustomGameOptions.TransformInvisDuration * (100f - CustomGameOptions.FinalTransparency) / 100f);
                    SetVisiblity(player, chameleon.Opacity);
                    continue;
                }
                else
                {
                    chameleon.Opacity = CustomGameOptions.FinalTransparency / 100;
                    SetVisiblity(player, chameleon.Opacity);
                    continue;
                }
            }
        }

        public static void SetVisiblity(PlayerControl player, float transparency, bool swooped = false)
        {
            var colour = player.myRend().color;
            var cosmetics = player.cosmetics;
            colour.a = transparency;
            player.myRend().color = colour;
            if (swooped) transparency = 0f;
            cosmetics.nameText.color = cosmetics.nameText.color.SetAlpha(transparency);
            if (DataManager.Settings.Accessibility.ColorBlindMode) cosmetics.colorBlindText.color = cosmetics.colorBlindText.color.SetAlpha(transparency);
            player.SetHatAndVisorAlpha(transparency);
            cosmetics.skin.layer.color = cosmetics.skin.layer.color.SetAlpha(transparency);
            foreach (var rend in player.cosmetics.currentPet.renderers)
                rend.color = rend.color.SetAlpha(transparency);
            foreach (var shadow in player.cosmetics.currentPet.shadows)
                shadow.color = shadow.color.SetAlpha(transparency);
        }
    }
}