using TownOfSushi.Roles.Abilities.AbilityMod.AssassinAbility;
using UnityEngine.UI;

namespace TownOfSushi.Roles.Crewmates.Power.MayorRole
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class AddRevealButton
    {
        public static Sprite RevealSprite => TownOfSushi.RevealSprite;

        public static void GenButton(Mayor role, int index)
        {
            var confirmButton = MeetingHud.Instance.playerStates[index].Buttons.transform.GetChild(0).gameObject;

            var newButton = Object.Instantiate(confirmButton, MeetingHud.Instance.playerStates[index].transform);
            var renderer = newButton.GetComponent<SpriteRenderer>();
            var passive = newButton.GetComponent<PassiveButton>();

            renderer.sprite = RevealSprite;
            newButton.transform.position = confirmButton.transform.position - new Vector3(0.75f, 0f, 0f);
            newButton.transform.localScale *= 0.8f;
            newButton.layer = 5;
            newButton.transform.parent = confirmButton.transform.parent.parent;

            passive.OnClick = new Button.ButtonClickedEvent();
            passive.OnClick.AddListener(Reveal(role));
            role.RevealButton = newButton;
        }


        private static Action Reveal(Mayor role)
        {
            void Listener()
            {
                role.RevealButton.Destroy();
                role.Revealed = true;
                Rpc(CustomRPC.Reveal, role.Player.PlayerId);
            }

            return Listener;
        }

        public static void RemoveAssassin(Mayor mayor)
        {
            PlayerVoteArea voteArea = MeetingHud.Instance.playerStates.First(
                x => x.TargetPlayerId == mayor.Player.PlayerId);
            if (PlayerControl.LocalPlayer.Is(AbilityEnum.Assassin))
            {
                if (AddButton.assassinUI != null)
                {
                    if (AddButton.assassinCurrentTarget == mayor.Player.PlayerId) 
                    {
                        AddButton.assassinUIExitButton.OnClick.Invoke();
                        MeetingHud.Instance.playerStates.ToList().ForEach(x => { if (x.TargetPlayerId == mayor.Player.PlayerId && x.transform.FindChild("ShootButton") != null) UnityEngine.Object.Destroy(x.transform.FindChild("ShootButton").gameObject); });
                    }
                    MeetingHud.Instance.playerStates.ToList().ForEach(x => { if (x.TargetPlayerId == mayor.Player.PlayerId && x.transform.FindChild("ShootButton") != null) UnityEngine.Object.Destroy(x.transform.FindChild("ShootButton").gameObject); });
                }
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Doomsayer))
            {
                if (Neutral.Evil.DoomsayerRole.AddButton.doomsayerUI != null)
                {
                    if (Neutral.Evil.DoomsayerRole.AddButton.doomsayerCurrentTarget == mayor.Player.PlayerId) 
                    {
                        Neutral.Evil.DoomsayerRole.AddButton.doomsayerUIExitButton.OnClick.Invoke();
                        MeetingHud.Instance.playerStates.ToList().ForEach(x => { if (x.TargetPlayerId == mayor.Player.PlayerId && x.transform.FindChild("ShootButton") != null) UnityEngine.Object.Destroy(x.transform.FindChild("ShootButton").gameObject); });
                    }
                    MeetingHud.Instance.playerStates.ToList().ForEach(x => { if (x.TargetPlayerId == mayor.Player.PlayerId && x.transform.FindChild("ShootButton") != null) UnityEngine.Object.Destroy(x.transform.FindChild("ShootButton").gameObject); });
                }
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Vigilante))
            {
                if (Killing.VigilanteRole.AddButton.vigilanteUI != null)
                {
                    if (Killing.VigilanteRole.AddButton.vigilanteCurrentTarget == mayor.Player.PlayerId) 
                    {
                        Killing.VigilanteRole.AddButton.vigilanteUIExitButton.OnClick.Invoke();
                        MeetingHud.Instance.playerStates.ToList().ForEach(x => { if (x.TargetPlayerId == mayor.Player.PlayerId && x.transform.FindChild("ShootButton") != null) UnityEngine.Object.Destroy(x.transform.FindChild("ShootButton").gameObject); });
                    }
                    MeetingHud.Instance.playerStates.ToList().ForEach(x => { if (x.TargetPlayerId == mayor.Player.PlayerId && x.transform.FindChild("ShootButton") != null) UnityEngine.Object.Destroy(x.transform.FindChild("ShootButton").gameObject); });
                }
            }
            return;
        }

        public static void Postfix(MeetingHud __instance)
        {
            foreach (var role in GetRoles(RoleEnum.Mayor))
            {
                var mayor = (Mayor)role;
                mayor.RevealButton.Destroy();
            }

            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Mayor)) return;
            if (PlayerControl.LocalPlayer.IsJailed()) return;
            var mayorrole = GetRole<Mayor>(PlayerControl.LocalPlayer);
            if (mayorrole.Revealed) return;
            for (var i = 0; i < __instance.playerStates.Length; i++)
                if (PlayerControl.LocalPlayer.PlayerId == __instance.playerStates[i].TargetPlayerId)
                {
                    GenButton(mayorrole, i);
                }
        }
    }
}