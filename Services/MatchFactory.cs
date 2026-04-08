using Lightspeed.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Lightspeed.Services;

/// <summary>
/// This factory is used to create view models from models or other data.
/// </summary>
public class MatchFactory(IServiceProvider serviceProvider)
{
    /// <summary>
    /// Creates a new match with the given participants and an optional match number.
    /// </summary>
    public T NewMatch<T>(ParticipantViewModel first, ParticipantViewModel second, int? number = null) where T : MatchViewModel
    {
        var match = serviceProvider.GetRequiredService<T>();
        match.Number = number;
        match.First = NewScore(first);
        match.Second = NewScore(second);
        return match;
    }

    /// <summary>
    /// Creates a new match with the given participants and seeds. This is used for creating matches from a ranked list of participants,
    /// where some participants may have byes. If a participant is null, it will be treated as a bye.
    /// </summary>
    public T NewMatch<T>(ParticipantViewModel? first, int firstSeed, ParticipantViewModel? second, int secondSeed) where T : MatchViewModel
    {
        var match = serviceProvider.GetRequiredService<T>();
        match.First = first is not null ? NewScore(first, firstSeed) : NewScore(serviceProvider.GetRequiredService<ByeViewModel>());
        match.Second = second is not null ? NewScore(second, secondSeed) : NewScore(serviceProvider.GetRequiredService<ByeViewModel>());
        return match;
    }

    /// <summary>
    /// Creates a new match that will be bound to the winners of the given matches.
    /// This is used for creating the next round of a bracket after the first round is created from a ranked list.
    /// </summary>
    public T NewEmptyMatch<T>() where T : MatchViewModel
    {
        var match = serviceProvider.GetRequiredService<T>();
        match.First = NewScore(serviceProvider.GetRequiredService<EmptyParticipantViewModel>());
        match.Second = NewScore(serviceProvider.GetRequiredService<EmptyParticipantViewModel>());
        return match;
    }

    /// <summary>
    /// Creates a new view model with the specified participant.
    /// </summary>
    /// <param name="participant"></param>
    /// <returns></returns>
    protected ScoreViewModel NewScore(ParticipantViewModel participant, int? seed = null)
    {
        var vm = serviceProvider.GetRequiredService<ScoreViewModel>();
        vm.Participant = participant;
        vm.Seed = seed;
        return vm;
    }
}
