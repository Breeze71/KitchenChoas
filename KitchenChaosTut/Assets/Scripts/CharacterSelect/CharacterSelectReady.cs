using System.Collections.Generic;
using Unity.Netcode;
using System;

public class CharacterSelectReady : NetworkBehaviour
{
    public static CharacterSelectReady Instance
    {
        get;
        private set;
    }

    public event EventHandler OnClientReady;

    private Dictionary<ulong, bool> palyerReadyDictionary;

    private void Awake() 
    {
        Instance = this;

        palyerReadyDictionary = new Dictionary<ulong, bool>();
    }
        
    public void CheckPlayerReady()
    {
        SetPlayerReadyServerRpc();
    }

    /* Netcode isReady? */
    #region Netcode Ready
    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        SetPlayerReadyClientRpc(serverRpcParams.Receive.SenderClientId);    // 同步 Client 顯示 Ready
        palyerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;

        bool isAllClientReady = true;
        foreach(ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            // if does not contain this player or not ready
            if(!palyerReadyDictionary.ContainsKey(clientId) || !palyerReadyDictionary[clientId])
            {
                isAllClientReady = false;
                return;
            }
        }

        if(isAllClientReady)
        {
            GameLobby.Instance.DeleteLobby();   // 連上一致 Server 就不必 Lobby 了
            
            Loader.LoadSceneNetWork(Loader.Scene.GameScene);
        }
    }

    // 由於是 Rpc 傳遞的 Event 所以也會 Sync
    [ClientRpc]
    private void SetPlayerReadyClientRpc(ulong clientId)
    {
        palyerReadyDictionary[clientId] = true;

        OnClientReady?.Invoke(this, EventArgs.Empty);
    }
    #endregion

    public bool IsPlayerReady(ulong clientId)
    {
        return palyerReadyDictionary.ContainsKey(clientId) && palyerReadyDictionary.ContainsKey(clientId);
    }
}
