using UnityEngine;
using System;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI recipeDeliverdText;

    private void Start() 
    {
        GameManager.Instance.OnGameStateChanged += GameManager_OnGameStateChanged;
        
        Hide();
    }

    private void GameManager_OnGameStateChanged(object sender, EventArgs e)
    {
        if(GameManager.Instance.isGameOver())
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
