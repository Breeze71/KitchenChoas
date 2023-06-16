using System;
using UnityEngine;

public class SelectedCounterVisual : MonoBehaviour
{
    [SerializeField] private BaseCounter baseCounter;
    [SerializeField] private GameObject[] visualGameObjArray;

    private void Start() 
    {
        if(Player.LocalInstance != null)
        {
            Player.LocalInstance.OnSelectCounterChanged += Player_OnSelectedCounterChanged;
        }
        else
        {
            // LocalInstace 已經設定好了
            Player.OnAnyPlayerSpawned += Player_OnAnyPlayerSpawned;
        }
    }

    private void Player_OnAnyPlayerSpawned(object sender, EventArgs e)
    {
        if(Player.LocalInstance != null)
        {
            // 避免多玩家加入重複邏輯
            Player.LocalInstance.OnSelectCounterChanged -= Player_OnSelectedCounterChanged;
            Player.LocalInstance.OnSelectCounterChanged += Player_OnSelectedCounterChanged;
        }
    }

    private void Player_OnSelectedCounterChanged(object sender, Player.OnSelectCounterChangedEventArgs e)
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
