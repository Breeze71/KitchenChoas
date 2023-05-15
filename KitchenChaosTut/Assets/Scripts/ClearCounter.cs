using UnityEngine;

public class ClearCounter : BaseCounter, IKitchenObjParent
{
    [SerializeField] private KitchenObjSO kitchenObjSO;
    [SerializeField] private Transform counterTopPoint;

    private KitchenObj kitchenObj;

    /* Extend BaseCounter*/
    public override void Interact(PlayerMovement player)
    {   
        // 如果 Counter 上沒有則生成一個，並確保生成在其上
        if(kitchenObj == null)
        {
            Transform kitchenObjTransform = Instantiate(kitchenObjSO.prefab, counterTopPoint);

            kitchenObjTransform.GetComponent<KitchenObj>().SetKitchenObjParent(this);
        }
        else
        {
            // give it to player
            kitchenObj.SetKitchenObjParent(player);
        }
    
        /* 調用 return 回傳確認是哪個(Scriptable 只能存讀數據，不能 mono)
            Debug.Log(kitchenObjTransform.GetComponent<KitchenObj>().GetKitchenObjSO());*/
    }

    /* IKitchenObj Interface */
    public Transform GetKitchenObjectFollowTransform()
    {
        return counterTopPoint;
    }
    public void SetKitchenObj(KitchenObj kitchenObj)
    {
        this.kitchenObj = kitchenObj;
    }
    public KitchenObj GetKitchenObj()
    {
        return kitchenObj;
    }
    public void ClearKitchObj()
    {
        kitchenObj = null;
    }
    public bool HasKitchenObj()
    {
        return kitchenObj != null;
    }
}
