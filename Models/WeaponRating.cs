using System.Text.Json.Nodes;

namespace Lightspeed;

/// <summary>
/// The fencer's rating for a specific weapon class.
/// </summary>
public sealed class WeaponRating
{
    public WeaponClass Class { get; set; } = WeaponClass.Rey;
    public Rank Rank { get; set; } = Rank.U;

    public WeaponRating() { }
    public WeaponRating(WeaponClass weaponClass, Rank rank)
    {
        Class = weaponClass;
        Rank = rank;
    }

    /// <summary>
    /// Creates a new instance of the WeaponRating class from a JSON node representing a SaberSport weapon rating.
    /// </summary>
    /// <remarks>If the JSON node does not contain valid or expected data, the method returns null instead of
    /// throwing an exception.</remarks>
    /// <param name="node">A JsonNode containing the weapon rating data to parse. Can be null.</param>
    /// <returns>A WeaponRating instance parsed from the specified JSON node, or null if the node is null or cannot be parsed.</returns>
    public static WeaponRating? FromSaberSport(JsonNode? node)
    {
        if (node is null)
            return null;

        try
        {
            return new()
            {
                Class = Enum.Parse<WeaponClass>(node["type"]?.GetValue<string>() ?? "", true),
                Rank = node["rank"]?.GetValue<string>()
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error parsing weapon rating: {ex.Message}");
            return null;
        }
    }
}
