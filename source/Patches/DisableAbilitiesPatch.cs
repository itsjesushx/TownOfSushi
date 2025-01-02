using System.Collections;
using InnerNet;

namespace TownOfSushi.Patches
{
    public class DisableAbilities
    {
        public class DisableAbility
        {
            public static Dictionary<byte, DateTime> tickDictionary = new();
            public static IEnumerator StopAbility(float duration)
            {
                if (tickDictionary.ContainsKey(PlayerControl.LocalPlayer.PlayerId))
                {
                    if (CustomGameOptions.HackDuration - (DateTime.UtcNow - tickDictionary[PlayerControl.LocalPlayer.PlayerId]).TotalMilliseconds / 1000 < duration)
                    {
                        tickDictionary[PlayerControl.LocalPlayer.PlayerId] = DateTime.UtcNow.AddSeconds((tickDictionary[PlayerControl.LocalPlayer.PlayerId] - DateTime.UtcNow).TotalMilliseconds / 1000);
                    }
                    yield break;
                }
                tickDictionary.Add(PlayerControl.LocalPlayer.PlayerId, DateTime.UtcNow);

                while (true)
                {
                    var disableKill = true;
                    var disableExtra = true;
                    
                    if (PlayerControl.LocalPlayer.Is(RoleEnum.Hunter))
                    {
                        var hunter = GetRole<Hunter>(PlayerControl.LocalPlayer);
                        if (hunter.Stalking) disableExtra = false;
                    }
                    else if (PlayerControl.LocalPlayer.Is(RoleEnum.Veteran))
                    {
                        var veteran = GetRole<Veteran>(PlayerControl.LocalPlayer);
                        if (veteran.OnAlert) disableKill = false;
                    }
                    else if (PlayerControl.LocalPlayer.Is(RoleEnum.GuardianAngel))
                    {
                        var ga = GetRole<GuardianAngel>(PlayerControl.LocalPlayer);
                        if (ga.Protecting) disableKill = false;
                    }
                    else if (PlayerControl.LocalPlayer.Is(RoleEnum.Bomber))
                    {
                        var bomber = GetRole<Bomber>(PlayerControl.LocalPlayer);
                        if (bomber.Detonating) disableExtra = false;
                    }
                    else if (PlayerControl.LocalPlayer.Is(RoleEnum.Grenadier))
                    {
                        var gren = GetRole<Grenadier>(PlayerControl.LocalPlayer);
                        if (gren.Flashed) disableExtra = false;
                    }
                    else if (PlayerControl.LocalPlayer.Is(RoleEnum.Venerer))
                    {
                        var venerer = GetRole<Venerer>(PlayerControl.LocalPlayer);
                        if (venerer.IsCamouflaged) disableExtra = false;
                    }
                    else if (PlayerControl.LocalPlayer.Is(RoleEnum.Swooper))
                    {
                        var swooper = GetRole<Swooper>(PlayerControl.LocalPlayer);
                        if (swooper.IsSwooped) disableExtra = false;
                    }
                    else if (PlayerControl.LocalPlayer.Is(RoleEnum.Morphling))
                    {
                        var morph = GetRole<Morphling>(PlayerControl.LocalPlayer);
                        if (morph.Morphed) disableExtra = false;
                    }
                    else if (PlayerControl.LocalPlayer.Is(RoleEnum.Hitman))
                    {
                        var morph = GetRole<Hitman>(PlayerControl.LocalPlayer);
                        if (morph.IsUsingMorph) disableExtra = false;
                        if (!morph.CurrentlyDragging)
                            HitmanKillButtonTarget.SetTarget(HudManager.Instance.KillButton, null, morph);
                    }
                    else if (PlayerControl.LocalPlayer.Is(RoleEnum.Glitch))
                    {
                        var glitch = GetRole<Glitch>(PlayerControl.LocalPlayer);
                        if (glitch.IsUsingMimic) disableExtra = false;
                    }
                    else if (PlayerControl.LocalPlayer.Is(RoleEnum.SerialKiller))
                    {
                        var ww = GetRole<SerialKiller>(PlayerControl.LocalPlayer);
                        if (ww.Stabbed) disableExtra = false;
                    }

                    if (HudManager.Instance.KillButton != null && disableKill)
                    {
                        HudManager.Instance.KillButton.enabled = false;
                        HudManager.Instance.KillButton.graphic.color = Palette.DisabledClear;
                        HudManager.Instance.KillButton.graphic.material.SetFloat("_Desat", 1f);
                    }
                    if (PlayerControl.LocalPlayer.Is(RoleEnum.Janitor)) JanitorKillButtonTarget.SetTarget(HudManager.Instance.KillButton, null, GetRole<Janitor>(PlayerControl.LocalPlayer));
                    else if (PlayerControl.LocalPlayer.Is(RoleEnum.Vulture)) VultureKillButtonTarget.SetTarget(HudManager.Instance.KillButton, null, GetRole<Vulture>(PlayerControl.LocalPlayer));
                    else if (PlayerControl.LocalPlayer.Is(RoleEnum.Undertaker))
                    {
                        var undertaker = GetRole<Undertaker>(PlayerControl.LocalPlayer);
                        if (!undertaker.CurrentlyDragging)
                            UndertakerKillButtonTarget.SetTarget(HudManager.Instance.KillButton, null, undertaker);
                    }
                    else if (PlayerControl.LocalPlayer.Is(RoleEnum.Engineer))
                    {
                        var engi = GetRole<Engineer>(PlayerControl.LocalPlayer);
                        engi.UsesText.color = Palette.DisabledClear;
                        engi.UsesText.material.SetFloat("_Desat", 1f);
                    }
                    else if (PlayerControl.LocalPlayer.Is(RoleEnum.Tracker))
                    {
                        var track = GetRole<Tracker>(PlayerControl.LocalPlayer);
                        track.UsesText.color = Palette.DisabledClear;
                        track.UsesText.material.SetFloat("_Desat", 1f);
                    }
                    else if (PlayerControl.LocalPlayer.Is(RoleEnum.Transporter))
                    {
                        var trans = GetRole<Transporter>(PlayerControl.LocalPlayer);
                        trans.UsesText.color = Palette.DisabledClear;
                        trans.UsesText.material.SetFloat("_Desat", 1f);
                    }
                    else if (PlayerControl.LocalPlayer.Is(RoleEnum.Trapper))
                    {
                        var trap = GetRole<Trapper>(PlayerControl.LocalPlayer);
                        trap.UsesText.color = Palette.DisabledClear;
                        trap.UsesText.material.SetFloat("_Desat", 1f);
                    }
                    else if (PlayerControl.LocalPlayer.Is(RoleEnum.Veteran) && disableKill)
                    {
                        var vet = GetRole<Veteran>(PlayerControl.LocalPlayer);
                        vet.UsesText.color = Palette.DisabledClear;
                        vet.UsesText.material.SetFloat("_Desat", 1f);
                    }
                    else if (PlayerControl.LocalPlayer.Is(RoleEnum.GuardianAngel) && disableKill)
                    {
                        var ga = GetRole<GuardianAngel>(PlayerControl.LocalPlayer);
                        ga.UsesText.color = Palette.DisabledClear;
                        ga.UsesText.material.SetFloat("_Desat", 1f);
                    }

                    var role = GetPlayerRole(PlayerControl.LocalPlayer);
                    if (role?.ExtraButtons.Count > 0 && disableExtra)
                    {
                        role.ExtraButtons[0].enabled = false;
                        role.ExtraButtons[0].graphic.color = Palette.DisabledClear;
                        role.ExtraButtons[0].graphic.material.SetFloat("_Desat", 1f);
                    }

                    if (PlayerControl.LocalPlayer.Is(RoleEnum.Investigator)) GetRole<Investigator>(PlayerControl.LocalPlayer).ExamineButton.SetTarget(null);
                    else if (PlayerControl.LocalPlayer.Is(RoleEnum.Hunter) && disableExtra)
                    {
                        var hunter = GetRole<Hunter>(PlayerControl.LocalPlayer);
                        hunter.StalkButton.SetTarget(null);
                        hunter.UsesText.color = Palette.DisabledClear;
                        hunter.UsesText.material.SetFloat("_Desat", 1f);
                    }
                    else if (PlayerControl.LocalPlayer.Is(RoleEnum.Arsonist)) GetRole<Arsonist>(PlayerControl.LocalPlayer).IgniteButton.SetTarget(null);
                    else if (PlayerControl.LocalPlayer.Is(RoleEnum.Blackmailer)) GetRole<Blackmailer>(PlayerControl.LocalPlayer).BlackmailButton.SetTarget(null);
                    else if (PlayerControl.LocalPlayer.Is(RoleEnum.Morphling)) GetRole<Morphling>(PlayerControl.LocalPlayer).MorphButton.SetTarget(null);
                    else if (PlayerControl.LocalPlayer.Is(RoleEnum.Hitman)) GetRole<Hitman>(PlayerControl.LocalPlayer).MorphButton.SetTarget(null);

                    if (PlayerControl.LocalPlayer.Is(RoleEnum.Glitch))
                    {
                        var glitch = GetRole<Glitch>(PlayerControl.LocalPlayer);
                        if (disableExtra)
                        {
                            glitch.MimicButton.enabled = false;
                            glitch.MimicButton.graphic.color = Palette.DisabledClear;
                            glitch.MimicButton.graphic.material.SetFloat("_Desat", 1f);
                        }
                        glitch.HackButton.enabled = false;
                        glitch.HackButton.graphic.color = Palette.DisabledClear;
                        glitch.HackButton.graphic.material.SetFloat("_Desat", 1f);
                    }

                    if (PlayerControl.LocalPlayer.Is(RoleEnum.Hitman))
                    {
                        var Hitman = GetRole<Hitman>(PlayerControl.LocalPlayer);
                        if (disableExtra)
                        {
                            Hitman.MorphButton.enabled = false;
                            Hitman.MorphButton.graphic.color = Palette.DisabledClear;
                            Hitman.MorphButton.graphic.material.SetFloat("_Desat", 1f);
                            Hitman.DragDropButtonHitman.graphic.color = Palette.DisabledClear;
                            Hitman.DragDropButtonHitman.graphic.material.SetFloat("_Desat", 1f);
                        }
                    }

                    var disableTimer = (DateTime.UtcNow - tickDictionary[PlayerControl.LocalPlayer.PlayerId]).TotalMilliseconds/1000;
                    if (MeetingHud.Instance || disableTimer > duration || PlayerControl.LocalPlayer?.Data.IsDead != false || AmongUsClient.Instance.GameState != InnerNetClient.GameStates.Started)
                    {
                        HudManager.Instance.KillButton.enabled = true;
                        if (role?.ExtraButtons.Count > 0) role.ExtraButtons[0].enabled = true;
                        if (PlayerControl.LocalPlayer.Is(RoleEnum.Glitch))
                        {
                            var glitch = GetRole<Glitch>(PlayerControl.LocalPlayer);
                            glitch.MimicButton.enabled = true;
                            glitch.HackButton.enabled = true;
                        }
                        if (PlayerControl.LocalPlayer.Is(RoleEnum.Hitman))
                        {
                            var glitch = GetRole<Hitman>(PlayerControl.LocalPlayer);
                            glitch.MorphButton.enabled = true;
                        }

                        tickDictionary.Remove(PlayerControl.LocalPlayer.PlayerId);
                        yield break;
                    }

                    yield return null;
                }
            }
        }

        [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.ClimbLadder))]
        public class SaveLadderPlayer
        {
            public static void Prefix(PlayerPhysics __instance, [HarmonyArgument(0)] Ladder source, [HarmonyArgument(1)] byte climbLadderSid)
            {
                if (PlayerControl.LocalPlayer.PlayerId == __instance.myPlayer.PlayerId) Coroutines.Start(DisableAbility.StopAbility(0.75f));
            }
        }

        [HarmonyPatch(typeof(MovingPlatformBehaviour), nameof(MovingPlatformBehaviour.Use), new Type[] {})]
        public class SavePlatformPlayer
        {
            public static void Prefix(MovingPlatformBehaviour __instance)
            {
                Coroutines.Start(DisableAbility.StopAbility(1.5f));
            }
        }

        [HarmonyPatch(typeof(ZiplineBehaviour), nameof(ZiplineBehaviour.Use), new Type[] { typeof(PlayerControl), typeof(bool)})]
        public class SaveZiplinePlayer
        {
            public static void Prefix(ZiplineBehaviour __instance, [HarmonyArgument(0)] PlayerControl player, [HarmonyArgument(1)] bool fromTop)
            {
                Coroutines.Start(DisableAbility.StopAbility(2.5f));
            }
        }
    }
}