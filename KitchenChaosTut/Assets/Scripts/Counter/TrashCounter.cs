
public class TrashCounter : BaseCounter
{
    public override void Interact(PlayerMovement player)
    {
        if(player.HasKitchenObj())
        {
            player.GetKitchenObj().DestroySelf();
        }
    }
}
