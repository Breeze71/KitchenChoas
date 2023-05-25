using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance
    {
        get;
        private set;
    }

    public event EventHandler OnGameStateChanged;

    private enum GameState
    {
        waitingToStart,
        countdownToStart,
        gamePlaying,
        gameOver,
    }

    # region variable
    private GameState gameState;
    private float waitingToStartTimer = 1f;
    private float countdownToStartTimer = 3f;
    private float gamePlayingTimer;
    [SerializeField] private float gamePlayingTimerMax = 15f;
    
    #endregion

    private void Awake() 
    {
        Instance = this;
        
        gameState = GameState.waitingToStart;
    }

    private void Update() 
    {
        switch(gameState)
        {
            case GameState.waitingToStart:
                State_waitingToStart();
                OnGameStateChanged?.Invoke(this, EventArgs.Empty);
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

        Debug.Log(gameState);
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

    private void State_waitingToStart()
    {
        waitingToStartTimer -= Time.deltaTime;
        if(waitingToStartTimer < 0f)
        {
            gameState = GameState.countdownToStart;
        }        
    }

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
