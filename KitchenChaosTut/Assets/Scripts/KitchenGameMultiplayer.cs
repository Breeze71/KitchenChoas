using UnityEngine;
using Unity.Netcode;
using System;
using UnityEngine.SceneManagement;

public class KitchenGameMultiplayer : NetworkBehaviour
{
    private const int Max_Player_Amount = 4;

    public static KitchenGameMultiplayer Instance
    {
        get;
        private set;
    }
    

    public event EventHandler OnTryingToJoin;
    public event EventHandler OnFailedToJoin;


    [SerializeField] private KitchenObjListSO kitchenObjListSO;

    private void Awake() 
    {
        Instance = this;

        DontDestroyOnLoad(gameObject);  
    }

    public void StartHost()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
        NetworkManager.Singleton.StartHost();
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

    public void StartClient()
    {
        OnTryingToJoin?.Invoke(this, EventArgs.Empty);
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;

        NetworkManager.Singleton.StartClient();
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
    {
        OnFailedToJoin?.Invoke(this, EventArgs.Empty);
    }


    #region Spawn
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


        Transform kitchenObjTransform = Instantiate(kitchenObjSO.prefab); 
        NetworkObject kitchenObjNetworkTransform = kitchenObjTransform.GetComponent<NetworkObject>();
        kitchenObjNetworkTransform.Spawn(true);


        KitchenObj kitchenObj = kitchenObjTransform.GetComponent<KitchenObj>();

        kitchenObjParentNetworkObjectReference.TryGet(out NetworkObject kitchenObjParentNetworkObject);
        IKitchenObjParent kitchenObjParent = kitchenObjParentNetworkObject.GetComponent<IKitchenObjParent>();
        
        kitchenObj.SetKitchenObjParent(kitchenObjParent);
    }
    #endregion

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


    #region Destory
    public void DestroyKitchObj(KitchenObj kitchenObj)
    {
        DestroyKitchObjServerRpc(kitchenObj.NetworkObject);
    }

    /* Netcode */
    [ServerRpc(RequireOwnership = false)]
    private void DestroyKitchObjServerRpc(NetworkObjectReference kitchenObjNetworkBehaviourReference)
    {
        kitchenObjNetworkBehaviourReference.TryGet(out NetworkObject kitchenObjNetworkObject);
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
    #endregion
}
