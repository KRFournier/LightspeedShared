using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Lightspeed.Network.Messages;

namespace Lightspeed.MatchComponents;

/// <summary>
/// This class tracks honor.
/// </summary>
public partial class HonorViewModel : ObservableObject, IRecipient<HonorStateMessage>
{
    /// <summary>
    /// The honor of the player. Honor is a positive integer that can be increased or decreased during a match.
    /// </summary>
    [ObservableProperty]
    public partial int HonorAmount { get; set; } = 0;

    /// <summary>
    /// Increases the honor by one
    /// </summary>
    [RelayCommand]
    public void AddHonor() => HonorAmount++;

    /// <summary>
    /// Decreases the honor, but not below zero
    /// </summary>
    [RelayCommand]
    public void RemoveHonor()
    {
        if (HonorAmount > 0)
            HonorAmount--;
    }

    /// <summary>
    /// Updates the honor when a new HonorStateMessage is received.
    /// </summary>
    public void Receive(HonorStateMessage message)
    {
        HonorAmount = message.Honor;
    }

    public HonorViewModel(Guid participantGuid, IMessenger messenger)
    {
        messenger.Register(this, participantGuid);
    }

    public override string ToString() => $"{HonorAmount} Honor";
}
