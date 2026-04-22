using MessagePack;

namespace Lightspeed;

/// <summary>
/// Identifies a participant's side in the match.
/// </summary>
public enum SideReference
{
    Neither,
    Left,
    Right
}

/// <summary>
/// Represents a two-sided match with a left and right side.
/// </summary>
[MessagePackObject]
public class LeftRightSide
{
    [Key(0)]
    public Side? Left { get; set; }

    [Key(1)]
    public Side? Right { get; set; }

    [Key(2)]
    public bool IsSwapped { get; set; } = false;

    [Key(3)]
    public SideReference WinningSide { get; set; } = SideReference.Neither;
}
