using UnityEngine;
using System;
using Unity.Netcode;

public class StoveCounter : BaseCounter, IHasProgress
{
    /* Interface IHasProgress */
    public event EventHandler<IHasProgress.OnProgressBarChangeEventArgs> OnProgressBarChanged;

    #region Control Stove Visual Event
    public event EventHandler<OnFoodStateChangedEventArgs> OnFoodStateChanged;
    public class OnFoodStateChangedEventArgs : EventArgs
    {
        public FoodState foodState;
    }

    #endregion

    public enum FoodState
    {
        Idle,
        Frying,
        Fried,
        Burned,
    }
    private NetworkVariable<FoodState> foodState = new NetworkVariable<FoodState>(FoodState.Idle);

    # region FoodSO Variable
    [SerializeField] private FryingFoodSO[] fryingFoodSOArray;
    [SerializeField] private BurningFoodSO[] burningFoodSOArray;

    private NetworkVariable<float> fryingTimer = new NetworkVariable<float>(0f);
    private NetworkVariable<float> burningTimer = new NetworkVariable<float>(0f);

    private FryingFoodSO fryingFoodSO;
    private BurningFoodSO burningFoodSO;

    # endregion
    

    /* Netcode */
    #region Network Variable Event
    public override void OnNetworkSpawn()
    {
        fryingTimer.OnValueChanged += FryingTimer_OnValueSetFryingRecipeSOd;
        burningTimer.OnValueChanged += burningTimer_OnValueSetFryingRecipeSOd;
        foodState.OnValueChanged += foodState_OnValueSetFryingRecipeSOd;
    }

    // Client can auto read network Variable
    private void FryingTimer_OnValueSetFryingRecipeSOd(float previousValue, float newValue)
    {   
        // 避免 Server　還沒同步　fringTimerMax
        float fringTimerMax = fryingFoodSO != null ? fryingFoodSO.fringTimerMax : 1f;

        // start progress
        OnProgressBarChanged?.Invoke(this, new IHasProgress.OnProgressBarChangeEventArgs
        {
            progressBarNormalized = fryingTimer.Value / fringTimerMax
        });
    }

    private void burningTimer_OnValueSetFryingRecipeSOd(float previousValue, float newValue)
    {
        float burningTimerMax = burningFoodSO != null ? burningFoodSO.BurningTimerMax : 1f;

        OnProgressBarChanged?.Invoke(this, new IHasProgress.OnProgressBarChangeEventArgs
        {
            progressBarNormalized = burningTimer.Value / burningTimerMax
        });        
    }

    private void foodState_OnValueSetFryingRecipeSOd(FoodState previousState, FoodState newState)
    {
        // 訂閱更新 state
        OnFoodStateChanged?.Invoke(this, new OnFoodStateChangedEventArgs
        {
            foodState = foodState.Value
        });

        if(foodState.Value == FoodState.Burned || foodState.Value == FoodState.Idle)
        {
            OnProgressBarChanged?.Invoke(this, new IHasProgress.OnProgressBarChangeEventArgs
            {
                // Hide() progress bar
                progressBarNormalized = 0f
            });
        }

    }
    #endregion


    private void Update() 
    {
        if(!IsServer || !HasKitchenObj())
            return;

        switch(foodState.Value)
        {
            case FoodState.Idle:
                break;

            case FoodState.Frying:

                /* Auto Trigger Network Variable Event */
                fryingTimer.Value += Time.deltaTime;

                if(fryingTimer.Value > fryingFoodSO.fringTimerMax)
                {
                    KitchenObj.DestroyKitchObj(GetKitchenObj());
                    KitchenObj.SpawnKitchObj(fryingFoodSO.output, this);

                    /* Auto Trigger Network Variable Event */
                    foodState.Value = FoodState.Fried;

                    burningTimer.Value = 0f;

                    SetBurningRecipeSOClientRpc(KitchenGameMultiplayer.Instance.GettKitchenObjSOIndex(
                        GetKitchenObj().GetKitchenObjSO()
                    ));
                }
                break;

            case FoodState.Fried:

                /* Auto Trigger Network Variable Event */
                burningTimer.Value += Time.deltaTime;

                if(burningTimer.Value > burningFoodSO.BurningTimerMax)
                {
                    KitchenObj.DestroyKitchObj(GetKitchenObj());
                    KitchenObj.SpawnKitchObj(burningFoodSO.output, this);
                    
                    /* Auto Trigger Network Variable Event */
                    foodState.Value = FoodState.Burned;                
                }            
                break;

            case FoodState.Burned:
                break;
        }
    }

    public override void Interact(Player player)
    {
        // nothing on counter,  Player is carrying something
        if(!HasKitchenObj() && player.HasKitchenObj())
        {
            // can be fried
            if(CanFryingWithInput(player.GetKitchenObj().GetKitchenObjSO()))
            {
                KitchenObj kitchenObj = player.GetKitchenObj();
                kitchenObj.SetKitchenObjParent(this);

                SetFryingRecipeSOServerRpc(
                    KitchenGameMultiplayer.Instance.GettKitchenObjSOIndex(kitchenObj.GetKitchenObjSO())
                );
            }
        }

        else
        {
            // player have a plate
            if(player.HasKitchenObj() && player.GetKitchenObj().TryGetPlate(out PlateKitchenObj plateKitchenObj))
            {
                if(plateKitchenObj.TryAddIngredient(GetKitchenObj().GetKitchenObjSO()))
                {
                    KitchenObj.DestroyKitchObj(GetKitchenObj());
                    
                    /* Only Server Can Change Network Variable */
                    SetStateIdleServerRpc();
                }
            }

            else
            {
                GetKitchenObj().SetKitchenObjParent(player);

                /* Only Server Can Change Network Variable */
                SetStateIdleServerRpc();
            }
        }
    }


    /* Netcode */
    #region Netcode Interact Logic Put Object On Counter
    [ServerRpc(RequireOwnership = false)]
    private void SetFryingRecipeSOServerRpc(int kitchenObjSOIndex)
    {
        fryingTimer.Value = 0;

        /* Auto Trigger Network Variable Event */
        foodState.Value = FoodState.Frying;

        SetFryingRecipeSOClientRpc(kitchenObjSOIndex);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetStateIdleServerRpc()
    {
        /* Auto Trigger Network Variable Event */
        foodState.Value = FoodState.Idle;   
    }

    [ClientRpc]
    private void SetFryingRecipeSOClientRpc(int kitchenObjSOIndex)
    {
        KitchenObjSO kitchenObjSO = KitchenGameMultiplayer.Instance.GettKitchenObjSOFromIndex(kitchenObjSOIndex);
        fryingFoodSO = GetFryingFoodSOWithInput(kitchenObjSO);
    }

    [ClientRpc]
    private void SetBurningRecipeSOClientRpc(int kitchenObjSOIndex)
    {
        KitchenObjSO kitchenObjSO = KitchenGameMultiplayer.Instance.GettKitchenObjSOFromIndex(kitchenObjSOIndex);
        burningFoodSO = GetBurningFoodSOWithInput(kitchenObjSO);
    }
    #endregion


    /* Can Fried */
    private bool CanFryingWithInput(KitchenObjSO inputKitchenObjSO)
    {
        FryingFoodSO fryingFoodSO = GetFryingFoodSOWithInput(inputKitchenObjSO);

        return fryingFoodSO != null;
    }

    private KitchenObjSO GetOutputForInput(KitchenObjSO inputKitchenObjSO)
    {
        FryingFoodSO fryingFoodSO = GetFryingFoodSOWithInput(inputKitchenObjSO);
        
        if(fryingFoodSO != null)
            return fryingFoodSO.output;
        else
            return null;
    }

    private FryingFoodSO GetFryingFoodSOWithInput(KitchenObjSO inputKitchenObjSO)
    {
        foreach(FryingFoodSO fryingFoodSO in fryingFoodSOArray)
        {
            if(fryingFoodSO.input == inputKitchenObjSO)
            {
                return fryingFoodSO;
            }
        }
        return null;        
    }

    private BurningFoodSO GetBurningFoodSOWithInput(KitchenObjSO inputKitchenObjSO)
    {
        foreach(BurningFoodSO burningFoodSO in burningFoodSOArray)
        {
            if(burningFoodSO.input == inputKitchenObjSO)
            {
                return burningFoodSO;
            }
        }
        return null;        
    }

    public bool IsFried()
    {
        return foodState.Value == FoodState.Fried;
    }
}
