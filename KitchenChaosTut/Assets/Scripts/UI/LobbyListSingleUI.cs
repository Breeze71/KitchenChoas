using UnityEngine;
using Unity.Services.Lobbies.Models;
using TMPro;
using UnityEngine.UI;

public class LobbyListSingleUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI lobbyNameText;
    private Lobby lobby;

    private void Awake() 
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            GameLobby.Instance.JoinById(lobby.Id);
        });
    }

    public void SetLobby(Lobby _lobby)
    {
        this.lobby = _lobby;
        lobbyNameText.text = _lobby.Name;
    }
}
