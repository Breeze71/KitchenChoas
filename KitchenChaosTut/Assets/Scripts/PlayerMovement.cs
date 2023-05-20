using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour, IKitchenObjParent
{
    public static PlayerMovement Instance{get; private set;}
    [SerializeField] private InputManager inputManager;

    # region OnInteract Event
    
    public event EventHandler<OnSelectCounterChangedEventArgs> OnSelectCounterChanged;

    // 通過繼承 EventArgs 來傳遞數據(自定義的泛型委託)
    public class OnSelectCounterChangedEventArgs : EventArgs
    {
        public BaseCounter seletedCounter;
    }

    #endregion

    # region Counter and KitchenObj
    private BaseCounter selectedCounter;
    private KitchenObj kitchenObj;
    [SerializeField] private Transform kitchenObjHoldPoint;
    
    # endregion

    # region Move Properties
    [SerializeField] private float moveSpeed;
    [SerializeField] private LayerMask counterLayer;
    [SerializeField] private float interactRange;

    private Vector3 moveDirection;
    private Vector3 lastMoveDirection;
    private bool isWalking;
    #endregion

    private void Awake() 
    {
        if(Instance != null)
            Debug.LogError("more than one player");
        
        Instance = this;
    }

    private void Start() 
    {
        // InputManager_OnInteraction 訂閱 inputManager.OnInteraction
        inputManager.OnInteraction += InputManager_OnInteraction;
        inputManager.OnInteractAlternate += InputManager_OnInteractAlternateAction;
    }

    // Oninteract
    private void InputManager_OnInteraction(object sender, System.EventArgs e)
    {
        if(selectedCounter != null)
        {
            selectedCounter.Interact(this);
        }
    }
    private void InputManager_OnInteractAlternateAction(object sender, System.EventArgs e)
    {
        if(selectedCounter != null)
        {
            selectedCounter.InteractAlternate(this);
        }
    }

    private void Update()
    {
        PlayerMove();
        PlayerInteraction();
    }

    // 持續判斷是否 detect
    private void PlayerInteraction()
    {
        // Direction
        Vector2 inputVector = inputManager.GetMovementInput();
        moveDirection = new Vector3(inputVector.x, 0f, inputVector.y);
        
        // 沒有鍵入時也能 interact
        if(moveDirection != Vector3.zero)
        {
            lastMoveDirection = moveDirection;
        }

        if(Physics.Raycast(transform.position, lastMoveDirection, out RaycastHit raycastHit, interactRange, counterLayer))
        {   
            // has clearCounter
            if(raycastHit.transform.TryGetComponent(out BaseCounter baseCounter))
            {
                // 當前的 c
                if(baseCounter != selectedCounter)
                {
                    SetSeletedCounter(baseCounter);
                    //selectedCounter = clearCounter;
                }
            }
            else
            {
                SetSeletedCounter(null);
                //selectedCounter = null;
            }
        }
        // 取消選取
        else
        {
            SetSeletedCounter(null);
            //selectedCounter = null;
        }
    }

    private void PlayerMove()
    {
        // Direction
        Vector2 inputVector = inputManager.GetMovementInput();
        moveDirection = new Vector3(inputVector.x, 0f, inputVector.y);
        
        // Colision detect
        float detectLength = 0.7f;
        float playerHeight = 2f;
        float moveDistance =  moveSpeed * Time.deltaTime;

        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, detectLength , moveDirection, moveDistance);

        if(!canMove)
        {
            Vector3 moveDirX = new Vector3(moveDirection.x, 0, 0);

            /* moveDirection.x !=0
            // 避免因為上方有障礙物而被判定只能水平走
            // 應該要 輸入水平走 且 上方有障礙物 才水平走
            // 這樣 private Vec3 moveDir 會是之前的數值，isWalking = moveDirection != Vector3.zero 即對著牆跑動畫
            // 且向上走時， canMove = false 
            */
            canMove = moveDirection.x !=0  && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, detectLength , moveDirX, moveDistance);
        
            // move only on x
            if(canMove)
            {
                Debug.Log("only x");
                moveDirection = moveDirX;
            }
            
            else
            {
                Vector3 moveDirZ = new Vector3(0, 0, moveDirection.z);
                canMove = moveDirection.z != 0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, detectLength , moveDirZ, moveDistance);
                
                // move only on z
                if(canMove)
                {
                    Debug.Log("only z");
                    moveDirection = moveDirZ;
                }
            }
            
        }
        
        // move
        if(canMove)
        {
            //Debug.Log("canMove");
            transform.position += moveDirection * moveDistance;
        }

        // isWalking
        isWalking = moveDirection != Vector3.zero;

        // Slerp 圓弧插值，連同向量角度一起
        float rotateSpeed = 10f;
        transform.forward = Vector3.Slerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);
    }

    public bool IsWalking()
    {
        return isWalking;
    }

    private void SetSeletedCounter(BaseCounter selectedCounter)
    {
        this.selectedCounter = selectedCounter;

        OnSelectCounterChanged?.Invoke(this, new OnSelectCounterChangedEventArgs{
            seletedCounter = selectedCounter
        });
    }


    /* IKitchenObj Interface */
    public Transform GetKitchenObjectFollowTransform()
    {
        return kitchenObjHoldPoint;
    }
    public void SetKitchenObj(KitchenObj kitchenObj)
    {
        this.kitchenObj = kitchenObj;
    }
    public KitchenObj GetKitchenObj()
    {
        return kitchenObj;
    }
    public void ClearKitchObj()
    {
        kitchenObj = null;
    }
    public bool HasKitchenObj()
    {
        return kitchenObj != null;
    }
}
