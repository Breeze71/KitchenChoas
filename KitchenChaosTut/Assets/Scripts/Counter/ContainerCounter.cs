using System.Collections;
using System.Collections.Generic;
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
            // Interact 時觸發
            OnPlayerGrabbedObj?.Invoke(this, EventArgs.Empty);
        }
    }

}
