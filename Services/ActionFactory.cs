using Lightspeed.MatchComponents;

namespace LightspeedShared.Services;

/// <summary>
/// Instead of instantiating this, this service is written as an extension class since all the methods
/// follow a simple formula and all want access to the sides.
/// </summary>
public static class ActionFactory
{
    /// <summary>
    /// Creates a new clean action
    /// </summary>
    public static ActionViewModel NewClean(this LeftRightViewModel sides, SideReference actor) => new()
    {
        Type = ActionType.Clean,
        Actor = sides.ToSide(actor),
        Scorer = sides.ToSide(actor),
        Points = PointValues.Clean
    };

    /// <summary>
    /// Creates a new concession
    /// </summary>
    public static ActionViewModel NewConcession(this LeftRightViewModel sides, SideReference actor) => new()
    {
        Type = ActionType.Conceded,
        Actor = sides.ToSide(actor),
        Scorer = sides.ToSide(actor.ToOtherSide()),
        Points = PointValues.Conceded
    };

    /// <summary>
    /// Creates a new disarm action
    /// </summary>
    public static ActionViewModel NewDisarm(this LeftRightViewModel sides, SideReference actor) => new()
    {
        Type = ActionType.Disarm,
        Actor = sides.ToSide(actor),
        Scorer = sides.ToSide(actor.ToOtherSide()),
        Points = PointValues.Disarm
    };

    /// <summary>
    /// Creates a new first contact action
    /// </summary>
    public static ActionViewModel NewFirstContact(this LeftRightViewModel sides, SideReference actor) => new()
    {
        Type = ActionType.FirstContact,
        Actor = sides.ToSide(actor),
        Scorer = sides.ToSide(actor),
        Points = PointValues.FirstContact
    };

    /// <summary>
    /// Creates a new headshot action
    /// </summary>
    public static ActionViewModel NewHeadshot(this LeftRightViewModel sides, SideReference actor) => new()
    {
        Type = ActionType.Headshot,
        Actor = sides.ToSide(actor),
        Scorer = sides.ToSide(actor),
        Points = PointValues.Headshot
    };

    /// <summary>
    /// Creates a new headshot override action
    /// </summary>
    public static ActionViewModel NewHeadshotOverride(this LeftRightViewModel sides, SideReference actor) => new()
    {
        Type = ActionType.HeadshotOverride,
        Actor = sides.ToSide(actor),
        Scorer = sides.ToSide(actor),
        Points = PointValues.HeadshotOverride
    };

    /// <summary>
    /// Creates a new indirect action
    /// </summary>
    public static ActionViewModel NewIndirect(this LeftRightViewModel sides, SideReference actor) => new()
    {
        Type = ActionType.Indirect,
        Actor = sides.ToSide(actor),
        Scorer = sides.ToSide(actor),
        Points = PointValues.Indirect
    };

    /// <summary>
    /// Creates a new Out of Bounds action
    /// </summary>
    public static ActionViewModel NewOutOfBounds(this LeftRightViewModel sides, SideReference actor) => new()
    {
        Type = ActionType.OutOfBounds,
        Actor = sides.ToSide(actor),
        Scorer = sides.ToSide(actor.ToOtherSide()),
        Points = PointValues.OutOfBounds
    };

    /// <summary>
    /// Creates a new Overtime action
    /// </summary>
    public static ActionViewModel NewOvertime(this LeftRightViewModel sides) => new()
    {
        Type = ActionType.Overtime
    };

    /// <summary>
    /// Creates a new priority action
    /// </summary>
    public static ActionViewModel NewPriority(this LeftRightViewModel sides, SideReference actor, int points) => new()
    {
        Type = ActionType.Priority,
        Actor = sides.ToSide(actor),
        Scorer = sides.ToSide(actor),
        Points = points
    };

    /// <summary>
    /// Creates a new return action
    /// </summary>
    public static ActionViewModel NewReturn(this LeftRightViewModel sides, SideReference actor) => new()
    {
        Type = ActionType.Return,
        Actor = sides.ToSide(actor),
        Scorer = sides.ToSide(actor),
        Points = PointValues.Return
    };
}
