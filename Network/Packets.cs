using MessagePack;

namespace Lightspeed.Network;

/// <summary>
/// Open tournament description
/// </summary>
[MessagePackObject]
public sealed class OpenTournament
{
    [Key(0)]
    public Guid Id { get; set; }

    [Key(1)]
    public string? Name { get; set; }

    [Key(2)]
    public string? Competition { get; set; }
}

/// <summary>
/// Client response for open tournaments
/// </summary>
[MessagePackObject]
public sealed class OpenTournaments
{
    [Key(0)]
    public OpenTournament[] Tournaments { get; set; } = [];
}

/// <summary>
/// A description of a known fencing ring
/// </summary>
[MessagePackObject]
public sealed class FencingRing
{
    [Key(0)]
    public Guid Id { get; set; }

    [Key(1)]
    public string? Name { get; set; }
}

/// <summary>
/// A listing of all known fencing rings
/// </summary>
[MessagePackObject]
public sealed class FencingRings
{
    [Key(0)]
    public FencingRing[] Rings { get; set; } = [];
}

/// <summary>
/// Match group description
/// </summary>
[MessagePackObject]
public sealed class MatchGroupState
{
    [Key(0)]
    public string? Name { get; set; }

    [Key(1)]
    public string? Color { get; set; }

    [Key(2)]
    public bool IsCompleted { get; set; }
}

/// <summary>
/// Client response for match groups
/// </summary>
[MessagePackObject]
public sealed class MatchGroupsState
{
    [Key(0)]
    public string? Type { get; set; }

    [Key(1)]
    public MatchGroupState[] Groups { get; set; } = [];
}

/// <summary>
/// Match summary
/// </summary>
[MessagePackObject]
public class MatchSummary
{
    [Key(0)]
    public Guid Id { get; set; }

    [Key(1)]
    public int Number { get; set; }

    [Key(2)]
    public ScoreState? First { get; set; }

    [Key(3)]
    public ScoreState? Second { get; set; }

    [Key(4)]
    public Side Winner { get; set; }

    [Key(5)]
    public string? Color { get; set; }

    [Key(6)]
    public bool IsStarted { get; set; }

    [Key(7)]
    public bool IsCompleted { get; set; }

    [Key(8)]
    public ClockState? Clock { get; set; }
}

/// <summary>
/// Client response for match summaries
/// </summary>
[MessagePackObject]
public sealed class MatchSummaries
{
    [Key(0)]
    public MatchSummary[] Summaries { get; set; } = [];

    [Key(1)]
    public bool ShowWeapons { get; set; } = false;
}

/// <summary>
/// Match Summaries request packet
/// </summary>
[MessagePackObject]
public sealed class MatchGoLiveRequest
{
    [Key(0)]
    public Guid GroupId { get; set; }

    [Key(1)]
    public Guid MatchId { get; set; }

    [Key(2)]
    public Guid RingId { get; set; }

    [Key(3)]
    public Guid ClientId { get; set; }
}

/// <summary>
/// Base class of all participants
/// </summary>
[Union(0, typeof(PlayerState))]
[MessagePackObject]
public abstract class ParticipantState
{
    [Key(0)]
    public Guid Id { get; set; }

    [Key(1)]
    public int Score { get; set; } = 0;

    [Key(2)]
    public bool ShowWeapon { get; set; } = false;
}

/// <summary>
/// The state of a player
/// </summary>
[MessagePackObject]
public sealed class PlayerState : ParticipantState
{
    [Key(3)]
    public Card Card { get; set; }

    [Key(4)]
    public bool Ejected { get; set; }

    [Key(5)]
    public int Honor { get; set; }

    [Key(6)]
    public string? Club { get; set; }

    [Key(7)]
    public string? Weapon { get; set; }

    [Key(8)]
    public char Rank { get; set; }
}

/// <summary>
/// A participant's score
/// </summary>
[MessagePackObject]
public sealed class ScoreChangedState
{
    [Key(0)]
    public Guid MatchId { get; set; }

    [Key(1)]
    public Side Side { get; set; }

    [Key(2)]
    public int Score { get; set; }
}

/// <summary>
/// A single action in a match
/// </summary>
[MessagePackObject]
public sealed class ActionState
{
    [Key(0)]
    public Guid Id { get; set; }

    [Key(1)]
    public Side Actor { get; set; }

    [Key(2)]
    public Side Scorer { get; set; }

    [Key(3)]
    public int Points { get; set; }

    [Key(4)]
    public ActionType Type { get; set; }

    [Key(5)]
    public string? SubType { get; set; }
}

/// <summary>
/// Base class for matches
/// </summary>
[Union(0, typeof(StandardMatchState))]
[MessagePackObject]
public abstract class MatchState
{
    [Key(0)]
    public Guid Id { get; set; }

    [Key(1)]
    public ScoreState? First { get; set; }

    [Key(2)]
    public ScoreState? Second { get; set; }

    [Key(3)]
    public MatchSettingsState? Settings { get; set; }

    [Key(4)]
    public PriorityState? Priority { get; set; }
}

/// <summary>
/// A standard match's complete description
/// </summary>
[MessagePackObject]
public sealed class StandardMatchState : MatchState
{
    [Key(5)]
    public ClockState? Clock { get; set; }

    [Key(6)]
    public ActionState[] Actions { get; set; } = [];
}

/// <summary>
/// Timer state
/// </summary>
[MessagePackObject]
public sealed class TimerState
{
    [Key(0)]
    public Guid MatchId { get; set; }

    [Key(1)]
    public Guid RingId { get; set; }

    [Key(2)]
    public ClockState? Clock { get; set; }
}

/// <summary>
/// The state of the scores and the new action that changed it
/// </summary>
[MessagePackObject]
public sealed class NewActionState
{
    [Key(0)]
    public Guid MatchId { get; set; }

    [Key(1)]
    public Guid RingId { get; set; }

    [Key(2)]
    public Guid ActionId { get; set; }

    [Key(3)]
    public ScoreState? First { get; set; }

    [Key(4)]
    public ScoreState? Second { get; set; }

    [Key(5)]
    public ActionState? Action { get; set; }

    [Key(6)]
    public ClockState? Clock { get; set; }
}

/// <summary>
/// The state of the scores and the new action that changed it
/// </summary>
[MessagePackObject]
public sealed class ActionModified
{
    [Key(0)]
    public Guid MatchId { get; set; }

    [Key(1)]
    public Guid RingId { get; set; }

    [Key(2)]
    public Guid ActionId { get; set; }

    [Key(3)]
    public ScoreState? First { get; set; }

    [Key(4)]
    public ScoreState? Second { get; set; }

    [Key(5)]
    public int Points { get; set; }
}

/// <summary>
/// The state of the scores after undoing the last action
/// </summary>
[MessagePackObject]
public sealed class UndoActionState
{
    [Key(0)]
    public Guid MatchId { get; set; }

    [Key(1)]
    public Guid RingId { get; set; }

    [Key(2)]
    public Guid ActionId { get; set; }

    [Key(3)]
    public ScoreState? First { get; set; }

    [Key(4)]
    public ScoreState? Second { get; set; }

    [Key(5)]
    public ClockState? Clock { get; set; }
}

/// <summary>
/// The state of the scores after priority has changed
/// </summary>
[MessagePackObject]
public sealed class PriorityChanged
{
    [Key(0)]
    public Guid MatchId { get; set; }

    [Key(1)]
    public Guid RingId { get; set; }

    [Key(2)]
    public PriorityState? Priority { get; set; }
}

/// <summary>
/// The state of a player's honor
/// </summary>
[MessagePackObject]
public sealed class HonorState
{
    [Key(0)]
    public Guid PlayerId { get; set; }

    [Key(1)]
    public int Honor { get; set; }
}

/// <summary>
/// Used to make sure scoreboards have players on the correct side
/// </summary>
[MessagePackObject]
public sealed class OrientationState
{
    [Key(0)]
    public Guid RingId { get; set; }

    [Key(1)]
    public Side Blue { get; set; }

    [Key(2)]
    public Side Red { get; set; }
}

/// <summary>
/// Match Settings
/// </summary>
[MessagePackObject]
public sealed class MatchSettingsState
{
    [Key(0)]
    public int WinningScore { get; set; }

    [Key(1)]
    public TimeSpan TimeLimit { get; set; }

    [Key(2)]
    public int Rounds { get; set; }
}

/// <summary>
/// Clock state
/// </summary>
[MessagePackObject]
public sealed class ClockState
{
    [Key(0)]
    public TimeSpan TimeRemaining { get; set; }

    [Key(1)]
    public bool IsRunning { get; set; }

    [Key(2)]
    public int CurrentRound { get; set; }
}

/// <summary>
/// Priority state
/// </summary>
[MessagePackObject]
public sealed class PriorityState
{
    [Key(0)]
    public Side PrioritySide { get; set; }

    [Key(1)]
    public int PriorityPoints { get; set; }

    [Key(2)]
    public bool InPriority { get; set; }
}

/// <summary>
/// Score state
/// </summary>
[MessagePackObject]
public sealed class ScoreState
{
    [Key(0)]
    public ParticipantState? Participant { get; set; }

    [Key(1)]
    public int Points { get; set; } = 0;

    [Key(2)]
    public int? Seed { get; set; }

    [Key(3)]
    public int MinorViolationCount { get; set; } = 0;
}
