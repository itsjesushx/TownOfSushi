using TMPro;
using System.Collections;
using TownOfSushi.Roles.Crewmates.Support.MedicRole;
using TownOfSushi.Roles.Abilities.AbilityMod.ChameleonAbility;

namespace TownOfSushi.Roles.Crewmates
{
    public class Transporter : Role
    {
        public DateTime LastTransported { get; set; }
        public PlayerControl TransportPlayer1 { get; set; }
        public PlayerControl TransportPlayer2 { get; set; }
        public int MaxUses;
        public TextMeshPro UsesText;
        public bool ButtonUsable => MaxUses != 0 && !SwappingMenus;
        public bool SwappingMenus = false;
        public Dictionary<byte, DateTime> UntransportablePlayers = new Dictionary<byte, DateTime>();
        public Transporter(PlayerControl player) : base(player)
        {
            Name = "Transporter";
            StartText = () => "Choose Two Players To Swap Locations";
            TaskText = () => "Choose two players to swap locations";
            
            RoleAlignment = RoleAlignment.CrewSupport;
            Color = Colors.Transporter;
            LastTransported = DateTime.UtcNow;
            RoleType = RoleEnum.Transporter;
            AddToRoleHistory(RoleType);
            TransportPlayer1 = null;
            TransportPlayer2 = null;
            MaxUses = CustomGameOptions.TransportMaxUses;
        }

        public float TransportTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastTransported;
            var num = CustomGameOptions.TransportCooldown * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        public static IEnumerator TransportPlayers(byte player1, byte player2, bool die)
        {
            var TP1 = PlayerById(player1);
            var TP2 = PlayerById(player2);
            var deadBodies = Object.FindObjectsOfType<DeadBody>();
            DeadBody Player1Body = null;
            DeadBody Player2Body = null;
            if (TP1.Data.IsDead)
            {
                foreach (var body in deadBodies) if (body.ParentId == TP1.PlayerId) Player1Body = body;
                if (Player1Body == null) yield break;
            }
            if (TP2.Data.IsDead)
            {
                foreach (var body in deadBodies) if (body.ParentId == TP2.PlayerId) Player2Body = body;
                if (Player2Body == null) yield break;
            }

            if (TP1.Is(AbilityEnum.Chameleon))
            {
                var shy = GetAbility<Chameleon>(TP1);
                shy.Opacity = 1f;
                Abilities.AbilityMod.ChameleonAbility.HudManagerUpdate.SetVisiblity(TP1, shy.Opacity);
                shy.Moving = true;
            }
            if (TP2.Is(AbilityEnum.Chameleon))
            {
                var shy = GetAbility<Chameleon>(TP1);
                shy.Opacity = 1f;
                Abilities.AbilityMod.ChameleonAbility.HudManagerUpdate.SetVisiblity(TP1, shy.Opacity);
                shy.Moving = true;
            }

            if (TP1.inVent && PlayerControl.LocalPlayer.PlayerId == TP1.PlayerId)
            {
                while (SubmergedCompatibility.getInTransition())
                {
                    yield return null;
                }
                TP1.MyPhysics.ExitAllVents();
            }
            if (TP2.inVent && PlayerControl.LocalPlayer.PlayerId == TP2.PlayerId)
            {
                while (SubmergedCompatibility.getInTransition())
                {
                    yield return null;
                }
                TP2.MyPhysics.ExitAllVents();
            }

            if (Player1Body == null && Player2Body == null)
            {
                TP1.MyPhysics.ResetMoveState();
                TP2.MyPhysics.ResetMoveState();
                var TempPosition = TP1.GetTruePosition();
                TP1.transform.position = new Vector2(TP2.GetTruePosition().x, TP2.GetTruePosition().y + 0.3636f);
                TP1.NetTransform.SnapTo(new Vector2(TP2.GetTruePosition().x, TP2.GetTruePosition().y + 0.3636f));
                if (die) MurderPlayer(TP1, TP2, true);
                else
                {
                    TP2.transform.position = new Vector2(TempPosition.x, TempPosition.y + 0.3636f);
                    TP2.NetTransform.SnapTo(new Vector2(TempPosition.x, TempPosition.y + 0.3636f));
                }

                if (SubmergedCompatibility.isSubmerged())
                {
                    if (PlayerControl.LocalPlayer.PlayerId == TP1.PlayerId)
                    {
                        SubmergedCompatibility.ChangeFloor(TP1.GetTruePosition().y > -7);
                        SubmergedCompatibility.CheckOutOfBoundsElevator(PlayerControl.LocalPlayer);
                    }
                    if (PlayerControl.LocalPlayer.PlayerId == TP2.PlayerId)
                    {
                        SubmergedCompatibility.ChangeFloor(TP2.GetTruePosition().y > -7);
                        SubmergedCompatibility.CheckOutOfBoundsElevator(PlayerControl.LocalPlayer);
                    }
                }

            }
            else if (Player1Body != null && Player2Body == null)
            {
                StopDragging(Player1Body.ParentId);
                TP2.MyPhysics.ResetMoveState();
                var TempPosition = Player1Body.TruePosition;
                Player1Body.transform.position = TP2.GetTruePosition();
                TP2.transform.position = new Vector2(TempPosition.x, TempPosition.y + 0.3636f);
                TP2.NetTransform.SnapTo(new Vector2(TempPosition.x, TempPosition.y + 0.3636f));

                if (SubmergedCompatibility.isSubmerged())
                {
                    if (PlayerControl.LocalPlayer.PlayerId == TP2.PlayerId)
                    {
                        SubmergedCompatibility.ChangeFloor(TP2.GetTruePosition().y > -7);
                        SubmergedCompatibility.CheckOutOfBoundsElevator(PlayerControl.LocalPlayer);
                    }

                }
            }
            else if (Player1Body == null && Player2Body != null)
            {
                StopDragging(Player2Body.ParentId);
                TP1.MyPhysics.ResetMoveState();
                var TempPosition = TP1.GetTruePosition();
                TP1.transform.position = new Vector2(TP2.GetTruePosition().x, TP2.GetTruePosition().y + 0.3636f);
                TP1.NetTransform.SnapTo(new Vector2(Player2Body.TruePosition.x, Player2Body.TruePosition.y + 0.3636f));
                Player2Body.transform.position = TempPosition;
                if (SubmergedCompatibility.isSubmerged())
                {
                    if (PlayerControl.LocalPlayer.PlayerId == TP1.PlayerId)
                    {
                        SubmergedCompatibility.ChangeFloor(TP1.GetTruePosition().y > -7);
                        SubmergedCompatibility.CheckOutOfBoundsElevator(PlayerControl.LocalPlayer);
                    }
                }
            }
            else if (Player1Body != null && Player2Body != null)
            {
                StopDragging(Player1Body.ParentId);
                StopDragging(Player2Body.ParentId);
                var TempPosition = Player1Body.TruePosition;
                Player1Body.transform.position = Player2Body.TruePosition;
                Player2Body.transform.position = TempPosition;
            }

            if (PlayerControl.LocalPlayer.PlayerId == TP1.PlayerId ||
                PlayerControl.LocalPlayer.PlayerId == TP2.PlayerId)
            {
                Flash(Colors.Transporter, 1.5f);
                if (Minigame.Instance) Minigame.Instance.Close();
            }

            TP1.moveable = true;
            TP2.moveable = true;
            TP1.Collider.enabled = true;
            TP2.Collider.enabled = true;
            TP1.NetTransform.enabled = true;
            TP2.NetTransform.enabled = true;
        }

        public static void StopDragging(byte PlayerId)
        {
            var Undertaker = (Undertaker)AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Undertaker);
            if (Undertaker != null && Undertaker.CurrentlyDragging != null &&
                Undertaker.CurrentlyDragging.ParentId == PlayerId)
                Undertaker.CurrentlyDragging = null;
        }

        public void HandleMedicPlague(HudManager __instance)
        {
            var abilityUsed = AbilityUsed(PlayerControl.LocalPlayer);
            if (!abilityUsed) return;
            if (!UntransportablePlayers.ContainsKey(TransportPlayer1.PlayerId) && !UntransportablePlayers.ContainsKey(TransportPlayer2.PlayerId))
            {
                if (Player.IsInfected() || TransportPlayer1.IsInfected())
                {
                    foreach (var pb in GetRoles(RoleEnum.Plaguebearer)) ((Plaguebearer)pb).RpcSpreadInfection(Player, TransportPlayer1);
                }
                if (Player.IsInfected() || TransportPlayer2.IsInfected())
                {
                    foreach (var pb in GetRoles(RoleEnum.Plaguebearer)) ((Plaguebearer)pb).RpcSpreadInfection(Player, TransportPlayer2);
                }
                var role = GetPlayerRole(Player);
                var transRole = (Transporter)role;
                if (TransportPlayer1.Is(RoleEnum.Pestilence) || TransportPlayer1.IsOnAlert())
                {
                    if (Player.IsShielded())
                    {
                        Rpc(CustomRPC.AttemptSound, Player.GetMedic().Player.PlayerId, Player.PlayerId);

                        System.Console.WriteLine(CustomGameOptions.ShieldBreaks + "- shield break");
                        if (CustomGameOptions.ShieldBreaks)
                            transRole.LastTransported = DateTime.UtcNow;
                        StopKill.BreakShield(Player.GetMedic().Player.PlayerId, Player.PlayerId, CustomGameOptions.ShieldBreaks);
                        return;
                    }
                    else if (!Player.IsProtected())
                    {
                        Coroutines.Start(TransportPlayers(TransportPlayer1.PlayerId, Player.PlayerId, true));

                        Rpc(CustomRPC.Transport, TransportPlayer1.PlayerId, Player.PlayerId, true);
                        return;
                    }
                    transRole.LastTransported = DateTime.UtcNow;
                    return;
                }
                else if (TransportPlayer2.Is(RoleEnum.Pestilence) || TransportPlayer2.IsOnAlert())
                {
                    if (Player.IsShielded())
                    {
                        Rpc(CustomRPC.AttemptSound, Player.GetMedic().Player.PlayerId, Player.PlayerId);

                        System.Console.WriteLine(CustomGameOptions.ShieldBreaks + "- shield break");
                        if (CustomGameOptions.ShieldBreaks)
                            transRole.LastTransported = DateTime.UtcNow;
                        StopKill.BreakShield(Player.GetMedic().Player.PlayerId, Player.PlayerId, CustomGameOptions.ShieldBreaks);
                        return;
                    }
                    else if (!Player.IsProtected())
                    {
                        Coroutines.Start(TransportPlayers(TransportPlayer2.PlayerId, Player.PlayerId, true));

                        Rpc(CustomRPC.Transport, TransportPlayer2.PlayerId, Player.PlayerId, true);
                        return;
                    }
                    transRole.LastTransported = DateTime.UtcNow;
                    return;
                }
                LastTransported = DateTime.UtcNow;
                MaxUses--;

                Coroutines.Start(TransportPlayers(TransportPlayer1.PlayerId, TransportPlayer2.PlayerId, false));

                Rpc(CustomRPC.Transport, TransportPlayer1.PlayerId, TransportPlayer2.PlayerId, false);
            }
            else
            {
                __instance.StartCoroutine(Effects.SwayX(__instance.KillButton.transform));
            }
        }

        public IEnumerator OpenSecondMenu()
        {
            try
            {
                PlayerMenu.singleton.Menu.ForceClose();
            }
            catch
            {

            }
            yield return (object)new WaitForSeconds(0.05f);
            SwappingMenus = false;
            if (MeetingHud.Instance || !PlayerControl.LocalPlayer.Is(RoleEnum.Transporter)) yield break;
            List<byte> transportTargets = new List<byte>();
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (!player.Data.Disconnected && player != TransportPlayer1)
                {
                    if (!player.Data.IsDead) transportTargets.Add(player.PlayerId);
                    else
                    {
                        foreach (var body in Object.FindObjectsOfType<DeadBody>())
                        {
                            if (body.ParentId == player.PlayerId) transportTargets.Add(player.PlayerId);
                        }
                    }
                }
            }
            byte[] transporttargetIDs = transportTargets.ToArray();
            var pk = new PlayerMenu((x) =>
            {
                TransportPlayer2 = x;
                HandleMedicPlague(HudManager.Instance);
            }, (y) =>
            {
                return transporttargetIDs.Contains(y.PlayerId);
            });
            Coroutines.Start(pk.Open(0f, true));
        }
    }
}