using System;

public class TrashCounter : BaseCounter
{
    public static event EventHandler OnAnyObjTrashedSound;

    public override void Interact(PlayerMovement player)
    {
        if(player.HasKitchenObj())
        {
            player.GetKitchenObj().DestroySelf();

            OnAnyObjTrashedSound?.Invoke(this, EventArgs.Empty);
        }
    }
}
