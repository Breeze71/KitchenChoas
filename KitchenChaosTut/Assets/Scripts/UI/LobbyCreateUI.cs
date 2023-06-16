using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class LobbyCreateUI : MonoBehaviour
{
    [SerializeField] private Button createPublicBtn;
    [SerializeField] private Button createPrivateBtn;
    [SerializeField] private TMP_InputField lobbyNameInputField;
    [SerializeField] private Button closeBtn;     

    private void Awake() 
    {
        createPublicBtn.onClick.AddListener(() =>
        {
            GameLobby.Instance.CreateLobby(lobbyNameInputField.text, false);
        });

        createPrivateBtn.onClick.AddListener(() =>
        {
            GameLobby.Instance.CreateLobby(lobbyNameInputField.text, true);
        });

        closeBtn.onClick.AddListener(() =>
        {
            Hide();
        });
    }

    private void Start()
    {
        Hide();
    }
    public void Show()
    {
        gameObject.SetActive(true);
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
