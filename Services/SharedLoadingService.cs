using CommunityToolkit.Mvvm.Messaging;
using Lightspeed.MatchComponents;
using Lightspeed.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Lightspeed.Services;

/// <summary>
/// This factory is used to create view models from models after they have been loaded from the database.
/// </summary>
public class SharedLoadingService(IServiceProvider serviceProvider, IMessenger messenger)
{
    #region Caches

    private readonly Dictionary<Guid, CompetitorViewModel> _competitors = [];
    private readonly Dictionary<Guid, ParticipantViewModel> _participants = [];
    private readonly Dictionary<Guid, MatchViewModel> _matches = [];

    public CompetitorViewModel FindCompetitor(Guid id)
    {
        if (_competitors.TryGetValue(id, out var competitor))
            return competitor;
        throw new InvalidOperationException($"Competitor with id {id} not found.");
    }

    public ParticipantViewModel FindParticipant(Guid id)
    {
        if (_participants.TryGetValue(id, out var participant))
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
    /// Loads a new action view model from an <see cref="Action"/> model.
    /// </summary>
    protected static ActionViewModel LoadAction(LeftRightViewModel sides, Action model) => new()
    {
        Guid = model.Id,
        Actor = sides.ToSide(model.Actor),
        Scorer = sides.ToSide(model.Scorer),
        Points = model.Points,
        Type = model.Type,
        SubType = model.SubType
    };

    /// <summary>
    /// Loads a new actions component for a match
    /// </summary>
    protected static ActionsViewModel LoadActions(ClockViewModel clock, LeftRightViewModel sides, PriorityViewModel? priority, IEnumerable<Action> models) => new(clock, sides, priority)
    {
        Actions = [.. models.Select(a => LoadAction(sides, a))]
    };

    /// <summary>
    /// Loads a clock component for a match
    /// </summary>
    protected ClockViewModel LoadClock(Clock model)
    {
        var vm = New<ClockViewModel>();
        vm.CurrentRound = model.CurrentRound;
        vm.TimeRemaining = model.Timer;
        return vm;
    }

    /// <summary>
    /// Loads a priority component for a match
    /// </summary>
    protected PriorityViewModel LoadPriority(Guid matchGuid, LeftRightViewModel sides, Priority model) => new(matchGuid, sides, messenger)
    {
        PrioritySide = model.PrioritySide,
        PriorityPoints = model.PriorityPoints,
        InPriority = model.InPriority
    };

    /// <summary>
    /// Loads a new match settings view model from a <see cref="MatchSettings"/> model.
    /// </summary>
    public StandardMatchSettingsViewModel LoadStandardMatchSettings(StandardMatchSettings model)
    {
        var vm = New<StandardMatchSettingsViewModel>();

        LoadMatchSettingsBase(vm, model);

        vm.WinningScore = model.WinningScore;
        vm.TimeLimit = model.TimeLimit;
        vm.Rounds = model.Rounds;

        return vm;
    }

    /// <summary>
    /// Called by other match setting loaders to set the base class properties
    /// </summary>
    protected static void LoadMatchSettingsBase(MatchSettingsViewModel settings, MatchSettings model) => settings.IsLocked = model.IsLocked;

    /// <summary>
    /// Loads a new match view model from a <see cref="Match"/> model.
    /// The type of the match view model will depend on the type of the match model.
    /// </summary>
    public MatchViewModel LoadMatch(Match? model, MatchSettingsViewModel settings)
    {
        if (model is null)
            return New<MatchNotFoundViewModel>();

        if (model is StandardMatch standardMatch && settings is StandardMatchSettingsViewModel standardSettings)
            return LoadStandardMatch(standardMatch, standardSettings);

        throw new NotSupportedException($"Match type {model.GetType().Name} is not supported.");
    }

    /// <summary>
    /// Loads a left/right component for a match
    /// </summary>
    protected LeftRightViewModel LoadLeftRight(LeftRightSide model)
    {
        var vm = new LeftRightViewModel
        {
            Left = LoadSide(model.Left),
            Right = LoadSide(model.Right)
        };
        return vm;
    }

    /// <summary>
    /// Loads a side that belongs to one side of a left/right match.
    /// </summary>
    protected SideViewModel? LoadSide(Side? model)
    {
        if (model is null)
            return null;

        return new SideViewModel
        {
            Participant = FindParticipant(model.Participant),
            Points = model.Points,
            MinorViolations = model.MinorViolations
        };
    }

    /// <summary>
    /// Loads a new view model from a <see cref="StandardMatch"/>
    /// </summary>
    protected StandardMatchViewModel LoadStandardMatch(StandardMatch model, StandardMatchSettingsViewModel settings)
    {
        var vm = New<StandardMatchViewModel>();

        LoadMatchBase(vm, model);

        vm.Settings = settings;
        vm.Clock = LoadClock(model.Clock);
        vm.Scores = LoadLeftRight(model.Scores);
        vm.Priority = LoadPriority(vm.Guid, vm.Scores, model.Priority);
        vm.Actions = LoadActions(vm.Clock, vm.Scores, vm.Priority, model.Actions);

        _matches.Add(vm.Guid, vm);

        return vm;
    }

    /// <summary>
    /// Called by other match loaders to load the base class properties
    /// </summary>
    protected static void LoadMatchBase(MatchViewModel match, Match model)
    {
        match.Guid = model.Id;
        match.Number = model.Number;
    }
}
