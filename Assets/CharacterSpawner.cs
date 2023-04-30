using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using UnityEngine.UI;

public class CharacterSpawner : NetworkBehaviour
{
    [SerializeField] private CharacterDatabase characterDatabase;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) { return; }

        foreach (var client in HostManager.Instance.clientData)
        {
            var character = characterDatabase.GetCharacterById(client.Value.characterId);

            if (character != null)
            {
                var spawnPos = new Vector2(Random.Range(-4f, 4f), 0);

                var characterInstance = Instantiate(character.gameplayPrefab, spawnPos, Quaternion.identity);
                characterInstance.SpawnAsPlayerObject(client.Value.clientId);
            }
        }
    }
}
