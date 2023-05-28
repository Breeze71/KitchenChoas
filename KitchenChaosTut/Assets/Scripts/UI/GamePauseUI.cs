using System;
using UnityEngine;
using UnityEngine.UI;

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
        GameManager.Instance.OnGamePause += GameManager_OnGamePause;
        GameManager.Instance.OnGameResume += GameManager_OnGameResume;        
        
        Hide();
    }

    private void GameManager_OnGamePause(object sender, EventArgs e)
    {
        Show();
    }

    private void GameManager_OnGameResume(object sender, EventArgs e)
    {
        Hide();
    }


    public void Show()
    {
        gameObject.SetActive(true);

        // Controller 玩家沒法點擊
        resumeButton.Select();
    }

    public void Hide()
    {
        gameObject.SetActive(false);        
    }
}
