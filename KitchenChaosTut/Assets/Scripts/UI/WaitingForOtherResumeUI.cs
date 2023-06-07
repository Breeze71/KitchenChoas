using System;
using UnityEngine;

public class WaitingForOtherResumeUI : MonoBehaviour
{
    private void Start() 
    {
        GameManager.Instance.OnMutlityPlayerGamePaused += GameManager_OnMutlityPlayerGamePause;
        GameManager.Instance.OnMutlityPlayerGameResumed += GameManager_OnMutlityPlayerGameResume;

        Hide();
    }

    private void GameManager_OnMutlityPlayerGameResume(object sender, EventArgs e)
    {
        Hide();
    }

    private void GameManager_OnMutlityPlayerGamePause(object sender, EventArgs e)
    {
        Show();
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
