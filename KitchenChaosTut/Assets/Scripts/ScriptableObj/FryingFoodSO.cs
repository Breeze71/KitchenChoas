using UnityEngine;

[CreateAssetMenu()]
public class FryingFoodSO : ScriptableObject
{
    public KitchenObjSO input;
    public KitchenObjSO output;
    public float fringTimerMax;
}
