using System;
using Unity.Netcode;


public class TrashCounter : BaseCounter
{
    public static event EventHandler OnAnyObjTrashedSound;
    new public static void ResetStaticData()
    {
        // clear all the listener of static event
        OnAnyObjTrashedSound = null;
    }

    public override void Interact(PlayerMovement player)
    {
        if(player.HasKitchenObj())
        {
            KitchenObj.DestroyKitchObj(player.GetKitchenObj());

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
        OnAnyObjTrashedSound?.Invoke(this, EventArgs.Empty);  // Destory Sound
    }
}
