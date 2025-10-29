namespace TownOfSushi.Roles;

public interface ITOSCrewRole : ITownOfSushiRole
{
    bool IsPowerCrew { get; }
}