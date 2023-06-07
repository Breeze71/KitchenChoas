using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Netcode;

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
            AddIngredientToPlateServerRpc(KitchenGameMultiplayer.Instance.GettKitchenObjSOIndex(kitchenObjSO)); // 轉 int

            return true;
        }
        else
        {
            return false;
        }
    }


    /* Netcode */
    #region Add Ingredient To The Plate
    [ServerRpc(RequireOwnership = false)]   // 任何一個 Client 都有 ownerShip AddIngredient
    private void AddIngredientToPlateServerRpc(int kitchenObjSOIndex)
    {
        AddIngredientToPlateClientRpc(kitchenObjSOIndex);
    }
    [ClientRpc]
    private void AddIngredientToPlateClientRpc(int kitchenObjSOIndex)
    {
        KitchenObjSO kitchenObjSO = KitchenGameMultiplayer.Instance.GettKitchenObjSOFromIndex(kitchenObjSOIndex);

        kitchenObjSOListOnPlate.Add(kitchenObjSO);

        // 傳遞被加入的食材是哪個 update visual
        OnIngredientAdded?.Invoke(this, new OnIngredientAddedEventArgs{
            kitchenObjSO = kitchenObjSO
        });
    }
    #endregion


    // 目前盤上有的食材
    public List<KitchenObjSO> GetKitchenObjSOListOnPlate()
    {
        return kitchenObjSOListOnPlate;
    }
}
