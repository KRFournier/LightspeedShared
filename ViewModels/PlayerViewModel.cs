using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Lightspeed.Network;
using Lightspeed.Network.Messages;
using Lightspeed.Services;

namespace Lightspeed.ViewModels;

/// <summary>
/// A player
/// </summary>
public sealed partial class PlayerViewModel : ParticipantViewModel
{
    #region Abstract Implementation

    public override string Name => $"{FirstName} {LastName}";
    public override string? Subtitle => Club;

    public override int PowerLevel => Rank.Weight;

    public override bool IsBye => false;
    public override bool IsEmpty => false;

    public override PlayerViewModel CurrentPlayer => this;

    #endregion

    #region Properties

    [ObservableProperty]
    public partial int? OnlineId { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Name))]
    public partial string FirstName { get; set; } = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Name))]
    public partial string LastName { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string? Club { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsDisqualified))]
    public partial Card Card { get; set; } = Card.None;

    [ObservableProperty]
    public partial int Honor { get; set; } = 0;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsDisqualified))]
    public partial bool IsEjected { get; set; } = false;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Rank))]
    [NotifyPropertyChangedFor(nameof(IsRey))]
    [NotifyPropertyChangedFor(nameof(IsRen))]
    [NotifyPropertyChangedFor(nameof(IsTano))]
    public partial WeaponClass WeaponOfChoice { get; set; } = WeaponClass.Rey;
    public bool IsRey => WeaponOfChoice == WeaponClass.Rey;
    public bool IsRen => WeaponOfChoice == WeaponClass.Ren;
    public bool IsTano => WeaponOfChoice == WeaponClass.Tano;

    [ObservableProperty]
    public partial bool ShowWeapon { get; set; } = false;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PowerLevel))]
    public partial Rank Rank { get; set; } = Rank.U;

    /// <summary>
    /// Determines if the Player is disqualified either by card or ejection
    /// </summary>
    public override bool IsDisqualified => Card == Card.Black || IsEjected;

    #endregion

    public PlayerViewModel(IServiceProvider serviceProvider, IMessenger messenger, IActiveTournamentService activeTournamentService)
        : base(serviceProvider, messenger)
    {
        if (Design.IsDesignMode)
            ShowWeapon = true;
        else
        {
            ShowWeapon = activeTournamentService.ShowingWeapons;
            messenger.Register<HonorStateMessage, Guid>(this, Guid, (_, m) => Honor = m.State.Honor);
        }
    }

    /// <summary>
    /// Converts to a <see cref="Player"/>
    /// </summary>
    public override Player ToModel() => new()
    {
        Id = Guid,
        FirstName = FirstName,
        LastName = LastName,
        OnlineId = OnlineId,
        Club = Club,
        Rank = Rank,
        Card = Card,
        Honor = Honor,
        IsEjected = IsEjected,
        WeaponOfChoice = WeaponOfChoice
    };

    public override PlayerState ToState() => new()
    {
        Card = Card,
        Ejected = IsEjected,
        Honor = Honor,
        Club = Club,
        Weapon = WeaponOfChoice.ToString().ToLower(),
        Rank = Rank.Letter
    };


    public override void UpdateState(ParticipantState? state)
    {
        if (state is PlayerState playerState)
        {
            Card = playerState.Card;
            IsEjected = playerState.Ejected;
            Honor = playerState.Honor;
        }
        else
        {
            throw new InvalidOperationException("Cannot load PlayerViewModel from non-PlayerState");
        }
    }
}
