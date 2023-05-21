using System;
using System.Collections.Generic;
using UnityEngine;

public class PlateCompleteVisual : MonoBehaviour
{
    // linked the gameObj to kitchenObjSO
    [Serializable]
    public struct kitchenObjSO_GameObj
    {
        public KitchenObjSO kitchenObjSO;
        public GameObject gameObject;
    }

    [SerializeField] private PlateKitchenObj plateKitchenObj;
    [SerializeField] private List<kitchenObjSO_GameObj> kitchenObjSO_GameObjList;

    private void Start() 
    {
        plateKitchenObj.OnIngredientAdded += PlateKitchenObj_OnIngredientAdded;

        foreach(kitchenObjSO_GameObj kitchenObjSOGameObj in kitchenObjSO_GameObjList)
        {
            kitchenObjSOGameObj.gameObject.SetActive(false);
        }
    }

    private void PlateKitchenObj_OnIngredientAdded(object sender, PlateKitchenObj.OnIngredientAddedEventArgs e)
    {
        //  SetActive(true)
        foreach(kitchenObjSO_GameObj kitchenObjSOGameObj in kitchenObjSO_GameObjList)
        {
            if(kitchenObjSOGameObj.kitchenObjSO == e.kitchenObjSO)
            {
                kitchenObjSOGameObj.gameObject.SetActive(true);
            }
        }
    }
}
