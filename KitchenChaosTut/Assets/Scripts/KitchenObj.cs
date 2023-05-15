using UnityEngine;

public class KitchenObj : MonoBehaviour
{
    [SerializeField] private KitchenObjSO kitchenObjSO;

    // Interface
    private IKitchenObjParent kitchenObjParent;

    // 確認是拿到哪個 KitchenObj

    // 用 scriptable 存 name, prefabs
    // 生成 kitchenObj.prefabs
    // prefabs 有 return 能確定是哪個 scripatable
    // 即可確定當前生成物是番茄還是起司
    public KitchenObjSO GetKitchenObjSO()
    {
        return kitchenObjSO;
    }

    // Set Parent
    public void SetKitchenObjParent(IKitchenObjParent kitchenObjParent)
    {
        // 每次先重製　Parent
        if(this.kitchenObjParent != null)
        {
            this.kitchenObjParent.ClearKitchObj();
        }
        // 再設定為　kitchenObjParent　觸發的為 Parent
        this.kitchenObjParent = kitchenObjParent;

        if(kitchenObjParent.HasKitchenObj())
        {
            Debug.LogError("IKitchenObjParent already has a KitchObj");
        }

        //　更換 Parent 持有的為 this
        kitchenObjParent.SetKitchenObj(this);

        // 隨 counterTop or HoldPoint 移動
        transform.parent = kitchenObjParent.GetKitchenObjectFollowTransform();
        transform.localPosition = Vector3.zero;
    }
    public IKitchenObjParent GetKitchenObjParent()
    {
        return kitchenObjParent;
    }
}
