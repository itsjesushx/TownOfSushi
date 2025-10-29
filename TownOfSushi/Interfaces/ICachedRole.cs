namespace TownOfSushi.Interfaces;

public interface ICachedRole
{
    // if shown first, the cached role appears like so: Bomber (Traitor). Alternatively, if it's not, it'll appear like this: Imitator (Vigilante)
    bool ShowCurrentRoleFirst { get; }
    bool Visible { get; }
    RoleBehaviour CachedRole { get; }
}