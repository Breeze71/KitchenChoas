using UnityEngine;

public class ClearCounter : BaseCounter
{
    [SerializeField] private KitchenObjSO kitchenObjSO;

    /* Extend BaseCounter*/
    public override void Interact(PlayerMovement player)
    {   
        // 桌上沒東西
        if(!HasKitchenObj())
        {
            // Player is carrying something
            if(player.HasKitchenObj())
            {
                player.GetKitchenObj().SetKitchenObjParent(this);
            }
        }

        // 桌上有東西
        else
        {
            // player is holding plate
            if(player.HasKitchenObj())
            {
                if(player.GetKitchenObj().TryGetPlate(out PlateKitchenObj plateKitchenObj))
                {
                    if(plateKitchenObj.TryAddIngredient(GetKitchenObj().GetKitchenObjSO()))
                    {
                        GetKitchenObj().DestroySelf();
                    }
                }
                else
                {
                    // player is holding 食材
                    if(GetKitchenObj().TryGetPlate(out plateKitchenObj))
                    {
                        if(plateKitchenObj.TryAddIngredient(player.GetKitchenObj().GetKitchenObjSO()))
                        {
                            player.GetKitchenObj().DestroySelf();
                        }
                    }
                }
            }

            // hold nothing
            else
            {
                GetKitchenObj().SetKitchenObjParent(player);
            }
        }
    }

}
