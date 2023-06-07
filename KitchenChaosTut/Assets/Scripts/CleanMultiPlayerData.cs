using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CleanMultiPlayerData : MonoBehaviour
{
    private void Awake() {
        if(NetworkManager.Singleton.gameObject != null)
        {
            Destroy(NetworkManager.Singleton.gameObject);
        }

        if(KitchenGameMultiplayer.Instance != null)
        {
            Destroy(KitchenGameMultiplayer.Instance.gameObject);
        }
    }
}
