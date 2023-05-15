using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
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
        anim.SetBool(isWalking, playermovement.IsWalking());
    }
}
