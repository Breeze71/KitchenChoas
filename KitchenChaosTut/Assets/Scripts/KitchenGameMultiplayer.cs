using UnityEngine;
using Unity.Netcode;

public class KitchenGameMultiplayer : NetworkBehaviour
{
    public static KitchenGameMultiplayer Instance
    {
        get;
        private set;
    }
    
    [SerializeField] private KitchenObjListSO kitchenObjListSO;

    private void Awake() 
    {
        Instance = this;    
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
        kitchenObj.DestroySelf();   //only server can destory networkObject
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
