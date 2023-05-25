using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameClockUI : MonoBehaviour
{
    [SerializeField] private Image timerImg;

    private void FixedUpdate() 
    {
        timerImg.fillAmount = GameManager.Instance.GetGamePlayingTimerNormalized();
    }
}
