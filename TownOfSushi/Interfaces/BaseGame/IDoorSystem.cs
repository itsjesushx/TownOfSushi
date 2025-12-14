namespace TownOfSushi.Interfaces.BaseGame;

// ReSharper disable once InconsistentNaming
public sealed partial class BaseGame
{
    public interface IDoorSystem
    {
        void CloseDoorsOfType(SystemTypes room);

        void SetInitialSabotageCooldown();
    }
}