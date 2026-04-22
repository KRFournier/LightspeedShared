using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Lightspeed.Network;
using Lightspeed.Network.Messages;

namespace Lightspeed.MatchComponents;

/// <summary>
/// A competitor is an individual in a tournament.
/// </summary>
public partial class CompetitorViewModel : ObservableObject, IRecipient<CompetitorStateMessage>
{
    #region Properties

    /// <summary>
    /// The competitor's unique identifier. This is used to identify the player across matches and tournaments.
    /// </summary>
    public Guid Guid { get; set; } = Guid.NewGuid();

    /// <summary>
    /// The compititor's online identifier if they have one
    /// </summary>
    [ObservableProperty]
    public partial int? OnlineId { get; set; }

    /// <summary>
    /// The competitor's first name
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FullName))]
    public partial string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// The compeitor's last name
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FullName))]
    public partial string LastName { get; set; } = string.Empty;

    /// <summary>
    /// The competitor's full name
    /// </summary>
    public string FullName => $"{LastName}, {FirstName}";

    /// <summary>
    /// The competitor's club affiliation
    /// </summary>
    [ObservableProperty]
    public partial string? Club { get; set; }

    /// <summary>
    /// The competitor's weapon of choice in this tournament
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Rank))]
    [NotifyPropertyChangedFor(nameof(IsRey))]
    [NotifyPropertyChangedFor(nameof(IsRen))]
    [NotifyPropertyChangedFor(nameof(IsTano))]
    public partial WeaponClass WeaponOfChoice { get; set; } = WeaponClass.Rey;
    public bool IsRey => WeaponOfChoice == WeaponClass.Rey;
    public bool IsRen => WeaponOfChoice == WeaponClass.Ren;
    public bool IsTano => WeaponOfChoice == WeaponClass.Tano;

    /// <summary>
    /// The competitor's effective rank in this tournament
    /// </summary>
    [ObservableProperty]
    public partial Rank Rank { get; set; } = Rank.U;

    #endregion

    #region Message Handlers

    /// <summary>
    /// Updates the competitor with the information sent via the network
    /// </summary>
    public void Receive(CompetitorStateMessage message)
    {
        Guid = message.Competitor.Id;
        OnlineId = message.Competitor.OnlineId;
        FirstName = message.Competitor.FirstName;
        LastName = message.Competitor.LastName;
        Club = message.Competitor.Club;
        WeaponOfChoice = message.Competitor.WeaponOfChoice;
        Rank = message.Competitor.Rank;
    }

    #endregion

    public CompetitorViewModel(Guid id, IMessenger messenger)
    {
        messenger.Register(this, id);
    }

    public Competitor ToModel() => new()
    {
        Id = Guid,
        OnlineId = OnlineId,
        FirstName = FirstName,
        LastName = LastName,
        Club = Club,
        WeaponOfChoice = WeaponOfChoice,
        Rank = Rank
    };
}
