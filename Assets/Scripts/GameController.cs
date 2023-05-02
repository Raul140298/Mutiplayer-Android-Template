using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Touch;

public class GameController : MonoBehaviour
{
    [SerializeField] private InputController inputController;
    [SerializeField] private WaveController waveController;

    [SerializeField] private Player player;
    [SerializeField] private List<GameObject> markersList;

    void OnEnable()
    {
        inputController.onSwipe += HandleSwipe;
    }

    void Start()
    {
        //waveController.StartFirstWave();
    }

    // void 

    public void SetPlayer(Player player)
    {
        this.player = player;
    }

    private void HandleSwipe(eDirection dir)
    {
        player.MoveServerRpc(dir);
    }

    void OnDisable()
    {
        inputController.onSwipe -= HandleSwipe;
    }
}
