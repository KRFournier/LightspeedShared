using Avalonia.Threading;
using MessagePack;
using Network;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Lightspeed.Network;

public static class Protocol
{
    #region Message Pack Helpers

    public static void RequestMessagePack<T>(this Connection connection, string key, T request)
    {
        try
        {
            connection.SendRawData(key, MessagePackSerializer.Serialize<T>(request));
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }

    public static void OnMessagePackRequested<T>(this Connection connection, string key, Action<Connection, T> handler)
    {
        try
        {
            connection.RegisterRawDataHandler(key, (p, c) =>
                Dispatcher.UIThread.Post(
                    () => handler(c, MessagePackSerializer.Deserialize<T>(p.Data))
                )
            );
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }

    public static void SendMessagePack<T>(this Connection connection, string key, T packet)
    {
        try
        {
            connection?.SendRawData(key, MessagePackSerializer.Serialize<T>(packet));
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }

    public static void OnMessagePackReceived<T>(this Connection connection, string key, Action<T> handler, Action<string> error, [CallerMemberName] string caller = "") => connection.RegisterRawDataHandler(key, (p, c) =>
                                                                                                                                                                                {
                                                                                                                                                                                    try
                                                                                                                                                                                    {
                                                                                                                                                                                        var response = MessagePackSerializer.Deserialize<T>(p.Data);
                                                                                                                                                                                        Dispatcher.UIThread.Post(() => handler(response));
                                                                                                                                                                                    }
                                                                                                                                                                                    catch (Exception ex)
                                                                                                                                                                                    {
                                                                                                                                                                                        Debug.WriteLine(ex.Message);
                                                                                                                                                                                        Dispatcher.UIThread.Post(() => error($"{caller}: {ex.Message}"));
                                                                                                                                                                                    }
                                                                                                                                                                                });

    #endregion

    #region Rings

    public static void RequestRings(this Connection connection) => connection.SendRawData("RequestRings", []);

    public static void OnRingsRequested(this Connection connection, Action<Connection> handler) => connection.RegisterRawDataHandler("RequestRings", (_, c) => Dispatcher.UIThread.Post(() => handler(c)));

    public static void SendRings(this Connection connection, FencingRings tournaments) => connection.SendMessagePack("Rings", tournaments);

    public static void OnRingsReceived(this Connection connection, Action<FencingRings> handler, Action<string> error) => connection.OnMessagePackReceived("Rings", handler, error);

    public static void RegisterScoreboardRing(this Connection connection, Guid ringId) => connection.SendRawData("RegisterScoreboardRing", ringId.ToByteArray());

    public static void OnScoreboardRegistered(this Connection connection, Action<Connection, Guid> handler)
    {
        try
        {
            connection.RegisterRawDataHandler("RegisterScoreboardRing", (p, c) => Dispatcher.UIThread.Post(() => handler(c, new Guid(p.Data))));
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }

    public static void RegisterKeeperRing(this Connection connection, Guid ringId) => connection.SendRawData("RegisterKeeperRing", ringId.ToByteArray());

    public static void OnKeeperRegistered(this Connection connection, Action<Connection, Guid> handler)
    {
        try
        {
            connection.RegisterRawDataHandler("RegisterKeeperRing", (p, c) => Dispatcher.UIThread.Post(() => handler(c, new Guid(p.Data))));
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }

    #endregion

    #region Tournaments

    public static void RequestTournaments(this Connection connection) => connection.SendRawData("RequestTournaments", []);

    public static void OnTournamentsRequested(this Connection connection, Action<Connection> handler) => connection.RegisterRawDataHandler("RequestTournaments", (_, c) => Dispatcher.UIThread.Post(() => handler(c)));

    public static void SendTournaments(this Connection connection, OpenTournaments tournaments) => connection.SendMessagePack("Tournaments", tournaments);

    public static void OnTournamentsReceived(this Connection connection, Action<OpenTournaments> handler, Action<string> error) => connection.OnMessagePackReceived("Tournaments", handler, error);

    #endregion

    #region Match Groups

    public static void RequestMatchGroups(this Connection connection, Guid tournamentId) => connection.SendRawData("RequestMatchGroups", tournamentId.ToByteArray());

    public static void OnMatchGroupsRequested(this Connection connection, Action<Connection, Guid> handler)
    {
        try
        {
            connection.RegisterRawDataHandler("RequestMatchGroups", (p, c) => Dispatcher.UIThread.Post(() => handler(c, new Guid(p.Data))));
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }

    public static void SendMatchGroups(this Connection connection, MatchGroupsState groups) => connection.SendMessagePack("MatchGroups", groups);

    public static void OnMatchGroupsReceived(this Connection connection, Action<MatchGroupsState> handler, Action<string> error) => connection.OnMessagePackReceived("MatchGroups", handler, error);

    #endregion

    #region Match Summaries

    public static void RequestMatchSummaries(this Connection connection, Guid groupId) => connection.RequestMessagePack("RequestMatches", groupId);

    public static void OnMatchSummariesRequested(this Connection connection, Action<Connection, Guid> handler) => connection.OnMessagePackRequested("RequestMatches", handler);

    public static void SendMatchSummaries(this Connection connection, MatchSummaries summaries) => connection.SendMessagePack("MatchSummaries", summaries);

    public static void OnMatchSummariesReceived(this Connection connection, Action<MatchSummaries> handler, Action<string> error) => connection.OnMessagePackReceived("MatchSummaries", handler, error);

    #endregion

    #region Go Live

    public static void RequestGoLive(this Connection connection, MatchGoLiveRequest request) => connection.RequestMessagePack("RequestGoLive", request);

    public static void OnGoLiveRequested(this Connection connection, Action<Connection, MatchGoLiveRequest> handler) => connection.OnMessagePackRequested("RequestGoLive", handler);

    public static void SendMatch(this Connection connection, MatchState match)
    {
        switch (match)
        {
            case StandardMatchState standardMatch:
                connection.SendStandardMatch(standardMatch);
                break;

            // Future match types can be handled here
            default:
                throw new NotSupportedException($"Match type {match.GetType().Name} is not supported.");
        }
    }

    public static void SendStandardMatch(this Connection connection, StandardMatchState match) => connection.SendMessagePack("StandardMatch", match);

    public static void OnStandardMatchReceived(this Connection connection, Action<StandardMatchState> handler, Action<string> error) => connection.OnMessagePackReceived("StandardMatch", handler, error);

    #endregion

    #region Timer

    public static void SendTimerState(this Connection connection, TimerState state) => connection.SendMessagePack("Timer", state);

    public static void OnTimerStateReceived(this Connection connection, Action<Connection, TimerState> handler) => connection.OnMessagePackRequested("Timer", handler);

    #endregion

    #region Actions

    public static void SendNewAction(this Connection connection, NewActionState state) => connection.SendMessagePack("Action", state);

    public static void OnNewActionReceived(this Connection connection, Action<Connection, NewActionState> handler) => connection.OnMessagePackRequested("Action", handler);

    public static void SendActionModified(this Connection connection, ActionModified state) => connection.SendMessagePack("ActionMod", state);

    public static void OnActionModifiedReceived(this Connection connection, Action<Connection, ActionModified> handler) => connection.OnMessagePackRequested("ActionMod", handler);

    public static void SendUndoAction(this Connection connection, UndoActionState state) => connection.SendMessagePack("Undo", state);

    public static void OnUndoActionReceived(this Connection connection, Action<Connection, UndoActionState> handler) => connection.OnMessagePackRequested("Undo", handler);

    #endregion

    #region Priority

    public static void SendPriorityChanged(this Connection connection, PriorityChanged state) => connection.SendMessagePack("Priority", state);

    public static void OnPriorityChangedReceived(this Connection connection, Action<Connection, PriorityChanged> handler) => connection.OnMessagePackRequested("Priority", handler);

    #endregion

    #region Honor

    public static void SendHonorState(this Connection connection, HonorState state) => connection.SendMessagePack("Honor", state);

    public static void OnHonorStateReceived(this Connection connection, Action<Connection, HonorState> handler) => connection.OnMessagePackRequested("Honor", handler);

    #endregion

    #region Orientation

    public static void SendOrientationState(this Connection connection, OrientationState state) => connection.SendMessagePack("Orientation", state);

    public static void OnOrientationStateReceived(this Connection connection, Action<Connection, OrientationState> handler) => connection.OnMessagePackRequested("Orientation", handler);

    #endregion
}
