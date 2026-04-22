using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Lightspeed.Network.Messages;
using Lightspeed.ViewModels;

namespace Lightspeed.MatchComponents;

/// <summary>
/// This class manages priority for a left-right match.
/// </summary>
public partial class PriorityViewModel<T> : ObservableObject, IRecipient<PriorityChangedMessage>
    where T : ParticipantViewModel
{
    /// <summary>
    /// used to assign random priority in the case of a coin flip
    /// </summary>
    private readonly Random _random = new();

    /// <summary>
    /// The two-sides of the match
    /// </summary>
    private readonly LeftRightViewModel<T> _sides;

    #region Properties

    /// <summary>
    /// The points to assign a player who scores a priority action.
    /// Currently set to 3, but may be changed in the future if we want to adjust how priority actions affect the match.
    /// </summary>
    [ObservableProperty]
    public partial int PriorityPoints { get; set; } = PointValues.Priority;

    /// <summary>
    /// The side with priority. Null if priority has not been assigned.
    /// Does not get cleared! We need to know who had priority last so we can alternate.
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(LeftHasPriority))]
    [NotifyPropertyChangedFor(nameof(RightHasPriority))]
    [NotifyPropertyChangedFor(nameof(PrioritySide))]
    public partial SideViewModel<T>? Priority { get; set; }

    /// <summary>
    /// Determines if priority is active
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(LeftHasPriority))]
    [NotifyPropertyChangedFor(nameof(RightHasPriority))]
    [NotifyPropertyChangedFor(nameof(PrioritySide))]
    public partial bool InPriority { get; set; } = false;

    /// <summary>
    /// Determines if Left has priority
    /// </summary>
    public bool LeftHasPriority => InPriority && _sides.Left == Priority;

    /// <summary>
    /// Determines if Right has priority
    /// </summary>
    public bool RightHasPriority => InPriority && _sides.Right == Priority;

    /// <summary>
    /// The priority participant referenced by position in the match
    /// </summary>
    public SideReference PrioritySide
    {
        get => _sides.ToReference(Priority);
        set => Priority = _sides.ToSide(value);
    }

    #endregion

    #region Commands

    /// <summary>
    /// Assigns priority using Honor if possible
    /// </summary>
    [RelayCommand]
    public void AssignPriority()
    {
        if (_sides.Left is null || _sides.Right is null || _sides.HasWinner || InPriority)
            return;

        if (Priority is null)
        {
            if ((_sides.Left?.Participant.Honor.HonorAmount ?? 0) > (_sides.Right?.Participant.Honor.HonorAmount ?? 0))
                Priority = _sides.Left;
            else if ((_sides.Left?.Participant.Honor.HonorAmount ?? 0) < (_sides.Right?.Participant.Honor.HonorAmount ?? 0))
                Priority = _sides.Right;
            else
                Priority = _random.Next(2) == 0 ? _sides.Left : _sides.Right;
        }
        else
        {
            if (Priority == _sides.Left)
                Priority = _sides.Right;
            else
                Priority = _sides.Left;
        }

        InPriority = true;
    }

    /// <summary>
    /// Switches priority to the other player.
    /// </summary>
    [RelayCommand]
    public void SwapPriority()
    {
        if (LeftHasPriority)
            Priority = _sides.Right;
        else if (RightHasPriority)
            Priority = _sides.Left;
    }

    #endregion

    #region Message Handlers

    /// <summary>
    /// Listens for changes from the network
    /// </summary>
    public void Receive(PriorityChangedMessage message)
    {
        PrioritySide = message.Priority.PrioritySide;
        PriorityPoints = message.Priority.PriorityPoints;
        InPriority = message.Priority.InPriority;
    }

    #endregion

    public PriorityViewModel(Guid matchGuid, LeftRightViewModel<T> sides, IMessenger messenger)
    {
        _sides = sides;
        messenger.RegisterAll(this, matchGuid);
    }

    public Priority ToModel() => new()
    {
        PrioritySide = PrioritySide,
        PriorityPoints = PriorityPoints,
        InPriority = InPriority
    };

    public override string ToString() => Priority is null ? "No priority" : $"{Priority?.Participant} has priority";
}
