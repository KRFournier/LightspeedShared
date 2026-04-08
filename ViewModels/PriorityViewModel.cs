using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Lightspeed.Network;

namespace Lightspeed.ViewModels;

/// <summary>
/// Adds support for priority to a match
/// </summary>
public partial class PriorityViewModel(IServiceProvider serviceProvider, IMessenger messenger) : ViewModelBase(serviceProvider, messenger)
{
    #region Properties

    [ObservableProperty]
    public partial Side PreviousPriority { get; set; } = Side.Neither;

    [ObservableProperty]
    public partial int PriorityPoints { get; set; } = 3;

    [ObservableProperty]
    public partial bool InPriority { get; set; } = false;

    #endregion

    public Priority ToModel() => new()
    {
        PreviousPriority = PreviousPriority,
        PriorityPoints = PriorityPoints,
        InPriority = InPriority
    };

    public override string ToString()
    {
        if (InPriority)
            return PreviousPriority == Side.First ? $"<- {PriorityPoints}" : $"{PriorityPoints} ->";
        return "< 0 >";
    }

    public PriorityState ToState() => new()
    {
        PreviousPriority = PreviousPriority,
        PriorityPoints = PriorityPoints,
        InPriority = InPriority
    };

    public void UpdateState(PriorityState? state)
    {
        if (state is null)
            return;

        PreviousPriority = state.PreviousPriority;
        PriorityPoints = state.PriorityPoints;
        InPriority = state.InPriority;
    }
}