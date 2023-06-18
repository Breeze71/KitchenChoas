using UnityEngine;
using Unity.Netcode;
using System;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Unity.Services.Authentication;

public class KitchenGameMultiplayer : NetworkBehaviour
{
    public const int Max_Player_Amount = 4;
    private const string PlayerPrefs_PlayerName_Multiplayer = "PlayerNameMultiplayer";

    public static KitchenGameMultiplayer Instance{ get; private set; }
    public event EventHandler OnTryingToJoin;
    public event EventHandler OnFailedToJoin;
    public event EventHandler OnPlayerDataNetworkListChanged;
    [SerializeField] private List<Color> playerColorList;


    [SerializeField] private KitchenObjListSO kitchenObjListSO;

    private NetworkList<PlayerData> playerDataNetworkList;
    private string playerName;

    private void Awake() 
    {
        Instance = this;

        DontDestroyOnLoad(gameObject);

        playerName = PlayerPrefs.GetString(PlayerPrefs_PlayerName_Multiplayer, "Player" +UnityEngine.Random.Range(100,1000));

        playerDataNetworkList = new NetworkList<PlayerData>();
        playerDataNetworkList.OnListChanged += playerDataNetworkList_OnListChanged;
    }

    private void playerDataNetworkList_OnListChanged(NetworkListEvent<PlayerData> changeEvent)
    {
        OnPlayerDataNetworkListChanged?.Invoke(this, EventArgs.Empty);
    }
    public bool IsPlayerIndexConnected(int playerIndex)
    {
        return playerIndex < playerDataNetworkList.Count;
    }
    public PlayerData GetPlayerDataFromPlayerIndex(int playerIndex)
    {
        return playerDataNetworkList[playerIndex];
    }


    /* GetAllData From ID */
    #region GetAllData From ID
    public PlayerData GetPlayerData_From_ClientId(ulong clientId)
    {
        foreach(PlayerData playerData in playerDataNetworkList)
        {
            if(playerData.clientId == clientId)
            {
                return playerData;
            }
        }

        return default;
    }
    // 用 client 去取得 playerDataNetworkList 的 index
    public int GetPlayerDataIndex_From_ClientId(ulong clientId)
    {
        for(int i = 0; i < playerDataNetworkList.Count; i++)
        {
            if(playerDataNetworkList[i].clientId == clientId)
            {
                return i;
            }
        }

        return -1;
    }

    public PlayerData GetPlayerData()
    {
        return GetPlayerData_From_ClientId(NetworkManager.Singleton.LocalClientId);
    }

    #endregion
    //

    /* Netcode Change Character */
    #region Netcode Change Character
    // 同步邏輯的顏色改
    public void ChangePlayerCharacter(int colorId)
    {
        ChangePlayerCharacterServerRpc(colorId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangePlayerCharacterServerRpc(int _colorId, ServerRpcParams serverRpcParams = default)
    {
        if(!IsCharacterAvailable(_colorId))
        {
            return;
        }

        // 先取得當前　ID 更改　colorID 再更新 playerData(Grab, modify, attach)
        int playerDataIndex = GetPlayerDataIndex_From_ClientId(serverRpcParams.Receive.SenderClientId);

        PlayerData playerData  = playerDataNetworkList[playerDataIndex];
        playerData.colorId = _colorId;

        playerDataNetworkList[playerDataIndex] = playerData;
    }

    private bool IsCharacterAvailable(int colorId)
    {
        foreach(PlayerData playerData in playerDataNetworkList)
        {   
            if(playerData.colorId == colorId)
            {
                // already use
                return false;
            }
        }
        return true;
    }
    // 自動選角
    private int GetFirstUnusedCharacter()
    {
        for(int i = 0; i < playerColorList.Count; i++)
        {
            if(IsCharacterAvailable(i))
            {
                return i;
            }
        }

        return -1;
    }
    public Color GetPlayerIconColor(int colorId)
    {
        return playerColorList[colorId];
    }
    #endregion -----
    //

    /* Host */
    #region Start Host
    public void StartHost()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_Server_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Server_OnClientDisconnectCallback;    /// 清除斷線後的資料

        NetworkManager.Singleton.StartHost();
    }

    private void NetworkManager_Server_OnClientDisconnectCallback(ulong _clientId)
    {
        for(int i = 0; i < playerDataNetworkList.Count; i++)
        {
            PlayerData playerData = playerDataNetworkList[i];
            if(playerData.clientId == _clientId)
            {
                // disconnected
                playerDataNetworkList.RemoveAt(i);
            }
        }
    }

    private void NetworkManager_Server_OnClientConnectedCallback(ulong _clientId)
    {
        playerDataNetworkList.Add(new PlayerData
        {
            clientId = _clientId,
            colorId = GetFirstUnusedCharacter()
        });

        SetPlayerNameServerRpc(GetPlayerName_TMP());
        SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);
    }

    private void NetworkManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest connectionApprovalRequest, NetworkManager.ConnectionApprovalResponse connectionApprovalResponse)
    {
        if(SceneManager.GetActiveScene().name != Loader.Scene.CharacterSelectScene.ToString())
        {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "already start";
            return;
        }

        if(NetworkManager.Singleton.ConnectedClientsIds.Count >= Max_Player_Amount)
        {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "room full";
            return;
        }


        connectionApprovalResponse.Approved = true;
    }

    public void KickPlayer(ulong _clientId)
    {
        NetworkManager.Singleton.DisconnectClient(_clientId);
        NetworkManager_Server_OnClientDisconnectCallback(_clientId);
    }
    #endregion
    //

    /* Client */
    #region Start Client
    public void StartClient()
    {
        OnTryingToJoin?.Invoke(this, EventArgs.Empty);
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Client_OnClientDisconnectCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_Client_OnClientConnectedCallback;

        NetworkManager.Singleton.StartClient();
    }
    private void NetworkManager_Client_OnClientDisconnectCallback(ulong _clientId)
    {
        OnFailedToJoin?.Invoke(this, EventArgs.Empty);
    }
    #endregion
    //

    /* Set Player Name, playerLobbyId */
    #region Set Player Name, playerLobbyId
    private void NetworkManager_Client_OnClientConnectedCallback(ulong _clientId)
    {
        SetPlayerNameServerRpc(GetPlayerName_TMP());
        SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerNameServerRpc(string _playerName, ServerRpcParams serverRpcParams = default)
    {
        // 先取得當前　ID 更改　colorID 再更新 playerData(Grab, modify, attach)
        int playerDataIndex = GetPlayerDataIndex_From_ClientId(serverRpcParams.Receive.SenderClientId);

        PlayerData playerData  = playerDataNetworkList[playerDataIndex];
        playerData.playerName = _playerName;

        playerDataNetworkList[playerDataIndex] = playerData;
    }

    public string GetPlayerName_TMP()
    {
        return playerName;
    }
    public void SetPlayerName(string _playerName)
    {
        this.playerName = _playerName;

        PlayerPrefs.SetString(PlayerPrefs_PlayerName_Multiplayer, playerName);
    }
    public string GetPlayerName(ulong _clientId)
    {
        int playerDataIndex = GetPlayerDataIndex_From_ClientId(_clientId);

        return playerDataNetworkList[playerDataIndex].playerName.ToString();
    }
    
    // Set Id
    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerIdServerRpc(string _playerLobbyId, ServerRpcParams serverRpcParams = default)
    {
        // 先取得當前　ID 更改　colorID 再更新 playerData(Grab, modify, attach)
        int playerDataIndex = GetPlayerDataIndex_From_ClientId(serverRpcParams.Receive.SenderClientId);

        PlayerData playerData  = playerDataNetworkList[playerDataIndex];
        playerData.playerLobbyId = _playerLobbyId;

        playerDataNetworkList[playerDataIndex] = playerData;
    }
    #endregion
    //

    /* Netcode Spawn KitchObj */
    #region Netcode Spawn KitchObj
    public void SpawnKitchObj(KitchenObjSO kitchenObjSO, IKitchenObjParent kitchenObjParent)
    {
        //Debug.Log("spawn rpc");
        SpawnKitchenObjServerRpc(GettKitchenObjSOIndex(kitchenObjSO), kitchenObjParent.GetNetworkObject());
    }
    
    /* Netcode */
    [ServerRpc(RequireOwnership = false)]
    private void SpawnKitchenObjServerRpc(int kitchenObjSOIndex, NetworkObjectReference kitchenObjParentNetworkObjectReference)
    {
        KitchenObjSO kitchenObjSO = GettKitchenObjSOFromIndex(kitchenObjSOIndex);

        // Obj parent
        kitchenObjParentNetworkObjectReference.TryGet(out NetworkObject kitchenObjParentNetworkObject);
        IKitchenObjParent kitchenObjParent = kitchenObjParentNetworkObject.GetComponent<IKitchenObjParent>();

        if(kitchenObjParent.HasKitchenObj())
        {
            // 避免卡比
            return;
        }

        // Obj instantiate position
        Transform kitchenObjTransform = Instantiate(kitchenObjSO.prefab); 
        NetworkObject kitchenObjNetworkTransform = kitchenObjTransform.GetComponent<NetworkObject>();
        kitchenObjNetworkTransform.Spawn(true);
        
        // Set Parent
        KitchenObj kitchenObj = kitchenObjTransform.GetComponent<KitchenObj>();
        kitchenObj.SetKitchenObjParent(kitchenObjParent);
    }
    #endregion -----
    //

    /* Netcode Destory KitchObj */
    #region Netcode Destory KitchObj
    public void DestroyKitchObj(KitchenObj kitchenObj)
    {
        DestroyKitchObjServerRpc(kitchenObj.NetworkObject);
    }

    /* Netcode */
    [ServerRpc(RequireOwnership = false)]
    private void DestroyKitchObjServerRpc(NetworkObjectReference kitchenObjNetworkBehaviourReference)
    {
        kitchenObjNetworkBehaviourReference.TryGet(out NetworkObject kitchenObjNetworkObject);

        if(kitchenObjNetworkObject == null)
        {
            // already destory 避免卡比造成 bug
            return;
        }

        KitchenObj kitchenObj = kitchenObjNetworkObject.GetComponent<KitchenObj>();

        // clear parent then destory
        ClearKitchenObjOnParentClientRpc(kitchenObjNetworkBehaviourReference);
        kitchenObj.DestroyKitchenObj();   //only server can destory networkObject
    }


    [ClientRpc]
    private void ClearKitchenObjOnParentClientRpc(NetworkObjectReference kitchenObjNetworkObjectReference)
    {
        kitchenObjNetworkObjectReference.TryGet(out NetworkObject kitchenObjNetworkObject);
        KitchenObj kitchenObj = kitchenObjNetworkObject.GetComponent<KitchenObj>();

        kitchenObj.ClearKitchenObjOnParent();
    }
    #endregion ----- 
    //


    // 輸入的 KitchenObj 的 Index
    public int GettKitchenObjSOIndex(KitchenObjSO kitchenObjSO)
    {
        return kitchenObjListSO.kitchenObjSOList.IndexOf(kitchenObjSO);
    }
    // 取得 Index 對應 KJ
    public KitchenObjSO GettKitchenObjSOFromIndex(int kitchenObjSOIndex)
    {
        return kitchenObjListSO.kitchenObjSOList[kitchenObjSOIndex];
    }
}
