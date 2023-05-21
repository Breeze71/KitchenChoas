using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateIconUI : MonoBehaviour
{
    [SerializeField] private PlateKitchenObj plateKitchenObj;
    [SerializeField] private Transform iconTemplate;

    private void Awake() 
    {
        iconTemplate.gameObject.SetActive(false);
    }
    private void Start() 
    {
        plateKitchenObj.OnIngredientAdded += PlateKitchenObj_OnIngredientAdded;
    }

    private void PlateKitchenObj_OnIngredientAdded(object sender, PlateKitchenObj.OnIngredientAddedEventArgs e)
    {
        UpdateIcon();
    }

    private void UpdateIcon()
    {
        // 先刪再重新生成
        foreach(Transform child in transform)
        {
            if(child == iconTemplate)
                continue;
            
            Destroy(child.gameObject);
        }

        foreach(KitchenObjSO kitchenObjSO in plateKitchenObj.GetKitchenObjSOListOnPlate())
        {
            // prefab, parent
            Transform iconTransform =  Instantiate(iconTemplate, transform);
            iconTransform.gameObject.SetActive(true);

            // 設定 icon
            iconTransform.GetComponent<PlateIconSingle>().SetKitchenObjSO(kitchenObjSO);
        }
    }
}
