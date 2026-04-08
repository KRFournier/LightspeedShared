using Lightspeed.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Lightspeed.Services;

/// <summary>
/// This factory is used to create view models from models after they have been loaded from the database.
/// </summary>
public class SharedLoadingService(IServiceProvider serviceProvider)
{
    #region Caches

    private readonly Dictionary<Guid, ParticipantViewModel> _participants = [];
    private readonly Dictionary<Guid, MatchViewModel> _matches = [];

    public ParticipantViewModel FindParticipant(Guid id)
    {
        if (id == Guid.Empty)
            return New<EmptyParticipantViewModel>();
        else if (id == ByeParticipant.ByeGuid)
            return New<ByeViewModel>();
        else if (_participants.TryGetValue(id, out var participant))
            return participant;
        throw new InvalidOperationException($"Participant with id {id} not found.");
    }

    public MatchViewModel FindMatch(Guid id)
    {
        if (_matches.TryGetValue(id, out var match))
            return match;
        throw new InvalidOperationException($"Match with id {id} not found.");
    }

    #endregion

    /// <summary>
    /// Wrapper around the service provider to create a new view model of the specified type.
    /// </summary>
    public T New<T>() where T : class => serviceProvider.GetRequiredService<T>();

    /// <summary>
    /// Loads a clock view model from a <see cref="Clock"/> model.
    /// </summary>
    protected ClockViewModel LoadClock(Clock model)
    {
        var vm = New<ClockViewModel>();
        vm.Overtime = model.Overtime;
        vm.TimeRemaining = model.Timer;
        return vm;
    }

    /// <summary>
    /// Loads a new match settings view model from a <see cref="MatchSettings"/> model.
    /// </summary>
    public MatchSettingsViewModel LoadMatchSettings(MatchSettings model)
    {
        var vm = New<MatchSettingsViewModel>();
        vm.WinningScore = model.WinningScore;
        vm.TimeLimit = model.TimeLimit;
        vm.Rounds = model.Rounds;
        vm.IsLocked = model.IsLocked;
        return vm;
    }

    /// <summary>
    /// Loads a new match view model from a <see cref="Match"/> model.
    /// The type of the match view model will depend on the type of the match model.
    /// </summary>
    public MatchViewModel LoadMatch(Match? model) => model switch
    {
        StandardMatch standardMatch => LoadStandardMatch(standardMatch),
        null => New<MatchNotFoundViewModel>(),
        _ => throw new NotSupportedException($"Match type {model.GetType().Name} is not supported."),
    };

    /// <summary>
    /// Loads a new participant from the model.
    /// The type of the participant view model will depend on the type of the participant model.
    /// </summary>
    public ParticipantViewModel LoadParticipant(IParticipant participant) => participant switch
    {
        Player player => LoadPlayer(player),
        ByeParticipant => New<ByeViewModel>(),
        EmptyParticipant => New<EmptyParticipantViewModel>(),
        _ => throw new NotSupportedException($"Participant type {participant.GetType().Name} is not supported."),
    };

    /// <summary>
    /// Loads a new player view model from a <see cref="Player"/>
    /// </summary>
    protected PlayerViewModel LoadPlayer(Player player)
    {
        var vm = New<PlayerViewModel>();
        vm.Guid = player.Id;
        vm.FirstName = player.FirstName;
        vm.LastName = player.LastName;
        vm.OnlineId = player.OnlineId;
        vm.Club = player.Club;
        vm.Rank = player.Rank;
        vm.Card = player.Card;
        vm.Honor = player.Honor;
        vm.IsEjected = player.IsEjected;
        vm.WeaponOfChoice = player.WeaponOfChoice;
        vm.ShowWeapon = player.ShowWeapon;

        _participants.Add(vm.Guid, vm);

        return vm;
    }

    /// <summary>
    /// Loads a new priority view model from a <see cref="Priority" model./>
    /// </summary>
    protected PriorityViewModel LoadPriority(Priority model)
    {
        var vm = New<PriorityViewModel>();
        vm.PreviousPriority = model.PreviousPriority;
        vm.PriorityPoints = model.PriorityPoints;
        vm.InPriority = model.InPriority;
        return vm;
    }

    /// <summary>
    /// Loads a new score view model from a <see cref="Score"/>.
    /// If the score has a parent match, it will bind to the winner or loser of that match.
    /// </summary>
    protected ScoreViewModel LoadScore(Score model)
    {
        var vm = New<ScoreViewModel>();
        vm.Participant = FindParticipant(model.Participant);
        vm.Points = model.Points;
        vm.Seed = model.Seed;
        return vm;
    }

    /// <summary>
    /// Loads a new view model from a <see cref="StandardMatch"/>
    /// </summary>
    protected StandardMatchViewModel LoadStandardMatch(StandardMatch model)
    {
        var vm = New<StandardMatchViewModel>();
        vm.Guid = model.Id;
        vm.Number = model.Number;
        vm.Clock = LoadClock(model.Clock);
        vm.First = LoadScore(model.First);
        vm.Second = LoadScore(model.Second);
        vm.IsMatchStarted = model.IsMatchStarted;
        vm.Actions = [.. model.Actions];
        vm.Priority = LoadPriority(model.Priority);
        vm.WinningSide = model.Winner;

        _matches.Add(vm.Guid, vm);

        return vm;
    }
}
