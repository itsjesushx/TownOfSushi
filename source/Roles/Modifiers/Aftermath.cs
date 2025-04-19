using System.Collections;

namespace TownOfSushi.Roles.Modifiers
{
    public class Aftermath : Modifier
    {
        public Aftermath(PlayerControl player) : base(player)
        {
            Name = "Aftermath";
            TaskText = () => "Force your killer to use their ability";
            Color = ColorManager.Aftermath;
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
            Coroutines.Start(Delay(player, corpse, db));
        }

        private static IEnumerator Delay(PlayerControl player, PlayerControl corpse, DeadBody db)
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
                        blackmailer.Blackmailed.nameText().color = ColorManager.ImpostorRed;
                    else blackmailer.Blackmailed.nameText().color = Color.clear;
                }
                blackmailer.Blackmailed = player;

                StartRPC(CustomRPC.Blackmail, player.PlayerId, player.PlayerId);
            }
            else if (role is Glitch glitch)
            {
                if (glitch.Player.GetCustomOutfitType() != CustomPlayerOutfitType.Morph) glitch.RpcSetMimicked(corpse);
            }
            else if (role is Escapist escapist)
            {
                if (escapist.EscapePoint != new Vector3(0f, 0f, 0f))
                {
                    StartRPC(CustomRPC.Escape, PlayerControl.LocalPlayer.PlayerId, escapist.EscapePoint);
                    escapist.LastEscape = DateTime.UtcNow;
                    Escapist.Escape(escapist.Player);
                }
            }
            else if (role is Poisoner poisoner)
            {
                // Poisoner gets poisoned at the same time as the Aftermath hence die at the same time.
                try
                {
                    poisoner.PoisonedPlayer = poisoner.Player;
                    poisoner.PoisonButton.graphic.sprite = LoadSpriteFromResources("TownOfSushi.Resources.PoisonButton.png", 115f);
                    if (poisoner.PoisonedPlayer != null && poisoner.PoisonedPlayer == PlayerControl.LocalPlayer)
                    {
                        poisoner.Poison();
                    }
                    poisoner.PoisonedPlayer = PlayerControl.LocalPlayer; //Only do this to stop repeatedly trying to re-kill poisoned player. null didn't work for some reason
                }
                catch{}
            }
            else if (role is Grenadier grenadier)
            {
                if (!grenadier.Enabled)
                {
                    StartRPC(CustomRPC.FlashGrenade, PlayerControl.LocalPlayer.PlayerId);
                    grenadier.TimeRemaining = CustomGameOptions.GrenadeDuration;
                    grenadier.Flash();
                }
            }
            else if (role is Janitor janitor)
            {
                StartRPC(CustomRPC.JanitorClean, PlayerControl.LocalPlayer.PlayerId, db.ParentId);

                Coroutines.Start(JanitorCoroutine.CleanCoroutine(db, janitor));
            }
            else if (role is Miner miner)
            {
                var position = PlayerControl.LocalPlayer.transform.position;
                var id = PlaceVent.GetAvailableId();
                StartRPC(CustomRPC.Mine, id, PlayerControl.LocalPlayer.PlayerId, position, position.z + 0.001f);
                PlaceVent.SpawnVent(id, miner, position, position.z + 0.001f);
                miner.LastMined = DateTime.UtcNow;
            }
            else if (role is Morphling morphling)
            {
                if (morphling.Player.GetCustomOutfitType() != CustomPlayerOutfitType.Morph)
                {
                    StartRPC(CustomRPC.Morph, PlayerControl.LocalPlayer.PlayerId, corpse.PlayerId);
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
                    StartRPC(CustomRPC.Swoop, PlayerControl.LocalPlayer.PlayerId);
                    swooper.TimeRemaining = CustomGameOptions.SwoopDuration;
                    swooper.Swoop();
                }
            }
            else if (role is Undertaker undertaker)
            {
                if (undertaker.CurrentlyDragging)
                {
                    Vector3 position = PlayerControl.LocalPlayer.transform.position;

                    if (IsSubmerged())
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

                    StartRPC(CustomRPC.Drop, PlayerControl.LocalPlayer.PlayerId, position, position.z);

                    var body = undertaker.CurrentlyDragging;
                    undertaker.CurrentlyDragging = null;
                    body.transform.position = position;
                }

                StartRPC(CustomRPC.Drag, PlayerControl.LocalPlayer.PlayerId, db.ParentId);
                undertaker.CurrentlyDragging = db;
                UndertakerKillButtonTarget.SetTarget(undertaker._dragDropButton, null, undertaker);
                undertaker._dragDropButton.graphic.sprite = TownOfSushi.DropSprite;

            }
            else if (role is Hitman hitman)
            {
                if (hitman.CurrentlyDragging)
                {
                    Vector3 position = PlayerControl.LocalPlayer.transform.position;

                    if (IsSubmerged())
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

                    StartRPC(CustomRPC.HitmanDrop, PlayerControl.LocalPlayer.PlayerId, position, position.z);

                    var body = hitman.CurrentlyDragging;
                    hitman.CurrentlyDragging = null;
                    body.transform.position = position;
                }

                StartRPC(CustomRPC.HitmanDrag, PlayerControl.LocalPlayer.PlayerId, db.ParentId);
                hitman.CurrentlyDragging = db;
                HitmanKillButtonTarget.SetTarget(hitman._dragDropButtonHitman, null, hitman);
                hitman._dragDropButtonHitman.graphic.sprite = TownOfSushi.DropSprite;

                if (hitman.Player.GetCustomOutfitType() != CustomPlayerOutfitType.Morph) hitman.RpcSetMorphed(corpse);

            }
            else if (role is Venerer venerer)
            {
                if (!venerer.Enabled)
                {
                    StartRPC(CustomRPC.Camouflage, PlayerControl.LocalPlayer.PlayerId, venerer.Kills);
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
                    var lowerKC = OptionsManager().currentNormalGameOptions.KillCooldown - CustomGameOptions.UnderdogKillBonus + CustomGameOptions.DetonateDelay;
                    var normalKC = OptionsManager().currentNormalGameOptions.KillCooldown + CustomGameOptions.DetonateDelay;
                    var upperKC = OptionsManager().currentNormalGameOptions.KillCooldown + CustomGameOptions.UnderdogKillBonus + CustomGameOptions.DetonateDelay;
                    PlayerControl.LocalPlayer.SetKillTimer(UnderdogPerformKill.LastImp() ? lowerKC : (UnderdogPerformKill.IncreasedKC() ? normalKC : upperKC));
                }
                else PlayerControl.LocalPlayer.SetKillTimer(OptionsManager().currentNormalGameOptions.KillCooldown + CustomGameOptions.DetonateDelay);
                HUDManager().KillButton.SetTarget(null);
                bomber.Bomb = BombExtentions.CreateBomb(pos);
                StartRPC(CustomRPC.Plant, pos.x, pos.y, pos.z);
            }
        }
    }
}