using System.Collections.Generic;
using UnityEngine;
using System;

public class PlateKitchenObj : KitchenObj
{
    public event EventHandler<OnIngredientAddedEventArgs> OnIngredientAdded;
    public class OnIngredientAddedEventArgs : EventArgs
    {
        public KitchenObjSO kitchenObjSO;
    }

    [SerializeField] private List<KitchenObjSO> validKitchenObjSOList;
    private List<KitchenObjSO> kitchenObjSOListOnPlate;

    protected override void Awake() 
    {
        base.Awake();   // 先運行 KitchenObj 的 followTransform
        kitchenObjSOListOnPlate = new List<KitchenObjSO>();
    }
    
    // 把不重複食材加入
    public bool TryAddIngredient(KitchenObjSO kitchenObjSO)
    {
        // not a valid ingredient
        if(!validKitchenObjSOList.Contains(kitchenObjSO))
        {
            return false;
        }

        if(!kitchenObjSOListOnPlate.Contains(kitchenObjSO))
        {
            kitchenObjSOListOnPlate.Add(kitchenObjSO);

            // 傳遞被加入的食材是哪個
            OnIngredientAdded?.Invoke(this, new OnIngredientAddedEventArgs{
                kitchenObjSO = kitchenObjSO
            });

            return true;
        }
        else
        {
            return false;
        }
    }

    // 目前盤上有的食材
    public List<KitchenObjSO> GetKitchenObjSOListOnPlate()
    {
        return kitchenObjSOListOnPlate;
    }
}
