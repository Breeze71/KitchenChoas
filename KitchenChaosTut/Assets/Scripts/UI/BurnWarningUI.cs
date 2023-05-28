using System;
using UnityEngine;

public class BurnWarningUI : MonoBehaviour
{
    [SerializeField] private StoveCounter stoveCounter;

    private void Start() 
    {
        stoveCounter.OnProgressBarChange += stoveCounter_OnProgressBarChange;

        Hide();
    }
    
    // Warning to burn
    private void stoveCounter_OnProgressBarChange(object sender, IHasProgress.OnProgressBarChangeEventArgs e)
    {
        float burnProgressAmount = .5f;

        bool isGoingToFried = stoveCounter.IsFried() && e.progressBarNormalized >= burnProgressAmount;

        if(isGoingToFried)
            Show();
        else
            Hide();
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
