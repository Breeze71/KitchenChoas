using UnityEngine;
using Unity.Netcode;

public class CleanMultiPlayerData : MonoBehaviour
{
    private void Awake() 
    {
        if(KitchenGameMultiplayer.Instance != null)
        {
            Destroy(KitchenGameMultiplayer.Instance.gameObject);
        }
        if(NetworkManager.Singleton != null)
        {
            Destroy(NetworkManager.Singleton.gameObject);
        }
        if(GameLobby.Instance != null)
        {
            Destroy(GameLobby.Instance.gameObject);
        }
    }
}
