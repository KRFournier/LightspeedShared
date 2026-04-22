using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Lightspeed.Network;
using Lightspeed.Network.Messages;
using System.ComponentModel;

namespace Lightspeed.MatchComponents;

/// <summary>
/// This class tracks penalty information.
/// </summary>
public partial class PenaltiesViewModel : ObservableObject, IRecipient<PenaltiesStateMessage>
{
    #region Properties

    /// <summary>
    /// The player/team's current card
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsDisqualified))]
    public partial Card Card { get; set; } = Card.None;

    /// <summary>
    /// Whether or not the player/team has been ejected
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsDisqualified))]
    public partial bool IsEjected { get; set; } = false;

    /// <summary>
    /// Determines if the player/team is disqualified either by card or ejection
    /// </summary>
    public bool IsDisqualified => Card == Card.Black || IsEjected;

    #endregion

    #region Commands

    /// <summary>
    /// Moves to the next card
    /// </summary>
    [RelayCommand]
    public void GiveCard()
    {
        Card = Card switch
        {
            Card.None => Card.White,
            Card.White => Card.Yellow,
            Card.Yellow => Card.Red,
            Card.Red => Card.Black,
            _ => Card.Black
        };
    }

    /// <summary>
    /// Moves to the previous card
    /// </summary>
    public void RemoveCard()
    {
        Card = Card switch
        {
            Card.Black => Card.Red,
            Card.Red => Card.Yellow,
            Card.Yellow => Card.White,
            Card.White => Card.None,
            _ => Card.None
        };
    }

    /// <summary>
    /// Ejects the player/team if they haven't already been ejected
    /// </summary>
    [RelayCommand]
    public void Eject()
    {
        if (!IsEjected)
            IsEjected = true;
    }

    /// <summary>
    /// Unejects the player/team if they were ejected. Usually used to undo an ejection.
    /// </summary>
    public void Uneject()
    {
        if (IsEjected)
            IsEjected = false;
    }

    #endregion

    #region Message Handlers

    /// <summary>
    /// Listens for changes from the network
    /// </summary>
    public void Receive(PenaltiesStateMessage message)
    {
        Card = message.Penalties.Card;
        IsEjected = message.Penalties.IsEjected;
    }

    #endregion

    public PenaltiesViewModel(Guid participantGuid, IMessenger messenger)
    {
        messenger.Register(this, participantGuid);
    }

    public Penalties ToModel() => new()
    {
        Card = this.Card,
        IsEjected = this.IsEjected
    };

    public override string ToString()
    {
        if (IsEjected)
            return "Ejected";
        else if (Card != Card.None)
            return $"{Card} Card";
        else
            return "No Penalties";
    }
}
