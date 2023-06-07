using UnityEngine;
using TMPro;
using System;

public class TutorialUI : MonoBehaviour
{
    # region UI property
    [SerializeField] private TextMeshProUGUI moveUpText;
    [SerializeField] private TextMeshProUGUI moveDownText;
    [SerializeField] private TextMeshProUGUI moveRightText;
    [SerializeField] private TextMeshProUGUI moveLeftText;
    [SerializeField] private TextMeshProUGUI interactText;
    [SerializeField] private TextMeshProUGUI cutText;
    [SerializeField] private TextMeshProUGUI PauseText;

    [SerializeField] private TextMeshProUGUI gamePad_interactText;
    [SerializeField] private TextMeshProUGUI gamePad_cutText;
    [SerializeField] private TextMeshProUGUI gamePad_PauseText;
    #endregion

    private void Start()
    {
        InputManager.Instance.OnRebindKey += InputManager_OnRebindKey;
        GameManager.Instance.OnLocalPlayerReadyChanged += GameManager_OnLocalPlayerReadyChanged;
        
        UpdateVisual();
        Show();
    }

    private void GameManager_OnLocalPlayerReadyChanged(object sender, EventArgs e)
    {
        if(GameManager.Instance.IsLocalPlayerReady())
        {
            Hide();
        }
    }

    private void InputManager_OnRebindKey(object sender, EventArgs e)
    {
        UpdateVisual();
    }

    private void UpdateVisual()
    {
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

    public void Show()
    {
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
