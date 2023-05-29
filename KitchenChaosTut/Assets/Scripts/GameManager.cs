using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance
    {
        get;
        private set;
    }

    #region event
    public event EventHandler OnGameStateChanged;
    public event EventHandler OnGamePause;
    public event EventHandler OnGameResume;
    #endregion
    private enum GameState
    {
        waitingToStart,
        countdownToStart,
        gamePlaying,
        gameOver,
    }

    # region variable
    private GameState gameState;
    private float countdownToStartTimer = 1f;
    private float gamePlayingTimer;
    private float gamePlayingTimerMax = 300f;
    
    #endregion
    private bool isPaused = false;

    private void Awake() 
    {
        Instance = this;
        
        gameState = GameState.waitingToStart;
    }

    private void Start() 
    {
        InputManager.Instance.OnPauseAction += InputManager_OnPauseAction;
        InputManager.Instance.OnInteraction += InputManager_OnInteraction;

        /*   Debug Trigger Automatic   */
        gameState = GameState.countdownToStart;
        OnGameStateChanged?.Invoke(this, EventArgs.Empty);
    }

    // Tut UI Change
    private void InputManager_OnInteraction(object sender, EventArgs e)
    {
        if(gameState == GameState.waitingToStart)
        {
            gameState = GameState.countdownToStart;
            OnGameStateChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private void InputManager_OnPauseAction(object sender, EventArgs e)
    {
        Pause_Resume();
    }

    private void Update() 
    {
        switch(gameState)
        {
            case GameState.waitingToStart:
                break;
            
            case GameState.countdownToStart:
                State_countDownToStart();
                OnGameStateChanged?.Invoke(this, EventArgs.Empty);
                break;

            case GameState.gamePlaying:
                State_gamePlaying();
                OnGameStateChanged?.Invoke(this, EventArgs.Empty);
                break;

            case GameState.gameOver:
                break;
        }
    }

    // return gameState
    public bool IsGamePlaying()
    {
        return gameState == GameState.gamePlaying;
    }
    public bool IsCountdownToStartActive()
    {
        return gameState == GameState.countdownToStart;
    }
    public float GetCountdownToStartTimer()
    {
        return countdownToStartTimer;
    }
    public bool isGameOver()
    {
        return gameState == GameState.gameOver;
    }
    public float GetGamePlayingTimerNormalized()
    {
        return 1 - (gamePlayingTimer / gamePlayingTimerMax);
    }

    public void Pause_Resume()
    {
        isPaused = !isPaused;
        if(isPaused)
        {
            Time.timeScale = 0f;
            OnGamePause?.Invoke(this, EventArgs.Empty);
        }

        else
        {
            Time.timeScale = 1f;
            OnGameResume?.Invoke(this, EventArgs.Empty);
        }
    }

    // gameState Logic
    private void State_countDownToStart()
    {
        gamePlayingTimer = gamePlayingTimerMax;

        countdownToStartTimer -= Time.deltaTime;
        if(countdownToStartTimer < 0f)
        {
            gameState = GameState.gamePlaying;
        }        
    }

    private void State_gamePlaying()
    {
        gamePlayingTimer -= Time.deltaTime;
        if(gamePlayingTimer < 0f)
        {
            gameState = GameState.gameOver;
        }        
    }
}
