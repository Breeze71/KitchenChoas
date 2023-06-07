using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using Unity.Netcode;


public class ConnectingResponseUi : MonoBehaviour
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


        Hide();
    }
    private void KitchenGameMultiplayer_OnFailedToJoin(object sender, EventArgs e)
    {
        Show();

        messageText.text = NetworkManager.Singleton.DisconnectReason;

        // 避免 OnDestory 之後加入的玩家沒提示
        if(messageText.text == "")
        {
            messageText.text = "Failed to Connected";
        }
    }
    private void OnDestroy() 
    {
        KitchenGameMultiplayer.Instance.OnFailedToJoin -= KitchenGameMultiplayer_OnFailedToJoin;
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
