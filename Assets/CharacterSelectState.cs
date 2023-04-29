using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Netcode;

public struct CharacterSelectState : INetworkSerializable, IEquatable<CharacterSelectState>
{
    public ulong clientId;
    public int characterId;
    public bool isReady;

    public CharacterSelectState(ulong clientId, int characterId = -1, bool isReady = false)
    {
        this.clientId = clientId;
        this.characterId = characterId;
        this.isReady = isReady;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref clientId);
        serializer.SerializeValue(ref characterId);
        serializer.SerializeValue(ref isReady);
    }

    public bool Equals(CharacterSelectState other)
    {
        return clientId == other.clientId && characterId == other.characterId && isReady == other.isReady;
    }
}
