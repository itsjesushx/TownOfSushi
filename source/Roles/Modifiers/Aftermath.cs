
using System.Collections;
using TownOfSushi.Roles.Impostors.Power.BomberRole;
using TownOfSushi.Modifiers.UnderdogMod;

namespace TownOfSushi.Roles.Modifiers
{
    public class Aftermath : Modifier
    {
        public Aftermath(PlayerControl player) : base(player)
        {
            Name = "Aftermath";
            TaskText = () => "Force your killer to use their ability";
            Color = Colors.Aftermath;
            ModifierType = ModifierEnum.Aftermath;
        }

        public static void ForceAbility(PlayerControl player, PlayerControl corpse)
        {
            if (!player.AmOwner) return;
            DeadBody db = null;
            var bodies = Object.FindObjectsOfType<DeadBody>();
            foreach (var body in bodies)
            {
                try
                {
                    if (body?.ParentId == corpse.PlayerId) { db = body; break; }
                }
                catch
                {
                }
            }
            Coroutines.Start(delay(player, corpse, db));
        }

        private static IEnumerator delay(PlayerControl player, PlayerControl corpse, DeadBody db)
        {
            yield return new WaitForSecondsRealtime(0.2f);
            var role = GetPlayerRole(player);

            if (role is Blackmailer blackmailer)
            {
                blackmailer.Blackmailed?.myRend().material.SetFloat("_Outline", 0f);
                if (blackmailer.Blackmailed != null && blackmailer.Blackmailed.Data.IsImpostor())
                {
                    if (blackmailer.Blackmailed.GetCustomOutfitType() != CustomPlayerOutfitType.Camouflage &&
                        blackmailer.Blackmailed.GetCustomOutfitType() != CustomPlayerOutfitType.Swooper)
                        blackmailer.Blackmailed.nameText().color = Colors.Impostor;
                    else blackmailer.Blackmailed.nameText().color = Color.clear;
                }
                blackmailer.Blackmailed = player;

                Rpc(CustomRPC.Blackmail, player.PlayerId, player.PlayerId);
            }
            else if (role is Glitch glitch)
            {
                if (glitch.Player.GetCustomOutfitType() != CustomPlayerOutfitType.Morph) glitch.RpcSetMimicked(corpse);
            }
            else if (role is Escapist escapist)
            {
                if (escapist.EscapePoint != new Vector3(0f, 0f, 0f))
                {
                    Rpc(CustomRPC.Escape, PlayerControl.LocalPlayer.PlayerId, escapist.EscapePoint);
                    escapist.LastEscape = DateTime.UtcNow;
                    Escapist.Escape(escapist.Player);
                }
            }
            else if (role is Grenadier grenadier)
            {
                if (!grenadier.Enabled)
                {
                    Rpc(CustomRPC.FlashGrenade, PlayerControl.LocalPlayer.PlayerId);
                    grenadier.TimeRemaining = CustomGameOptions.GrenadeDuration;
                    grenadier.Flash();
                }
            }
            else if (role is Janitor janitor)
            {
                Rpc(CustomRPC.JanitorClean, PlayerControl.LocalPlayer.PlayerId, db.ParentId);

                Coroutines.Start(Impostors.Support.JanitorRole.Coroutine.CleanCoroutine(db, janitor));
            }
            else if (role is Miner miner)
            {
                var position = PlayerControl.LocalPlayer.transform.position;
                var id = Impostors.Support.MinerRole.PlaceVent.GetAvailableId();
                Rpc(CustomRPC.Mine, id, PlayerControl.LocalPlayer.PlayerId, position, position.z + 0.001f);
                Impostors.Support.MinerRole.PlaceVent.SpawnVent(id, miner, position, position.z + 0.001f);
                miner.LastMined = DateTime.UtcNow;
            }
            else if (role is Morphling morphling)
            {
                if (morphling.Player.GetCustomOutfitType() != CustomPlayerOutfitType.Morph)
                {
                    Rpc(CustomRPC.Morph, PlayerControl.LocalPlayer.PlayerId, corpse.PlayerId);
                    morphling.TimeRemaining = CustomGameOptions.MorphlingDuration;
                    if (morphling.SampledPlayer == null) morphling._morphButton.graphic.sprite = TownOfSushi.MorphSprite;
                    morphling.SampledPlayer = corpse;
                    morphling.MorphedPlayer = corpse;
                    Morph(morphling.Player, corpse, true);
                }
            }
            else if (role is Swooper swooper)
            {
                if (swooper.Player.GetCustomOutfitType() != CustomPlayerOutfitType.Swooper)
                {
                    Rpc(CustomRPC.Swoop, PlayerControl.LocalPlayer.PlayerId);
                    swooper.TimeRemaining = CustomGameOptions.SwoopDuration;
                    swooper.Swoop();
                }
            }
            else if (role is Undertaker undertaker)
            {
                if (undertaker.CurrentlyDragging)
                {
                    Vector3 position = PlayerControl.LocalPlayer.transform.position;

                    if (SubmergedCompatibility.isSubmerged())
                    {
                        if (position.y > -7f)
                        {
                            position.z = 0.0208f;
                        }
                        else
                        {
                            position.z = -0.0273f;
                        }
                    }

                    position.y -= 0.3636f;

                    Rpc(CustomRPC.Drop, PlayerControl.LocalPlayer.PlayerId, position, position.z);

                    var body = undertaker.CurrentlyDragging;
                    undertaker.CurrentlyDragging = null;
                    body.transform.position = position;
                }

                Rpc(CustomRPC.Drag, PlayerControl.LocalPlayer.PlayerId, db.ParentId);
                undertaker.CurrentlyDragging = db;
                Impostors.Deception.UndertakerRole.KillButtonTarget.SetTarget(undertaker._dragDropButton, null, undertaker);
                undertaker._dragDropButton.graphic.sprite = TownOfSushi.DropSprite;

            }
            else if (role is Hitman hitman)
            {
                if (hitman.CurrentlyDragging)
                {
                    Vector3 position = PlayerControl.LocalPlayer.transform.position;

                    if (SubmergedCompatibility.isSubmerged())
                    {
                        if (position.y > -7f)
                        {
                            position.z = 0.0208f;
                        }
                        else
                        {
                            position.z = -0.0273f;
                        }
                    }

                    position.y -= 0.3636f;

                    Rpc(CustomRPC.HitmanDrop, PlayerControl.LocalPlayer.PlayerId, position, position.z);

                    var body = hitman.CurrentlyDragging;
                    hitman.CurrentlyDragging = null;
                    body.transform.position = position;
                }

                Rpc(CustomRPC.HitmanDrag, PlayerControl.LocalPlayer.PlayerId, db.ParentId);
                hitman.CurrentlyDragging = db;
                Neutral.Killing.HitmanRole.KillButtonTarget2.SetTarget(hitman._dragDropButtonHitman, null, hitman);
                hitman._dragDropButtonHitman.graphic.sprite = TownOfSushi.DropSprite;

                if (hitman.Player.GetCustomOutfitType() != CustomPlayerOutfitType.Morph) hitman.RpcSetMorphed(corpse);

            }
            else if (role is Venerer venerer)
            {
                if (!venerer.Enabled)
                {
                    Rpc(CustomRPC.Camouflage, PlayerControl.LocalPlayer.PlayerId, venerer.Kills);
                    venerer.TimeRemaining = CustomGameOptions.AbilityDuration;
                    venerer.KillsAtStartAbility = venerer.Kills;
                    venerer.Ability();
                }
            }
            else if (role is Bomber bomber)
            {
                bomber.Detonated = false;
                var pos = PlayerControl.LocalPlayer.transform.position;
                pos.z += 0.001f;
                bomber.DetonatePoint = pos;
                bomber.PlantButton.graphic.sprite = TownOfSushi.DetonateSprite;
                bomber.TimeRemaining = CustomGameOptions.DetonateDelay;
                bomber.PlantButton.SetCoolDown(bomber.TimeRemaining, CustomGameOptions.DetonateDelay);
                if (PlayerControl.LocalPlayer.Is(ModifierEnum.Underdog))
                {
                    var lowerKC = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown - CustomGameOptions.UnderdogKillBonus + CustomGameOptions.DetonateDelay;
                    var normalKC = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown + CustomGameOptions.DetonateDelay;
                    var upperKC = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown + CustomGameOptions.UnderdogKillBonus + CustomGameOptions.DetonateDelay;
                    PlayerControl.LocalPlayer.SetKillTimer(PerformKill.LastImp() ? lowerKC : (PerformKill.IncreasedKC() ? normalKC : upperKC));
                }
                else PlayerControl.LocalPlayer.SetKillTimer(GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown + CustomGameOptions.DetonateDelay);
                DestroyableSingleton<HudManager>.Instance.KillButton.SetTarget(null);
                bomber.Bomb = BombExtentions.CreateBomb(pos);
                Rpc(CustomRPC.Plant, pos.x, pos.y, pos.z);
            }
        }
    }
}