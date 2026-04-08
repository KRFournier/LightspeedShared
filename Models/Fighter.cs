using LiteDB;

namespace Lightspeed;

/// <summary>
/// A fighter is a Lightspeed competitor who has or will participate in events.
/// This is a global representation not tied to any specific event or team.
/// Fighters persist between application sessions.
/// </summary>
public class Fighter : CollectionObject
{
    public int? OnlineId { get; set; } = null;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Club { get; set; }
    public Rank Rey { get; set; } = Rank.U;
    public Rank Ren { get; set; } = Rank.U;
    public Rank Tano { get; set; } = Rank.U;

    /// <summary>
    /// Represents the fighter as a string in "FirstName LastName" format.
    /// </summary>
    public override string ToString() => Name;

    /// <summary>
    /// Gets the full name, consisting of the first and last name combined.
    /// </summary>
    [BsonIgnore]
    public string Name => $"{FirstName} {LastName}";
}
