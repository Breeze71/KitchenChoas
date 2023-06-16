using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using System;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private LobbyCreateUI lobbyCreateUI;
    
    [Header("Button")]
    [SerializeField] private Button mainMenuBtn;
    [SerializeField] private Button createLobby;
    [SerializeField] private Button quickJoin;
    [SerializeField] private Button joinCodeBtn;

    [Header("Input Field")]
    [SerializeField] private TMP_InputField playerNameInputField;
    [SerializeField] private TMP_InputField joinCodeInputField;

    // Lobby List
    [Header("Lobby Container")]
    [SerializeField] private Transform lobbyContainer;
    [SerializeField] private Transform lobbyTemplate;
    

    private void Awake() 
    {
        mainMenuBtn.onClick.AddListener(() =>
        {
            GameLobby.Instance.LeaveLobby();
            Loader.LoadScene(Loader.Scene.MainMenu);
        });
        createLobby.onClick.AddListener(() =>
        {
            lobbyCreateUI.Show();
        });
        quickJoin.onClick.AddListener(() =>
        {
            GameLobby.Instance.QuickJoin();
        });

        joinCodeBtn.onClick.AddListener(() =>
        {
            GameLobby.Instance.JoinByCode(joinCodeInputField.text);
        });

        lobbyTemplate.gameObject.SetActive(false);
    }

    private void Start() 
    {
        playerNameInputField.text = KitchenGameMultiplayer.Instance.GetPlayerName_TMP();

        /* InputField Event */
        playerNameInputField.onValueChanged.AddListener((string _newText) =>
        {
            KitchenGameMultiplayer.Instance.SetPlayerName(_newText);
        });

        UpdateLobbyList(new List<Lobby>());
        GameLobby.Instance.OnLobbyListChanged += GameLobby_OnLobbyListChanged;
    }

    /* OnLobbyListChanged */
    private void GameLobby_OnLobbyListChanged(object sender, GameLobby.OnLobbyListChangedEventArgs e)
    {
        UpdateLobbyList(e.lobbyList);
    }
    //
    private void OnDestroy() 
    {
        GameLobby.Instance.OnLobbyListChanged -= GameLobby_OnLobbyListChanged;
    }
    
    private void UpdateLobbyList(List<Lobby> _lobbyList)
    {
        foreach(Transform child in lobbyContainer)
        {
            if(child == lobbyTemplate) continue;

            Destroy(child.gameObject);
        }

        foreach(Lobby lobby in _lobbyList)
        {
            Transform lobbyTransform = Instantiate(lobbyTemplate, lobbyContainer);
            lobbyTransform.gameObject.SetActive(true);

            lobbyTransform.GetComponent<LobbyListSingleUI>().SetLobby(lobby);
        }
    }
}
