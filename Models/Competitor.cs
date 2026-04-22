using MessagePack;

namespace Lightspeed;

/// <summary>
/// A competitor is an individual in a tournament.
/// </summary>
[MessagePackObject]
public class Competitor
{
    [Key(0)]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Key(1)]
    public int? OnlineId { get; set; } = null;

    [Key(2)]
    public string FirstName { get; set; } = string.Empty;

    [Key(3)]
    public string LastName { get; set; } = string.Empty;

    [Key(4)]
    public string? Club { get; set; }

    [Key(5)]
    public WeaponClass WeaponOfChoice { get; set; } = WeaponClass.Rey;

    [Key(6)]
    public Rank Rank { get; set; } = Rank.U;
}