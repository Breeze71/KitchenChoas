using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CharacterSelectReady : NetworkBehaviour
{
    public static CharacterSelectReady Instance
    {
        get;
        private set;
    }
    private Dictionary<ulong, bool> palyerReadyDictionary;

    private void Awake() 
    {
        Instance = this;

        palyerReadyDictionary = new Dictionary<ulong, bool>();
    }
        
    public void SetPlayerReady()
    {
        SetPlayerReadyServerRpc();
    }

    /* Netcode isReady? */
    #region Netcode Ready
    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
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
            Loader.LoadSceneNetWork(Loader.Scene.GameScene);
        }
    }
    #endregion
}
