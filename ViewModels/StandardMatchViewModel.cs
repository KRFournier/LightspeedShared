using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Lightspeed.Network;
using Lightspeed.Network.Messages;
using Lightspeed.Services;
using System.Collections.ObjectModel;

namespace Lightspeed.ViewModels;

/// <summary>
/// A match
/// </summary>
public partial class StandardMatchViewModel : MatchViewModel,
    IRecipient<ClockStateMessage>, IRecipient<NewActionMessage>, IRecipient<UndoActionMessage>,
    IRecipient<ActionModifiedMessage>
{
    #region Properties

    /// <summary>
    /// The timer and rounds
    /// </summary>
    [ObservableProperty]
    public partial ClockViewModel Clock { get; set; }

    /// <summary>
    /// Actions
    /// </summary>
    public ObservableCollection<ActionViewModel> Actions { get; set; } = [];

    #endregion

    #region Message Handlers

    public void Receive(ClockStateMessage message)
    {
        Clock.UpdateState(message.State);
    }

    public void Receive(NewActionMessage message)
    {
        SetNewAction(message.State);
    }

    public void Receive(UndoActionMessage message)
    {
        UndoAction(message.State);
    }

    public void Receive(ActionModifiedMessage message)
    {
        ModifyAction(message.State);
    }

    #endregion

    public StandardMatchViewModel(IServiceProvider serviceProvider, IMessenger messenger, MatchFactory matchFactory) : base(serviceProvider, messenger)
    {
        Clock = matchFactory.NewClock(Settings);

        if (!Design.IsDesignMode)
            messenger.RegisterAll(this, Guid);
    }

    public override StandardMatch ToModel()
    {
        var model = new StandardMatch()
        {
            Clock = Clock.ToModel(),
            Actions = [.. Actions.Select(a => a.ToModel(this))]
        };
        UpdateMatchModel(model);
        return model;
    }

    public override StandardMatchState ToState() => new()
    {
        Id = Guid,
        First = First.ToState(),
        Second = Second.ToState(),
        Settings = Settings.ToState(),
        Clock = Clock.ToState(),
        Actions = [.. Actions.Select(a => a.ToState(this))],
        Priority = new()
        {
            PrioritySide = PrioritySide,
            PriorityPoints = PriorityPoints,
            InPriority = InPriority
        }
    };

    protected override ClockState? GetClockState() => Clock.ToState();

}
