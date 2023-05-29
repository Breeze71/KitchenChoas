using UnityEngine;
using Unity.Netcode;

public class PlayerAnimator : NetworkBehaviour
{
    #region Anim Parameters
    private const string isWalking = "IsWalking";

    #endregion


    private Animator anim;
    [SerializeField] private PlayerMovement playermovement;

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
        
        anim.SetBool(isWalking, playermovement.IsWalking());
    }
}
