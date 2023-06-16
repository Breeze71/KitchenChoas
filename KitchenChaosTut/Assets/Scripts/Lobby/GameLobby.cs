using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;

public class GameLobby : MonoBehaviour
{
    public static GameLobby Instance{ get; private set; }

    public event EventHandler OnCreateLobbyStarted;
    public event EventHandler OnCreateLobbyFailed;
    public event EventHandler OnJoinStarted;
    public event EventHandler OnJoinFailed;
    public event EventHandler OnQuickJoinFailed;

    public event EventHandler<OnLobbyListChangedEventArgs> OnLobbyListChanged;
    public class OnLobbyListChangedEventArgs : EventArgs
    {
        public List<Lobby> lobbyList;
    }
    

    private Lobby joinedLobby;
    private float heartbeatTimer;
    private float listLobbyTimer;

    private void Awake() 
    {
        Instance = this;

        DontDestroyOnLoad(gameObject);

        InitializeUnityAuthentication();
    }
    private void Update() 
    {
        HandleHeartbeat();
        HandleListLobby();
    }

    private void HandleHeartbeat()
    {
        if(IsLobbyHost())
        {
            heartbeatTimer -= Time.deltaTime;
            if(heartbeatTimer <= 0f)
            {
                float heartbeatTimerMax = 15f;
                heartbeatTimer = heartbeatTimerMax;

                LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
            }
        }
    }
    private void HandleListLobby()
    {
        if(joinedLobby != null || !AuthenticationService.Instance.IsSignedIn && 
            SceneManager.GetActiveScene().name == Loader.Scene.LobbyScene.ToString())    // Asyn 可能在還沒 signIn 前要求
        {
            return;
        }

        listLobbyTimer -= Time.deltaTime;
        if(listLobbyTimer <= 0)
        {
            float listLobbyTimerMax = 3f;
            listLobbyTimer = listLobbyTimerMax;

            ListLobbies();
        }
    }

    /* Init Authentication, Create, Join Lobby, Delete*/
    #region Lobby Mode
    private async void InitializeUnityAuthentication()
    {
        if(UnityServices.State != ServicesInitializationState.Initialized)
        {
            InitializationOptions initializationOptions = new InitializationOptions();
            initializationOptions.SetProfile(UnityEngine.Random.Range(0, 10000).ToString());

            await UnityServices.InitializeAsync(initializationOptions);

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

    }
    public async void CreateLobby(string _lobbyName, bool _isPrivate)
    {
        OnCreateLobbyStarted?.Invoke(this, EventArgs.Empty);    // Event

        try
        {
            joinedLobby = await LobbyService.Instance.CreateLobbyAsync(_lobbyName, KitchenGameMultiplayer.Max_Player_Amount, new CreateLobbyOptions
            {
                IsPrivate = _isPrivate,
            });

            KitchenGameMultiplayer.Instance.StartHost();
            Loader.LoadSceneNetWork(Loader.Scene.CharacterSelectScene);
        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e);
            OnCreateLobbyFailed?.Invoke(this, EventArgs.Empty); // Event
        }
    }
    public async void QuickJoin()
    {
        OnJoinStarted?.Invoke(this, EventArgs.Empty);

        try
        {
            joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();
            
            KitchenGameMultiplayer.Instance.StartClient();        
        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e);
            OnQuickJoinFailed?.Invoke(this, EventArgs.Empty);
        }
    }
    public async void JoinByCode(string _lobbyCode)
    {
        OnJoinStarted?.Invoke(this, EventArgs.Empty);

        try
        {
            joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(_lobbyCode);

            KitchenGameMultiplayer.Instance.StartClient();     
        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e);
            OnJoinFailed?.Invoke(this, EventArgs.Empty);
        }
    }
    public async void JoinById(string _lobbyId)
    {
        OnJoinStarted?.Invoke(this, EventArgs.Empty);

        try
        {
            joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(_lobbyId);

            KitchenGameMultiplayer.Instance.StartClient();     
        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e);
            OnJoinFailed?.Invoke(this, EventArgs.Empty);
        }
    }
    public async void DeleteLobby()
    {
        if(joinedLobby != null)
        {
            try
            {
                await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);
                joinedLobby = null;
            }
            catch(LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }            
    }
    public async void LeaveLobby()
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);

            joinedLobby = null;
        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
    public async void KickPlayer(string _playerLobbyId)
    {
        if(IsLobbyHost())
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, _playerLobbyId);

                joinedLobby = null;
            }
            catch(LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }
    #endregion
    //
    
    /* List Lobby */
    private async void ListLobbies()
    {
        try
        {
            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions
            {
                Filters = new List<QueryFilter>
                {
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT) // Grather Than 0
                }
            };
            
            QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync(queryLobbiesOptions);
            
            OnLobbyListChanged?.Invoke(this,new OnLobbyListChangedEventArgs
            {
                lobbyList = queryResponse.Results
            });
        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
    //

    /* Get Information */
    public Lobby GetLobby()
    {
        return joinedLobby;
    }
    private bool IsLobbyHost()
    {
        return joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
    }
    //
}
