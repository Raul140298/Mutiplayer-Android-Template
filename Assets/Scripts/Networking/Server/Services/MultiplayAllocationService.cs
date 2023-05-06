using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Unity.Services.Matchmaker.Models;
#if UNITY_SERVER || UNITY_EDITOR
using Unity.Services.Multiplay;
#endif
using UnityEngine;

public class MultiplayAllocationService : IDisposable
{
#if UNITY_SERVER || UNITY_EDITOR
    private IMultiplayService multiplayService;
    private MultiplayEventCallbacks serverCallbacks;
    private IServerQueryHandler serverCheckManager;
    private IServerEvents serverEvents;
    private CancellationTokenSource serverCheckCancel;
    string allocationId;
#endif

    public MultiplayAllocationService()
    {
#if UNITY_SERVER || UNITY_EDITOR
        try
        {
            multiplayService = MultiplayService.Instance;
            serverCheckCancel = new CancellationTokenSource();
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"Error creating Multiplay allocation service.\n{ex}");
        }
#endif
    }

    public async Task<MatchmakingResults> SubscribeAndAwaitMatchmakerAllocation()
    {
#if UNITY_SERVER || UNITY_EDITOR
        if (multiplayService == null) { return null; }

        allocationId = null;
        serverCallbacks = new MultiplayEventCallbacks();
        serverCallbacks.Allocate += OnMultiplayAllocation;
        serverEvents = await multiplayService.SubscribeToServerEventsAsync(serverCallbacks);

        string allocationID = await AwaitAllocationID();
        MatchmakingResults matchmakingPayload = await GetMatchmakerAllocationPayloadAsync();

        return matchmakingPayload;
#else
    return null;
#endif
    }

    private async Task<string> AwaitAllocationID()
    {
#if UNITY_SERVER || UNITY_EDITOR
        var config = multiplayService.ServerConfig;
        Debug.Log($"Awaiting Allocation. Server Config is:\n" +
            $"-ServerID: {config.ServerId}\n" +
            $"-AllocationID: {config.AllocationId}\n" +
            $"-Port: {config.Port}\n" +
            $"-QPort: {config.QueryPort}\n" +
            $"-logs: {config.ServerLogDirectory}");

        while (string.IsNullOrEmpty(allocationId))
        {
            var configID = config.AllocationId;

            if (!string.IsNullOrEmpty(configID) && string.IsNullOrEmpty(allocationId))
            {
                Debug.Log($"Config had AllocationID: {configID}");
                allocationId = configID;
            }

            await Task.Delay(100);
        }

        return allocationId;
#else
    return null;
#endif
    }

    private async Task<MatchmakingResults> GetMatchmakerAllocationPayloadAsync()
    {
#if UNITY_SERVER || UNITY_EDITOR
        var payloadAllocation = await MultiplayService.Instance.GetPayloadAllocationFromJsonAs<MatchmakingResults>();
        string modelAsJson = JsonConvert.SerializeObject(payloadAllocation, Formatting.Indented);
        Debug.Log(nameof(GetMatchmakerAllocationPayloadAsync) + ":" + Environment.NewLine + modelAsJson);
        return payloadAllocation;
#else
    return null;
#endif
    }

    public async Task BeginServerCheck()
    {
#if UNITY_SERVER || UNITY_EDITOR
        if (multiplayService == null) { return; }

        serverCheckManager = await multiplayService.StartServerQueryHandlerAsync((ushort)20, "", "", "0", "");

#pragma warning disable 4014
        ServerCheckLoop(serverCheckCancel.Token);
#pragma warning restore 4014

#else
    return;
#endif
    }

    public void SetServerName(string name)
    {
#if UNITY_SERVER || UNITY_EDITOR
        serverCheckManager.ServerName = name;
#endif
    }
    public void SetBuildID(string id)
    {
#if UNITY_SERVER || UNITY_EDITOR
        serverCheckManager.BuildId = id;
#endif
    }

    public void SetMaxPlayers(ushort players)
    {
#if UNITY_SERVER || UNITY_EDITOR
        serverCheckManager.MaxPlayers = players;
#endif
    }

    public void AddPlayer()
    {
#if UNITY_SERVER || UNITY_EDITOR
        serverCheckManager.CurrentPlayers++;
#endif
    }

    public void RemovePlayer()
    {
#if UNITY_SERVER || UNITY_EDITOR
        serverCheckManager.CurrentPlayers--;
#endif
    }

    public void SetMap(string newMap)
    {
#if UNITY_SERVER || UNITY_EDITOR
        serverCheckManager.Map = newMap;
#endif
    }

    public void SetMode(string mode)
    {
#if UNITY_SERVER || UNITY_EDITOR
        serverCheckManager.GameType = mode;
#endif
    }

    private async Task ServerCheckLoop(CancellationToken cancellationToken)
    {
#if UNITY_SERVER || UNITY_EDITOR
        while (!cancellationToken.IsCancellationRequested)
        {
            serverCheckManager.UpdateServerCheck();
            await Task.Delay(100);
        }
#else
    return;
#endif
    }

#if UNITY_SERVER || UNITY_EDITOR
    private void OnMultiplayAllocation(MultiplayAllocation allocation)
    {

        Debug.Log($"OnAllocation: {allocation.AllocationId}");

        if (string.IsNullOrEmpty(allocation.AllocationId)) { return; }

        allocationId = allocation.AllocationId;

    }

    private void OnMultiplayDeAllocation(MultiplayDeallocation deallocation)
    {

        Debug.Log(
                $"Multiplay Deallocated : ID: {deallocation.AllocationId}\nEvent: {deallocation.EventId}\nServer{deallocation.ServerId}");
    }

    private void OnMultiplayError(MultiplayError error)
    {
        Debug.Log($"MultiplayError : {error.Reason}\n{error.Detail}");

    }
#endif

    public void Dispose()
    {
#if UNITY_SERVER || UNITY_EDITOR
        if (serverCallbacks != null)
        {
            serverCallbacks.Allocate -= OnMultiplayAllocation;
            serverCallbacks.Deallocate -= OnMultiplayDeAllocation;
            serverCallbacks.Error -= OnMultiplayError;
        }

        if (serverCheckCancel != null)
        {
            serverCheckCancel.Cancel();
        }

        serverEvents?.UnsubscribeAsync();

#endif
    }
}