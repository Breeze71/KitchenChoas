using System;
using Unity.Netcode;
using Unity.Collections;

public struct PlayerData : IEquatable<PlayerData>, INetworkSerializable
{
    public ulong clientId;
    public int colorId;
    public FixedString64Bytes playerName;
    public FixedString64Bytes playerLobbyId;    // Lobby ID 跟 Client ID 不同，Lobby Id 只用於連線，Client ID 連至 Server


    public bool Equals(PlayerData other)
    {
        return 
            clientId == other.clientId && 
            colorId == other.colorId &&
            playerName == other.playerName &&
            playerLobbyId == other.playerLobbyId
            ;
    }

    // Serialize 由於 Netcode 函數不能直接傳遞 Serialize 的數值或物件
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref clientId);
        serializer.SerializeValue(ref colorId);
        serializer.SerializeValue(ref playerName);
        serializer.SerializeValue(ref playerLobbyId);
    }
}
