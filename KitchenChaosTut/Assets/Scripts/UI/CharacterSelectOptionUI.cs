using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;
using Unity.Services.Lobbies.Models;

public class CharacterSelectOptionUI : MonoBehaviour
{
    [SerializeField] private Button mainMenuBtn;
    [SerializeField] private Button readyBtn;

    [SerializeField] private TextMeshProUGUI lobbyNameText;
    [SerializeField] private TextMeshProUGUI lobbyCodeText;

    private void Awake() 
    {
        mainMenuBtn.onClick.AddListener(() =>
        {
            GameLobby.Instance.LeaveLobby();
            NetworkManager.Singleton.Shutdown();
            Loader.LoadScene(Loader.Scene.MainMenu);
        });
        readyBtn.onClick.AddListener(() =>
        {
            CharacterSelectReady.Instance.CheckPlayerReady();
        });
    }

    private void Start() 
    {
        // Get Lobby Information
        Lobby lobby =  GameLobby.Instance.GetLobby();

        lobbyNameText.text = "Lobby Name: " + lobby.Name;
        lobbyCodeText.text = "Lobby Code: " + lobby.LobbyCode;
    }
}
