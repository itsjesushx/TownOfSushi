﻿using HarmonyLib;
using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Meeting;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Roles;
using TownOfSushi.Buttons.Impostor;
using TownOfSushi.Options.Roles.Impostor;
using TownOfSushi.Roles.Impostor;

namespace TownOfSushi.Events.Impostor;

public static class MorphlingEvents
{
    [RegisterEvent]
    public static void EjectionEventEventHandler(EjectionEvent @event)
    {
        CustomRoleUtils.GetActiveRolesOfType<MorphlingRole>().Do(x => x.Clear());
        var button = CustomButtonSingleton<MorphlingMorphButton>.Instance;
        button.SetUses((int)OptionGroupSingleton<MorphlingOptions>.Instance.MaxMorphs);
        if ((int)OptionGroupSingleton<MorphlingOptions>.Instance.MaxMorphs == 0)
        {
            button.Button?.usesRemainingText.gameObject.SetActive(false);
            button.Button?.usesRemainingSprite.gameObject.SetActive(false);
        }
        else
        {
            button.Button?.usesRemainingText.gameObject.SetActive(true);
            button.Button?.usesRemainingSprite.gameObject.SetActive(true);
        }
    }
}
