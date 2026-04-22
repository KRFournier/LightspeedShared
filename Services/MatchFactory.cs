using Lightspeed.MatchComponents;
using Lightspeed.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Lightspeed.Services;

/// <summary>
/// This factory is used to create view models for matches and match related view models, such as score view models.
/// </summary>
public class MatchFactory(IServiceProvider serviceProvider)
{
    /// <summary>
    /// Creates a new match with the given participants and an optional match number.
    /// </summary>
    public StandardMatchViewModel NewStandardMatch(StandardPlayerViewModel left, StandardPlayerViewModel right, int? number = null)
    {
        var match = serviceProvider.GetRequiredService<StandardMatchViewModel>();
        match.Number = number;
        match.Scores = NewScores(left, right);
        return match;
    }

    /// <summary>
    /// Creates a new set of scores for a match
    /// </summary>
    public LeftRightViewModel<T> NewScores<T>(T left, T right) where T : ParticipantViewModel
    {
        var scores = serviceProvider.GetRequiredService<LeftRightViewModel<T>>();
        scores.Left = NewScore(left);
        scores.Right = NewScore(right);
        return scores;
    }

    /// <summary>
    /// Creates a new view model with the specified participant.
    /// </summary>
    /// <param name="participant"></param>
    /// <returns></returns>
    public static SideViewModel<T> NewScore<T>(T participant) where T : ParticipantViewModel => new()
    {
        Participant = participant
    };

    /// <summary>
    /// Creates a new match with the given participants and seeds. This is used for creating matches from a ranked list of participants,
    /// where some participants may have byes. If a participant is null, it will be treated as a bye.
    /// </summary>
    public BracketMatchViewModel NewSeededStandardMatch<T>(StandardPlayerViewModel? left, int leftSeed, StandardPlayerViewModel? right, int rightSeed)
    {
        var match = serviceProvider.GetRequiredService<BracketMatchViewModel>();
        match.Match = serviceProvider.GetRequiredService<StandardMatchViewModel>();
        match.LeftSeed = leftSeed;
        match.RightSeed = rightSeed;
        return match;
    }

    /// <summary>
    /// Creates a new match that will be bound to the winners of the given matches.
    /// This is used for creating the next round of a bracket after the first round is created from a ranked list.
    /// </summary>
    public T NewEmptyMatch<T>() where T : MatchViewModel
    {
        var match = serviceProvider.GetRequiredService<T>();
        return match;
    }
}
