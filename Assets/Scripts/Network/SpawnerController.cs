using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using UnityEngine.UI;

public class SpawnerController : NetworkBehaviour
{
    [SerializeField] private CharacterDatabase characterDatabase;
    [SerializeField] private GameController gameController;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            int aux = 0;

            foreach (var client in HostManager.Instance.clientData)
            {
                var character = characterDatabase.GetCharacterById(client.Value.characterId);

                if (character != null)
                {
                    var characterInstance = Instantiate(character.gameplayPrefab);
                    characterInstance.SpawnWithOwnership(client.Value.clientId);

                    Player player = characterInstance.GetComponent<Player>();
                    player.ClientId.Value = client.Value.clientId;
                    player.SetStartingPosition(2, 3 + 5 * aux);
                    player.gameObject.transform.localScale = new Vector3(1 - 2 * aux, 1, 1);
                    player.gameObject.name = "Player" + aux;
                    aux += 1;
                }
            }
        }

        StartCoroutine(SetPlayers());
    }

    IEnumerator SetPlayers()
    {
        yield return new WaitForSeconds(0.5f);

        GameObject[] playersList = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject playerObject in playersList)
        {
            Player player = playerObject.GetComponent<Player>();

            if (player.ClientId.Value == NetworkManager.Singleton.LocalClientId)
            {
                gameController.SetPlayer(player);
            }
        }
    }
}
