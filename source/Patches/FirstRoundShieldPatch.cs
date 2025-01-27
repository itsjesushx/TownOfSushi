namespace TownOfSushi.Patches
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class ShowRoundOneShield
    {
        public static Color ShieldColor = Color.blue;
        public static string DiedFirst = "";
        public static PlayerControl FirstRoundShielded = null;
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public static class PlayerControlUpdatePatch
    {
        static void SetBasePlayerOutlines() 
        {
            foreach (PlayerControl target in PlayerControl.AllPlayerControls) 
            {
                if (target == null || target.cosmetics?.currentBodySprite?.BodySprite == null) continue;

                bool isMorphedMorphling = false;
                foreach (var morphlingRole in GetRoles(RoleEnum.Morphling))
                {
                    var morphling = (Morphling)morphlingRole;
                    isMorphedMorphling = target == morphling.Player && morphling.MorphedPlayer != null && morphling.Morphed;
                }
                bool isMorphedHitman = false;
                foreach (var hitmanRole in GetRoles(RoleEnum.Hitman))
                {
                    var hitman = (Hitman)hitmanRole;
                    isMorphedHitman = target == hitman.Player && hitman.MorphTarget != null && hitman.IsUsingMorph;
                }
                bool isMorphedGlitch = false;
                foreach (var glitchRole in GetRoles(RoleEnum.Glitch))
                {
                    var glitch = (Glitch)glitchRole;
                    isMorphedGlitch = target == glitch.Player && glitch.MimicTarget != null && glitch.IsUsingMimic;
                }
                bool isMorphedVenerer = false;
                foreach (var venererRole in GetRoles(RoleEnum.Venerer))
                {
                    var venerer = (Venerer)venererRole;
                    isMorphedVenerer = target == venerer.Player && venerer.Enabled;
                }
                bool isSwoopedSwooper = false;
                foreach (var swooperRole in GetRoles(RoleEnum.Swooper))
                {
                    var swooper = (Swooper)swooperRole;
                    isSwoopedSwooper = target == swooper.Player && swooper.Enabled;
                }

                bool hasVisibleShield = false;
                Color color = Color.cyan;

                if (!CamouflageUnCamouflagePatch.IsCamouflaged && ShowRoundOneShield.FirstRoundShielded != null && CustomGameOptions.FirstDeathShield && ((target == ShowRoundOneShield.FirstRoundShielded && !isMorphedMorphling && !isMorphedGlitch && !isMorphedVenerer && !isSwoopedSwooper) || (isMorphedMorphling && GetRole<Morphling>(target).MorphedPlayer == ShowRoundOneShield.FirstRoundShielded) || (isMorphedGlitch && GetRole<Glitch>(target).MimicTarget == ShowRoundOneShield.FirstRoundShielded))) {
                    hasVisibleShield = true;
                    color = ShowRoundOneShield.ShieldColor;
                }

                foreach (var medicRole in GetRoles(RoleEnum.Medic))
                {
                    var medic = (Medic)medicRole;

                    if (!CamouflageUnCamouflagePatch.IsCamouflaged && medic.Player != null && medic.ShieldedPlayer != null && ((target == medic.ShieldedPlayer && !isMorphedMorphling && !isMorphedHitman && !isMorphedGlitch && !isMorphedVenerer && !isSwoopedSwooper) || (isMorphedMorphling && GetRole<Morphling>(target).MorphedPlayer == medic.ShieldedPlayer) || (isMorphedGlitch && GetRole<Glitch>(target).MimicTarget == medic.ShieldedPlayer)|| (isMorphedHitman && GetRole<Hitman>(target).MorphTarget == medic.ShieldedPlayer))) {
                        hasVisibleShield = CustomGameOptions.ShowShielded == ShieldOptions.Everyone // Everyone
                            || CustomGameOptions.ShowShielded == ShieldOptions.Medic && PlayerControl.LocalPlayer == medic.Player // Medic Only
                            || CustomGameOptions.ShowShielded == ShieldOptions.Self && PlayerControl.LocalPlayer == medic.ShieldedPlayer // Shield only
                            || CustomGameOptions.ShowShielded == ShieldOptions.SelfAndMedic && (PlayerControl.LocalPlayer == medic.Player || PlayerControl.LocalPlayer == medic.ShieldedPlayer); // Medic + Shield
                        color = Color.cyan;
                    }
                }

                foreach (var gaRole in GetRoles(RoleEnum.GuardianAngel))
                {
                    var ga = (GuardianAngel)gaRole;

                    if (!CamouflageUnCamouflagePatch.IsCamouflaged && ga.Protecting && ga.Player != null && ga.target != null && ((target == ga.target && !isMorphedMorphling && !isMorphedGlitch && !isMorphedVenerer && !isSwoopedSwooper) || (isMorphedMorphling && GetRole<Morphling>(target).MorphedPlayer == ga.target) || (isMorphedGlitch && GetRole<Glitch>(target).MimicTarget == ga.target)|| (isMorphedHitman && GetRole<Hitman>(target).MorphTarget == ga.target))) {
                        hasVisibleShield = CustomGameOptions.ShowProtect == ProtectOptions.Everyone // Everyone
                            || CustomGameOptions.ShowProtect == ProtectOptions.GA && PlayerControl.LocalPlayer == ga.Player // GA Only
                            || CustomGameOptions.ShowProtect == ProtectOptions.Self && PlayerControl.LocalPlayer == ga.target // target only
                            || CustomGameOptions.ShowProtect == ProtectOptions.SelfAndGA && (PlayerControl.LocalPlayer == ga.Player || PlayerControl.LocalPlayer == ga.target); // ga + target
                        color = Color.yellow;
                    }
                }

                if (hasVisibleShield) 
                {
                    target.cosmetics.currentBodySprite.BodySprite.material.SetFloat("_Outline", 1f);
                    target.cosmetics.currentBodySprite.BodySprite.material.SetColor("_OutlineColor", color);
                }
            }
        }

        public static void Postfix(PlayerControl __instance) 
        {
            if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started || GameOptionsManager.Instance.currentGameOptions.GameMode == GameModes.HideNSeek) return;
            
            if (PlayerControl.LocalPlayer == __instance) {
                // Update player outlines
                SetBasePlayerOutlines();
            }
        }
    }
}