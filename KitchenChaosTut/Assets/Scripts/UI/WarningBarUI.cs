using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarningBarUI : MonoBehaviour
{
    private const string IsFlashing = "isFlashing";

    [SerializeField] private StoveCounter stoveCounter;
    private Animator anim;

    private void Awake() 
    {
        anim = GetComponent<Animator>();    
    }
    private void Start() 
    {
        stoveCounter.OnProgressBarChanged += stoveCounter_OnProgressBarChange;

        anim.SetBool(IsFlashing, false);
    }
    
    // Warning to burn
    private void stoveCounter_OnProgressBarChange(object sender, IHasProgress.OnProgressBarChangeEventArgs e)
    {
        float burnProgressAmount = .5f;

        // 由於 OnProgressBarChange 基本等於 Update
        bool isGoingToFried = stoveCounter.IsFried() && e.progressBarNormalized >= burnProgressAmount;
        anim.SetBool(IsFlashing, isGoingToFried);
    }

}
