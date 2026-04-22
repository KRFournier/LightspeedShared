using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Lightspeed.MatchComponents;

/// <summary>
/// Represents a single action that occurred during a match, including the type of action, the actor, the scorer, and any points awarded.
/// </summary>
public partial class ActionViewModel : ObservableObject
{
    #region Properties

    [ObservableProperty]
    public partial Guid Guid { get; set; } = Guid.NewGuid();

    [ObservableProperty]
    public partial ActionType Type { get; set; } = ActionType.Unknown;

    [ObservableProperty]
    public partial string? SubType { get; set; }

    [ObservableProperty]
    public partial SideViewModel? Actor { get; set; }

    [ObservableProperty]
    public partial SideViewModel? Scorer { get; set; }

    [ObservableProperty]
    public partial int Points { get; set; } = 0;

    public string? Name => Type switch
    {
        ActionType.Card => SubType,
        ActionType.Clean => "Clean",
        ActionType.Conceded => "Conceded",
        ActionType.Disarm => "Disarm",
        ActionType.Ejected => SubType,
        ActionType.FirstContact => "First Contact",
        ActionType.Headshot => "Head",
        ActionType.Indirect => "Indirect",
        ActionType.OutOfBounds => "Out of Bounds",
        ActionType.Overtime => "Overtime",
        ActionType.Penalty => SubType,
        ActionType.Priority => "Priority",
        ActionType.HeadshotOverride => "Head Override",
        ActionType.Return => "Return",
        _ => ""
    };

    #endregion

    #region Commands

    [RelayCommand]
    private void AddPoint()
    {
        if (Scorer is not null)
        {
            Points++;
            Scorer.Points++;
        }
    }

    [RelayCommand]
    private void RemovePoint()
    {
        if (Points > 0 && Scorer is not null)
        {
            Points--;
            Scorer.Points--;
        }
    }

    public void Apply()
    {
        Scorer?.Points += Points;

        if (Actor is not null)
        {
            switch (Type)
            {
                case ActionType.Card: Actor.Participant.Penalties.GiveCard(); break;
                case ActionType.Conceded: Actor.Participant.Honor.AddHonor(); break;
                case ActionType.Ejected: Actor.Participant.Penalties.Eject(); break;
                case ActionType.Penalty: Actor.MinorViolations++; break;
            }
        }
    }

    public void Undo()
    {
        Scorer?.Points -= Points;

        if (Actor is not null)
        {
            switch (Type)
            {
                case ActionType.Card: Actor.Participant.Penalties.RemoveCard(); break;
                case ActionType.Conceded: Actor.Participant.Honor.RemoveHonor(); break;
                case ActionType.Ejected: Actor.Participant.Penalties.Uneject(); break;
                case ActionType.Penalty: Actor.MinorViolations--; break;
            }
        }
    }

    #endregion

    #region Factories

    public static ActionViewModel NewConcession(SideViewModel? actor, SideViewModel? scorer) => new()
    {
        Type = ActionType.Conceded,
        Actor = actor,
        Scorer = scorer,
        Points = PointValues.Conceded
    };

    public static ActionViewModel NewOutOfBounds(SideViewModel? actor, SideViewModel? scorer) => new()
    {
        Type = ActionType.OutOfBounds,
        Actor = actor,
        Scorer = scorer
    };

    #endregion

    public Lightspeed.Action ToModel(LeftRightViewModel sides) => new()
    {
        Id = Guid,
        Actor = sides.ToReference(Actor),
        Scorer = sides.ToReference(Scorer),
        Points = Points,
        Type = Type,
        SubType = SubType
    };
}
