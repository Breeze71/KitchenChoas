using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using Unity.Netcode;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI recipeDeliverdText;
    [SerializeField] private Button playAgainButton;

    private void Awake() 
    {
        playAgainButton.onClick.AddListener(() =>
        {
            // 中斷連接
            NetworkManager.Singleton.Shutdown();

            Loader.LoadScene(Loader.Scene.LobbyScene);
        });
    }

    private void Start() 
    {
        GameManager.Instance.OnGameStateChanged += GameManager_OnGameStateChanged;
        
        Hide();
    }

    private void GameManager_OnGameStateChanged(object sender, EventArgs e)
    {
        if(GameManager.Instance.IsGameOver())
        {
            Show();

            recipeDeliverdText.text = DeliveryManager.Instance.GetSuccessfulRecipe().ToString();
        }
        else
        {
            Hide();
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
