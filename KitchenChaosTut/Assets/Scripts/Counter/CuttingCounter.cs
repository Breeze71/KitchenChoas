using UnityEngine;
using System;

public class CuttingCounter : BaseCounter, IHasProgress
{
    /* Interface IHasProgress*/
    public event EventHandler<IHasProgress.OnProgressBarChangeEventArgs> OnProgressBarChange;

    // Event
    public event EventHandler OnCut;

    [SerializeField] private CuttingRecipeSO[] cutKitchObjSOArray;
    private int cuttingProgress;


    public override void Interact(PlayerMovement player)
    {
        // nothing on counter,  Player is carrying something
        if(!HasKitchenObj() && player.HasKitchenObj())
        {
            // 可切
            if(HasRecipeWithInput(player.GetKitchenObj().GetKitchenObjSO()))
            {
                player.GetKitchenObj().SetKitchenObjParent(this);
                cuttingProgress = 0;

                /* Cutting Event */
                CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(GetKitchenObj().GetKitchenObjSO());
                // 初始為 0 
                OnProgressBarChange?.Invoke(this, new IHasProgress.OnProgressBarChangeEventArgs{
                    progressBarNormalized = (float)cuttingProgress / cuttingRecipeSO.cuttingProgressMax
                });
            }
        }

        else
        {
            if(player.HasKitchenObj())
            {
                if(player.GetKitchenObj().TryGetPlate(out PlateKitchenObj plateKitchenObj))
                {
                    if(plateKitchenObj.TryAddIngredient(GetKitchenObj().GetKitchenObjSO()))
                    {
                        GetKitchenObj().DestroySelf();
                    }
                }
            }

            // player
            else
            {
                GetKitchenObj().SetKitchenObjParent(player);

                // 拿走後關掉 bar
                OnProgressBarChange?.Invoke(this, new IHasProgress.OnProgressBarChangeEventArgs{
                    progressBarNormalized = 0
                });
            }
        }
    }

    public override void InteractAlternate(PlayerMovement player)
    {
        // has a kitchen obj, cut it    // 避免已經切好報錯
        if(HasKitchenObj() && HasRecipeWithInput(GetKitchenObj().GetKitchenObjSO()))
        {
            // 取得每種 obj 的 maxCut
            cuttingProgress++;
            OnCut?.Invoke(this, EventArgs.Empty);

            CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(GetKitchenObj().GetKitchenObjSO());
    
            /* Cutting Event */
            OnProgressBarChange?.Invoke(this, new IHasProgress.OnProgressBarChangeEventArgs{
                progressBarNormalized = (float)cuttingProgress / cuttingRecipeSO.cuttingProgressMax
            });            

            // cutting
            if(cuttingProgress >= cuttingRecipeSO.cuttingProgressMax)
            {
                // 確認其切片
                KitchenObjSO outputKitchenObjSO = GetOutputForInput(GetKitchenObj().GetKitchenObjSO());

                GetKitchenObj().DestroySelf();

                // 產生切片
                KitchenObj.SpawnKitchObj(outputKitchenObjSO, this);
            }
        }
    }

    // 不能切的不能放上去
    private bool HasRecipeWithInput(KitchenObjSO inputKitchenObjSO)
    {
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(inputKitchenObjSO);

        return cuttingRecipeSO != null;
    }

    // 切片後
    private KitchenObjSO GetOutputForInput(KitchenObjSO inputKitchenObjSO)
    {
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(inputKitchenObjSO);
        
        if(cuttingRecipeSO != null)
            return cuttingRecipeSO.output;
        else
            return null;
    }

    // 判斷能否切
    private CuttingRecipeSO GetCuttingRecipeSOWithInput(KitchenObjSO inputKitchenObjSO)
    {
        foreach(CuttingRecipeSO cuttingRecipeSO in cutKitchObjSOArray)
        {
            if(cuttingRecipeSO.input == inputKitchenObjSO)
            {
                return cuttingRecipeSO;
            }
        }
        return null;        
    }
}
