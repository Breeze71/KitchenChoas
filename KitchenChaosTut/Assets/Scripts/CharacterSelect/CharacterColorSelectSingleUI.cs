using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterColorSelectSingleUI : MonoBehaviour
{
    [SerializeField] private int colorId;
    [SerializeField] private Image characterIcon;
    [SerializeField] private GameObject selectedGameObj;

    private void Awake() 
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            KitchenGameMultiplayer.Instance.ChangePlayerCharacter(colorId);
        });
    }
    private void Start() 
    {
        KitchenGameMultiplayer.Instance.OnPlayerDataNetworkListChanged += KitchenGameMultiplayer_OnPlayerDataNetworkListChanged;
        
        characterIcon.color = KitchenGameMultiplayer.Instance.GetPlayerIconColor(colorId); 

        UpdateVisual();
    }
    private void OnDestroy()
    {
        KitchenGameMultiplayer.Instance.OnPlayerDataNetworkListChanged -= KitchenGameMultiplayer_OnPlayerDataNetworkListChanged;
    }

    private void KitchenGameMultiplayer_OnPlayerDataNetworkListChanged(object sender, EventArgs e)
    {
        // 有人退出取消選擇
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        if(KitchenGameMultiplayer.Instance.GetPlayerData().colorId == colorId)
        {
            selectedGameObj.SetActive(true);
        }
        else
        {
            selectedGameObj.SetActive(false);
        }

    }
}
