using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lightspeed.Network;

namespace Lightspeed.ViewModels;

/// <summary>
/// Base class for all action view models. Each action is a view model so we can customize it's
/// appearance with a matching view.
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
    public partial ScoreViewModel? Actor { get; set; }

    [ObservableProperty]
    public partial ScoreViewModel? Scorer { get; set; }

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

        if (Actor is not null && Actor.Participant.CurrentPlayer is not null)
        {
            switch (Type)
            {
                case ActionType.Card: Actor.Participant.CurrentPlayer.Card++; break;
                case ActionType.Conceded: Actor.Participant.CurrentPlayer.Honor++; break;
                case ActionType.Ejected: Actor.Participant.CurrentPlayer.IsEjected = true; break;
                case ActionType.Penalty: Actor.MinorViolations++; break;
            }
        }
    }

    public void Undo()
    {
        Scorer?.Points -= Points;

        if (Actor is not null && Actor.Participant.CurrentPlayer is not null)
        {
            switch (Type)
            {
                case ActionType.Card: Actor.Participant.CurrentPlayer.Card--; break;
                case ActionType.Conceded: Actor.Participant.CurrentPlayer.Honor--; break;
                case ActionType.Ejected: Actor.Participant.CurrentPlayer.IsEjected = false; break;
                case ActionType.Penalty: Actor.MinorViolations--; break;
            }
        }
    }

    #endregion

    public Lightspeed.Action ToModel(MatchViewModel match) => new()
    {
        Id = Guid,
        Actor = Actor == match.First ? Side.First : (Actor == match.Second ? Side.Second : Side.Neither),
        Scorer = Scorer == match.First ? Side.First : (Scorer == match.Second ? Side.Second : Side.Neither),
        Points = Points,
        Type = Type,
        SubType = SubType
    };

    public ActionState ToState(MatchViewModel match) => new()
    {
        Id = Guid,
        Actor = Actor == match.First ? Side.First : (Actor == match.Second ? Side.Second : Side.Neither),
        Scorer = Scorer == match.First ? Side.First : (Scorer == match.Second ? Side.Second : Side.Neither),
        Points = Points,
        Type = Type,
        SubType = SubType
    };
}