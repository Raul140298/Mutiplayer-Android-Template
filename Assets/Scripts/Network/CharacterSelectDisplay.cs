using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using UnityEngine.UI;

public class CharacterSelectDisplay : NetworkBehaviour
{
    [SerializeField] private GameObject characterInfoPanel;
    [SerializeField] private TMP_Text characterNameText;
    [SerializeField] private CharacterDatabase characterDatabase;
    [SerializeField] private Transform charactersHolder;
    [SerializeField] private CharacterSelectButton selectButtonPrefab;
    [SerializeField] private PlayerCard[] playerCards;
    [SerializeField] private Button readyButton;
    [SerializeField] private TMP_Text joinCodeText;

    private NetworkList<CharacterSelectState> players;

    private void Awake()
    {
        players = new NetworkList<CharacterSelectState>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            Character[] allCharacters = characterDatabase.GetAllCharacters();

            foreach (var character in allCharacters)
            {
                var selectButtonInstance = Instantiate(selectButtonPrefab, charactersHolder);

                selectButtonInstance.SetCharacter(this, character);
            }

            players.OnListChanged += HandlePlayersStateChanged;
        }

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnected;

            foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
            {
                HandleClientConnected(client.ClientId);
            }
        }

        if (IsHost)
        {
            joinCodeText.text = HostManager.Instance.joinCode;
        }
        else
        {
            joinCodeText.text = "";
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsClient)
        {
            players.OnListChanged -= HandlePlayersStateChanged;
        }

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnected;
        }
    }

    private void HandleClientConnected(ulong clientID)
    {
        players.Add(new CharacterSelectState(clientID));
    }

    private void HandleClientDisconnected(ulong clientID)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].clientId == clientID)
            {
                players.RemoveAt(i);
                break;
            }
        }
    }

    public void Select(Character character)
    {
        characterNameText.text = character.displayName;

        characterInfoPanel.SetActive(true);

        SelectServerRpc(character.id);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SelectServerRpc(int characterId, ServerRpcParams serverRpcParams = default)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].clientId == serverRpcParams.Receive.SenderClientId)
            {
                players[i] = new CharacterSelectState(players[i].clientId, characterId, players[i].isReady);
            }
        }
    }

    public void SetReady()
    {
        ReadyOnServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void ReadyOnServerRpc(ServerRpcParams serverRpcParams = default)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].clientId == serverRpcParams.Receive.SenderClientId)
            {
                players[i] = new CharacterSelectState(players[i].clientId, players[i].characterId, true);
            }
        }

        foreach (var player in players)
        {
            if (!player.isReady) { return; }
        }

        foreach (var player in players)
        {
            HostManager.Instance.SetCharacter(player.clientId, player.characterId);
        }

        HostManager.Instance.StartGame();
    }

    private void HandlePlayersStateChanged(NetworkListEvent<CharacterSelectState> changeEvent)
    {
        for (int i = 0; i < playerCards.Length; i++)
        {
            if (players.Count > i)
            {
                playerCards[i].UpdateDisplay(players[i]);
            }
            else
            {
                playerCards[i].DisableDisplay();
            }
        }

        foreach (var player in players)
        {
            if (player.clientId != NetworkManager.Singleton.LocalClientId) { continue; }

            if (player.isReady)
            {
                readyButton.interactable = false;
                break;
            }

            readyButton.interactable = true;
            break;
        }
    }
}
