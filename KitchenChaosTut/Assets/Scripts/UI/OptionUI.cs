using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class OptionUI : MonoBehaviour
{
    public static OptionUI Instance
    {
        get;
        private set;
    }

    /* Options */
    #region Options
    [SerializeField] private Button soundEffectButton;
    [SerializeField] private Button musicButton;
    [SerializeField] private TextMeshProUGUI soundEfxText;
    [SerializeField] private TextMeshProUGUI musicText;
    [SerializeField] private Button closeButton;
    [SerializeField] private Transform RebindPanel;
    #endregion

    /* Input Key */
    #region  Input Key
    [SerializeField] private Button moveUpButton;
    [SerializeField] private Button moveDownButton;
    [SerializeField] private Button moveRightButton;
    [SerializeField] private Button moveLeftButton;
    [SerializeField] private Button interactButton;
    [SerializeField] private Button cutButton;
    [SerializeField] private Button PauseButton;
    [SerializeField] private TextMeshProUGUI moveUpText;
    [SerializeField] private TextMeshProUGUI moveDownText;
    [SerializeField] private TextMeshProUGUI moveRightText;
    [SerializeField] private TextMeshProUGUI moveLeftText;
    [SerializeField] private TextMeshProUGUI interactText;
    [SerializeField] private TextMeshProUGUI cutText;
    [SerializeField] private TextMeshProUGUI PauseText;

    //Controller
    [SerializeField] private Button gamePad_interact;
    [SerializeField] private Button gamePad_cut;
    [SerializeField] private Button gamePad_Pause;
    [SerializeField] private TextMeshProUGUI gamePad_interactText;
    [SerializeField] private TextMeshProUGUI gamePad_cutText;
    [SerializeField] private TextMeshProUGUI gamePad_PauseText;

    #endregion

    private Action OnCloseBtnAction;

    private void Awake() 
    {
        Instance = this;

        soundEffectButton.onClick.AddListener(() => 
        {
            SoundManager.Instance.ChangeVolume();
            UpdateVisual();
        });

        musicButton.onClick.AddListener(() => 
        {
            MusicManager.Instance.ChangeVolume();
            UpdateVisual();
        });

        closeButton.onClick.AddListener(() => 
        {
            Hide();

            // 觸發在 Show 那綁定的 Action，即 OnCloseAction  Pause 的 Show ()
            OnCloseBtnAction();
        });        
    
        /* Key Binding */
        moveUpButton.onClick.AddListener(() =>
        {
            RebindKeyUpdate(InputManager.Binding.moveUp);
        });
        moveDownButton.onClick.AddListener(() =>
        {
            RebindKeyUpdate(InputManager.Binding.moveDown);
        });
        moveLeftButton.onClick.AddListener(() =>
        {
            RebindKeyUpdate(InputManager.Binding.moveLeft);
        });
        moveRightButton.onClick.AddListener(() =>
        {
            RebindKeyUpdate(InputManager.Binding.moveRight);
        });
        interactButton.onClick.AddListener(() =>
        {
            RebindKeyUpdate(InputManager.Binding.Interact);
        });
        cutButton.onClick.AddListener(() =>
        {
            RebindKeyUpdate(InputManager.Binding.Cut);
        });
        PauseButton.onClick.AddListener(() =>
        {
            RebindKeyUpdate(InputManager.Binding.Pause);
        });

        //Controller
        gamePad_interact.onClick.AddListener(() =>
        {
            RebindKeyUpdate(InputManager.Binding.gamePad_Interact);
        });
        gamePad_cut.onClick.AddListener(() =>
        {
            RebindKeyUpdate(InputManager.Binding.gamePad_Cut);
        });
        gamePad_Pause.onClick.AddListener(() =>
        {
            RebindKeyUpdate(InputManager.Binding.gamePad_Pause);
        });
    }

    private void Start() 
    {
        GameManager.Instance.OnGameResume += GameManager_OnGameResume;

        UpdateVisual();

        HideRebindPanel();
        Hide();
    }

    private void GameManager_OnGameResume(object sender, EventArgs e)
    {
        Hide();
    }

    private void RebindKeyUpdate(InputManager.Binding binding)
    {
        ShowRebindPanel();
        InputManager.Instance.RebindKey(binding, () => 
        {
            HideRebindPanel();
            UpdateVisual();
        });
    }

    private void UpdateVisual()
    {
        soundEfxText.text = "Sound Effect: " + Mathf.Round(SoundManager.Instance.GetVolume() * 10f);
        musicText.text = "Music: " + Mathf.Round(MusicManager.Instance.GetVolume() * 10f);

        /* Update Key Binding */
        moveUpText.text = InputManager.Instance.GetBindingText(InputManager.Binding.moveUp);
        moveDownText.text = InputManager.Instance.GetBindingText(InputManager.Binding.moveDown);
        moveLeftText.text = InputManager.Instance.GetBindingText(InputManager.Binding.moveLeft);
        moveRightText.text = InputManager.Instance.GetBindingText(InputManager.Binding.moveRight);
        interactText.text = InputManager.Instance.GetBindingText(InputManager.Binding.Interact);
        cutText.text = InputManager.Instance.GetBindingText(InputManager.Binding.Cut);
        PauseText.text = InputManager.Instance.GetBindingText(InputManager.Binding.Pause);
        // Controller
        gamePad_interactText.text = InputManager.Instance.GetBindingText(InputManager.Binding.gamePad_Interact);
        gamePad_cutText.text = InputManager.Instance.GetBindingText(InputManager.Binding.gamePad_Cut);
        gamePad_PauseText.text = InputManager.Instance.GetBindingText(InputManager.Binding.gamePad_Pause);
    }

    public void Show(Action OnCloseBtnAction)
    {
        this.OnCloseBtnAction = OnCloseBtnAction;

        gameObject.SetActive(true);

        soundEffectButton.Select();
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void HideRebindPanel()
    {
        RebindPanel.gameObject.SetActive(false);
    }
    private void ShowRebindPanel()
    {
        RebindPanel.gameObject.SetActive(true);
    }
}
