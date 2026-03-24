using LiteDB;

namespace Lightspeed;

/// <summary>
/// Represents an individual fencer in a tournament.
/// </summary>
public sealed class Player : IParticipant
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public int? OnlineId { get; set; } = null;
    public string? Club { get; set; }
    public Rank Rank { get; set; } = Rank.U;

    public Card Card { get; set; } = Card.None;
    public int Honor { get; set; } = 0;
    public bool IsEjected { get; set; } = false;
    public WeaponClass WeaponOfChoice { get; set; } = WeaponClass.Rey;

    public int StartingLife { get; set; } = 0;

    [BsonIgnore]
    public string Name => $"{FirstName} {LastName}";
}
