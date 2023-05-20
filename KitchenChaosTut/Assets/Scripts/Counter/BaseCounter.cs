using UnityEngine;

public class BaseCounter : MonoBehaviour, IKitchenObjParent
{
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
