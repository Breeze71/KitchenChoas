using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance
    {
        get;
        private set;
    }

    #region event
    public event EventHandler OnGameStateChanged;
    public event EventHandler OnLocalGamePause;
    public event EventHandler OnLocalGameResume;

    public event EventHandler OnMutlityPlayerGamePaused;
    public event EventHandler OnMutlityPlayerGameResumed;
    #endregion
    public event EventHandler OnLocalPlayerReadyChanged;

    private enum GameState
    {
        waitingToStart,
        countdownToStart,
        gamePlaying,
        gameOver,
    }
    private NetworkVariable<GameState> gameState = new NetworkVariable<GameState>(GameState.waitingToStart);
    [SerializeField] private Transform palyerPrefabs;
    # region variable
    private NetworkVariable<float> countdownToStartTimer =  new NetworkVariable<float>(3f);
    private NetworkVariable<float> gamePlayingTimer = new NetworkVariable<float>(0f);
    [SerializeField] private float gamePlayingTimerMax = 90f;
    private bool isLocalPlayerReady = false;
    private bool isLocalPaused = false;
    private NetworkVariable<bool> isGamePaused = new NetworkVariable<bool>(false);
    private Dictionary<ulong, bool> palyerReadyDictionary;
    private Dictionary<ulong, bool> palyerPausedDictionary;

    private bool TestGamePause;
    #endregion
    private void Awake() 
    {
        Instance = this;
        palyerReadyDictionary = new Dictionary<ulong, bool>();
        palyerPausedDictionary = new Dictionary<ulong, bool>();
    }

    /* OnNetworkSpawn TriggerEvent */
    public override void  OnNetworkSpawn()
    {
        gameState.OnValueChanged += gameState_OnValueChanged;
        isGamePaused.OnValueChanged += isGamePaused_OnValueChanged;

        if(IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += NetworkManager_OnLoadEventCompleted;
        }
    }

    private void NetworkManager_OnLoadEventCompleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        foreach(ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            Transform palyerTransform = Instantiate(palyerPrefabs);
            palyerTransform.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
        }
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
    {
        TestGamePause = true;
    }

    private void isGamePaused_OnValueChanged(bool previousValue, bool newValue)
    {
        if(isGamePaused.Value)
        {
            Time.timeScale = 0f;
            OnMutlityPlayerGamePaused?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            Time.timeScale = 1f;
            OnMutlityPlayerGameResumed?.Invoke(this, EventArgs.Empty);
        }
    }

    private void gameState_OnValueChanged(GameState previousValue, GameState newValue)
    {
        OnGameStateChanged?.Invoke(this, EventArgs.Empty);
    }

    private void Start() 
    {
        InputManager.Instance.OnPauseAction += InputManager_OnPauseAction;
        InputManager.Instance.OnInteraction += InputManager_OnInteraction;
    }

    /* Change UI */
    private void InputManager_OnInteraction(object sender, EventArgs e)
    {
        if(gameState.Value == GameState.waitingToStart)
        {
            isLocalPlayerReady = true;

            OnLocalPlayerReadyChanged?.Invoke(this, EventArgs.Empty);

            SetPlayerReadyServerRpc();

        }
    }

    /* Netcode isReady? */
    #region Netcode Ready
    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        palyerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;

        bool isAllClientReady = true;
        foreach(ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            // if does not contain this player or not ready
            if(!palyerReadyDictionary.ContainsKey(clientId) || !palyerReadyDictionary[clientId])
            {
                isAllClientReady = false;
                return;
            }
        }

        if(isAllClientReady)
        {
            gameState.Value = GameState.countdownToStart;
        }
    }
    #endregion

    private void InputManager_OnPauseAction(object sender, EventArgs e)
    {
        Pause_Resume();
    }


    private void Update() 
    {
        if(!IsServer)
        {
            return;
        }

        switch(gameState.Value)
        {
            case GameState.waitingToStart:
                break;
            
            case GameState.countdownToStart:
                State_countDownToStart();
                break;

            case GameState.gamePlaying:
                State_gamePlaying();
                break;

            case GameState.gameOver:
                break;
        }
    }

    private void LateUpdate() 
    {
        if(TestGamePause)
        {
            TestGamePause = false;
            TestIsAnyPlayerPause();
        }
    }

    /* Return GameState And Value */
    #region Return GameState And Value
    public bool IsGamePlaying()
    {
        return gameState.Value == GameState.gamePlaying;
    }
    public bool IsWaitingToStart()
    {
        return gameState.Value == GameState.waitingToStart;
    }
    public bool IsCountdownToStartActive()
    {
        return gameState.Value == GameState.countdownToStart;
    }
    public float GetCountdownToStartTimer()
    {
        return countdownToStartTimer.Value;
    }
    public bool IsGameOver()
    {
        return gameState.Value == GameState.gameOver;
    }
    public float GetGamePlayingTimerNormalized()
    {
        return 1 - (gamePlayingTimer.Value / gamePlayingTimerMax);
    }
    #endregion
    
    // Sync
    public bool IsLocalPlayerReady()
    {
        return isLocalPlayerReady;
    }

    public void Pause_Resume()
    {
        isLocalPaused = !isLocalPaused;
        if(isLocalPaused)
        {
            PauseGameServerRpc();

            OnLocalGamePause?.Invoke(this, EventArgs.Empty);
        }

        else
        {
            ResumeGameServerRpc();

            OnLocalGameResume?.Invoke(this, EventArgs.Empty);
        }
    }

    /* Netcode Pause Sync */
    #region Netcode Pause
    [ServerRpc(RequireOwnership = false)]
    private void PauseGameServerRpc(ServerRpcParams serverRpcParams = default)
    {
        // 收到的該 Client 為 true
        palyerPausedDictionary[serverRpcParams.Receive.SenderClientId] = true;

        TestIsAnyPlayerPause();
    }
    [ServerRpc(RequireOwnership = false)]
    private void ResumeGameServerRpc(ServerRpcParams serverRpcParams = default)
    {
        palyerPausedDictionary[serverRpcParams.Receive.SenderClientId] = false;

        TestIsAnyPlayerPause();
    }
    // Client 可讀 variable 不能改
    private void TestIsAnyPlayerPause()
    {
        foreach(ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if(palyerPausedDictionary.ContainsKey(clientId) && palyerPausedDictionary[clientId])
            {
                isGamePaused.Value = true;  //Trigger

                return;
            }
        }

        // all player resume
        isGamePaused.Value = false; //Trigger
    }
    #endregion

    /* gameState Logic */
    private void State_countDownToStart()
    {
        gamePlayingTimer.Value = gamePlayingTimerMax;

        countdownToStartTimer.Value -= Time.deltaTime;
        if(countdownToStartTimer.Value < 0f)
        {
            gameState.Value = GameState.gamePlaying;    // Trigger OnValueChange
        }        
    }
    private void State_gamePlaying()
    {
        gamePlayingTimer.Value -= Time.deltaTime;
        if(gamePlayingTimer.Value < 0f)
        {
            gameState.Value = GameState.gameOver;    // Trigger OnValueChange
        }        
    }
}
