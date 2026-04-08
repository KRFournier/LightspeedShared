using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Lightspeed.Network;
using Lightspeed.Network.Messages;
using System.Collections.ObjectModel;

namespace Lightspeed.ViewModels;

/// <summary>
/// A match
/// </summary>
public partial class StandardMatchViewModel : MatchViewModel
{
    #region Properties

    [ObservableProperty]
    public partial ClockViewModel Clock { get; set; }

    public ObservableCollection<Lightspeed.Action> Actions { get; set; } = [];

    [ObservableProperty]
    public partial PriorityViewModel Priority { get; set; }

    #endregion

    public StandardMatchViewModel(IServiceProvider serviceProvider, IMessenger messenger) : base(serviceProvider, messenger)
    {
        Clock = New<ClockViewModel>();
        Priority = New<PriorityViewModel>();

        if (!Design.IsDesignMode)
        {
            messenger.Register<ClockStateMessage, Guid>(this, Guid, (_, m) => Clock.UpdateState(m.State));
            messenger.Register<NewActionMessage, Guid>(this, Guid, (_, m) => SetNewAction(m.State));
            messenger.Register<UndoActionMessage, Guid>(this, Guid, (_, m) => UndoAction(m.State));
            messenger.Register<ActionModifiedMessage, Guid>(this, Guid, (_, m) => ModifyAction(m.State));
            messenger.Register<PriorityChangedMessage, Guid>(this, Guid, (_, m) => Priority.UpdateState(m.State.Priority));
        }
    }

    public override StandardMatch ToModel() => new()
    {
        Id = Guid,
        Number = Number,
        Clock = Clock.ToModel(),
        First = First.ToModel(),
        Second = Second.ToModel(),
        IsMatchStarted = IsMatchStarted,
        Actions = [.. Actions],
        Priority = Priority.ToModel(),
        Winner = WinningSide
    };

    public override StandardMatchState ToState() => new()
    {
        Id = Guid,
        First = First.ToState(),
        Second = Second.ToState(),
        Settings = Settings.ToState(),
        Clock = Clock.ToState(),
        Actions = [.. Actions.Select(a => a.ToState())],
        Priority = Priority.ToState()
    };


    public override MatchSummary ToSummary() => new()
    {
        Id = Guid,
        Number = Number ?? 0,
        First = First.ToState(),
        Second = Second.ToState(),
        Winner = WinningSide,
        IsStarted = IsMatchStarted,
        IsCompleted = IsMatchCompleted,
        Clock = Clock.ToState()
    };

    #region Actions

    /// <summary>
    /// Adds the action and updates the player states
    /// </summary>
    public void SetNewAction(NewActionState state)
    {
        Clock.UpdateState(state.Clock);
        First.UpdateState(state.First);
        Second.UpdateState(state.Second);

        if (state.Action is not null)
            Actions.Insert(0, state.Action.ToModel());
    }

    /// <summary>
    /// Adds the action and updates the player states
    /// </summary>
    public void ModifyAction(ActionModified state)
    {
        First.Points = state.Points;
        Second.Points = state.Points;

        var action = Actions.FirstOrDefault(a => a.Id == state.ActionId);
        action?.Points = state.Points;
    }

    /// <summary>
    /// Updates the players states and removes the last action
    /// </summary>
    public void UndoAction(UndoActionState state)
    {
        Clock.UpdateState(state.Clock);
        First.UpdateState(state.First);
        Second.UpdateState(state.Second);

        var action = Actions.FirstOrDefault(a => a.Id == state.ActionId);
        if (action is not null)
            Actions.Remove(action);
    }

    public IEnumerable<Lightspeed.Action> FirstActions => Actions.Where(a => a.Actor == Side.First);
    public IEnumerable<Lightspeed.Action> SecondActions => Actions.Where(a => a.Actor == Side.Second);

    #endregion

}
