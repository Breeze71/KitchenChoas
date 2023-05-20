using UnityEngine;

public class StoveCounterVisual : MonoBehaviour
{
    [SerializeField] private StoveCounter stoveCounter;
    [SerializeField] private GameObject stoveOnGameObj;
    [SerializeField] private GameObject stoveParticle;

    private void Start() 
    {
        stoveCounter.OnFoodStateChanged += StoveCounter_OnFoodStateChanged;
    }

    private void StoveCounter_OnFoodStateChanged(object sender, StoveCounter.OnFoodStateChangedEventArgs e)
    {
        // fried and frying 會啟用 visual
        bool showVisual = e.foodState == StoveCounter.FoodState.Frying || e.foodState == StoveCounter.FoodState.Fried;

        stoveOnGameObj.SetActive(showVisual);
        stoveParticle.SetActive(showVisual);
    }
}
