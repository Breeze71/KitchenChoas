using System.Collections.Generic;
using UnityEngine;
using System;

public class DeliveryManager : MonoBehaviour
{
    public event EventHandler OnRecipeSpawned;
    public event EventHandler OnRecipeCompleted;

    // sound event 
    public event EventHandler OnRecipeSuccess;
    public event EventHandler OnRecipeFailed;

    public static DeliveryManager Instance
    {
        get;
        private set;
    }

    [SerializeField] private RecipeListSO recipeListSO;

    # region variable
    private List<RecipeSO> waitingRecipeSOList;
    private float spawnRecipeTimer;
    private float spawnRecipeTimerMax = 4f;
    private int waitingRecipesMax = 4;
    private int successfulRecipeAmount;
    # endregion

    private void Awake() 
    {
        Instance = this;

        waitingRecipeSOList = new List<RecipeSO>();
    }

    private void Update() 
    {
        spawnRecipeTimer -= Time.deltaTime;

        if(spawnRecipeTimer <= 0)
        {
            spawnRecipeTimer = spawnRecipeTimerMax;
            
            // 如果目前訂單數小於 waitingRecipesMax ， 隨機生成
            if(waitingRecipeSOList.Count < waitingRecipesMax)
            {
                RecipeSO waitingRecipeSO = recipeListSO.recipeSOList[UnityEngine.Random.Range(0, recipeListSO.recipeSOList.Count)];
                waitingRecipeSOList.Add(waitingRecipeSO);

                // event
                OnRecipeSpawned?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public void DeliverRecipe(PlateKitchenObj plateKitchenObj)
    {
        for(int i = 0; i < waitingRecipeSOList.Count; i++)
        {
            RecipeSO waitingRecipeSO = waitingRecipeSOList[i];

            // 需要的食材跟盤上一樣
            if(waitingRecipeSO.kitchenObjSOList.Count == plateKitchenObj.GetKitchenObjSOListOnPlate().Count)
            {
                bool plateContentsMatchesRecipe = true; 

                foreach(KitchenObjSO recipeKitchenObjSO in waitingRecipeSO.kitchenObjSOList)
                {
                    bool ingredientFound = false;

                    foreach(KitchenObjSO plateKitchenObjSO in plateKitchenObj.GetKitchenObjSOListOnPlate())
                    {
                        if(plateKitchenObjSO == recipeKitchenObjSO)
                        {
                            ingredientFound = true;
                            break;
                        }
                    }

                    // this gredient was not found on the plate
                    if(!ingredientFound)
                    {
                        plateContentsMatchesRecipe = false;
                    }
                }

                // player delivered the correct recipe
                if(plateContentsMatchesRecipe)
                {
                    waitingRecipeSOList.RemoveAt(i);

                    successfulRecipeAmount++;

                    // event
                    OnRecipeCompleted?.Invoke(this, EventArgs.Empty);
                    OnRecipeSuccess?.Invoke(this, EventArgs.Empty);

                    return;
                }
            }
        }

        // no match found
        OnRecipeFailed?.Invoke(this, EventArgs.Empty);
    }

    public List<RecipeSO> GetWaitingRecipeSOList()
    {
        return waitingRecipeSOList;
    }

    public int GetSuccessfulRecipe()
    {
        return successfulRecipeAmount;
    }
}
