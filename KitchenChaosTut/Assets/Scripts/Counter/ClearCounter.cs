using UnityEngine;

public class ClearCounter : BaseCounter
{
    [SerializeField] private KitchenObjSO kitchenObjSO;

    /* Extend BaseCounter*/
    public override void Interact(Player player)
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
            // player is holding sth
            if(player.HasKitchenObj())
            {
                // player is holding plate
                if(player.GetKitchenObj().TryGetPlate(out PlateKitchenObj plateKitchenObj))
                {
                    if(plateKitchenObj.TryAddIngredient(GetKitchenObj().GetKitchenObjSO()))
                    {
                        KitchenObj.DestroyKitchObj(GetKitchenObj());
                    }
                }

                // player is holding KitchenObj
                else
                {
                    // Counter have plate
                    if(GetKitchenObj().TryGetPlate(out plateKitchenObj))
                    {
                        if(plateKitchenObj.TryAddIngredient(player.GetKitchenObj().GetKitchenObjSO()))
                        {
                            KitchenObj.DestroyKitchObj(player.GetKitchenObj());
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
