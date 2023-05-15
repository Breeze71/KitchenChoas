using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IKitchenObjParent
{
    // 當前位置
    public Transform GetKitchenObjectFollowTransform();

    // 將用此 Interface 的 KitchenObj 設為此 KitchenObj
    public void SetKitchenObj(KitchenObj kitchenObj);

    // 回傳當拿的　kitchenObj
    public KitchenObj GetKitchenObj();

    // 放下或拿走時要清空
    public void ClearKitchObj();

    // 有無
    public bool HasKitchenObj();

}
