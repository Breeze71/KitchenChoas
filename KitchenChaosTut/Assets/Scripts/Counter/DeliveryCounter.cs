using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryCounter : BaseCounter
{
    public static DeliveryCounter Instance
    {
        get;
        private set;
    }
    private void Awake() {
        Instance = this;
    }

    public override void Interact(Player player)
    {
        // only access plates
        if(player.HasKitchenObj() && player.GetKitchenObj().TryGetPlate(out PlateKitchenObj plateKitchenObj))
        {
            // judge
            DeliveryManager.Instance.DeliverRecipe(plateKitchenObj);

            KitchenObj.DestroyKitchObj(player.GetKitchenObj());
        }
    }
}
