using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Lightspeed.Network.Messages;

public sealed class NetworkConnectionsChangedMessage { }
public sealed class RequestOpenTournaments : RequestMessage<OpenTournaments> { }
public sealed class RequestActiveMatchGroups : RequestMessage<MatchGroupsState> { }

public sealed class RequestMatchState(string? next = null) : RequestMessage<MatchState>
{
    public readonly string? Next = next;
}

public sealed class RequestMatchGroupSummaries : RequestMessage<MatchSummaries>
{
}

public sealed class RequestNextMatch(Guid matchId) : RequestMessage<string?>
{
    public Guid MatchId { get; set; } = matchId;
}

public sealed class SetLiveMessage(bool isLive)
{
    public bool IsLive { get; set; } = isLive;
}

public abstract class StateMessage<T>(T state)
{
    public T State { get; set; } = state;
}

public sealed class ClockStateMessage(ClockState state) : StateMessage<ClockState>(state) { }
public sealed class NewActionMessage(NewActionState state) : StateMessage<NewActionState>(state) { }
public sealed class ActionModifiedMessage(ActionModified state) : StateMessage<ActionModified>(state) { }
public sealed class UndoActionMessage(UndoActionState state) : StateMessage<UndoActionState>(state) { }
public sealed class PriorityChangedMessage(PriorityChanged state) : StateMessage<PriorityChanged>(state) { }
public sealed class HonorStateMessage(HonorState state) : StateMessage<HonorState>(state) { }
