using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectingUI : MonoBehaviour
{
    private void Start() 
    {
        KitchenGameMultiplayer.Instance.OnTryingToJoin += KitchenGameMultiplayer_OnTryingToJoin;
        KitchenGameMultiplayer.Instance.OnFailedToJoin += KitchenGameMultiplayer_OnFailedToJoin;


        Hide();
    }

    private void KitchenGameMultiplayer_OnTryingToJoin(object sender, EventArgs e)
    {
        Show();
    }
    private void KitchenGameMultiplayer_OnFailedToJoin(object sender, EventArgs e)
    {
        Hide();
    }
    private void OnDestroy() 
    {
        KitchenGameMultiplayer.Instance.OnTryingToJoin -= KitchenGameMultiplayer_OnTryingToJoin;
        KitchenGameMultiplayer.Instance.OnFailedToJoin -= KitchenGameMultiplayer_OnFailedToJoin;
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
