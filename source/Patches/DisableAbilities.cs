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
                if (tickDictionary.ContainsKey(LocalPlayer().PlayerId))
                {
                    if (CustomGameOptions.HackDuration - (DateTime.UtcNow - tickDictionary[LocalPlayer().PlayerId]).TotalMilliseconds / 1000 < duration)
                    {
                        tickDictionary[LocalPlayer().PlayerId] = DateTime.UtcNow.AddSeconds((tickDictionary[LocalPlayer().PlayerId] - DateTime.UtcNow).TotalMilliseconds / 1000);
                    }
                    yield break;
                }
                tickDictionary.Add(LocalPlayer().PlayerId, DateTime.UtcNow);

                while (true)
                {
                    var disableKill = true;
                    var disableExtra = true;
                    
                    if (LocalPlayer().Is(RoleEnum.Hunter))
                    {
                        var hunter = GetRole<Hunter>(LocalPlayer());
                        if (hunter.Stalking) disableExtra = false;
                    }
                    else if (LocalPlayer().Is(RoleEnum.Veteran))
                    {
                        var veteran = GetRole<Veteran>(LocalPlayer());
                        if (veteran.OnAlert) disableKill = false;
                    }
                    else if (LocalPlayer().Is(RoleEnum.GuardianAngel))
                    {
                        var ga = GetRole<GuardianAngel>(LocalPlayer());
                        if (ga.Protecting) disableKill = false;
                    }
                    else if (LocalPlayer().Is(RoleEnum.Bomber))
                    {
                        var bomber = GetRole<Bomber>(LocalPlayer());
                        if (bomber.Detonating) disableExtra = false;
                    }
                    else if (LocalPlayer().Is(RoleEnum.Grenadier))
                    {
                        var gren = GetRole<Grenadier>(LocalPlayer());
                        if (gren.Flashed) disableExtra = false;
                    }
                    else if (LocalPlayer().Is(RoleEnum.Venerer))
                    {
                        var venerer = GetRole<Venerer>(LocalPlayer());
                        if (venerer.IsCamouflaged) disableExtra = false;
                    }
                    else if (LocalPlayer().Is(RoleEnum.Swooper))
                    {
                        var swooper = GetRole<Swooper>(LocalPlayer());
                        if (swooper.IsSwooped) disableExtra = false;
                    }
                    else if (LocalPlayer().Is(RoleEnum.Morphling))
                    {
                        var morph = GetRole<Morphling>(LocalPlayer());
                        if (morph.Morphed) disableExtra = false;
                    }
                    else if (LocalPlayer().Is(RoleEnum.Hitman))
                    {
                        var morph = GetRole<Hitman>(LocalPlayer());
                        if (morph.IsUsingMorph) disableExtra = false;
                        if (!morph.CurrentlyDragging)
                            HitmanKillButtonTarget.SetTarget(HUDManager().KillButton, null, morph);
                    }
                    else if (LocalPlayer().Is(RoleEnum.Glitch))
                    {
                        var glitch = GetRole<Glitch>(LocalPlayer());
                        if (glitch.IsUsingMimic) disableExtra = false;
                    }
                    else if (LocalPlayer().Is(RoleEnum.SerialKiller))
                    {
                        var ww = GetRole<SerialKiller>(LocalPlayer());
                        if (ww.Stabbing) disableExtra = false;
                    }

                    if (HUDManager().KillButton != null && disableKill)
                    {
                        HUDManager().KillButton.enabled = false;
                        HUDManager().KillButton.graphic.color = Palette.DisabledClear;
                        HUDManager().KillButton.graphic.material.SetFloat("_Desat", 1f);
                    }
                    if (LocalPlayer().Is(RoleEnum.Janitor)) JanitorKillButtonTarget.SetTarget(HUDManager().KillButton, null, GetRole<Janitor>(LocalPlayer()));
                    else if (LocalPlayer().Is(RoleEnum.Vulture)) VultureKillButtonTarget.SetTarget(HUDManager().KillButton, null, GetRole<Vulture>(LocalPlayer()));
                    else if (LocalPlayer().Is(RoleEnum.Undertaker))
                    {
                        var undertaker = GetRole<Undertaker>(LocalPlayer());
                        if (!undertaker.CurrentlyDragging)
                            UndertakerKillButtonTarget.SetTarget(HUDManager().KillButton, null, undertaker);
                    }
                    else if (LocalPlayer().Is(RoleEnum.Engineer))
                    {
                        var engi = GetRole<Engineer>(LocalPlayer());
                        engi.UsesText.color = Palette.DisabledClear;
                        engi.UsesText.material.SetFloat("_Desat", 1f);
                    }
                    else if (LocalPlayer().Is(RoleEnum.Lookout))
                    {
                        var lo = GetRole<Lookout>(LocalPlayer());
                        lo.UsesText.color = Palette.DisabledClear;
                        lo.UsesText.material.SetFloat("_Desat", 1f);
                    }
                    else if (LocalPlayer().Is(RoleEnum.Tracker))
                    {
                        var track = GetRole<Tracker>(LocalPlayer());
                        track.UsesText.color = Palette.DisabledClear;
                        track.UsesText.material.SetFloat("_Desat", 1f);
                    }
                    else if (LocalPlayer().Is(RoleEnum.Trapper))
                    {
                        var trap = GetRole<Trapper>(LocalPlayer());
                        trap.UsesText.color = Palette.DisabledClear;
                        trap.UsesText.material.SetFloat("_Desat", 1f);
                    }
                    else if (LocalPlayer().Is(RoleEnum.Veteran) && disableKill)
                    {
                        var vet = GetRole<Veteran>(LocalPlayer());
                        vet.UsesText.color = Palette.DisabledClear;
                        vet.UsesText.material.SetFloat("_Desat", 1f);
                    }
                    else if (LocalPlayer().Is(RoleEnum.GuardianAngel) && disableKill)
                    {
                        var ga = GetRole<GuardianAngel>(LocalPlayer());
                        ga.UsesText.color = Palette.DisabledClear;
                        ga.UsesText.material.SetFloat("_Desat", 1f);
                    }

                    var role = GetPlayerRole(LocalPlayer());
                    if (role?.ExtraButtons.Count > 0 && disableExtra)
                    {
                        role.ExtraButtons[0].enabled = false;
                        role.ExtraButtons[0].graphic.color = Palette.DisabledClear;
                        role.ExtraButtons[0].graphic.material.SetFloat("_Desat", 1f);
                    }

                    if (LocalPlayer().Is(RoleEnum.Investigator)) GetRole<Investigator>(LocalPlayer()).ExamineButton.SetTarget(null);
                    else if (LocalPlayer().Is(RoleEnum.Hunter) && disableExtra)
                    {
                        var hunter = GetRole<Hunter>(LocalPlayer());
                        hunter.StalkButton.SetTarget(null);
                        hunter.UsesText.color = Palette.DisabledClear;
                        hunter.UsesText.material.SetFloat("_Desat", 1f);
                    }
                    else if (LocalPlayer().Is(RoleEnum.Arsonist)) GetRole<Arsonist>(LocalPlayer()).IgniteButton.SetTarget(null);
                    else if (LocalPlayer().Is(RoleEnum.Blackmailer)) GetRole<Blackmailer>(LocalPlayer()).BlackmailButton.SetTarget(null);
                    else if (LocalPlayer().Is(RoleEnum.Morphling)) GetRole<Morphling>(LocalPlayer()).MorphButton.SetTarget(null);
                    else if (LocalPlayer().Is(RoleEnum.Hitman)) GetRole<Hitman>(LocalPlayer()).MorphButton.SetTarget(null);

                    if (LocalPlayer().Is(RoleEnum.Glitch))
                    {
                        var glitch = GetRole<Glitch>(LocalPlayer());
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

                    if (LocalPlayer().Is(RoleEnum.Hitman))
                    {
                        var Hitman = GetRole<Hitman>(LocalPlayer());
                        if (disableExtra)
                        {
                            Hitman.MorphButton.enabled = false;
                            Hitman.MorphButton.graphic.color = Palette.DisabledClear;
                            Hitman.MorphButton.graphic.material.SetFloat("_Desat", 1f);
                            Hitman.DragDropButtonHitman.graphic.color = Palette.DisabledClear;
                            Hitman.DragDropButtonHitman.graphic.material.SetFloat("_Desat", 1f);
                        }
                    }

                    var disableTimer = (DateTime.UtcNow - tickDictionary[LocalPlayer().PlayerId]).TotalMilliseconds/1000;
                    if (Meeting() || disableTimer > duration || LocalPlayer()?.Data.IsDead != false || AmongUsClient.Instance.GameState != InnerNetClient.GameStates.Started)
                    {
                        HUDManager().KillButton.enabled = true;
                        if (role?.ExtraButtons.Count > 0) role.ExtraButtons[0].enabled = true;
                        if (LocalPlayer().Is(RoleEnum.Glitch))
                        {
                            var glitch = GetRole<Glitch>(LocalPlayer());
                            glitch.MimicButton.enabled = true;
                            glitch.HackButton.enabled = true;
                        }
                        if (LocalPlayer().Is(RoleEnum.Hitman))
                        {
                            var glitch = GetRole<Hitman>(LocalPlayer());
                            glitch.MorphButton.enabled = true;
                        }

                        tickDictionary.Remove(LocalPlayer().PlayerId);
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
                if (LocalPlayer().PlayerId == __instance.myPlayer.PlayerId) Coroutines.Start(DisableAbility.StopAbility(0.75f));
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