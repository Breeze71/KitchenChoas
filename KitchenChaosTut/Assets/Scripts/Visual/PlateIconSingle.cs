using UnityEngine;
using UnityEngine.UI;

public class PlateIconSingle : MonoBehaviour
{
    [SerializeField] private Image img;

    public void SetKitchenObjSO(KitchenObjSO kitchenObjSO)
    {
        img.sprite = kitchenObjSO.sprite;
    }
}
