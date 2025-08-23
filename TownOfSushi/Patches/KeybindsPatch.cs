using HarmonyLib;
using MiraAPI.Hud;
using Rewired;
using Rewired.Data;
using TownOfSushi.Buttons;


namespace TownOfSushi.Patches;

// original patch taken from TheOtherRolesAU/TheOtherRoles/pull/347 by dadoum
[HarmonyPatch(typeof(InputManager_Base), nameof(InputManager_Base.Awake))]
public static class Keybinds
{
    [HarmonyPrefix]
    private static void Prefix(InputManager_Base __instance)
    {
        // change the text shown on the screen for the keybinds menu
        try
        {
            // var blankAction = new Action(() => { });
            __instance.userData.GetAction("ActionSecondary").descriptiveName = "Kill / Secondary Ability";
            __instance.userData.GetAction("ActionQuaternary").descriptiveName = "Primary Ability";
            __instance.userData.RegisterBind("tou.ActionCustom", "Tertiary Ability (Hack Ability)");
            __instance.userData.RegisterBind("tou.ActionCustom2", "Modifier Ability");
            /*KeybindManager.Register("tou.ActionCustom", "Tertiary Ability (Hack Ability)", KeyboardKeyCode.C,
                blankAction);
            KeybindManager.Register("tou.ActionCustom2", "Modifier Ability", KeyboardKeyCode.X, blankAction);*/
        }
        catch
        {
            // Logger<TownOfSushiPlugin>.Error($"Error applying names for custom keybinds: {e}");
        }
    }
    
    private static int RegisterBind(this UserData self, string name, string description, int elementIdentifierId = -1,
        int category = 0, InputActionType type = InputActionType.Button)
    {
        self.AddAction(category);
        var action = self.GetAction(self.actions.Count - 1)!;

        action.name = name;
        action.descriptiveName = description;
        action.categoryId = category;
        action.type = type;
        action.userAssignable = true;

        var map = new ActionElementMap
        {
            _elementIdentifierId = elementIdentifierId,
            _actionId = action.id,
            _elementType = ControllerElementType.Button,
            _axisContribution = Pole.Positive,
            _modifierKey1 = ModifierKey.None,
            _modifierKey2 = ModifierKey.None,
            _modifierKey3 = ModifierKey.None
        };
        self.keyboardMaps[0].actionElementMaps.Add(map);
        self.joystickMaps[0].actionElementMaps.Add(map);

        return action.id;
    }
}

[HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
public static class Bindings
{
    public static void Postfix(HudManager __instance)
    {
        if (PlayerControl.LocalPlayer == null)
        {
            return;
        }

        if (PlayerControl.LocalPlayer.Data == null)
        {
            return;
        }

        var rewirePlayer = ReInput.players.GetPlayer(0);
        var contPlayer = ConsoleJoystick.player;
        var buttonList = CustomButtonManager.Buttons.Where(x =>
            x.Enabled(PlayerControl.LocalPlayer.Data.Role) && x.Button != null && x.Button.isActiveAndEnabled && x.CanUse()).ToList();

        foreach (var button in buttonList.Where(x => x is TownOfSushiButton))
        {
            var touButton = button as TownOfSushiButton;
            if (touButton == null || touButton.Keybind == string.Empty)
            {
                continue;
            }
            
            if ((rewirePlayer.GetButtonDown(touButton.Keybind) ||
                 contPlayer.GetButtonDown(touButton.ConsoleBind())))
            {
                touButton.PassiveComp.OnClick.Invoke();
            }
        }
        foreach (var button in buttonList.Where(x => x is TownOfSushiTargetButton<DeadBody>))
        {
            var touButton = button as TownOfSushiTargetButton<DeadBody>;
            if (touButton == null || touButton.Keybind == string.Empty)
            {
                continue;
            }
            
            if ((rewirePlayer.GetButtonDown(touButton.Keybind) ||
                 contPlayer.GetButtonDown(touButton.ConsoleBind())))
            {
                touButton.PassiveComp.OnClick.Invoke();
            }
        }
        
        foreach (var button in buttonList.Where(x => x is TownOfSushiTargetButton<Vent>))
        {
            var touButton = button as TownOfSushiTargetButton<Vent>;
            if (touButton == null || touButton.Keybind == string.Empty)
            {
                continue;
            }
            
            if ((rewirePlayer.GetButtonDown(touButton.Keybind) ||
                 contPlayer.GetButtonDown(touButton.ConsoleBind())))
            {
                touButton.PassiveComp.OnClick.Invoke();
            }
        }
        
        foreach (var button in buttonList.Where(x => x is TownOfSushiTargetButton<PlayerControl>))
        {
            var touButton = button as TownOfSushiTargetButton<PlayerControl>;
            if (touButton == null || touButton.Keybind == string.Empty)
            {
                continue;
            }
            
            if ((rewirePlayer.GetButtonDown(touButton.Keybind) ||
                 contPlayer.GetButtonDown(touButton.ConsoleBind())))
            {
                touButton.PassiveComp.OnClick.Invoke();
            }
        }

        if (PlayerControl.LocalPlayer.Data.IsDead)
        {
            return;
        }

        if (PlayerControl.LocalPlayer.IsImpostor())
        {
            return;
        }

        // for neutrals

        var kill = __instance.KillButton;
        var vent = __instance.ImpostorVentButton;

        if (kill.isActiveAndEnabled)
        {
            var killKey = ReInput.players.GetPlayer(0).GetButtonDown("ActionSecondary");
            var controllerKill = ConsoleJoystick.player.GetButtonDown(8);
            if (killKey || controllerKill)
            {
                kill.DoClick();
            }
        }

        if (vent.isActiveAndEnabled)
        {
            var ventKey = ReInput.players.GetPlayer(0).GetButtonDown("UseVent");
            var controllerVent = ConsoleJoystick.player.GetButtonDown(50);
            if (ventKey || controllerVent)
            {
                vent.DoClick();
            }
        }
    }
}