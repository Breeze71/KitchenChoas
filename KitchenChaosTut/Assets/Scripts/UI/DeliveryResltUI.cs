using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class DeliveryResltUI : MonoBehaviour
{
    private const string Popup = "Popup";

    [SerializeField] private Image backgroundImg;
    [SerializeField] private Image iconImg;
    [SerializeField] private TextMeshProUGUI messageText;

    [SerializeField] private Color successColor;
    [SerializeField] private Color failedColor;
    [SerializeField] private Sprite successSprite;
    [SerializeField] private Sprite failedSprite;

    private Animator anim;

    private void Awake() 
    {
        anim = GetComponent<Animator>();    
    }

    private void Start() 
    {
        DeliveryManager.Instance.OnRecipeSuccess += DeliveryManager_OnRecipeSuccess;
        DeliveryManager.Instance.OnRecipeFailed += DeliveryManager_OnFailed;

        gameObject.SetActive(false);
    }

    private void DeliveryManager_OnFailed(object sender, EventArgs e)
    {
        gameObject.SetActive(true);

        backgroundImg.color = failedColor;
        iconImg.sprite = failedSprite;
        messageText.text = "DELIVERY \n FAILED";

        anim.SetTrigger(Popup);
    }

    private void DeliveryManager_OnRecipeSuccess(object sender, EventArgs e)
    {
        gameObject.SetActive(true);
        backgroundImg.color = successColor;
        iconImg.sprite = successSprite;
        messageText.text = "DELIVERY \n SUCCESS";
        
        anim.SetTrigger(Popup);  
    }
}
