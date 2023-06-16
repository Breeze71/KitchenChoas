using System;
using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using TMPro;

public class Player : NetworkBehaviour, IKitchenObjParent
{
    // 只能　Static 因為　Intance noreferences
    public static event EventHandler OnAnyPlayerSpawned;
    public static event EventHandler OnAnyPlayerPickedSomethingSound;
    public static void ResetStaticData(){ OnAnyPlayerSpawned = null;}
    public static Player LocalInstance{get; private set;}
    

    # region Event
    
    // 通過繼承 EventArgs 來傳遞數據(自定義的泛型委託)
    public event EventHandler<OnSelectCounterChangedEventArgs> OnSelectCounterChanged;
    public class OnSelectCounterChangedEventArgs : EventArgs
    {
        public BaseCounter seletedCounter;
    }

    public event EventHandler OnPickupSound;

    #endregion

    # region Counter and KitchenObj
    private BaseCounter selectedCounter;
    private KitchenObj kitchenObj;
    [SerializeField] private Transform kitchenObjHoldPoint;
    
    # endregion

    # region Properties
    [Header("Move Properties")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float interactRange;
    [SerializeField] private LayerMask counterLayerMask;
    [SerializeField] private LayerMask collisionLayerMask;

    [Header("Spawn Point")]
    [SerializeField] private List<Vector3> spawnPosList;

    [Header("Visual")]
    [SerializeField] private PlayerVisual playerVisual; // visual
    [SerializeField] private TextMeshPro playerName;

    private Vector3 moveDirection;
    private Vector3 lastMoveDirection;
    private bool isWalking;
    #endregion

    # region OnNetworkSpawn(), Start()
    private void Start() 
    {
        InputManager.Instance.OnInteraction += InputManager_OnInteraction;
        InputManager.Instance.OnInteractAlternate += InputManager_OnInteractAlternateAction;

        // 當前　playerData // 和選擇的同一顏色
        PlayerData playerdata = KitchenGameMultiplayer.Instance.GetPlayerData_From_ClientId(OwnerClientId);
        playerVisual.SetPlayerColor(KitchenGameMultiplayer.Instance.GetPlayerIconColor(playerdata.colorId));

        playerName.text = KitchenGameMultiplayer.Instance.GetPlayerName(OwnerClientId);
    }

    public override void OnNetworkSpawn()
    {
        if(IsOwner)
        {
            LocalInstance = this;
        }

        // Spawn Point
        transform.position = spawnPosList[KitchenGameMultiplayer.Instance.GetPlayerDataIndex_From_ClientId(OwnerClientId)];

        // Static Event 每個玩家加入觸發一次
        OnAnyPlayerSpawned?.Invoke(this, EventArgs.Empty);

        // Handle Client Disconnect
        if(IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
        }
    }
    #endregion

    /* Netcode Handle Disconnect */
    private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
    {
        if(clientId == OwnerClientId && HasKitchenObj())
        {
            kitchenObj.DestroyKitchenObj();
        }
    }

    // Oninteract
    private void InputManager_OnInteraction(object sender, System.EventArgs e)
    {
        // can't pick until gamePlaying
        if(!GameManager.Instance.IsGamePlaying())   return;

        if(selectedCounter != null)
        {
            selectedCounter.Interact(this);
        }
    }
    private void InputManager_OnInteractAlternateAction(object sender, System.EventArgs e)
    {
        if(!GameManager.Instance.IsGamePlaying())   return;

        if(selectedCounter != null)
        {
            selectedCounter.InteractAlternate(this);
        }
    }

    private void Update()
    {
        if(!IsOwner)
        {
            return;
        }

        PlayerMove();
        PlayerInteraction();
    }

    // 持續判斷是否 detect
    private void PlayerInteraction()
    {
        // Direction
        Vector2 inputVector = InputManager.Instance.GetMovementInput();
        moveDirection = new Vector3(inputVector.x, 0f, inputVector.y);
        
        // 沒有鍵入時也能 interact
        if(moveDirection != Vector3.zero)
        {
            lastMoveDirection = moveDirection;
        }

        if(Physics.Raycast(transform.position, lastMoveDirection, out RaycastHit raycastHit, interactRange, counterLayerMask))
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
        Vector2 inputVector = InputManager.Instance.GetMovementInput();
        moveDirection = new Vector3(inputVector.x, 0f, inputVector.y);
        
        // Colision detect
        float playerRadius = 0.7f;
        float moveDistance =  moveSpeed * Time.deltaTime;

        bool canMove = !Physics.BoxCast(
            transform.position, Vector3.one * playerRadius , moveDirection, Quaternion.identity, moveDistance, collisionLayerMask);

        if(!canMove)
        {
            Vector3 moveDirX = new Vector3(moveDirection.x, 0, 0);

            /* moveDirection.x !=0
            // 避免因為上方有障礙物而被判定只能水平走
            // 應該要 輸入水平走 且 上方有障礙物 才水平走
            // 這樣 private Vec3 moveDir 會是之前的數值，isWalking = moveDirection != Vector3.zero 即對著牆跑動畫
            // 且向上走時， canMove = false 
            */
            canMove = (moveDirection.x < -.5f || moveDirection.x > .5f)  && !Physics.BoxCast(
                transform.position, Vector3.one * playerRadius, moveDirX, Quaternion.identity, moveDistance, collisionLayerMask);
            // (moveDirection.x < -.5f || moveDirection.x > .5f) 優化 controller 操作
            
            // move only on x
            if(canMove)
            {
                //Debug.Log("only x");
                moveDirection = moveDirX;
            }
            
            else
            {
                Vector3 moveDirZ = new Vector3(0, 0, moveDirection.z);
                canMove = (moveDirection.z < -.5f || moveDirection.z > .5f) && !Physics.BoxCast(
                    transform.position, Vector3.one * playerRadius , moveDirZ, Quaternion.identity, moveDistance, collisionLayerMask);
                
                // move only on z
                if(canMove)
                {
                    //Debug.Log("only z");
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

        isWalking = moveDirection != Vector3.zero;

        float rotateSpeed = 10f;
        if(isWalking)
        {
            // Slerp 圓弧插值，連同向量角度一起
            transform.forward = Vector3.Slerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);
        }
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
    #region IKitchen Interface
    public Transform GetKitchenObjectFollowTransform()
    {
        return kitchenObjHoldPoint;
    }
    public void SetKitchenObj(KitchenObj kitchenObj)
    {
        this.kitchenObj = kitchenObj;

        // 手上有 kitchenObj
        if(kitchenObj != null)
            OnPickupSound?.Invoke(this, EventArgs.Empty);
            OnAnyPlayerPickedSomethingSound?.Invoke(this, EventArgs.Empty);
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
    public NetworkObject GetNetworkObject()
    {
        return NetworkObject;
    }
    #endregion
}
