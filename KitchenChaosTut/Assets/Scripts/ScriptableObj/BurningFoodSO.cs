using UnityEngine;

[CreateAssetMenu()]
public class BurningFoodSO : ScriptableObject
{
    public KitchenObjSO input;
    public KitchenObjSO output;
    public float BurningTimerMax;
}
