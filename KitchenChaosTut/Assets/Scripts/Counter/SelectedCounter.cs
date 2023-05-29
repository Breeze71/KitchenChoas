using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedCounter : MonoBehaviour
{
    [SerializeField] private BaseCounter baseCounter;
    [SerializeField] private GameObject[] visualGameObjArray;

    private void Start() 
    {    
        // Player_OnSelectedCounterChanged 訂閱 OnSelectCounterChanged
        //PlayerMovement.Instance.OnSelectCounterChanged += Player_OnSelectedCounterChanged;
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
