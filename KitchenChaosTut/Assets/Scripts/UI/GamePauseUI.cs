using System;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class GamePauseUI : MonoBehaviour
{
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button optionsButton;
    private void Awake() 
    {
        resumeButton.onClick.AddListener(() =>
        {
            GameManager.Instance.Pause_Resume();
        });

        mainMenuButton.onClick.AddListener(() =>
        {
            // 中斷連接
            NetworkManager.Singleton.Shutdown();
            
            Loader.LoadScene(Loader.Scene.MainMenu);
        });

        optionsButton.onClick.AddListener(() =>
        {
            // Hide Pause Show Options
            Hide();

            OptionUI.Instance.Show(Show);   // Pause.Show() closeButton.onClick 時觸發
        });
    }

    private void Start() 
    {
        GameManager.Instance.OnLocalGamePause += GameManager_OnLocalGamePause;
        GameManager.Instance.OnLocalGameResume += GameManager_OnLocalGameResume;        
        
        Hide();
    }

    private void GameManager_OnLocalGamePause(object sender, EventArgs e)
    {
        Show();
    }

    private void GameManager_OnLocalGameResume(object sender, EventArgs e)
    {
        Hide();
    }


    private void Show()
    {
        gameObject.SetActive(true);

        // Controller 玩家沒法點擊
        resumeButton.Select();
    }

    private void Hide()
    {
        gameObject.SetActive(false);        
    }
}
