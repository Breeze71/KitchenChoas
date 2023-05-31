using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Netcode;

public class DeliveryManager : NetworkBehaviour
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
    private float spawnRecipeTimer = 4f;
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
        if(!IsServer)
        {
            return; // 由 Server 為主
        }
            
        spawnRecipeTimer -= Time.deltaTime;

        if(spawnRecipeTimer <= 0)
        {
            spawnRecipeTimer = spawnRecipeTimerMax;
            
            // 如果目前訂單數小於 waitingRecipesMax  && isPlaying() 隨機生成
            if(GameManager.Instance.IsGamePlaying() && waitingRecipeSOList.Count < waitingRecipesMax)
            {
                int waitingRecipeSOIndex = UnityEngine.Random.Range(0, recipeListSO.recipeSOList.Count);
                
                SpawnNewWaitingRecipeClientRpc(waitingRecipeSOIndex);    // Host run both Server and Client
            }
        }
    }

    [ClientRpc]
    private void SpawnNewWaitingRecipeClientRpc(int waitingRecipeSOIndex)
    {
        // Rpc 不能傳 Serilized
        RecipeSO waitingRecipeSO = recipeListSO.recipeSOList[waitingRecipeSOIndex];

        waitingRecipeSOList.Add(waitingRecipeSO);
        OnRecipeSpawned?.Invoke(this, EventArgs.Empty);
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
                    DeliverCorrectRecipeServerRpc(i);   // 由　Server Broadcast

                    return;
                }
            }
        }

        // no match found
        DeliverIncorrectRecipeServerRpc();
    }

    # region Netcode
    /* DeliverCorrect */
    [ServerRpc(RequireOwnership = false)]   // Client can trigger 
    private void DeliverCorrectRecipeServerRpc(int waitingRecipeSO)
    {
        // Send to all Client
        DeliverCorrectRecipeClientRpc(waitingRecipeSO);
    }
    [ClientRpc]
    private void DeliverCorrectRecipeClientRpc(int waitingRecipeSOListIndex)
    {
        successfulRecipeAmount++;

        waitingRecipeSOList.RemoveAt(waitingRecipeSOListIndex);

        OnRecipeCompleted?.Invoke(this, EventArgs.Empty);
        OnRecipeSuccess?.Invoke(this, EventArgs.Empty);        
    }

    /* DeliverIncorrect */
    [ServerRpc(RequireOwnership = false)]
    private void DeliverIncorrectRecipeServerRpc()
    {
        DeliverIncorrectRecipeClientRpc();
    }
    [ClientRpc]
    private void DeliverIncorrectRecipeClientRpc()
    {
        OnRecipeFailed?.Invoke(this, EventArgs.Empty);
    }    
    #endregion

    public List<RecipeSO> GetWaitingRecipeSOList()
    {
        return waitingRecipeSOList;
    }

    public int GetSuccessfulRecipe()
    {
        return successfulRecipeAmount;
    }
}
