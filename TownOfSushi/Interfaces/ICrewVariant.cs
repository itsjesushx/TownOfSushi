namespace TownOfSushi.Interfaces;

public interface ICrewVariant
{
    // Determines the closest crewmate role an Imitator can pick
    RoleBehaviour CrewVariant { get; }
}