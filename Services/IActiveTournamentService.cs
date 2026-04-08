namespace Lightspeed.Services;

public interface IActiveTournamentService
{
    bool IsRanked { get; }
    bool ShowingWeapons { get; }
}
