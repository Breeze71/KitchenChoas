using UnityEngine;
using System;
using Unity.Netcode;

public class ContainerCounter : BaseCounter
{   
    public event EventHandler OnPlayerGrabbedObj;

    [SerializeField] private KitchenObjSO kitchenObjSO;

    /* Extend BaseCounter*/
    public override void Interact(Player player)
    {   
        // 手上沒東西才能拿
        if(!player.HasKitchenObj())
        {
            KitchenObj.SpawnKitchObj(kitchenObjSO, player);

            InteractLogicServerRpc();
        }
    }

    /* Netcode */
    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicServerRpc()
    {
        InteractLogicClientRpc();
    }
    [ClientRpc]
    private void InteractLogicClientRpc()
    {
        OnPlayerGrabbedObj?.Invoke(this, EventArgs.Empty);  // Grab Anim
    }
}
