using UnityEngine;
using TMPro;
using System;

public class GameStartCountdownUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countdownText;

    private void Start() 
    {
        GameManager.Instance.OnGameStateChanged += GameManager_OnGameStateChanged;
        
        Hide();
    }

    private void GameManager_OnGameStateChanged(object sender, EventArgs e)
    {
        if(GameManager.Instance.IsCountdownToStartActive())
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    private void Update() 
    {                               // 取整
        countdownText.text = Mathf.Ceil(GameManager.Instance.GetCountdownToStartTimer()).ToString();
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
