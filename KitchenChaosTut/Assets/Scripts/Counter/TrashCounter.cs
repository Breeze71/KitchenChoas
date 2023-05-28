using System;

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
            player.GetKitchenObj().DestroySelf();

            OnAnyObjTrashedSound?.Invoke(this, EventArgs.Empty);
        }
    }
}
