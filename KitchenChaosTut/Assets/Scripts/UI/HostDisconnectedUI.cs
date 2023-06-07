using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;


public class HostDisconnectedUI : MonoBehaviour
{
    [SerializeField] private Button playAgainButton;

    private void Awake() 
    {
        playAgainButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.Shutdown();
            Loader.LoadScene(Loader.Scene.LobbyScene);
        });
    }

    private void Start() 
    {
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;

        Hide();
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
    {
        if(clientId == NetworkManager.ServerClientId)
        {
            // server disconnected
            Show();
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);

    }
    private void Hide()
    {
        gameObject.SetActive(false);        
    }

}
