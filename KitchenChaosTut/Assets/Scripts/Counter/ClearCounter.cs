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
            if(!player.HasKitchenObj())
            {
                GetKitchenObj().SetKitchenObjParent(player);
            }
        }
    }

}
