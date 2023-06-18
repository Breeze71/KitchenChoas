using UnityEngine;
using System;
using Unity.Netcode;

public class CuttingCounter : BaseCounter, IHasProgress
{
    // sound event
    public static event EventHandler OnAnyCutSound;
    new public static void ResetStaticData()
    {
        // clear all the listener of static event
        OnAnyCutSound = null;
    }

    /* Interface IHasProgress*/
    public event EventHandler<IHasProgress.OnProgressBarChangeEventArgs> OnProgressBarChanged;

    // visual Event
    public event EventHandler OnCut;

    [SerializeField] private CuttingRecipeSO[] cutKitchObjSOArray;
    private int cuttingProgress;

    public override void Interact(Player player)
    {
        // nothing on counter,  Player is carrying something
        if(!HasKitchenObj() && player.HasKitchenObj())
        {
            // 可切
            if(HasRecipeWithInput(player.GetKitchenObj().GetKitchenObjSO()))
            {
                /* player.kitchenObj.SetKitchenObjParent(this); 要等 Server 回傳消息，使之後的 code 來不急
                // 所以直接使用 player 手上的，不必等 Server 回傳已經換手的消息                             */
                KitchenObj kitchenObj = player.GetKitchenObj();
                kitchenObj.SetKitchenObjParent(this);

                InteractLogicPutObjOnCounterServerRpc();
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
                        KitchenObj.DestroyKitchObj(GetKitchenObj());
                    }
                }
            }

            // player
            else
            {
                GetKitchenObj().SetKitchenObjParent(player);

                // 拿走後關掉 bar
                OnProgressBarChanged?.Invoke(this, new IHasProgress.OnProgressBarChangeEventArgs{
                    progressBarNormalized = 0
                });
            }
        }
    }


    /* Netcode */
    #region Netcode Reset Progress Bar If Put On
    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicPutObjOnCounterServerRpc()
    {
        InteractLogicPutObjOnCounterClientRpc();
    }
    [ClientRpc]
    private void InteractLogicPutObjOnCounterClientRpc()
    {
        cuttingProgress = 0;

        // 初始為 0 
        OnProgressBarChanged?.Invoke(this, new IHasProgress.OnProgressBarChangeEventArgs{
            progressBarNormalized = 0f
        });
    }
    #endregion


    public override void InteractAlternate(Player player)
    {
        // has a kitchen obj, cut it    // 避免已經切好報錯
        if(HasKitchenObj() && HasRecipeWithInput(GetKitchenObj().GetKitchenObjSO()))
        {
            CutObjServerRpc();
            TestCuttingProgressDoneServerRpc();
        }
    }


    /*  Netcode */
    #region Netcode Cut
    // only execute on server
    [ServerRpc(RequireOwnership = false)]
    private void CutObjServerRpc()
    {
        if(HasKitchenObj() && HasRecipeWithInput(GetKitchenObj().GetKitchenObjSO()))
        {
            CutObjClientRpc();
        }
    }
    [ClientRpc]
    private void CutObjClientRpc()
    {
        // 取得每種 obj 的 maxCut
        cuttingProgress++;

        OnCut?.Invoke(this, EventArgs.Empty);
        OnAnyCutSound?.Invoke(this, EventArgs.Empty);

        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(GetKitchenObj().GetKitchenObjSO());   // 切幾刀
    
        /* Cutting Event */
        OnProgressBarChanged?.Invoke(this, new IHasProgress.OnProgressBarChangeEventArgs{
            progressBarNormalized = (float)cuttingProgress / cuttingRecipeSO.cuttingProgressMax
        });            
    }
    // 避免有 n 個 Client 刪除生成 n 次
    [ServerRpc(RequireOwnership = false)]
    private void TestCuttingProgressDoneServerRpc()
    {
        if(HasKitchenObj() && HasRecipeWithInput(GetKitchenObj().GetKitchenObjSO()))    //避免延遲產生 bug
        {
            CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(GetKitchenObj().GetKitchenObjSO());   // 切幾刀

            // cutting
            if(cuttingProgress >= cuttingRecipeSO.cuttingProgressMax)
            {
                // 確認其切片
                KitchenObjSO outputKitchenObjSO = GetOutputForInput(GetKitchenObj().GetKitchenObjSO());

                KitchenObj.DestroyKitchObj(GetKitchenObj());

                // 產生切片
                KitchenObj.SpawnKitchObj(outputKitchenObjSO, this);
            }
        }
    }
    #endregion


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
