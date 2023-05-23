using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DeliveryManagerIconSingle : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI recipeNameText;
    [SerializeField] private Transform iconContainer;
    [SerializeField] private Transform iconTemplate;

    private void Awake() 
    {
        iconTemplate.gameObject.SetActive(false);    
    }

    public void SetRecipeSO(RecipeSO recipeSO)
    {
        recipeNameText.text = recipeSO.recipeName;

        foreach(Transform child in iconContainer)
        {
            if(child == iconTemplate)
                continue;
            Destroy(child.gameObject);
        }

        // 生成 iconTemplate 並換成對應圖片
        foreach(KitchenObjSO kitchenObjSO in recipeSO.kitchenObjSOList)
        {
            Transform iconTransform = Instantiate(iconTemplate, iconContainer);
            iconTransform.gameObject.SetActive(true);

            iconTransform.GetComponent<Image>().sprite = kitchenObjSO.sprite;
        }
    }
}
