using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using DG.Tweening;
using PowerTools;

public class Player : NetworkBehaviour
{
    private SpriteAnim compAnim;

    [Header("POSITION")]
    [SerializeField] private NetworkVariable<ulong> clientId = new NetworkVariable<ulong>();
    [SerializeField] private NetworkVariable<Vector2> currentPos = new NetworkVariable<Vector2>();
    [SerializeField] private NetworkVariable<bool> inMovement = new NetworkVariable<bool>();
    [SerializeField] private NetworkVariable<eDirection> pendingDir;

    [Header("ANIMS")]
    [SerializeField] private AnimationClip animIdle;
    [SerializeField] private AnimationClip animMoveBack;
    [SerializeField] private AnimationClip animMoveForward;

    private const float CELL_WIDTH = 20;
    private const float CELL_HEIGHT = 21;

    private Vector2 ZERO_CELL_POSITION = new Vector2(-111, 34);

    void Awake()
    {
        compAnim = GetComponent<SpriteAnim>();
    }

    void Start()
    {
        inMovement.Value = false;
        /*         currentPos.OnValueChanged += (prev, curr) =>
                {
                    PlayMovementAnimation(pendingDir.Value);
                }; */
    }

    public void SetStartingPosition(int row, int column)
    {
        currentPos.Value = new Vector2(row, column);

        UpdatePositionInStage();
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
            case eDirection.Right: MoveForward(); break;
            case eDirection.Down: MoveDown(); break;
            case eDirection.Left: MoveBack(); break;
        }

        PlayMovementAnimation(dir);
        UpdatePositionInStage();
    }

    private void MoveForward()
    {
        currentPos.Value = new Vector2(currentPos.Value.x, currentPos.Value.y + 1);
    }

    private void MoveBack()
    {
        currentPos.Value = new Vector2(currentPos.Value.x, currentPos.Value.y - 1);
    }

    private void MoveUp()
    {
        currentPos.Value = new Vector2((byte)currentPos.Value.x - 1, currentPos.Value.y);
    }

    private void MoveDown()
    {
        currentPos.Value = new Vector2(currentPos.Value.x + 1, currentPos.Value.y);
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
            MoveServerRpc(pendingDir.Value);
            pendingDir.Value = eDirection.None;
        }
    }

    private void PlayMovementAnimation(eDirection dir)
    {
        AnimationClip anim = null;

        switch (dir)
        {
            case eDirection.Left: anim = this.transform.localScale.x == 1 ? animMoveBack : animMoveForward; break;
            case eDirection.Right: anim = this.transform.localScale.x == 1 ? animMoveForward : animMoveBack; break;
            case eDirection.Up: anim = animMoveForward; break;
            case eDirection.Down: anim = animMoveForward; break;
            case eDirection.None: anim = animIdle; break;
        }

        compAnim.Play(anim);
    }

    public NetworkVariable<ulong> ClientId
    {
        get { return clientId; }
        set { clientId = value; }
    }
}