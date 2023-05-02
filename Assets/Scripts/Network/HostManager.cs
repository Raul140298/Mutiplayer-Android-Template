using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HostManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int maxConnections;
    [SerializeField] private string gameplaySceneName;
    [SerializeField] private string characterSelectSceneName;
    public string joinCode { get; private set; }

    public static HostManager Instance { get; private set; }

    public Dictionary<ulong, ClientData> clientData { get; private set; }

    private bool gameHasStarted;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    public async void StartHost()
    {
        Allocation allocation;

        try
        {
            allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
        }
        catch (Exception e)
        {
            Debug.LogError("Relay creation request failed :" + e);
            throw;
        }

        try
        {
            joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        }
        catch
        {
            Debug.LogError("Relay creation joinCode failed :");
            throw;
        }

        var relayServerData = new RelayServerData(allocation, "dtls");

        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        NetworkManager.Singleton.OnServerStarted += OnNetworkReady;

        clientData = new Dictionary<ulong, ClientData>();

        NetworkManager.Singleton.StartHost();
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        if (clientData.Count >= 2 || gameHasStarted)
        {
            response.Approved = false;
            return;
        }

        response.Approved = true;
        response.CreatePlayerObject = false;
        response.Pending = false;

        clientData[request.ClientNetworkId] = new ClientData(request.ClientNetworkId);
    }

    private void OnNetworkReady()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;

        NetworkManager.Singleton.SceneManager.LoadScene(characterSelectSceneName, LoadSceneMode.Single);
    }

    private void OnClientDisconnect(ulong clientId)
    {
        if (clientData.ContainsKey(clientId))
        {
            if (clientData.Remove(clientId))
            {
                Debug.Log("Removed client " + clientId);
            }
        }
    }

    public void SetCharacter(ulong clientId, int characterId)
    {
        if (clientData.TryGetValue(clientId, out ClientData data))
        {
            data.characterId = characterId;
        }
    }

    public void StartGame()
    {
        gameHasStarted = true;

        NetworkManager.Singleton.SceneManager.LoadScene(gameplaySceneName, LoadSceneMode.Single);
    }
}
