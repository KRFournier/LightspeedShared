using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace Lightspeed.ViewModels;

/// <summary>
/// Base class for all action view models. Each action is a view model so we can customize it's
/// appearance with a matching view.
/// </summary>
public partial class ActionViewModel(IServiceProvider serviceProvider, IMessenger messenger) : ViewModelBase(serviceProvider, messenger)
{
    #region Properties

    public readonly Guid Guid = Guid.NewGuid();

    public readonly ActionType Type;

    [ObservableProperty]
    public partial string? SubType { get; set; }

    [ObservableProperty]
    private partial Side Actor { get; set; } = Side.Neither;

    [ObservableProperty]
    private partial Side Scorer { get; set; } = Side.Neither;

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

    //#region Commands

    //[RelayCommand]
    //private void AddPoint()
    //{
    //    if (Scorer is not null)
    //    {
    //        Points++;
    //        Scorer.Score++;
    //        Modified?.Invoke();
    //    }
    //}

    //[RelayCommand]
    //private void RemovePoint()
    //{
    //    if (Points > 0 && Scorer is not null)
    //    {
    //        Points--;
    //        Scorer.Score--;
    //        Modified?.Invoke();
    //    }
    //}

    //public void Apply()
    //{
    //    if (Scorer is not null)
    //        Scorer.Score += Points;

    //    if (Actor is not null)
    //    {
    //        switch (Type)
    //        {
    //            case ActionType.Card: Actor.Card++; break;
    //            case ActionType.Conceded: Actor.Honor++; break;
    //            case ActionType.Ejected: Actor.Ejected = true; break;
    //            case ActionType.Penalty:
    //                if (IsForce)
    //                {
    //                    Actor.ForceCalls++;
    //                    Actor.MatchForceCalls++;
    //                }
    //                Actor.MinorViolationCount++;
    //                break;
    //        }
    //    }
    //}

    //public void Undo()
    //{
    //    if (Scorer is not null)
    //        Scorer.Score -= Points;

    //    if (Actor is not null)
    //    {
    //        switch (Type)
    //        {
    //            case ActionType.Card: Actor.Card--; break;
    //            case ActionType.Conceded: Actor.Honor--; break;
    //            case ActionType.Ejected: Actor.Ejected = false; break;
    //            case ActionType.Penalty:
    //                if (IsForce)
    //                {
    //                    Actor.ForceCalls--;
    //                    Actor.MatchForceCalls--;
    //                }
    //                Actor.MinorViolationCount--;
    //                break;
    //        }
    //    }
    //}

    //#endregion

    //public ActionViewModel(ActionType type, PlayerViewModel? actor = null, int points = 0, PlayerViewModel? scorer = null, string subType = "")
    //{
    //    Type = type;
    //    Actor = actor;
    //    Scorer = scorer ?? actor;
    //    Points = points;
    //    SubType = subType;
    //}

    //public ActionViewModel(ActionState action, PlayerViewModel playerOne, PlayerViewModel playerTwo)
    //{
    //    Type = action.Type;
    //    Actor = action.Actor != Guid.Empty ? (action.Actor == playerOne.Guid ? playerOne : playerTwo) : null;
    //    Scorer = (action.Scorer != Guid.Empty ? (action.Scorer == playerOne.Guid ? playerOne : playerTwo) : null) ?? Actor;
    //    Points = action.Points;
    //    SubType = action.SubType;
    //}

    //public ActionState GetActionState(PlayerViewModel first, PlayerViewModel second)
    //{
    //    return new ActionState(Guid, Actor?.Guid ?? Guid.Empty, Scorer?.Guid ?? Guid.Empty, Points, Type, SubType);
    //}
}