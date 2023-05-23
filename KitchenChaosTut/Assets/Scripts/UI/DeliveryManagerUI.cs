using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManagerUI : MonoBehaviour
{
    [SerializeField] private Transform container;
    [SerializeField] private Transform recipeTemplate;

    private void Awake() 
    {
        recipeTemplate.gameObject.SetActive(false);
    }
    private void Start() 
    {
        DeliveryManager.Instance.OnRecipeSpawned += DeliveryManager_OnRecipeSpawned;
        DeliveryManager.Instance.OnRecipeCompleted += DeliveryManager_OnRecipeCompleted;

        UpdateIcon();
    }

    // when trigger event than update Icon
    private void DeliveryManager_OnRecipeCompleted(object sender, EventArgs e)
    {
        UpdateIcon();
    }
    private void DeliveryManager_OnRecipeSpawned(object sender, EventArgs e)
    {
        UpdateIcon();
    }

    private void UpdateIcon()
    {
        foreach(Transform child in container)
        {
            if(child == recipeTemplate)
                continue;

            Destroy(child.gameObject);
        }

        // 生成 icon
        foreach(RecipeSO recipeSO in DeliveryManager.Instance.GetWaitingRecipeSOList())
        {
            Transform recipeTransform = Instantiate(recipeTemplate, container);
            recipeTransform.gameObject.SetActive(true);

            // 將對應 recipe 輸入 命名
            recipeTransform.GetComponent<DeliveryManagerIconSingle>().SetRecipeSO(recipeSO);
        }
    }

    
}
