using UnityEngine;
using System;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private const string PlayerPrefsBindings = "InputBindings";

    public static InputManager Instance
    {
        get;
        private set;
    }

    public enum Binding
    {
        moveUp,
        moveDown,
        moveRight,
        moveLeft,
        Interact,
        Cut,
        Pause,
        gamePad_Interact,
        gamePad_Cut,
        gamePad_Pause,
    }

    private PlayerInputAction playerInputAction;

    public event EventHandler OnInteraction;
    public event EventHandler OnInteractAlternate;
    public event EventHandler OnPauseAction;
    public event EventHandler OnRebindKey; 

    private void Awake() 
    {
        Instance = this;

        // 回到主介面再回來會 new again
        playerInputAction = new PlayerInputAction();

        // 讀存檔
        if(PlayerPrefs.HasKey(PlayerPrefsBindings))
        {
            playerInputAction.LoadBindingOverridesFromJson(PlayerPrefs.GetString(PlayerPrefsBindings));
        }

        playerInputAction.Player.Enable();

        // InputAction
        playerInputAction.Player.Interact.performed += Interact_Performed;
        playerInputAction.Player.InteractAlternate.performed += InteractAlternate_Performed;
        playerInputAction.Player.Pause.performed += Pause_Performed;
    }

    private void OnDestroy() 
    {
        playerInputAction.Player.Interact.performed -= Interact_Performed;
        playerInputAction.Player.InteractAlternate.performed -= InteractAlternate_Performed;
        playerInputAction.Player.Pause.performed -= Pause_Performed;
        
        // free the new PlayerInputAction(); memory
        playerInputAction.Dispose();       
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

    private void Pause_Performed(InputAction.CallbackContext context)
    {
        OnPauseAction?.Invoke(this, EventArgs.Empty);
    }

    /* Binding */
    public string GetBindingText(Binding binding)
    {
        switch(binding)
        {
            default:
            // move
            case Binding.moveUp:
                return playerInputAction.Player.Move.bindings[1].ToDisplayString();

            case Binding.moveDown:
                return playerInputAction.Player.Move.bindings[2].ToDisplayString();
 
            case Binding.moveLeft:
                return playerInputAction.Player.Move.bindings[3].ToDisplayString();

            case Binding.moveRight:
                return playerInputAction.Player.Move.bindings[4].ToDisplayString();

            // interact
            case Binding.Interact:
                return playerInputAction.Player.Interact.bindings[0].ToDisplayString();

            case Binding.Cut:
                return playerInputAction.Player.InteractAlternate.bindings[0].ToDisplayString();
 
            case Binding.Pause:
                return playerInputAction.Player.Pause.bindings[0].ToDisplayString();

            // Controller
            case Binding.gamePad_Interact:
                return playerInputAction.Player.Interact.bindings[1].ToDisplayString();
            case Binding.gamePad_Cut:
                return playerInputAction.Player.InteractAlternate.bindings[1].ToDisplayString();
            case Binding.gamePad_Pause:
                return playerInputAction.Player.Pause.bindings[1].ToDisplayString();
        }
    }

    public void RebindKey(Binding binding, Action onActionRebound)
    {
        // 先停用再修改
        playerInputAction.Player.Disable();

        InputAction inputAction;
        int bindingIndex;

        switch(binding)
        {
            default:
            case Binding.moveUp:
                inputAction = playerInputAction.Player.Move;
                bindingIndex = 1;
                break;
            case Binding.moveDown:
                inputAction = playerInputAction.Player.Move;
                bindingIndex = 2;                
                break;
            case Binding.moveLeft:
                inputAction = playerInputAction.Player.Move;
                bindingIndex = 3;               
                break;
            case Binding.moveRight:
                inputAction = playerInputAction.Player.Move;                
                bindingIndex = 4;
                break;
            // interact
            case Binding.Interact:
                inputAction = playerInputAction.Player.Interact;
                bindingIndex = 0;
                break;
            case Binding.Cut:
                inputAction = playerInputAction.Player.InteractAlternate;
                bindingIndex = 0;
                break;
            case Binding.Pause:
                inputAction = playerInputAction.Player.Pause;
                bindingIndex = 0;
                break;

            // Controller
            case Binding.gamePad_Interact:
                inputAction = playerInputAction.Player.Interact;
                bindingIndex = 1;
                break;
            case Binding.gamePad_Cut:
                inputAction = playerInputAction.Player.InteractAlternate;
                bindingIndex = 1;
                break;
            case Binding.gamePad_Pause:
                inputAction = playerInputAction.Player.Pause;
                bindingIndex = 1;
                break;
        }

        // Rebind
        inputAction.PerformInteractiveRebinding(bindingIndex).OnComplete(callback => 
        {
            callback.Dispose();
            playerInputAction.Player.Enable();

            /* delegate 可以用變數呼叫函數 */
            onActionRebound();

            // 儲存玩家更改的按鍵偏好
            // playerInputAction.SaveBindingOverridesAsJson();
            PlayerPrefs.SetString(PlayerPrefsBindings, playerInputAction.SaveBindingOverridesAsJson());
            PlayerPrefs.Save();

            OnRebindKey?.Invoke(this, EventArgs.Empty); // tutUI
        }).Start();
    }
}
