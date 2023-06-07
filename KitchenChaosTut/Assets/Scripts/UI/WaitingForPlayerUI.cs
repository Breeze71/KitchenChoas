using System;
using UnityEngine;

public class WaitingForPlayerUI : MonoBehaviour
{
    private void Start() 
    {
        GameManager.Instance.OnLocalPlayerReadyChanged += GameManager_OnLocalPlayerReadyChanged;
        GameManager.Instance.OnGameStateChanged += GameManager_OnGameStateChanged;

        Hide();
    }

    private void GameManager_OnGameStateChanged(object sender, EventArgs e)
    {
        if(GameManager.Instance.IsCountdownToStartActive())
        {
            Hide();
            Debug.Log("hide");
        }
    }
    private void GameManager_OnLocalPlayerReadyChanged(object sender, EventArgs e)
    {
        if(GameManager.Instance.IsLocalPlayerReady() )
        {
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
