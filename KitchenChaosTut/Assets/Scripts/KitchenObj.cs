using UnityEngine;
using Unity.Netcode;

public class KitchenObj : NetworkBehaviour
{
    [SerializeField] private KitchenObjSO kitchenObjSO;

    // Interface
    private IKitchenObjParent kitchenObjParent;
    private FollowTransform followTransform;

    protected virtual void Awake() 
    {
        // plate kitchenObj
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

    // Set Follow
    public void SetKitchenObjParent(IKitchenObjParent kitchenObjParent)
    {
        SetKitchenObjParentServerRpc(kitchenObjParent.GetNetworkObject());
    }

    /* Netcode  Set Follow*/
    #region Netcode
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
    }
    #endregion

    public IKitchenObjParent GetKitchenObjParent()
    {
        return kitchenObjParent;
    }

    public void DestroyKitchenObj()
    {
        Destroy(gameObject);
    }

    public void ClearKitchenObjOnParent()
    {
        kitchenObjParent.ClearKitchObj();       
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

    /* Network obj */ 
    public static void SpawnKitchObj(KitchenObjSO kitchenObjSO, IKitchenObjParent kitchenObjParent)
    {
        KitchenGameMultiplayer.Instance.SpawnKitchObj(kitchenObjSO, kitchenObjParent);
    }
    public static void DestroyKitchObj(KitchenObj kitchenObj)
    {
        KitchenGameMultiplayer.Instance.DestroyKitchObj(kitchenObj);
    }
        
}
