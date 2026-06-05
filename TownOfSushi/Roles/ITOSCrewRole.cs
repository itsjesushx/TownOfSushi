namespace TownOfSushi.Roles;

public interface ITOSCrewRole : ITOSRole
{
    bool IsPowerCrew { get; }
}