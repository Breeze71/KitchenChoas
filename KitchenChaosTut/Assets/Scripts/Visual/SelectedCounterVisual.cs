using System;
using UnityEngine;

public class SelectedCounterVisual : MonoBehaviour
{
    [SerializeField] private BaseCounter baseCounter;
    [SerializeField] private GameObject[] visualGameObjArray;

    private void Start() 
    {
        if(PlayerMovement.LocalInstance != null)
        {
            PlayerMovement.LocalInstance.OnSelectCounterChanged += Player_OnSelectedCounterChanged;
        }
        else
        {
            // LocalInstace 設定好了
            PlayerMovement.OnAnyPlayerSpawned += PlayerMovement_OnAnyPlayerSpawned;
        }
    }

    private void PlayerMovement_OnAnyPlayerSpawned(object sender, EventArgs e)
    {
        if(PlayerMovement.LocalInstance != null)
        {
            // 避免多玩家加入重複邏輯
            PlayerMovement.LocalInstance.OnSelectCounterChanged -= Player_OnSelectedCounterChanged;
            PlayerMovement.LocalInstance.OnSelectCounterChanged += Player_OnSelectedCounterChanged;
        }
    }

    private void Player_OnSelectedCounterChanged(object sender, PlayerMovement.OnSelectCounterChangedEventArgs e)
    {
        // 觸發時傳遞數據
        if(e.seletedCounter == baseCounter)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    private void Show()
    {   
        foreach(GameObject visualGameObj in visualGameObjArray)
        {
            visualGameObj.SetActive(true);
        }
    }

    private void Hide()
    {
        foreach(GameObject visualGameObj in visualGameObjArray)
        {
            visualGameObj.SetActive(false);
        }
    }
}
