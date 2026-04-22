using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LightspeedShared.Services;
using System.Collections.ObjectModel;

namespace Lightspeed.MatchComponents;

/// <summary>
/// Adds actions to a match, which means the UI will allow users to input actions instead of just points
/// </summary>
public partial class ActionsViewModel<T>(ClockViewModel clock, LeftRightViewModel<T> sides, PriorityViewModel<T>? priority) : ObservableObject where T : ParticipantViewModel
{
    /// <summary>
    /// The actions, in order of their occurrence
    /// </summary>
    public ObservableCollection<ActionViewModel<T>> Actions { get; set; } = [];

    /// <summary>
    /// Converts to the model representation
    /// </summary>
    public IEnumerable<Lightspeed.Action> ToModel() => Actions.Select(a => a.ToModel(sides));

    /// <summary>
    /// Adds and applies the action, overriding with priority if necessary
    /// </summary>
    public void AddAction(ActionViewModel<T> action)
    {
        // process priority
        if (priority is not null && priority.InPriority)
            priority.InPriority = false;

        Actions.Insert(0, action);
        action.Apply();
    }

    /// <summary>
    /// Reverses the last action and removes it from the list
    /// </summary>
    [RelayCommand]
    public void Undo()
    {
        if (Actions.Count > 0)
        {
            if (Actions[0].Type == ActionType.Overtime)
                clock.RemoveOvertime();
            else
                Actions[0].Undo();
            Actions.RemoveAt(0);
        }
    }

    #region Clock Commands

    /// <summary>
    /// Starts the clock
    /// </summary>
    [RelayCommand]
    private void Fence()
    {
        if (!clock.IsTimeUp)
            clock.Start();
    }

    /// <summary>
    /// Stops the clock
    /// </summary>
    [RelayCommand]
    private void Break() => clock.Break();

    [RelayCommand]
    private void Overtime()
    {
        if (clock.IsTimeUp && !sides.HasWinner)
        {
            clock.AddOvertime();
            AddAction(sides.NewOvertime());
            if (priority is not null && clock.IsPriorityOvertime)
                priority.AssignPriority();
        }
    }

    #endregion

    #region Left Player Commands

    [RelayCommand]
    public void LeftConcession() => AddAction(sides.NewConcession(SideReference.Left));

    [RelayCommand]
    public void LeftOutOfBounds() => AddAction(sides.NewOutOfBounds(SideReference.Left));

    [RelayCommand]
    public void LeftDisarm() => AddAction(sides.NewDisarm(SideReference.Left));

    [RelayCommand]
    public void LeftFirstContact()
    {
        if (priority is not null && priority.RightHasPriority)
            AddAction(sides.NewPriority(SideReference.Right, priority.PriorityPoints));
        else
            AddAction(sides.NewFirstContact(SideReference.Left));
    }

    [RelayCommand]
    public void LeftIndirect() => AddAction(sides.NewIndirect(SideReference.Left));

    [RelayCommand]
    public void LeftHeadshotOverride() => AddAction(sides.NewHeadshotOverride(SideReference.Left));

    [RelayCommand]
    public void LeftClean() => AddAction(sides.NewClean(SideReference.Left));

    [RelayCommand]
    public void LeftHeadshot() => AddAction(sides.NewHeadshot(SideReference.Left));

    [RelayCommand]
    public void LeftReturn() => AddAction(sides.NewReturn(SideReference.Left));

    #endregion

    #region Right Player Commands

    [RelayCommand]
    public void RightConcession() => AddAction(sides.NewConcession(SideReference.Right));

    [RelayCommand]
    public void RightOutOfBounds() => AddAction(sides.NewOutOfBounds(SideReference.Right));

    [RelayCommand]
    public void RightDisarm() => AddAction(sides.NewDisarm(SideReference.Right));

    [RelayCommand]
    public void RightFirstContact()
    {
        if (priority is not null && priority.LeftHasPriority)
            AddAction(sides.NewPriority(SideReference.Left, priority.PriorityPoints));
        else
            AddAction(sides.NewFirstContact(SideReference.Right));
    }

    [RelayCommand]
    public void RightIndirect() => AddAction(sides.NewIndirect(SideReference.Right));

    [RelayCommand]
    public void RightHeadshotOverride() => AddAction(sides.NewHeadshotOverride(SideReference.Right));

    [RelayCommand]
    public void RightClean() => AddAction(sides.NewClean(SideReference.Right));

    [RelayCommand]
    public void RightHeadshot() => AddAction(sides.NewHeadshot(SideReference.Right));

    [RelayCommand]
    public void RightReturn() => AddAction(sides.NewReturn(SideReference.Right));

    #endregion

    /// <summary>
    /// Simultaneous action, which does nothing unless there is priority on one side,
    /// in which case it will add a priority action for the side with priority
    /// </summary>
    [RelayCommand]
    private void Simultaneous()
    {
        if (priority is not null && priority.InPriority && priority.Priority is not null)
            AddAction(sides.NewPriority(priority.PrioritySide, priority.PriorityPoints));
    }
}
