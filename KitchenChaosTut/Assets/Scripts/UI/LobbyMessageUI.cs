using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using Unity.Netcode;

public class LobbyMessageUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Button closeBtn;

    private void Awake() 
    {
        closeBtn.onClick.AddListener(Hide);
    }

    private void Start() 
    {
        KitchenGameMultiplayer.Instance.OnFailedToJoin += KitchenGameMultiplayer_OnFailedToJoin;
        GameLobby.Instance.OnCreateLobbyStarted += GameLobby_OnCreateLobbyStarted;
        GameLobby.Instance.OnCreateLobbyFailed += GameLobby_OnCreateLobbyFailed;
        GameLobby.Instance.OnJoinStarted += GameLobby_OnJoinStarted;
        GameLobby.Instance.OnJoinFailed += GameLobby_OnJoinFailed;
        GameLobby.Instance.OnQuickJoinFailed += GameLobby_OnQuickJoinFailed;

        Hide();
    }

    /* 提示 Message  */
    private void KitchenGameMultiplayer_OnFailedToJoin(object sender, EventArgs e)
    {
        messageText.text = NetworkManager.Singleton.DisconnectReason;

        if(messageText.text == "")
        {
            ShowMessage("Failed to Connected");
        }
        else
        {
            ShowMessage(messageText.text);
        }
    }
    private void GameLobby_OnCreateLobbyStarted(object sender, EventArgs e)
    {
        ShowMessage("Creating Lobby...");
    }
    private void GameLobby_OnCreateLobbyFailed(object sender, EventArgs e)
    {
        ShowMessage("Failed To Create Lobby");
    }
    private void GameLobby_OnJoinStarted(object sender, EventArgs e)
    {
        ShowMessage("Loading...");
    }
    private void GameLobby_OnJoinFailed(object sender, EventArgs e)
    {
        ShowMessage("Failed To Join");
    }
    private void GameLobby_OnQuickJoinFailed(object sender, EventArgs e)
    {
        ShowMessage("Cound Not Find A Lobby To Join");
    }
    private void OnDestroy() 
    {
        KitchenGameMultiplayer.Instance.OnFailedToJoin -= KitchenGameMultiplayer_OnFailedToJoin;
        GameLobby.Instance.OnCreateLobbyStarted -= GameLobby_OnCreateLobbyStarted;
        GameLobby.Instance.OnCreateLobbyFailed -= GameLobby_OnCreateLobbyFailed;
        GameLobby.Instance.OnJoinStarted -= GameLobby_OnJoinStarted;
        GameLobby.Instance.OnJoinFailed -= GameLobby_OnJoinFailed;
        GameLobby.Instance.OnQuickJoinFailed -= GameLobby_OnQuickJoinFailed;       
    }


    private void ShowMessage(string _message)
    {
        Show();

        messageText.text = _message;
    }


    public void Show()
    {
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
