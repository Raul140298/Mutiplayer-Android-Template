using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameController : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private InputController inputController;
    [SerializeField] private Fighter player;
    [SerializeField] private CharacterDatabase characterDatabase;
    [SerializeField] private Fighter[] players;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            bool leftPlayer = Random.Range(0, 2) == 0;

            foreach (var client in MatchplayNetworkServer.Instance.ClientData)
            {
                var character = characterDatabase.GetCharacterById(client.Value.characterId);
                if (character != null)
                {
                    var characterInstance = Instantiate(character.GameplayPrefab);
                    characterInstance.SpawnWithOwnership(client.Value.clientId);

                    Fighter player = characterInstance.GetComponent<Fighter>();

                    player.ClientId.Value = client.Value.clientId;
                    player.CharacterId.Value = client.Value.characterId;
                    player.gameObject.name = "Player " + (leftPlayer ? (0).ToString() : (1).ToString());

                    if (leftPlayer == true)
                    {
                        player.LeftPlayer.Value = leftPlayer;
                        player.SetStartingPosition(2, 3);
                        player.SetLimits(new Vector2(0, 3), new Vector2(0, 5));
                    }
                    else
                    {
                        player.LeftPlayer.Value = leftPlayer;
                        player.transform.localScale = new Vector3(-1, 1, 1);
                        player.SetStartingPosition(2, 8);
                        player.SetLimits(new Vector2(0, 3), new Vector2(6, 11));
                    }

                    leftPlayer = !leftPlayer;
                }
            }
        }

        if (IsClient)
        {
            StartCoroutine(CRTLookForPlayer());
        }
    }

    IEnumerator CRTLookForPlayer()
    {
        yield return new WaitForSeconds(1f);

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players)
        {
            Fighter fighter = player.GetComponent<Fighter>();

            if (fighter.ClientId.Value == NetworkManager.LocalClientId)
            {
                this.player = fighter;

                if (fighter.LeftPlayer.Value == false)
                {
                    FlipCamera();
                }
            }
        }
    }

    private void FlipCamera()
    {
        Matrix4x4 mat = Camera.main.projectionMatrix;
        mat *= Matrix4x4.Scale(new Vector3(-1, 1, 1));
        Camera.main.projectionMatrix = mat;
    }

    void OnEnable()
    {
        inputController.onSwipe += HandleSwipe;
    }

    private void HandleSwipe(eDirection dir)
    {
        player.MovePlayer(dir);
    }

    void OnDisable()
    {
        inputController.onSwipe -= HandleSwipe;
    }

    public void SetPlayer(Fighter player)
    {
        this.player = player;
    }
}
