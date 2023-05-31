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


    public void SpawnKitchObj(KitchenObjSO kitchenObjSO, IKitchenObjParent kitchenObjParent)
    {
        Debug.Log("spawn rpc");
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


    // 輸入的 KitchenObj 的 Index
    private int GettKitchenObjSOIndex(KitchenObjSO kitchenObjSO)
    {
        return kitchenObjListSO.kitchenObjSOList.IndexOf(kitchenObjSO);
    }
    // 取得 Index 對應 KJ
    private KitchenObjSO GettKitchenObjSOFromIndex(int kitchenObjSOIndex)
    {
        return kitchenObjListSO.kitchenObjSOList[kitchenObjSOIndex];
    }
}
