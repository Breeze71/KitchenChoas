using UnityEngine;
using System;

public class InputManager : MonoBehaviour
{
    private PlayerInputAction playerInputAction;

    public event EventHandler OnInteraction;
    public event EventHandler OnInteractAlternate;

    private void Awake() 
    {
        playerInputAction = new PlayerInputAction();
        playerInputAction.Player.Enable();

        // Interact_Performed 鍵訂閱 Interact
        playerInputAction.Player.Interact.performed += Interact_Performed;
        playerInputAction.Player.InteractAlternate.performed += InteractAlternate_Performed;
    }

    public Vector2 GetMovementInput()
    {
        Vector2 inputVector = playerInputAction.Player.Move.ReadValue<Vector2>();
        inputVector = inputVector.normalized;

        return inputVector;
    }
    
    // 不傳遞數據，只純訂閱
    private void Interact_Performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        //if(OnInteraction != null)
            //OnInteraction(this, EventArgs.Empty);

        OnInteraction?.Invoke(this, EventArgs.Empty);
    }

    private void InteractAlternate_Performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnInteractAlternate?.Invoke(this, EventArgs.Empty);
    }
}
