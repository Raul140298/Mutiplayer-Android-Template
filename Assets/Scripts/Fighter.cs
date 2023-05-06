using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using DG.Tweening;
using PowerTools;
using Unity.Netcode.Components;

public class Fighter : NetworkBehaviour
{
    [Header("POSITION")]
    [SerializeField] private NetworkVariable<ulong> clientId = new NetworkVariable<ulong>();
    [SerializeField] private NetworkVariable<int> characterId = new NetworkVariable<int>();
    [SerializeField] private NetworkVariable<bool> leftPlayer = new NetworkVariable<bool>();
    [SerializeField] private NetworkVariable<Vector2> currentPos = new NetworkVariable<Vector2>();
    [SerializeField] private NetworkVariable<Vector2> limitX = new NetworkVariable<Vector2>();
    [SerializeField] private NetworkVariable<Vector2> limitY = new NetworkVariable<Vector2>();
    [SerializeField] private NetworkVariable<bool> inMovement = new NetworkVariable<bool>();
    [SerializeField] private NetworkVariable<eDirection> pendingDir = new NetworkVariable<eDirection>();
    [SerializeField] private NetworkAnimator animator;

    private const float CELL_WIDTH = 20;
    private const float CELL_HEIGHT = 21;

    private Vector2 ZERO_CELL_POSITION = new Vector2(-111, 34);

    void Awake()
    {

    }

    void Start()
    {
        inMovement.Value = false;
    }

    public void SetStartingPosition(int row, int column)
    {
        currentPos.Value = new Vector2(row, column);

        UpdatePositionInStage();
    }

    public void SetLimits(Vector2 limitX, Vector2 limitY)
    {
        this.limitX.Value = limitX;
        this.limitY.Value = limitY;
    }

    public void MovePlayer(eDirection dir)
    {
        MoveServerRpc(dir);
    }

    [ServerRpc]
    public void MoveServerRpc(eDirection dir, ServerRpcParams serverRpcParams = default)
    {
        if (inMovement.Value)
        {
            pendingDir.Value = dir;
            return;
        }

        inMovement.Value = true;

        switch (dir)
        {
            case eDirection.Up: MoveUp(); break;
            case eDirection.Down: MoveDown(); break;
            case eDirection.Right:
                if (leftPlayer.Value == true)
                {
                    MoveForward();
                }
                else
                {
                    MoveBack();
                }
                break;

            case eDirection.Left:
                if (leftPlayer.Value == true)
                {
                    MoveBack();
                }
                else
                {
                    MoveForward();
                }
                break;
        }

        PlayMovementAnimation(dir);
        UpdatePositionServerRpc();
    }

    private void MoveUp()
    {
        if (currentPos.Value.x - 1 < limitX.Value.x)
        {
            inMovement.Value = false;
            return;
        }

        currentPos.Value = new Vector2((byte)currentPos.Value.x - 1, currentPos.Value.y);
    }

    private void MoveDown()
    {
        if (currentPos.Value.x + 1 > limitX.Value.y)
        {
            inMovement.Value = false;
            return;
        }

        currentPos.Value = new Vector2(currentPos.Value.x + 1, currentPos.Value.y);
    }

    private void MoveForward()
    {
        if (currentPos.Value.y + 1 > limitY.Value.y)
        {
            inMovement.Value = false;
            return;
        }

        currentPos.Value = new Vector2(currentPos.Value.x, currentPos.Value.y + 1);
    }

    private void MoveBack()
    {
        if (currentPos.Value.y - 1 < limitY.Value.x)
        {
            inMovement.Value = false;
            return;
        }

        currentPos.Value = new Vector2(currentPos.Value.x, currentPos.Value.y - 1);
    }

    [ServerRpc]
    private void UpdatePositionServerRpc(ServerRpcParams serverRpcParams = default)
    {
        UpdatePositionInStage();
    }

    private void UpdatePositionInStage()
    {
        Vector2 offset = new Vector2(currentPos.Value.y * CELL_WIDTH, -currentPos.Value.x * CELL_HEIGHT);
        Vector2 pos = ZERO_CELL_POSITION + offset;

        this.transform.DOMove(pos, 0.25f).SetEase(Ease.OutExpo).OnComplete(OnMovementTweenEnded);
    }

    private void OnMovementTweenEnded()
    {
        inMovement.Value = false;
        PlayMovementAnimation(pendingDir.Value);

        if (pendingDir.Value != eDirection.None)
        {
            MovePlayer(pendingDir.Value);
            pendingDir.Value = eDirection.None;
        }
    }

    private void PlayMovementAnimation(eDirection dir)
    {
        int s = 0;

        switch (dir)
        {
            case eDirection.Left: s = 1; break;
            case eDirection.Right: s = 2; break;
            case eDirection.Up: s = 1; break;
            case eDirection.Down: s = 2; break;
            case eDirection.None: s = 0; break;
        }

        animator.Animator.SetInteger("state", s);
    }

    public NetworkVariable<ulong> ClientId
    {
        get { return clientId; }
        set { clientId = value; }
    }

    public NetworkVariable<int> CharacterId
    {
        get { return characterId; }
        set { characterId = value; }
    }

    public NetworkVariable<bool> LeftPlayer
    {
        get { return leftPlayer; }
        set { leftPlayer = value; }
    }
}
