using MiraAPI.Modifiers.Types;
using MiraAPI.PluginLoading;
using TownOfSushi.Modules.Anims;

namespace TownOfSushi.Modifiers;

[MiraIgnore]
public abstract class BaseShieldModifier : TimedModifier, IAnimated
{
    public override string ModifierName => "Shield Modifier";
    public virtual string ShieldDescription => "You are protected!";
    public override float Duration => 1f;
    public override bool AutoStart => false;
    public override bool HideOnUi => !TownOfSushiPlugin.ShowShieldHud.Value;
    public override string GetDescription()
    {
        return !HideOnUi ? ShieldDescription : string.Empty;
    }
    public bool IsVisible { get; set; } = true;
    public virtual bool VisibleSymbol => false;
    public void SetVisible()
    {

    }
}
