using UnityEngine;
using TMPro;
using System;

public class GameStartCountdownUI : MonoBehaviour
{
    private const string number_PopUp = "PopUp";

    [SerializeField] private TextMeshProUGUI countdownText;
    private Animator anim;
    private int previousCountDownInterger;

    private void Awake() 
    {
        anim = GetComponent<Animator>();
    }
    
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
    {
        int countdownInterger =  Mathf.CeilToInt(GameManager.Instance.GetCountdownToStartTimer());                       
        countdownText.text = countdownInterger.ToString();

        if(previousCountDownInterger != countdownInterger)
        {
            previousCountDownInterger = countdownInterger;

            anim.SetTrigger(number_PopUp);
            SoundManager.Instance.PlayCountDownSound();
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
