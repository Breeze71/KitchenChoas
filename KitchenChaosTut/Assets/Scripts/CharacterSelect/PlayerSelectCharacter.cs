using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;

public class PlayerSelectCharacter : MonoBehaviour
{   
    [SerializeField] private int playerIndex;
    [SerializeField] private GameObject readyText;
    [SerializeField] private PlayerVisual playerVisual;
    [SerializeField] private Button kickButton;

    [SerializeField] private TextMeshPro playerNameText;

    private void Awake() 
    {
        kickButton.onClick.AddListener(() =>
        {
            PlayerData playerData = KitchenGameMultiplayer.Instance.GetPlayerDataFromPlayerIndex(playerIndex);
            
            GameLobby.Instance.KickPlayer(playerData.playerLobbyId.ToString());
            KitchenGameMultiplayer.Instance.KickPlayer(playerData.clientId);
        });
    }
    private void Start() 
    {
        KitchenGameMultiplayer.Instance.OnPlayerDataNetworkListChanged += KitchenGameMultiplayer_OnPlayerDataNetworkListChanged;
        CharacterSelectReady.Instance.OnClientReady += CharacterSelectReady_OnClientReady;

        kickButton.gameObject.SetActive(NetworkManager.Singleton.IsServer);

        Hide(readyText);
        UpdatePlayer();
    }

    private void CharacterSelectReady_OnClientReady(object sender, EventArgs e)
    {
        UpdatePlayer();
    }

    private void KitchenGameMultiplayer_OnPlayerDataNetworkListChanged(object sender, EventArgs e)
    {
        UpdatePlayer();
    }

    private void UpdatePlayer()
    {
        if(KitchenGameMultiplayer.Instance.IsPlayerIndexConnected(playerIndex))
        {
            Show(gameObject);

            PlayerData playerData = KitchenGameMultiplayer.Instance.GetPlayerDataFromPlayerIndex(playerIndex);

            readyText.SetActive(CharacterSelectReady.Instance.IsPlayerReady(playerData.clientId));
            playerNameText.text = playerData.playerName.ToString();

            // 真正的改
            playerVisual.SetPlayerColor(KitchenGameMultiplayer.Instance.GetPlayerIconColor(playerData.colorId));
        }
        else
        {
            Hide(gameObject);
        }
    }

    private void OnDestroy() {
        KitchenGameMultiplayer.Instance.OnPlayerDataNetworkListChanged -= KitchenGameMultiplayer_OnPlayerDataNetworkListChanged;
    }
    public void Show(GameObject _gameObject)
    {
        _gameObject.SetActive(true);
    }
    public void Hide(GameObject _gameObject)
    {
        _gameObject.SetActive(false);
    }
}
