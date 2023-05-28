using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StoveCounter : BaseCounter, IHasProgress
{
    /* Interface IHasProgress */
    public event EventHandler<IHasProgress.OnProgressBarChangeEventArgs> OnProgressBarChange;

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
    private FoodState foodState;

    # region FoodSO
    [SerializeField] private FryingFoodSO[] fryingFoodSOArray;
    [SerializeField] private BurningFoodSO[] burningFoodSOArray;
    private float fryingTimer;
    private float burningTimer;
    private FryingFoodSO fryingFoodSO;
    private BurningFoodSO burningFoodSO;

    # endregion
    
    private void Start() 
    {
        foodState = FoodState.Idle;
    }

    private void Update() 
    {
        if(!HasKitchenObj())
            return;

        switch(foodState)
        {
            case FoodState.Idle:
                break;

            case FoodState.Frying:

                // start progress
                fryingTimer += Time.deltaTime;
                OnProgressBarChange?.Invoke(this, new IHasProgress.OnProgressBarChangeEventArgs
                {
                    progressBarNormalized = fryingTimer / fryingFoodSO.fringTimerMax
                });

                if(fryingTimer > fryingFoodSO.fringTimerMax)
                {
                    GetKitchenObj().DestroySelf();
                    KitchenObj.SpawnKitchObj(fryingFoodSO.output, this);
                    //Debug.Log("Fried");

                    burningTimer = 0f;
                    foodState = FoodState.Fried;
                    burningFoodSO = GetBurningFoodSOWithInput(GetKitchenObj().GetKitchenObjSO());
                    
                    // 訂閱更新 state
                    OnFoodStateChanged?.Invoke(this, new OnFoodStateChangedEventArgs
                    {
                        foodState = foodState
                    });
                }
                break;

            case FoodState.Fried:

                burningTimer += Time.deltaTime;
                OnProgressBarChange?.Invoke(this, new IHasProgress.OnProgressBarChangeEventArgs
                {
                    progressBarNormalized = burningTimer / burningFoodSO.BurningTimerMax
                });

                if(burningTimer > burningFoodSO.BurningTimerMax)
                {
                    GetKitchenObj().DestroySelf();
                    KitchenObj.SpawnKitchObj(burningFoodSO.output, this);
                    
                    Debug.Log("Burned");
                    foodState = FoodState.Burned;

                    // 訂閱更新 state
                    OnFoodStateChanged?.Invoke(this, new OnFoodStateChangedEventArgs
                    {
                        foodState = foodState
                    });
                    OnProgressBarChange?.Invoke(this, new IHasProgress.OnProgressBarChangeEventArgs
                    {
                        // 燒焦
                        progressBarNormalized = 0f
                    });                    
                }            
                break;

            case FoodState.Burned:
                break;
        }
    }

    public override void Interact(PlayerMovement player)
    {
        // nothing on counter,  Player is carrying something
        if(!HasKitchenObj() && player.HasKitchenObj())
        {
            // can be fried
            if(CanFryingWithInput(player.GetKitchenObj().GetKitchenObjSO()))
            {
                player.GetKitchenObj().SetKitchenObjParent(this);

                // assign fryingFoodSO
                fryingFoodSO = GetFryingFoodSOWithInput(GetKitchenObj().GetKitchenObjSO());

                // event
                foodState = FoodState.Frying;
                fryingTimer = 0;
                OnFoodStateChanged?.Invoke(this, new OnFoodStateChangedEventArgs
                {
                    foodState = foodState
                });
                OnProgressBarChange?.Invoke(this, new IHasProgress.OnProgressBarChangeEventArgs
                {
                    progressBarNormalized = fryingTimer / fryingFoodSO.fringTimerMax
                });
            }
        }

        else
        {
            // player have a plate
            if(player.HasKitchenObj() && player.GetKitchenObj().TryGetPlate(out PlateKitchenObj plateKitchenObj))
            {
                if(plateKitchenObj.TryAddIngredient(GetKitchenObj().GetKitchenObjSO()))
                {
                    GetKitchenObj().DestroySelf();

                    foodState = FoodState.Idle;
                    OnFoodStateChanged?.Invoke(this, new OnFoodStateChangedEventArgs
                    {
                        foodState = foodState
                    });
                    OnProgressBarChange?.Invoke(this, new IHasProgress.OnProgressBarChangeEventArgs
                    {
                        progressBarNormalized = 0
                    });   
                }
            }

            else
            {
                GetKitchenObj().SetKitchenObjParent(player);

                foodState = FoodState.Idle;
                OnFoodStateChanged?.Invoke(this, new OnFoodStateChangedEventArgs
                {
                    foodState = foodState
                });
                
                // 拿走後
                OnProgressBarChange?.Invoke(this, new IHasProgress.OnProgressBarChangeEventArgs
                {
                    progressBarNormalized = 0
                });             
            }
        }
    }

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
        return foodState == FoodState.Fried;
    }
}
