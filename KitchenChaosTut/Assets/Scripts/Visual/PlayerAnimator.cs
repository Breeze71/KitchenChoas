using UnityEngine;
using Unity.Netcode;

public class PlayerAnimator : NetworkBehaviour
{
    #region Anim Parameters
    private const string isWalking = "IsWalking";

    #endregion


    private Animator anim;
    [SerializeField] private Player player;

    private void Awake() 
    {
        anim = GetComponent<Animator>();
        anim.SetBool(isWalking, false);    
    }

    private void Update()
    {
        if(!IsOwner)
        {
            return;
        }
        
        anim.SetBool(isWalking, player.IsWalking());
    }
}
