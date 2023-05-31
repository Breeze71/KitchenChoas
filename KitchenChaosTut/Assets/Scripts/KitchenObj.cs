using UnityEngine;
using Unity.Netcode;

public class KitchenObj : NetworkBehaviour
{
    [SerializeField] private KitchenObjSO kitchenObjSO;

    // Interface
    private IKitchenObjParent kitchenObjParent;
    private FollowTransform followTransform;

    private void Awake() 
    {
        followTransform = GetComponent<FollowTransform>();    
    }

    // 確認是拿到哪個 KitchenObj
    // 用 scriptable 存 name, prefabs
    // 生成 kitchenObj.prefabs
    // prefabs 有 return 能確定是哪個 scripatable
    // 即可確定當前生成物是番茄還是起司
    /* 調用 return 回傳確認是哪個(Scriptable 只能存讀數據，不能 mono)
        Debug.Log(kitchenObjTransform.GetComponent<KitchenObj>().GetKitchenObjSO());*/
    public KitchenObjSO GetKitchenObjSO()
    {
        return kitchenObjSO;
    }

    // Set Parent
    public void SetKitchenObjParent(IKitchenObjParent kitchenObjParent)
    {
        SetKitchenObjParentServerRpc(kitchenObjParent.GetNetworkObject());
    }

    /* Netcode */
    [ServerRpc(RequireOwnership = false)]
    private void SetKitchenObjParentServerRpc(NetworkObjectReference kitchenObjParentNetworkObjectReference)
    {
        SetKitchenObjParentClientRpc(kitchenObjParentNetworkObjectReference);
    }
    [ClientRpc]
    private void SetKitchenObjParentClientRpc(NetworkObjectReference kitchenObjParentNetworkObjectReference)
    {
        // 取得 NetworkObject 中的 <IKitchenObjParent> (player)
        kitchenObjParentNetworkObjectReference.TryGet(out NetworkObject kitchenObjParentNetworkObject);
        IKitchenObjParent kitchenObjParent = kitchenObjParentNetworkObject.GetComponent<IKitchenObjParent>();

        if(this.kitchenObjParent != null)
        {
            this.kitchenObjParent.ClearKitchObj();
        }

        this.kitchenObjParent = kitchenObjParent;

        if(kitchenObjParent.HasKitchenObj())
        {
            Debug.LogError("has another kobj");
        }

        kitchenObjParent.SetKitchenObj(this);

        followTransform.SetTargetTransform(kitchenObjParent.GetKitchenObjectFollowTransform());
        Debug.Log(kitchenObjParent);
    }
    
    public IKitchenObjParent GetKitchenObjParent()
    {
        return kitchenObjParent;
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    public bool TryGetPlate(out PlateKitchenObj plateKitchenObj)
    {
        if(this is PlateKitchenObj)
        {
            plateKitchenObj = this as PlateKitchenObj;

            return true;
        }
        else
        {
            plateKitchenObj = null;
            
            return false;
        }
    }

    /* Netcode */ 
    public static void SpawnKitchObj(KitchenObjSO kitchenObjSO, IKitchenObjParent kitchenObjParent)
    {
        Debug.Log("spawn");
        KitchenGameMultiplayer.Instance.SpawnKitchObj(kitchenObjSO, kitchenObjParent);
    }
}
