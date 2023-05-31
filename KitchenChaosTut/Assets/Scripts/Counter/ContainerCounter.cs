using UnityEngine;
using System;

public class ContainerCounter : BaseCounter
{   
    public event EventHandler OnPlayerGrabbedObj;

    [SerializeField] private KitchenObjSO kitchenObjSO;

    /* Extend BaseCounter*/
    public override void Interact(PlayerMovement player)
    {   
        // 手上沒東西才能拿
        if(!player.HasKitchenObj())
        {
            KitchenObj.SpawnKitchObj(kitchenObjSO, player);

            OnPlayerGrabbedObj?.Invoke(this, EventArgs.Empty);
        }
    }

}
