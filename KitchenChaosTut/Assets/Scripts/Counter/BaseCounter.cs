using UnityEngine;
using System;

public class BaseCounter : MonoBehaviour, IKitchenObjParent
{
    // drop sound
    public static event EventHandler OnAnyObjDropSound;
    public static void ResetStaticData()
    {
        // clear all the listener
        OnAnyObjDropSound = null;
    }

    [SerializeField] private Transform counterTopPoint;

    private KitchenObj kitchenObj;


    // Containter , ClearCounter
    // Override Interact
    public virtual void Interact(PlayerMovement player)
    {
        Debug.LogError("BaseCounter.Interact");
    }
    public virtual void InteractAlternate(PlayerMovement player)
    {
        //Debug.LogError("BaseCounter.InteractAlternate");
    }    


    /* IKitchenObj Interface */
    public Transform GetKitchenObjectFollowTransform()
    {
        return counterTopPoint;
    }

    public void SetKitchenObj(KitchenObj kitchenObj)
    {
        this.kitchenObj = kitchenObj;

        // 放到桌上時
        if(kitchenObj != null)
        {
            OnAnyObjDropSound?.Invoke(this, EventArgs.Empty);
        }
    }
    public KitchenObj GetKitchenObj()
    {
        return kitchenObj;
    }
    public void ClearKitchObj()
    {
        kitchenObj = null;
    }
    public bool HasKitchenObj()
    {
        return kitchenObj != null;
    }

}
