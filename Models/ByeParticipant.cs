namespace Lightspeed;

/// <summary>
/// Represents a bye, i.e., a placeholder participant that automatically loses.
/// </summary>
public sealed class ByeParticipant : IParticipant
{
    public readonly static Guid ByeGuid = new(0xffffffff, 0xffff, 0xffff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff);
    public Guid Id => ByeGuid;
    public string Name => "BYE";
}
